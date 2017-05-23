// 
//  OggPlayerVBN.cs
//  
//  Author:
//       El Dragon <thedragon@the-dragons-nest.co.uk>
//  
//  Copyright (c) 2010 Matthew Harris
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System.Threading;
using System.Collections;

namespace DragonOgg
{

	/// <summary>
	/// OggPlayerVBN class (Variable Buffer Number)
	/// This class takes OggFile objects and outputs them in a threaded player
	/// using OpenAL (through the OpenTK wrapper)
	/// The VBN player has a variable number of buffers
	/// This will load the song into the player as quickly as possible.
	/// This is useful for networked files as the actual period of reading is very
	/// short relative to the length of the file. It is less suitable for situations
	/// where memory is short or where spikes in processor demand when a file is first started cannot be tolerated.
	/// </summary>
	public class OggPlayerVBN : OggPlayer
	{

		// Configuration options
		private long m_BufferSize;			// Size of individual buffer segments
		private long m_MaxTotalBufferSize;	// Maximum size in bytes of the buffer heap
		private int m_PrebufferDelay;		// How long to wait (in ms) between a playback or seek command and actual initialisation of the playback thread to allow for buffering
		private bool m_PauseBuffer; 		// Whether the buffering thread should be paused as well as the playing thread on Playback_Pause
		
		// Data storage stuff
		private FloatQueue m_BufferedTimeHeap;
		private LongQueue m_BufferedSizeHeap;
		private Queue m_BufferRefs;
		
		// Internal state flags
		private bool m_PauseRequested;
		private bool m_SeekRequested;
		private bool m_BufferSeekRequested;
		private bool m_StopRequested;
		private bool m_ReachedEOF;
		
		// Internal threads
		private Thread PlayThread;
		private Thread BufferThread;
		
