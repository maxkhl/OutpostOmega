// 
//  OggPlayer.cs
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

namespace DragonOgg.MediaPlayer
{


	/// <summary>
	/// Abstraction for all OggPlayers to ensure standardisation of player formats
	/// </summary>
	public abstract class OggPlayer : IDisposable
	{
		
		#region "Current File"
		protected OggFile m_CurrentFile;				// Currently active file
		
		/// <summary>
		/// OggFile object representing the file currently loaded into the player
		/// </summary>
		public OggFile CurrentFile { get { return m_CurrentFile; } }
		
		/// <summary>
		/// Set the current file. Only valid when the player is stopped or no file has been set
		/// </summary>
		/// <param name="NewFile">
		/// An <see cref="OggFile"/> object containg the file to set
		/// </param>
		public abstract bool SetCurrentFile(string FileName);
		/// <summary>
		/// Set the current file. Only valid when the player is stopped or no file has been set
		/// </summary>
		/// <param name="NewFilename">
		/// A <see cref="System.String"/> containing the path to the file to set
		/// </param>
		public abstract bool SetCurrentFile(OggFile File);
		#endregion

		#region "State Event"
		protected OggPlayerStatus m_PlayerState; 		// Player state
		
		/// <summary>
		/// Raised when the state of the player changes
		/// </summary>
		public event OggPlayerStateChangedHandler StateChanged;
		
		/// <summary>
		/// Current state of the player as an OggPlayerStatus enumeration. 
		/// Use OggUtilities.GetEnumString to convert into human-readable information
		/// </summary>
		public OggPlayerStatus PlayerState { get { return m_PlayerState; } }
		
		protected void StateChange(OggPlayerStatus NewState) { StateChange(NewState, OggPlayerStateChanger.Internal); }
		protected void StateChange(OggPlayerStatus NewState, OggPlayerStateChanger Reason)
		{
			if (StateChanged!=null) { StateChanged(this, new OggPlayerStateChangedArgs(m_PlayerState, NewState, Reason)); }
			#if (DEBUG)
				Console.WriteLine(DateTime.Now.ToLongTimeString() + "\tOggPlayer::StateChange -- From: " + OggUtilities.GetEnumString(m_PlayerState) + " -- To: " + OggUtilities.GetEnumString(NewState));
			#endif
			m_PlayerState = NewState;
		}
		#endregion		
		
		#region "Tick Event"
		protected bool m_TickEnabled;				// Tick control flag
		protected float m_TickInterval = 1f;		// Interval between tick events
		protected float m_LastTick;				// Last tick
		
		/// <summary>
		/// OggPlayer.Tick events will be raised every OggPlayer.TickInterval seconds of audio output if this is true
		/// </summary>
		public bool TickEnabled { get { return m_TickEnabled; } set { m_TickEnabled = value; } }
		/// <summary>
		/// Seconds between OggPlayer.Tick events (when OggPlayer.TickEnabled is true)
		/// </summary>
		public float TickInterval { get { return m_TickInterval; } set { m_TickInterval = value; } }
		
		/// <summary>
		/// Raised every TickInterval seconds of playvack when TickEnabled is true
		/// </summary>
		public event OggPlayerTickHandler Tick;
		
		protected void SendTick(float PlaybackTime, float BufferTime)
		{
			if (Tick!=null) { Tick(this, new OggPlayerTickArgs(PlaybackTime, BufferTime)); }
		}
		                        
		#endregion		
		
		#region "Message Event"
		/// <summary>
		/// Raised when the player sends a message (e.g. a buffer under-run or on reaching the end of a file)
		/// </summary>
		public event OggPlayerMessageHandler PlayerMessage;	
		
		protected void SendMessage(OggPlayerMessageType Message) { SendMessage(Message, null); }
		protected void SendMessage(OggPlayerMessageType Message, object Params)
		{
			if (PlayerMessage!=null) { PlayerMessage(this, new OggPlayerMessageArgs(Message, Params)); }
			#if (DEBUG)
				Console.WriteLine(DateTime.Now.ToLongTimeString() + "\tOggPlayer::SendMessage -- Message: " + OggUtilities.GetEnumString(Message));
			#endif
		}
		#endregion

        #region "Deprecated Playback Control"
        /// <summary>
        /// Start playing the current file
        /// </summary>
        /// <returns>
        /// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
        /// </returns>
        [Obsolete("Method has been replaced with Play()", false)]
        public OggPlayerCommandReturn Playback_Play()
        {
            return Play();
        }

        /// <summary>
        /// Stop playback. 
        /// Only valid if the player is playing or paused
        /// </summary>
        /// <returns>
        /// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
        /// </returns>
        [Obsolete("Method has been replaced with Stop()", false)]
        public OggPlayerCommandReturn Playback_Stop()
        {
            return Stop();
        }

        /// <summary>
        /// Pause playback
        /// Only valid if the player is playing
        /// </summary>
        /// <returns>
        /// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
        /// </returns>
        [Obsolete("Method has been replaced with Pause()", false)]
        public OggPlayerCommandReturn Playback_Pause()
        {
            return Pause();
        }

