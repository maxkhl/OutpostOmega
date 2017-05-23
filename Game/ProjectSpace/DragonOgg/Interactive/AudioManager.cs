//
//  AudioManager.cs
//  
//  Author:
//      Caleb Leak (04.05.2011)
//      caleb@embergames.net
//      www.EmberGames.net
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

using csogg;
using csvorbis;

namespace DragonOgg.Interactive
{
    /// <summary>
    /// A manager for audio clips and audio channels.  Puts audio clips into
    /// empty channels when they're played.
    /// </summary>
    public class AudioManager : IDisposable
    {
        public int ChannelCount { get; private set; }
        public int BuffersPerChannel { get; private set; }
        public int BytesPerBuffer { get; private set; }

        public AudioChannel[] Channels { get; private set; }

        public Thread UpdateThread { get; private set; }

        public bool RunUpdates { get; set; }

        private static AudioManager manager = null;

        /// <summary>
        /// The sole instance of the audio manager.
        /// </summary>
        public static AudioManager Manager
        {
            get
            {
                if(manager == null)
                    manager = new AudioManager();

                return manager;
            }

            set
            {
                manager = value;
            }
        }

        /// <summary>
        /// Disposes of all resources used by the audio manager.
        /// </summary>
        ~AudioManager()
        {
            Dispose();
        }

        /// <summary>
        /// Constructs a default audio manager with 16 channels.
        /// </summary>
        public AudioManager()
        {
            Init(16, 4 * 8, 4096, true);
        }

        /// <summary>
        /// Initializes the audio manager.
        /// </summary>
        /// <param name="channels">The number of channels to use.</param>
        /// <param name="buffersPerChannel">The number of buffers each channel will contain.</param>
        /// <param name="bytesPerBuffer">The number of bytes in each buffer.</param>
        /// <param name="launchThread">If true, a separate thread will be launched to handle updating the sound manager.  
        ///                            Otherwise, a thread will not be launched and manual calls to Update() will be required.</param>
        public AudioManager(int channels, int buffersPerChannel, int bytesPerBuffer, bool launchThread)
        {
            Init(channels, buffersPerChannel, bytesPerBuffer, launchThread);
        }

        /// <summary>
        /// Initializes the audio manager.
        /// </summary>
        /// <param name="channels">The number of channels to use.</param>
        /// <param name="buffersPerChannel">The number of buffers each channel will contain.</param>
        /// <param name="bytesPerBuffer">The number of bytes in each buffer.</param>
        private void Init(int channels, int buffersPerChannel, int bytesPerBuffer, bool launchThread)
        {
            RunUpdates = launchThread;
            ChannelCount = channels;
            Channels = new AudioChannel[channels];

            for (int i = 0; i < channels; i++)
                Channels[i] = new AudioChannel(buffersPerChannel, bytesPerBuffer);

            Manager = this;
            
            if(launchThread)
            {
                UpdateThread = new Thread(RunUpdateLoop);
                UpdateThread.IsBackground = true;
                UpdateThread.Start();
            }
            else
            {
                UpdateThread = null;
            }
        }

        /// <summary>
        /// Plays the audio clip on the first free channel.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        public void PlayClip(VorbisFileInstance clip)
        {
            // TODO: If all channels are busy, the clip will be ignored.  There must be a more elegant way.
            foreach (AudioChannel channel in Channels)
            {
                try
                {
                    if (channel.IsFree)
                    {
                        channel.Play(clip);
                        return;
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    Debug.Print(e.StackTrace);
#endif
                }
            }
        }

        /// <summary>
        /// Performs a single update by updating all channels.
        /// </summary>
        public void Update()
        {
            foreach (AudioChannel channel in Channels)
            {
                lock (this)
                {
                    channel.Update();
                }
            }
        }

        /// <summary>
        /// Continuously updates the audio manager.  This method will not return
        /// unless it's interrupted, so it's best to run it in a separate 
        /// thread.
        /// </summary>
        public void RunUpdateLoop()
        {
            while (RunUpdates)
            {
                Update();
                // TODO: Is 1ms long enough to still have good performance outside
                // of the audio?
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Dispose of the audio manager and frees its audio memory.
        /// </summary>
        public void Dispose()
        {
            try
            {
                RunUpdates = false;
                UpdateThread.Join();
            }
            catch (Exception e)
            { }

            try
            {
                foreach (AudioChannel channel in Channels)
                {
                    try
                    {
                        channel.Dispose();
                    }
                    catch (Exception e1)
                    { }
                }
            }
            catch (Exception e2)
            { }
        }
    }
}