		/// <summary>
		/// Setting to false allows the buffer to continue to build (up to MaxTotalBufferSize) while playback is paused
		/// Default is true (pausing playback also pauses the buffering)
		/// </summary>
		public bool PauseBuffer { get { return m_PauseBuffer; } set { m_PauseBuffer = value; } }
		/// <summary>
		/// Size in bytes of each individual buffer chunk. Default is 8096.
		/// </summary>
		public long BufferSize { get { return m_BufferSize; } set { m_BufferSize = value; } }
		/// <summary>
		/// Maximum total size in bytes of the buffered data. Default is 16777216 (16MB)
		/// </summary>
		public long MaxTotalBufferSize { get { return m_MaxTotalBufferSize; } set { m_MaxTotalBufferSize = value; } }
		/// <summary>
		/// Time to wait in ms between buffering & beginning playback. Default is 250ms. Smaller values improve responsiveness but may cause underruns.
		/// Cannot be less than 2*UpdateDelay - setting to a value lower than this is equivalent to setting to 2*UpdateDelay
		/// </summary>
		public int PrebufferDelay { 
			get { return m_PrebufferDelay; } 
			set {
				if (value<m_UpdateDelay*2) { m_PrebufferDelay = m_UpdateDelay*2; }
				else { m_PrebufferDelay = value; }
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		public OggPlayerVBN()
		{
			m_PlayerState = OggPlayerStatus.Waiting;
			m_BufferSize = 8096;				// Using 8KB buffer segments
			m_MaxTotalBufferSize = 8388608;		// Max of 8MB buffered at any time
			m_TickInterval = 1;
			m_TickEnabled = false;
			m_PrebufferDelay = 250;				// 1/4 of a second to pre-buffer before playback/seeking
			m_UpdateDelay = 5;
			m_PauseBuffer = true;
			m_Context = new AudioContext();
			if (!InitSource()) { throw new OggPlayerSourceException("Source initialisation failed"); } 
			ResetPlayerCondition();
			
		}
		
		/// <summary>
		/// Dispose of the player safely
		/// </summary>
		public override void Dispose ()
		{
			this.Playback_Stop();
			ClearBuffers();
			if (!DestroySource()) { throw new OggPlayerSourceException("Source destruction failed"); }
			if (m_Context!=null) { m_Context.Dispose(); m_Context = null; }
			if (m_CurrentFile!=null) { m_CurrentFile.Dispose(); m_CurrentFile = null; }
			m_BufferedTimeHeap = null;
			m_BufferedSizeHeap = null;
			m_BufferRefs = null;
		}
		
		/// <summary>
		/// Destructor
		/// </summary>
		~OggPlayerVBN()
		{
			this.Playback_Stop();
			ClearBuffers();
			if (!DestroySource()) { throw new OggPlayerSourceException("Source destruction failed"); }
			if (m_Context!=null) { m_Context.Dispose(); m_Context = null; }
			if (m_CurrentFile!=null) { m_CurrentFile.Dispose(); m_CurrentFile = null; }
			m_BufferedTimeHeap = null;
			m_BufferedSizeHeap = null;
			m_BufferRefs = null;
		}
		
		/// <summary>
		/// Internal helper to clean up the buffered data
		/// </summary>
		private void ClearBuffers()
		{
			// Remove any queued buffers from the source
			int BufferCount; AL.GetSource(m_Source, ALGetSourcei.BuffersQueued, out BufferCount);
			if (BufferCount>0) { AL.SourceUnqueueBuffers((int)m_Source, BufferCount); }
			// De-allocate buffers from memory
			if (m_BufferRefs==null) { return; }
			while (m_BufferRefs.Count>0)
			{
				uint BufferID = (uint) m_BufferRefs.Dequeue();
				AL.DeleteBuffer(ref BufferID);
			}
			m_BufferedTimeHeap = new FloatQueue();
			m_BufferedSizeHeap = new LongQueue();
		}
		
		/// <summary>
		/// Internal helper to reset all the internal player flags & info
		/// </summary>
		private void ResetPlayerCondition()
		{
			m_PauseRequested = false;
			m_SeekRequested = false;
			m_BufferSeekRequested = false;
			m_StopRequested = false;
			m_ReachedEOF = false;
			m_BufferedTimeHeap = new FloatQueue();
			m_BufferedSizeHeap = new LongQueue();
			m_BufferRefs = new Queue();
			m_BufferOffset = 0;
			m_PlayingOffset = 0;
			m_LastTick = 0;
		}
		
		/// <summary>
		/// Buffering process
		/// </summary>
		private void BufferThreadle()
		{
			bool Running = true;
			while (Running)
			{
				if (m_BufferSeekRequested) { Thread.Sleep(10); continue; }
				if (m_StopRequested) { Running = false; continue; }
				if (m_ReachedEOF) { Running = false; continue; }
				if (m_PlayerState==OggPlayerStatus.Error) { Running = false; continue; }
				if (m_PauseRequested&&m_PauseBuffer) { Thread.Sleep(10); continue; }
				if (m_BufferedSizeHeap.Total>m_MaxTotalBufferSize) { Thread.Sleep(10); continue; }
				OggBufferSegment obs;
				obs = m_CurrentFile.GetBufferSegment((int)m_BufferSize);
				if (obs.ReturnValue==0) 
				{
					// EOF
					SendMessage(OggPlayerMessageType.BufferEndOfFile);
					m_ReachedEOF = true;
				}
				else if (obs.ReturnValue<0)
				{
					// Error!
					SendMessage(OggPlayerMessageType.FileReadError);
					StateChange(OggPlayerStatus.Error, OggPlayerStateChanger.Error);
					AL.SourceStop(m_Source);
				}
				else
				{
					// Read was OK, Buffer this data
					lock (OALLocker)
					{
						// Create a buffer reference
						uint BufferRef; AL.GenBuffer(out BufferRef);
						// Stick it on the end of the buffer ref heap
						m_BufferRefs.Enqueue(BufferRef);
						// Popuplate the buffer
						AL.BufferData((int)BufferRef, m_CurrentFile.Format, obs.Buffer, obs.ReturnValue, obs.RateHz);
						// Add it to the queue
						AL.SourceQueueBuffers(m_Source, 1, ref BufferRef);
						// Update counts
						m_BufferedSizeHeap.Push(obs.ReturnValue);
						float m_NewBufferOffset = m_CurrentFile.GetTime();
						m_BufferedTimeHeap.Push(m_NewBufferOffset - m_BufferOffset);
						m_BufferOffset = m_NewBufferOffset;
					}
				}
				// Check for errors
				m_LastError = AL.GetError();
				if (m_LastError!= ALError.NoError)
				{
					StateChange(OggPlayerStatus.Error, OggPlayerStateChanger.Error);
					lock (OALLocker) { AL.SourceStop(m_Source); }
					SendMessage(OggPlayerMessageType.OpenALError, m_LastError);
					Running = false;
				}
				
				if (m_UpdateDelay>0) { Thread.Sleep(m_UpdateDelay); }
			}
		}
		
		/// <summary>
		/// Playing process
		/// </summary>
		private void PlaybackThread()
		{
			bool Running = true;
			while (Running)
			{
				if (m_SeekRequested) { Thread.Sleep(10); continue; }
				if (m_StopRequested) { Running = false; continue; }
				if (m_PlayerState==OggPlayerStatus.Error) { Running = false; continue; }
				if (m_PauseRequested) { Thread.Sleep(10); continue; }
				
				
				// Check currently queued buffers
				int QueuedBuffers;
				AL.GetSource(m_Source, ALGetSourcei.BuffersQueued, out QueuedBuffers);
				// Check for underruns/playback complete
				// We're using -two- 'cos it doesn't seem to handle the last buffer properly (and the last 2 on windows systems
				// (not sure why yet - need to do some investigatling). This shouldn't cause any problems unless a large Buffer_Size is set
				// Anyway: Fear the evil magic hack 'cos this should really be if (QueuedBuffers<=0)
				if (QueuedBuffers<=2)
				{
					if (m_ReachedEOF)
					{
						if (AL.GetSourceState(m_Source)!=ALSourceState.Stopped) { AL.SourceStop(m_Source); }
						Playback_Stop(true);
						StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.EndOfFile);
						SendMessage(OggPlayerMessageType.PlaybackEndOfFile);
						Running = false;
					}
					else
					{
						SendMessage(OggPlayerMessageType.BufferUnderrun);
						if (!BufferThread.IsAlive) { BufferThread.Start(); }
						Thread.Sleep(m_PrebufferDelay);
					}
					continue;
				}
							
				// See if we're playing
				if (AL.GetSourceState(m_Source)!=ALSourceState.Playing)
				{
					lock (OALLocker) { AL.SourcePlay(m_Source); }	
					#if (DEBUG)
					Console.WriteLine("Source restarted");
					#endif
				}
				
				// Count processed buffers
				int ProcessedBuffers;
				AL.GetSource(m_Source, ALGetSourcei.BuffersProcessed, out ProcessedBuffers);
				if (ProcessedBuffers>0)
				{
					// We've got some buffers to desclurple
					lock (OALLocker)
					{
						while (ProcessedBuffers>0)
						{
							// Unqueue the first buffer from the source
							AL.SourceUnqueueBuffer((int) m_Source);
							// De-allocate the buffer from memory
							if (m_BufferRefs.Count>0) 
							{
								uint BufferRef = (uint) m_BufferRefs.Dequeue();
								if (!AL.IsBuffer(BufferRef)) { SendMessage(OggPlayerMessageType.BufferAnomaly); }
								AL.DeleteBuffer(ref BufferRef);
							}
							else
							{
								SendMessage(OggPlayerMessageType.BufferHeapAnomaly);
							}
							// Update the internal quantity tracking
							m_BufferedSizeHeap.Pop();
							m_PlayingOffset += m_BufferedTimeHeap.Pop();
							// Decrement processed buffers & loop
							ProcessedBuffers--;
						}
					}
					
					// Check tick events
					if (m_PlayingOffset >= m_LastTick + m_TickInterval)
					{
						m_LastTick = m_PlayingOffset;
						if (TickEnabled) { SendTick(m_PlayingOffset, m_BufferOffset); }
					}
				}
				else { Thread.Sleep(10); }
				
				if (m_UpdateDelay>0) { Thread.Sleep(m_UpdateDelay); }
			}
		}
		
		public override OggPlayerCommandReturn Playback_Play()
		{
			if (m_CurrentFile==null) { return OggPlayerCommandReturn.NoFile; }
			if (m_PlayerState!=OggPlayerStatus.Stopped) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			
			PlayThread = null;
			BufferThread = null;
			
			// Reset internal variables
			ResetPlayerCondition();
			
			// Start buffering
			StateChange(OggPlayerStatus.Buffering, OggPlayerStateChanger.UserRequest);
			BufferThread = new Thread(new ThreadStart(BufferThreadle));
			BufferThread.Start();
			// Wait for a little bit
			Thread.Sleep(m_PrebufferDelay);
			// Start playing
			StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest);
			PlayThread = new Thread(new ThreadStart(PlaybackThread));
			PlayThread.Start();
			
			return OggPlayerCommandReturn.Success;
		}
		
