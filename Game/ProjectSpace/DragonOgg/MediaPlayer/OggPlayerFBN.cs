//
//  OggPlayer.cs
//  
//  Author:
//       dragon@the-dragons-nest.co.uk
// 
//  Copyright (c) 2010 Matthew Harris
//
//  Updated (04/2011):
//      Caleb Leak
//      caleb@embergames.net
//      www.EmberGames.net
//      
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Threading;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace DragonOgg.MediaPlayer
{
	
	/// <summary>
	/// OggPlayerFBN class (Fixed Buffer Number)
	/// This class takes OggFile objects and outputs them in a threaded player
	/// using OpenAL (through the OpenTK wrapper)
	/// The FBN player has a fixed number of buffers - Use SetBufferInfo to configure them
	/// This is useful for background playing where memory is a significant issue.
	/// </summary>
	public class OggPlayerFBN : OggPlayer
	{


		private uint[] m_Buffers;
		private int m_BufferCount;
		private int m_BufferSize;
				
		// Property exposure


		/// <summary>
		/// The current size of each buffer block
		/// Use SetBufferInfo to change this value
		/// </summary>
		public int BufferSize { get { return m_BufferSize; } }
		/// <summary>
		/// The current number of buffer blocks
		/// Use SetBufferInfo to change this value
		/// </summary>
		public int BufferCount { get { return m_BufferCount; } }

		
		/// <summary>
		/// Constructor
		/// </summary>
		public OggPlayerFBN()
		{
			m_PlayerState = OggPlayerStatus.Waiting;
			
			m_UpdateDelay = 10;
			m_Context = new AudioContext();			// Initialise the AudioContext
			m_BufferCount = 32;
			m_BufferSize = 4096;
			m_Buffers = new uint[m_BufferCount];				// We're using four buffers so we always have a supply of data
			
			m_TickInterval = 1;			// Default tick is every second
			m_TickEnabled = false;		// Tick event is disabled by default
			
			if (!InitSource()) { throw new OggPlayerSourceException("Source initialisation failed"); }
		}
		
		/// <summary>
		/// Function for configuring buffer settings
		/// </summary>
		/// <param name="NumberOfBuffers">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="BufferSize">
		/// A <see cref="System.Int32"/>
		/// </param>
		public OggPlayerCommandReturn SetBufferInfo(int NumberOfBuffers, int BufferSize)
		{
			if (!((m_PlayerState==OggPlayerStatus.Stopped)||(m_PlayerState==OggPlayerStatus.Waiting))) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			m_BufferCount = NumberOfBuffers;
			m_BufferSize = BufferSize;
			m_Buffers = new uint[m_BufferCount];
			return OggPlayerCommandReturn.Success;
		}
		
		/// <summary>
		/// Dispose of the player safely
		/// </summary>
		public override void Dispose ()
		{
			this.Stop();
			AL.DeleteBuffers(m_Buffers);
			if (!DestroySource()) { throw new OggPlayerSourceException("Source destruction failed"); }
			if (m_Context!=null) { m_Context.Dispose(); m_Context = null; }
			if (m_CurrentFile!=null) { m_CurrentFile.Dispose(); m_CurrentFile = null; }
		}
		
		/// <summary>
		/// Destructor
		/// </summary>
		~OggPlayerFBN()
		{
			// Tidy up the OpenAL stuff
			AL.DeleteBuffers(m_Buffers);
			AL.DeleteSource(ref m_Source);
			if (m_Context!=null) { m_Context.Dispose(); m_Context = null; }
			if (m_CurrentFile!=null) { m_CurrentFile.Dispose(); m_CurrentFile = null; }
		}
				
		public override bool  SetCurrentFile(OggFile NewFile)
		{
			// Check current state
			if (!((m_PlayerState==OggPlayerStatus.Stopped)||(m_PlayerState==OggPlayerStatus.Waiting))) { return false; }
			m_CurrentFile = NewFile;
			StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.UserRequest);
			return true;
		}
		
		public override bool SetCurrentFile(string NewFilename)
		{
			return SetCurrentFile(new OggFile(NewFilename));	
		}
		
		public override OggPlayerCommandReturn Play() 
		{
			// We can only play if we're stopped (this should also stop us trying to play invalid files as we'll then be 'Waiting' or 'Error' rather than stopped)
			if (m_PlayerState == OggPlayerStatus.Stopped)
			{	

				// Begin buffering
				StateChange(OggPlayerStatus.Buffering, OggPlayerStateChanger.UserRequest);
                int usedBuffers = 0;

				// Create & Populate buffers
				for (int i=0;i<m_Buffers.Length;i++)
				{
					lock (OALLocker) {
						OggBufferSegment obs = m_CurrentFile.GetBufferSegment(0);
						if (obs.ReturnValue>0)
						{
							// Create a buffer
							AL.GenBuffer(out m_Buffers[i]);
							// Fill this buffer
							AL.BufferData((int)m_Buffers[i], m_CurrentFile.Format, obs.Buffer, obs.ReturnValue, obs.RateHz);
                            usedBuffers++;
						}
                        else if (obs.ReturnValue == 0)
                        {
                            // Probably a small file and we're at the end of it
                            break;
                        }
                        else
                        {
                            throw new Exception("Read error or EOF within initial buffer segment");
                        }
					}
				}
				lock (OALLocker)
				{
					// We've filled four buffers with data, give 'em to the source
                   AL.SourceQueueBuffers(m_Source, usedBuffers, m_Buffers);
					// Start playback
					AL.SourcePlay(m_Source);
				}
				m_LastTick = 0;
				m_LastError = ALError.NoError;
				// Spawn a new player thread
				StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest);
				new Thread(new ThreadStart(PlayerThread)).Start();
				return OggPlayerCommandReturn.Success; 
			}
			// If we're paused we'll be nice to the user and automatically call the Unpause function, which they should have done in the first place
			else if (m_PlayerState == OggPlayerStatus.Paused)
			{
				Unpause();
				return OggPlayerCommandReturn.Success;
			}
			else if (m_PlayerState == OggPlayerStatus.Waiting)
			{
				return OggPlayerCommandReturn.NoFile;
			}
			else
			{
				return OggPlayerCommandReturn.InvalidCommandInThisPlayerState;
			}
		}
		
		// Player thread
		private void PlayerThread()
		{
			bool Running = true; bool ReachedEOF = false; bool UnderRun = false;
			
			while (Running)
			{
				// See what we're doing
				if (m_PlayerState==OggPlayerStatus.Playing)
				{
					// Check number of buffers
					int QueuedBuffers = 0;
					AL.GetSource(m_Source, ALGetSourcei.BuffersQueued, out QueuedBuffers);
					// EOF stuff
					if (ReachedEOF)
					{
						// We've come to the end of the file, just see if there are any buffers left in the queue
						if (QueuedBuffers>0) 
						{
							// We want to remove the buffers, so carry on to the usual playing section
						}
						else
						{
							lock (OALLocker)
							{
								// End of file & all buffers played, exit.
								Running = false;
								// Stop the output device if it isn't already
								if (AL.GetSourceState(m_Source)!=ALSourceState.Stopped) { AL.SourceStop(m_Source); }
								m_CurrentFile.ResetFile();	// Reset file's internal pointer
								// De-allocate all buffers
								for(int i = 0; i<m_Buffers.Length; i++)
								{
									AL.DeleteBuffer(ref m_Buffers[i]);	
								}
								m_Buffers = new uint[m_BufferCount];
							}
							// Set state stuff & return
							StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.EndOfFile);
							SendMessage(OggPlayerMessageType.PlaybackEndOfFile);
							return;
						}
					}
					
					// If the number of buffers is greater than 0 & the source isn't playing, poke it so it does
					if ((!ReachedEOF)&&(QueuedBuffers>0)&&(AL.GetError()==ALError.NoError))
					{
						if (AL.GetSourceState(m_Source) != ALSourceState.Playing)
						{
							AL.SourcePlay(m_Source);
						}
					}
					
					// Check for buffer underrun
					int ProcessedBuffers = 0; uint BufferRef=0;
					lock (OALLocker)
					{
						AL.GetSource(m_Source, ALGetSourcei.BuffersProcessed, out ProcessedBuffers);	
					}				
					if (ProcessedBuffers>=m_BufferCount)
					{
						UnderRun = true;
						SendMessage(OggPlayerMessageType.BufferUnderrun);
					} else { UnderRun = false; }
					
					// Unbuffer any processed buffers
					while (ProcessedBuffers>0)
					{
						OggBufferSegment obs;
						lock (OALLocker)
						{
							// For each buffer thats been processed, reload and queue a new one
							AL.SourceUnqueueBuffers(m_Source, 1, ref BufferRef); 
							#if (DEBUG)
							if (AL.GetError()!=ALError.NoError) { Console.WriteLine("SourceUnqueueBuffers: ALError: " + OggUtilities.GetEnumString(AL.GetError())); }
							#endif
							if (ReachedEOF) { --ProcessedBuffers; continue; }	// If we're at the EOF loop to the next buffer here - we don't want to be trying to fill any more
							obs = m_CurrentFile.GetBufferSegment(m_BufferSize);	// Get chunk of tasty buffer data with the configured segment
						}
						// Check the buffer segment for errors
						if (obs.ReturnValue>0)
						{
							lock (OALLocker)
							{
								// No error, queue data
								AL.BufferData((int)BufferRef, m_CurrentFile.Format, obs.Buffer, obs.ReturnValue, obs.RateHz);
								#if (DEBUG)
								if (AL.GetError()!=ALError.NoError) { Console.WriteLine("BufferData: ALError: " + OggUtilities.GetEnumString(AL.GetError())); }
								#endif
								AL.SourceQueueBuffers(m_Source, 1, ref BufferRef);
								#if (DEBUG)
								if (AL.GetError()!=ALError.NoError) { Console.WriteLine("SourceQueueBuffers: ALError: " + OggUtilities.GetEnumString(AL.GetError())); }
								#endif
							}
						}
						else
						{
							if (obs.ReturnValue==0)
							{
								// End of file
								SendMessage(OggPlayerMessageType.BufferEndOfFile);
								ReachedEOF = true;
								break;
							}
							else
							{
								// Something went wrong with the read
								lock (OALLocker)
								{
									m_PlayerState = OggPlayerStatus.Error;
									AL.SourceStop(m_Source);
									Running = false;
								}
								SendMessage(OggPlayerMessageType.FileReadError);
								break;
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
							break;
						}
						
						--ProcessedBuffers;
					}
						
					// If we under-ran, restart the player
					if (UnderRun) { lock (OALLocker) { AL.SourcePlay(m_Source); } }
					
					// Do stuff with the time values & tick event
					m_PlayingOffset = m_CurrentFile.GetTime();
					if (m_TickEnabled)
					{
						if (m_PlayingOffset>=m_LastTick+m_TickInterval)
						{
							m_LastTick = m_PlayingOffset;
							SendTick(m_PlayingOffset, m_PlayingOffset);
						}
					}
				}
				else if (m_PlayerState==OggPlayerStatus.Seeking)
				{
					// Just wait for us to finish seeking
				}
				else if (m_PlayerState==OggPlayerStatus.Paused)
				{
					// Just wait for us to un-pause
				}
				else 
				{
					// Some other state, abort the playback 'cos we shouldn't
					// be in the PlayerThread in this case
					Running = false;
				}
				// Allow other shizzle to execute
				if (m_UpdateDelay>0) { Thread.Sleep(m_UpdateDelay); }
			}
		}
		
		public override OggPlayerCommandReturn Stop()
		{
			if (!((m_PlayerState == OggPlayerStatus.Paused)||(m_PlayerState == OggPlayerStatus.Playing))) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			// Stop the source and set the state to stop
			lock (OALLocker)
			{
				// Stop playing
				AL.SourceStop(m_Source);
				// See how many buffers are queued, and unqueue them
				int nBuffers;
				AL.GetSource(m_Source, ALGetSourcei.BuffersQueued, out nBuffers); 
				if (nBuffers>0) { AL.SourceUnqueueBuffers((int)m_Source,nBuffers); }
				// Reset the file object's internal location etc.
				m_CurrentFile.ResetFile();	
				// De-allocate all buffers
				for(int i = 0; i<m_Buffers.Length; i++)
				{
					AL.DeleteBuffer(ref m_Buffers[i]);	
				}
				m_Buffers = new uint[m_BufferCount];
			}
			m_LastTick = 0;
			// Set the new state
			StateChange(OggPlayerStatus.Stopped, OggPlayerStateChanger.UserRequest);
			return OggPlayerCommandReturn.Success;
		}
		
		public override OggPlayerCommandReturn Pause()
		{
			if (!(m_PlayerState == OggPlayerStatus.Playing)) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			lock (OALLocker)
			{
				AL.SourcePause(m_Source);
			}
			StateChange(OggPlayerStatus.Paused, OggPlayerStateChanger.UserRequest);
			return OggPlayerCommandReturn.Success;
			
		}
		
		public override OggPlayerCommandReturn Unpause()
		{
			if (!(m_PlayerState == OggPlayerStatus.Paused)) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			lock (OALLocker)
			{
				AL.SourcePlay(m_Source);
			}
			StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest);
			return OggPlayerCommandReturn.Success;
		}
		
		public override OggPlayerCommandReturn Seek(float RequestedTime)
		{
			if (!((m_PlayerState == OggPlayerStatus.Playing)||(m_PlayerState == OggPlayerStatus.Playing))) { return OggPlayerCommandReturn.InvalidCommandInThisPlayerState; }
			OggPlayerCommandReturn retVal = OggPlayerCommandReturn.Error;
			StateChange(OggPlayerStatus.Seeking, OggPlayerStateChanger.UserRequest);
			lock (OALLocker)
			{
				AL.SourcePause(m_Source);
				retVal = m_CurrentFile.SeekToTime(RequestedTime);
				AL.SourcePlay(m_Source);
			}
			m_LastTick = RequestedTime - m_TickInterval;
			StateChange(OggPlayerStatus.Playing, OggPlayerStateChanger.UserRequest);
			return retVal;
		}
	}
	
}
