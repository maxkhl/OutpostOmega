//
//  AudioChannel.cs
//  
//  Author:
//      Caleb Leak (04.05.2011)
//      caleb@embergames.net
//      www.EmberGames.net
//
// Based on OggPlayerFBN.cs by Matthew Harris, (c) 2011.
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
using System.Text;

using csogg;
using csvorbis;

using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace DragonOgg.Interactive
{
    /// <summary>
    /// An object for buffering and playing data on a single channel. 
    /// </summary>
    public class AudioChannel : IDisposable
    {
        public byte[] SegmentBuffer { get; private set; }
        public int[] Buffers { get; private set; }
        public int BufferCount { get; private set; }
        public int BufferSize { get; private set; }
        public int Source { get; private set; }
        public VorbisFileInstance CurrentClip { get; private set; }

        public ALFormat CurrentFormat { get; private set; }
        public int CurrentRate { get; private set; }

        private bool eof;

        public bool IsFree
        {
            get
            {
                return CurrentClip == null;
            }
        }

        private const int _BIGENDIANREADMODE = 0;		// Big Endian config for read operation: 0=LSB;1=MSB
        private const int _WORDREADMODE = 2;			// Word config for read operation: 1=Byte;2=16-bit Short
        private const int _SGNEDREADMODE = 1;			// Signed/Unsigned indicator for read operation: 0=Unsigned;1=Signed

        /// <summary>
        /// Constructs an empty channel, ready to play sound.
        /// </summary>
        /// <param name="bufferCount">The number of audio buffers to size.</param>
        /// <param name="bufferSize">The size, in bytes, of each audio buffer.</param>
        public AudioChannel(int bufferCount, int bufferSize)
        {
            Buffers = new int[bufferCount];
            BufferCount = bufferCount;
            BufferSize = bufferSize;
            CurrentClip = null;

            SegmentBuffer = new byte[bufferSize];

            // Make the source
            Source = AL.GenSource();

            // Make the buffers
            for(int i = 0; i < BufferCount; i++)
                Buffers[i] = AL.GenBuffer();
        }

        /// <summary>
        /// Deconstructs the channel, freeing its hardware resources.
        /// </summary>
        ~AudioChannel()
        {
            Dispose(); 
        }

        /// <summary>
        /// Disposes the channel, freeing its hardware resources.
        /// </summary>
        public void Dispose()
        {
            AL.SourceStop(Source);
            if(Buffers != null)
                AL.DeleteBuffers(Buffers);

            Buffers = null;
            CurrentClip = null;
        }

        /// <summary>
        /// Determines the format of the given audio clip.
        /// </summary>
        /// <param name="clip">The clip to determine the format of.</param>
        /// <returns>The audio format.</returns>
        public static ALFormat DetermineFormat(VorbisFileInstance clip)
        { 
            // TODO: Should probably do more than just check the format of the first stream.
            Info[] clipInfo = clip.vorbisFile.getInfo();

            if (clipInfo.Length < 1 || clipInfo[0] == null)
                throw new ArgumentException("Audio clip does not have track information");

            Info info = clipInfo[0];

            // The number of channels is determined by the clip.  The bit depth
            // however is the choice of the player.  If desired, 8-bit audio
            // could be supported here.
            if (info.channels == 1)
            {
                return ALFormat.Mono16;
            }
            else if (info.channels == 2)
            {
                return ALFormat.Stereo16;
            }
            else
            {
                throw new NotImplementedException("Only mono and stereo are implemented.  Audio has too many channels.");
            }
        }

        /// <summary>
        /// Determines the rate of the given audio clip.
        /// </summary>
        /// <param name="clip">The clip to determine the rate of.</param>
        /// <returns>The audio rate.</returns>
        public int DetermineRate(VorbisFileInstance clip)
        {
            // TODO: Should probably do more than just check the format of the first stream.
            Info[] clipInfo = clip.vorbisFile.getInfo();

            if (clipInfo.Length < 1 || clipInfo[0] == null)
                throw new ArgumentException("Audio clip does not have track information");

            Info info = clipInfo[0];

            return info.rate;
        }

        /// <summary>
        /// Begins playing the given clip.
        /// </summary>
        /// <param name="file">The clip to play.</param>
        public void Play(VorbisFileInstance clip)
        {
            DequeuUsedBuffers();

            CurrentFormat = DetermineFormat(clip);
            CurrentRate = DetermineRate(clip);

            CurrentClip = clip;
            eof = false;

            // Buffer initial audio
            int usedBuffers = 0;
            for (int i = 0; i < BufferCount; i++)
            {
                int bytesRead = clip.read(SegmentBuffer, SegmentBuffer.Length,
                    _BIGENDIANREADMODE, _WORDREADMODE, _SGNEDREADMODE, null);

                if (bytesRead > 0)
                {
                    // Buffer the segment
                    AL.BufferData(Buffers[i], CurrentFormat, SegmentBuffer, bytesRead,
                        CurrentRate);

                    usedBuffers++;
                }
                else if (bytesRead == 0)
                {
                    // Clip is too small to fill the initial buffer, so stop
                    // buffering.
                    break;
                }
                else
                {
                    // TODO: There was an error reading the file
                    throw new System.IO.IOException("Error reading or processing OGG file");
                }
            }

            // Start playing the clip
            AL.SourceQueueBuffers(Source, usedBuffers, Buffers);
            AL.SourcePlay(Source);
        }

        /// <summary>
        /// Removes all empty buffers from the audio queue.
        /// </summary>
        protected void DequeuUsedBuffers()
        {
            int processedBuffers;
            AL.GetSource(Source, ALGetSourcei.BuffersProcessed, out processedBuffers);

            int[] removedBuffers = new int[processedBuffers];
            AL.SourceUnqueueBuffers(Source, processedBuffers, removedBuffers);
        }

        /// <summary>
        /// Updates the channel, buffer addition audio if needed.  This method
        /// needs to be called frequently to maintain real-time performance.
        /// </summary>
        public void Update()
        {
            if (CurrentClip != null)
            {
                int buffersQueued;
                AL.GetSource(Source, ALGetSourcei.BuffersQueued, out buffersQueued);

                int processedBuffers;
                AL.GetSource(Source, ALGetSourcei.BuffersProcessed, out processedBuffers);

                if (eof)
                {
                    // Clip is done being buffered
                    if (buffersQueued <= processedBuffers)
                    {
                        // Clip has finished
                        AL.SourceStop(Source);
                        CurrentClip = null;

                        DequeuUsedBuffers();

                        return;
                    }
                }
                else
                {
                    // Still some buffering to do
                    if (buffersQueued - processedBuffers > 0 && AL.GetError() == ALError.NoError)
                    {
                        // Make sure we're playing (not sure why we would've stopped)
                        if (AL.GetSourceState(Source) != ALSourceState.Playing)
                        {
                            AL.SourcePlay(Source);
                        }
                    }

                    // Detect buffer under-runs
                    bool underRun = (processedBuffers >= BufferCount);

                    // Remove processed buffers
                    while (processedBuffers > 0)
                    {
                        int removedBuffer = 0;

                        // TODO: Can remove more than one buffer here.  Can also
                        // add the buffers back to the queue.
                        AL.SourceUnqueueBuffers(Source, 1, ref removedBuffer);

                        // Just remove the buffer and don't do anything else if
                        // we're at the end of the clip.
                        if (eof)
                        {
                            processedBuffers--;
                            continue;
                        }

                        // Buffer the next chunk
                        int bytesRead = CurrentClip.read(SegmentBuffer, SegmentBuffer.Length,
                            _BIGENDIANREADMODE, _WORDREADMODE, _SGNEDREADMODE, null);

                        if (bytesRead > 0)
                        {
                            // TOOD: Queue multiple buffers here
                            AL.BufferData(removedBuffer, CurrentFormat, SegmentBuffer, bytesRead,
                                CurrentRate);
                            AL.SourceQueueBuffer(Source, removedBuffer);
                        }
                        else if (bytesRead == 0)
                        {
                            // Reached the end of the file
                            eof = true;
                        }
                        else
                        {
                            // A file read error has occurred, stop playing
                            AL.SourceStop(Source);
                            CurrentClip = null;
                            break;
                        }

                        // Check for OpenAL errors
                        if (AL.GetError() != ALError.NoError)
                        {
                            AL.SourceStop(Source);
                            CurrentClip = null;
                            break;
                        }

                        processedBuffers--;
                    }
                }
            }
        }
    }
}