		public override OggPlayerCommandReturn Playback_Seek(float SeekTime)
		{
			if (!(m_PlayerState==OggPlayerStatus.Playing)||(m_PlayerState==OggPlayerStatus.Paused)||(m_PlayerState==OggPlayerStatus.Buffering)) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			
			// Validate that the requested time is viable
			if (SeekTime>this.FileLengthTime) { return OggPlayerCommandReturn.ValueOutOfRange; }
			if (SeekTime<0) { return OggPlayerCommandReturn.ValueOutOfRange; }
			
			// Put the playback & buffer threads on standby for now
			m_SeekRequested = true;
			m_BufferSeekRequested = true;
			// Wait for the threads to loop & stop
			Thread.Sleep(PrebufferDelay);
			// Stop the source	
			lock (OALLocker) { if (AL.GetSourceState(m_Source)!=ALSourceState.Stopped) { AL.SourceStop(m_Source); } }
			// Change state to reflect seeking
			StateChange(OggPlayerStatus.Seeking, OggPlayerStateChanger.UserRequest);
			// Empty the buffers
			ClearBuffers();
			// Move the file to the right place
			m_CurrentFile.SeekToTime(SeekTime);
			// Set timing values correctly
			m_BufferOffset = SeekTime;
			m_PlayingOffset = SeekTime;
			// Set tick values
			m_LastTick = SeekTime - m_TickInterval;
			// Unpause the buffer thread
			m_BufferSeekRequested = false;
			// Restart the buffer thread if it's already shut down due to EOF
			if (m_ReachedEOF||!BufferThread.IsAlive)
			{
				m_ReachedEOF = false;
				BufferThread = new Thread(new ThreadStart(BufferThreadle));
				BufferThread.Start();
			}
			// Wait for the pre-buffer delay to elapse
			Thread.Sleep(m_PrebufferDelay);
			// Unpause the player thread
			m_SeekRequested = false;
			// Jump out here if somethings gone wrong
			if (m_PlayerState==OggPlayerStatus.Error) { return OggPlayerCommandReturn.Error; }
			// Change state to reflect seek done
			if (!m_PauseRequested) 
			{ 
				StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest); 
			} 
			else 
			{ 
				StateChange(OggPlayerStatus.Paused, OggPlayerStateChanger.UserRequest); 
			}
			// Done!
			return OggPlayerCommandReturn.Success;
		}
		
