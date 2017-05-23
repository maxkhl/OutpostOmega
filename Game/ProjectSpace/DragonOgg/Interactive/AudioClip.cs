//
//  AudioClip.cs
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
using System.IO;
using System.Text;

using csogg;
using csvorbis;

namespace DragonOgg.Interactive
{
    /// <summary>
    /// A container for audio.  Represents a single piece of audio that can
    /// be repeatedly played.
    /// </summary>
    public class AudioClip
    {
        VorbisFile rawClip;

        /// <summary>
        /// Constructs an audio clip from the given file.
        /// </summary>
        /// <param name="fileName">The file which to read from.</param>
        public AudioClip(string fileName)
        {
            rawClip = new VorbisFile(fileName);
            Cache(64 * 1024);
        }

        /// <summary>
        /// Reads an audio clip from the given stream.
        /// </summary>
        /// <param name="inputStream">The stream to read from.</param>
        public AudioClip(Stream inputStream)
        {
            rawClip = new VorbisFile(inputStream);
            Cache(64 * 1024);
        }

        /// <summary>
        /// Caches the given number of bytes by reading them in and discarding
        /// them.  This is useful so that when the sound if first played,
        /// there's not a delay.
        /// </summary>
        /// <param name="bytes">Then number of PCM bytes to read.</param>
        protected void Cache(int bytes)
        {
            VorbisFileInstance instance = rawClip.makeInstance();

            int totalBytes = 0;
            byte[] buffer = new byte[4096];

            while (totalBytes < bytes)
            {
                int bytesRead = instance.read(buffer, buffer.Length, 0, 2, 1, null);

                if (bytesRead <= 0)
                    break;

                totalBytes += bytesRead;
            }
        }

        /// <summary>
        /// Plays the audio clip.
        /// </summary>
        public void Play()
        {
            lock (AudioManager.Manager)
            {
                AudioManager.Manager.PlayClip(rawClip.makeInstance());
            }
        }
    }
}