        /// <summary>
        /// Unpause playback
        /// Only valid if the player is paused
        /// </summary>
        /// <returns>
        /// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
        /// </returns>
        [Obsolete("Method has been replaced with Unpause()", false)]
        public OggPlayerCommandReturn Playback_UnPause()
        {
            return Unpause();
        }

        /// <summary>
        /// Seek to a time
        /// Only valid if the player is playing or paused
        /// </summary>
        /// <param name="RequestedTime">
        /// A <see cref="System.Single"/> indicating the position in seconds within the file to seek to
        /// </param>
        /// <returns>
        /// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
        /// </returns>
        [Obsolete("Method has been replaced with Seek()", false)]
        public OggPlayerCommandReturn Playback_Seek(float RequestedTime)
        {
            return Seek(RequestedTime);
        }

        #endregion

        #region "Playback Control"
        /// <summary>
		/// Start playing the current file
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public abstract OggPlayerCommandReturn Play();
		/// <summary>
		/// Stop playback. 
		/// Only valid if the player is playing or paused
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public abstract OggPlayerCommandReturn Stop();
		/// <summary>
		/// Pause playback
		/// Only valid if the player is playing
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public abstract OggPlayerCommandReturn Pause();
		/// <summary>
		/// Unpause playback
		/// Only valid if the player is paused
		/// </summary>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public abstract OggPlayerCommandReturn Unpause();
				/// <summary>
		/// Seek to a time
		/// Only valid if the player is playing or paused
		/// </summary>
		/// <param name="RequestedTime">
		/// A <see cref="System.Single"/> indicating the position in seconds within the file to seek to
		/// </param>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public abstract OggPlayerCommandReturn Seek(float RequestedTime);
		#endregion
		
		#region "Timing"
		protected float m_PlayingOffset;				// Current time in playback		
		/// <summary>
		/// The current time of playback within the file
		/// </summary>
		public float AmountPlayed { get { return m_PlayingOffset; } }
		/// <summary>
		/// How much of the file has been played as a fraction of it's total (always returns between 0 & 1)
		/// </summary>
		public float FractionPlayed { 
			get {
				if (m_CurrentFile==null) { return 0; }
				float FE = m_PlayingOffset/float.Parse(m_CurrentFile.GetQuickTag(OggTags.Length)); 
				if (FE>1) { return 1; } else if (FE<0) { return 0; } else { return FE; } 
			} 
		}
		
		protected float m_BufferOffset;				// Current time in buffer
		/// <summary>
		/// The current time of buffer within the file
		/// </summary>
		public float AmountBuffered { get { return m_BufferOffset; } }
		/// <summary>
		/// How much of the file has been buffered as a fraction of it's total (always returns between 0 & 1)
		/// </summary>
		public float FractionBuffered { 
			get { 
				if (m_CurrentFile==null) { return 0; }
				float FE = m_BufferOffset/float.Parse(m_CurrentFile.GetQuickTag(OggTags.Length)); 
				if (FE>1) { return 1; } else if (FE<0) { return 0; } else { return FE; } 
			} 
		}

		/// <summary>
		/// The length of the file in seconds
		/// </summary>
		public float FileLengthTime { get { if (m_CurrentFile==null) { return -1; } else { return float.Parse(m_CurrentFile.GetQuickTag(OggTags.Length)); } } }
		#endregion
		
		#region "OpenAL"
		protected AudioContext m_Context;				// Audio device context
		protected uint m_Source;						// Output source handle
		protected ALError m_LastError;				// OpenAL Error
		
		/// <summary>
		/// The last error from the OpenAL subsystem as an ALError enumeration. 
		/// Use OggUtilities.GetEnumString to convert into human readable information
		/// </summary>
		public ALError LastALError { get { return m_LastError; } }
		
		protected bool InitSource()
		{
			try 
			{
				// Create source
				AL.GenSource(out m_Source);
			
				// Configure the source listener
				AL.Source(m_Source, ALSource3f.Position, 0.0f, 0.0f, 0.0f);
				AL.Source(m_Source, ALSource3f.Velocity, 0.0f, 0.0f, 0.0f);
				AL.Source(m_Source, ALSource3f.Direction, 0.0f, 0.0f, 0.0f);
				AL.Source(m_Source, ALSourcef.RolloffFactor, 0.0f);
				AL.Source(m_Source, ALSourceb.SourceRelative, true);	
				return true;
			}
			catch (Exception ex)
			{
				return false;	
			}
		}
		