		public override OggPlayerCommandReturn Playback_Stop() { return Playback_Stop(false); }
		private OggPlayerCommandReturn Playback_Stop(bool Internal)
		{
			if (!(m_PlayerState==OggPlayerStatus.Playing)||(m_PlayerState==OggPlayerStatus.Paused)||(m_PlayerState==OggPlayerStatus.Buffering)) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			// Request stop
			m_StopRequested = true;
			// Wait a sensible time for the threads to enact this
			Thread.Sleep(50);
			// Stop player & clear buffers
			AL.SourceStop(m_Source);
			ClearBuffers();
			// Reset stuff
			ResetPlayerCondition();
			m_CurrentFile.SeekToTime(0);
			// Change state
			if (!Internal) { StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.UserRequest); }
			// Done!
			return OggPlayerCommandReturn.Success;
		}
		
		public override OggPlayerCommandReturn Playback_UnPause()
		{
			if (m_PlayerState!=OggPlayerStatus.Paused) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			// Cancel the pause request
			m_PauseRequested = false;
			// Give the buffer time to catch up
			if (m_PauseBuffer)
			{
				Thread.Sleep(m_PrebufferDelay);
			}
			// Start the playback
			AL.SourcePlay(m_Source);
			// Change state
			StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest);
			// Done!
			return OggPlayerCommandReturn.Success;
		}
		
		public override OggPlayerCommandReturn Playback_Pause()
		{
			if (m_PlayerState!=OggPlayerStatus.Playing && m_PlayerState!=OggPlayerStatus.Buffering) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			// Pause the source
			AL.SourcePause(m_Source);
			// Send pause requests to threads
			m_PauseRequested = true;
			// Change state
			StateChange(OggPlayerStatus.Paused, OggPlayerStateChanger.UserRequest);
			// Done!
			return OggPlayerCommandReturn.Success;
		}
		
		public override bool SetCurrentFile(OggFile File)
		{
			if (!((m_PlayerState==OggPlayerStatus.Stopped)||(m_PlayerState==OggPlayerStatus.Waiting))) { return false; }
			m_CurrentFile = File;
			ResetPlayerCondition();
			StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.UserRequest);
			return true;
		}
		
		public override bool SetCurrentFile(string FileName)
		{
			return SetCurrentFile(new OggFile(FileName));
		}		
		
	}
}