		protected bool DestroySource()
		{
			try
			{
				if ((AL.GetSourceState(m_Source)==ALSourceState.Paused)||(AL.GetSourceState(m_Source)==ALSourceState.Playing))
				{
					AL.SourceStop(m_Source);	
				}
				AL.DeleteSource(ref m_Source);
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
				
		}
		#endregion		
		
		#region "Threading"
		protected static readonly object StateLocker = new object();
		protected static object OALLocker = new object();
		
	
		protected int m_UpdateDelay;			// Amount of time to wait at the end of each thread loop to allow other stuff to execute
		/// <summary>
		/// The amount of time to wait after each pass
		/// </summary>
		public int UpdateDelay { get { return m_UpdateDelay; } set { m_UpdateDelay = value; } }
		#endregion
		
		/// <summary>
		/// IDisposable implementation
		/// </summary>
		public abstract void Dispose();
			
	}
	
	#region "Events"
	
	/// <summary>
	/// Event handler for changes in OggPlayer state
	/// </summary>
	public delegate void OggPlayerStateChangedHandler(object sender, OggPlayerStateChangedArgs e);
	
	/// <summary>
	/// Event arguments for OggPlayer StateChanged events
	/// </summary>
	public class OggPlayerStateChangedArgs : EventArgs
	{
		private OggPlayerStatus m_OldState; 
		private OggPlayerStatus m_NewState;
		private OggPlayerStateChanger m_Changer;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="eOldState">
		/// Original state of the player as an <see cref="OggPlayerStatus"/> enumeration
		/// </param>
		/// <param name="eNewState">
		/// New state of the player as an <see cref="OggPlayerStatus"/> enumeration
		/// </param>
		/// <param name="eChanger">
		/// Reason for the change in state as an <see cref="OggPlayerStateChanger"/> enumeration
		/// </param>
		public OggPlayerStateChangedArgs(OggPlayerStatus eOldState, OggPlayerStatus eNewState, OggPlayerStateChanger eChanger)
		{
			m_OldState = eOldState; m_NewState = eNewState; m_Changer = eChanger;	
		}
		
		/// <summary>
		/// Original state
		/// </summary>
		public OggPlayerStatus OldState { get { return m_OldState; } }
		/// <summary>
		/// New state
		/// </summary>
		public OggPlayerStatus NewState { get { return m_NewState; } }
		/// <summary>
		/// Reason for the change in state
		/// </summary>
		public OggPlayerStateChanger Changer { get { return m_Changer; } }
	}
	
	/// <summary>
	/// Event handler for messages from an OggPlayer
	/// </summary>
	public delegate void OggPlayerMessageHandler(object sender, OggPlayerMessageArgs e);
	
	/// <summary>
	/// Event arguments for OggPlayer Message events
	/// </summary>
	public class OggPlayerMessageArgs : EventArgs
	{
		private OggPlayerMessageType m_Message;
		private object m_Params;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="eMessage">
		/// Type of message as an <see cref="OggPlayerMessageType"/> enumerator
		/// </param>
		public OggPlayerMessageArgs(OggPlayerMessageType eMessage)
		{
			m_Message = eMessage; m_Params = null;	
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="eMessage">
		/// Type of message as an <see cref="OggPlayerMessageType"/> enumerator
		/// </param>
		/// <param name="eParams">
		/// Message parameter(s). Type depends on the type of message
		/// </param>
		public OggPlayerMessageArgs(OggPlayerMessageType eMessage, object eParams)
		{
			m_Message = eMessage; m_Params = eParams;
		}
		
		/// <summary>
		/// Type of message sent
		/// </summary>
		public OggPlayerMessageType Message { get { return m_Message; } }
		/// <summary>
		/// Parameter(s) for this message. Content depends on the type of message
		/// </summary>
		public object Params { get { return m_Params; } }
	}
	
	/// <summary>
	/// Event handler for player tick events
	/// </summary>
	public delegate void OggPlayerTickHandler(object sender, OggPlayerTickArgs e);
	
	/// <summary>
	/// Event arguments for OggPlayer Tick events
	/// </summary>
	public class OggPlayerTickArgs : EventArgs
	{
		private float m_PlaybackTime;
		private float m_BufferedTime;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ePlaybackTime">
		/// Current position in seconds of the audio output process as a <see cref="System.Single"/>
		/// </param>
		/// <param name="eBufferedTime">
		/// Current position in seconds of the buffer process as a <see cref="System.Single"/>
		/// </param>
		public OggPlayerTickArgs(float ePlaybackTime, float eBufferedTime)
		{
			m_PlaybackTime = ePlaybackTime; m_BufferedTime = eBufferedTime;
		}
		
		/// <summary>
		/// Current position in seconds of the audio output process
		/// </summary>
		public float PlaybackTime { get { return m_PlaybackTime; } }
		
		/// <summary>
		/// Current position in seconds of the buffer process
		/// </summary>
		public float BufferedTime { get { return m_BufferedTime; } }
	}
	#endregion
	
	#region "Exceptions"
	
	/// <summary>
	/// Exception raised when there is an issue with interactions with the OpenAL source.
	/// LastALError may have more information
	/// </summary>
	public class OggPlayerSourceException : Exception
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Msg">
		/// The message of the exception as a <see cref="System.String"/>
		/// </param>
		public OggPlayerSourceException(string Msg) : base(Msg) { }
	}
	
	#endregion
}
