// 
//  OggFile.cs
//  
//  Author:
//       dragon@the-dragons-nest.co.uk
// 
//  Copyright (c) 2010 Matthew Harris
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
using System.IO;
using System.Collections.Generic;
using csvorbis;
using TagLib;
using OpenTK.Audio.OpenAL;

namespace DragonOgg
{
	

	/*
	 *	OggFile Class
	 *	Combines the csvorbis, System.IO and Taglib functionality into one class
	 *	Designed for use with OggPlayer or OggPlaylist
	 */
	/// <summary>
	/// Combines the csvorbis, System.IO and TagLib functionality into one class
	/// Use for editting tags, or in conjunction with OggPlayer for audio output or OggPlaylist for playlist reading/writing
	/// </summary>
	public class OggFile : IDisposable 
	{
		
		private string m_Filename;			// Filename
		
		private VorbisFile m_CSVorbisFile; 	// CSVorbis file object
		private TagLib.File m_TagLibFile;	// TagLibSharp file object
		
		private int m_Streams;				// Number of Vorbis streams in the file
		private int m_Bitrate;				// ABR/NBR of the file
		private int m_LengthTime;			// Number of seconds in the file
		private Info[] m_Info;				// OggVorbis file info object					
		private ALFormat m_Format;			// Format of the file
		
		private const int _BIGENDIANREADMODE = 0;		// Big Endian config for read operation: 0=LSB;1=MSB
		private const int _WORDREADMODE = 1;			// Word config for read operation: 1=Byte;2=16-bit Short
		private const int _SGNEDREADMODE = 0;			// Signed/Unsigned indicator for read operation: 0=Unsigned;1=Signed
		private const int _SEGMENTLENGTH = 4096;		// Default number of segments to read if unspecified (Segment type is determined by _WORDREADMODE)
		
		/// <summary>
		/// The format of the current file in an ALFormat enumeration
		/// </summary>
		public ALFormat Format { get { return m_Format; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Filename">
		/// A <see cref="System.String"/> containing the path to the Ogg Vorbis file this instance represents
		/// </param>
		public OggFile (string Filename)
		{
			// Check that the file exists
			if (!(System.IO.File.Exists(Filename))) { throw new OggFileReadException("File not found", Filename); }
			// Load the relevant objects
			m_Filename = Filename;
			try
			{
				m_CSVorbisFile = new VorbisFile(m_Filename);
			}
			catch (Exception ex)
			{
				throw new OggFileReadException("Unable to open file for data reading\n" + ex.Message, Filename);	
			}
			try
			{
				m_TagLibFile = TagLib.File.Create(m_Filename);
			}
			catch (TagLib.UnsupportedFormatException ex)
			{
				throw new OggFileReadException("Unsupported format (not an ogg?)\n" + ex.Message, Filename);
			}
			catch (TagLib.CorruptFileException ex)
			{
				throw new OggFileCorruptException(ex.Message, Filename, "Tags");
			}
			
			// Populate some other info shizzle and do a little bit of sanity checking
			m_Streams = m_CSVorbisFile.streams();
			if (m_Streams<=0) { throw new OggFileReadException("File doesn't contain any logical bitstreams", Filename); }
			// Assuming <0 is for whole file and >=0 is for specific logical bitstreams
			m_Bitrate = m_CSVorbisFile.bitrate(-1);
			m_LengthTime = (int)m_CSVorbisFile.time_total(-1);
			// Figure out the ALFormat of the stream
			m_Info = m_CSVorbisFile.getInfo();	// Get the info of the first stream, assuming all streams are the same? Dunno if this is safe tbh
			if (m_Info[0] == null) { throw new OggFileReadException("Unable to determine Format{FileInfo.Channels} for first bitstream", Filename); }
			if (m_TagLibFile.Properties.AudioBitrate==16) {
				m_Format = (m_Info[0].channels)==1 ? ALFormat.Mono16 : ALFormat.Stereo16; // This looks like a fudge, but I've seen it a couple of times (what about the other formats I wonder?)
			}
			else 
			{
				m_Format = (m_Info[0].channels)==1 ? ALFormat.Mono8 : ALFormat.Stereo8;
			}
		}		
		
		/// <summary>
		/// Retrieve the value of a Tag from the Ogg Vorbis file
		/// </summary>
		/// <param name="TagID">
		/// A <see cref="OggTags"/> indicating which tag to read
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/> containing the data in the tag
		/// </returns>
		public string GetQuickTag(OggTags TagID)
		{
			switch (TagID)
			{
			case OggTags.Title: return m_TagLibFile.Tag.Title;
			case OggTags.Artist: return m_TagLibFile.Tag.FirstPerformer;
			case OggTags.Album: return m_TagLibFile.Tag.Album;
			case OggTags.Genre: return m_TagLibFile.Tag.FirstGenre;
			case OggTags.TrackNumber: return m_TagLibFile.Tag.Track.ToString();
			case OggTags.Filename: return m_Filename;
			case OggTags.Bitrate: return m_Bitrate.ToString();
			case OggTags.Length: return m_LengthTime.ToString();
			default: return null;
			}
			
		}
		
		/// <summary>
		/// Set the value of a tag in the Ogg Vorbis file (THIS FUNCTION WRITES TO DISK)
		/// </summary>
		/// <param name="TagID">
		/// A <see cref="OggTags"/> indicating which tag to change
		/// </param>
		/// <param name="Value">
		/// A <see cref="System.String"/> containing the value to write
		/// </param>
		/// <returns>
		/// A <see cref="OggTagWriteCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggTagWriteCommandReturn SetQuickTag(OggTags TagID, string Value)
		{
			switch (TagID)
			{
			case OggTags.Title: m_TagLibFile.Tag.Title = Value; break;
			case OggTags.Artist: m_TagLibFile.Tag.Performers = new string[] { Value }; break;
			case OggTags.Album: m_TagLibFile.Tag.Album = Value; break;
			case OggTags.Genre: m_TagLibFile.Tag.Genres = new string[] { Value }; break;
			case OggTags.TrackNumber: m_TagLibFile.Tag.Track = uint.Parse(Value); break;
			case OggTags.Filename: return OggTagWriteCommandReturn.ReadOnlyTag;
			case OggTags.Bitrate: return OggTagWriteCommandReturn.ReadOnlyTag;
			case OggTags.Length: return OggTagWriteCommandReturn.ReadOnlyTag;
			default: return OggTagWriteCommandReturn.UnknownTag;
			}
			try { m_TagLibFile.Save(); } catch (Exception ex) { return OggTagWriteCommandReturn.Error; }
			return OggTagWriteCommandReturn.Success;
		}
		
		/// <summary>
		/// Retrieve a tag with an arbitrary name.
		/// Returns an empty tag if the value isn't found or if no Xiph tags are present.
		/// </summary>
		/// <param name="TagName">
		/// A <see cref="System.String"/> containing the name of the tag to find
		/// </param>
		/// <returns>
		/// An <see cref="OggTag"/> containing the returned tag
		/// </returns>
		public OggTag GetTag(string TagName)
		{
			if (TagName.Length<=0) { return OggUtilities.GetEmptyTag(); } // Save some processing time and just exit if we haven't been given a tag name
			// Based on tasty examples @ "Accessing Hidden Gems": http://developer.novell.com/wiki/index.php/TagLib_Sharp:_Examples
			TagLib.Ogg.XiphComment XC = (TagLib.Ogg.XiphComment) m_TagLibFile.GetTag(TagTypes.Xiph);
			if (XC != null)
			{
				string[] TagValue = XC.GetField(TagName);
				if (TagValue.Length==0)
				{
					// Tag doesn't exist, return empty
					return OggUtilities.GetEmptyTag();
				}
				else
				{
					OggTag tmpTag;
					tmpTag.Name = TagName;
					tmpTag.IsArray = (TagValue.Length>1);
					tmpTag.IsEmpty = false;
					tmpTag.Values = TagValue;
					tmpTag.Value = TagValue[0];
					return tmpTag;
				}
			} 
			else 
			{ 
				// No valid Xiph tags found
				return OggUtilities.GetEmptyTag(); 
			}
		}
		
		/// <summary>
		/// Retrieve an array of all tag values
		/// Returns a zero-length array if no Xiph tags are found
		/// </summary>
		/// <returns>
		/// An <see cref="OggTag[]"/> containing the returned values
		/// </returns>
		public OggTag[] GetTags()
		{
			TagLib.Ogg.XiphComment XC = (TagLib.Ogg.XiphComment) m_TagLibFile.GetTag(TagTypes.Xiph);
			if (XC != null)
			{
				if (XC.FieldCount>0)
				{
					OggTag[] tmpOggTag = new OggTag[XC.FieldCount];
					int Index = 0;
					foreach (string FieldName in XC) 
					{
						string[] TagValue = XC.GetField(FieldName);
						if (TagValue.Length==0)
						{
							tmpOggTag[Index] = OggUtilities.GetEmptyTag();		// This should never happen, but I bet if I don't check it it will!
						}
						else
						{	
							// Populate this tag
							tmpOggTag[Index].Name = FieldName;
							tmpOggTag[Index].IsArray = (TagValue.Length>1);
							tmpOggTag[Index].IsEmpty = false;
							tmpOggTag[Index].Values = TagValue;
							tmpOggTag[Index].Value = TagValue[0];
						}
						++Index;	// Increment the index so we know which OggTag we're molesting
					}
					// Done! Return the heap of tags
					return tmpOggTag;
				}
				else
				{
					// Xiph tags contain no items
					return new OggTag[0];
				}
			}
			else
			{
				// No valid Xiph tags found
				return new OggTag[0];
			}
		}
		
		/// <summary>
		/// Write a single Xiph tag to the file
		/// This will overwrite the tag if it already exists, or create it if it doesn't
		/// It will also create the Xiph tag block within the file if it doesn't already have one
		/// This function writes to disk. If setting multiple tags consider using SetTags(OggTag[] Tags) to reduce the number of write operations
		/// If setting an array, Tag.Values must contain at least one item. Tag.Value is ignored in this case
		/// If setting a single value, Tag.Value must contain at least one character. Tag.Values is ignored in this case
		/// </summary>
		/// <param name="Tag">
		/// The <see cref="OggTag"/> to write
		/// </param>
		/// <returns>
		/// An <see cref="OggTagWriteCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggTagWriteCommandReturn SetTag(OggTag Tag)
		{
			// Validate Tag
			if (Tag.IsEmpty) { return OggTagWriteCommandReturn.InvalidValue; }
			if (Tag.Name.Length<=0) { return OggTagWriteCommandReturn.UnknownTag; }
			if (Tag.IsArray) { if (Tag.Values.Length<=0) { return OggTagWriteCommandReturn.InvalidValue; }	} else { if (Tag.Value.Length<=0) { return OggTagWriteCommandReturn.InvalidValue; } }
			// Tag valid, try and write it
			TagLib.Ogg.XiphComment XC = (TagLib.Ogg.XiphComment) m_TagLibFile.GetTag(TagTypes.Xiph, true);
			if (XC != null)
			{
				string[] tmpStrArray;
				if (Tag.IsArray) { tmpStrArray = Tag.Values; } else { tmpStrArray = new string[1]; tmpStrArray[0] = Tag.Value; }
				// Set field
				XC.SetField(Tag.Name, tmpStrArray);
				// Copy the XC instance into our file (not sure if this is needed)
				XC.CopyTo(m_TagLibFile.Tag, true);
				// Commit
				m_TagLibFile.Save();
				return OggTagWriteCommandReturn.Success;
			}
			else
			{
				// If we're null something went wrong (we tried to create the XiphComment block and it failed probably)
				return OggTagWriteCommandReturn.Error;
			}
		}
		
		/// <summary>
		/// Write multiple Xiph tags to the file
		/// This will overwrite any existing tags, and create them if they don't exist
		/// It will also create the Xiph tag block within the file if it doesn't already have one
		/// This function writes to disk. If setting only a single tag, consider using SetTag(OggTag Tag) to reduce the array handling overheads
		/// If setting an array value Tags[i].Values must contain at least one item. Tags[i].Value is ignored in this case
		/// If setting a single value, Tags[i].Value must contain at least one character. Tags[i].Values is ignored in this case
		/// This function will abort (and not write) if any tag is invalid. Use SetTags(OggTag[] Tags, bool AbortOnError) to override this behaviour
		/// </summary>
		/// <param name="Tags">
		/// An <see cref="OggTag[]"/> containing the tags to be written
		/// </param>
		/// <returns>
		/// An <see cref="OggTagWriteCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggTagWriteCommandReturn SetTags(OggTag[] Tags)
		{
			return SetTags(Tags, true);
		}
		
		/// <summary>
		/// Write multiple Xiph tags to the file
		/// This will overwrite any existing tags, and create them if they don't exist
		/// It will also create the Xiph tag block within the file if it doesn't already have one
		/// This function writes to disk. If setting only a single tag, consider using SetTag(OggTag Tag) to reduce the array handling overheads
		/// If setting an array value Tags[i].Values must contain at least one item. Tags[i].Value is ignored in this case
		/// If setting a single value, Tags[i].Value must contain at least one character. Tags[i].Values is ignored in this case
		/// If AbortOnError is true, this function will abort (and not write) if any item in the Tags array is invalid.
		/// If AbortOnError is false, this function will continue (and write) if items in the Tags array are invalid. It will still abort (and not write) if there are other errors.
		/// </summary>
		/// <param name="Tags">
		/// An <see cref="OggTag[]"/> containing the tags to be written
		/// </param>
		/// <param name="AbortOnError">
		/// A <see cref="System.bool"/> indicating whether to invalid items in the Tags array.
		/// <returns>
		/// An <see cref="OggTagWriteCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggTagWriteCommandReturn SetTags(OggTag[] Tags, bool AbortOnError)
		{
			// Check that the Tags array has at least one item in it
			if (Tags.Length<1) { return OggTagWriteCommandReturn.UnknownTag; }
			TagLib.Ogg.XiphComment XC = (TagLib.Ogg.XiphComment) m_TagLibFile.GetTag(TagTypes.Xiph, true);
			if (XC != null)
			{
				// Write the tags to the 'virtual' file
				foreach (OggTag Tag in Tags)
				{
					// Validate tag
					if (Tag.IsEmpty) { if (AbortOnError) { return OggTagWriteCommandReturn.InvalidValue; } else { continue; } }
					if (Tag.Name.Length<=0) { if (AbortOnError) { return OggTagWriteCommandReturn.UnknownTag; } else { continue; } }
					if (Tag.IsArray) { if (Tag.Values.Length<=0) { if (AbortOnError) {  return OggTagWriteCommandReturn.InvalidValue; } else { continue; } } } else { if (Tag.Value.Length<=0) { if (AbortOnError) { return OggTagWriteCommandReturn.InvalidValue; } else { continue; } } }
					string[] tmpStrArray;
					if (Tag.IsArray) { tmpStrArray = Tag.Values; } else { tmpStrArray = new string[1]; tmpStrArray[0] = Tag.Value; }
					// Write tag
					XC.SetField(Tag.Name, tmpStrArray);
				}
				// Copy the XC instance into our file (not sure if this is needed)
				XC.CopyTo(m_TagLibFile.Tag, true);
				// Save to disk
				m_TagLibFile.Save();
				return OggTagWriteCommandReturn.Success;
			}
			else
			{
				// If we're null something went wrong (we tried to create the XiphComment block and it failed probably)
				return OggTagWriteCommandReturn.Error;
			}
		}
		
		/// <summary>
		/// Remove a tag from the file.
		/// This command writes to disk.
		/// </summary>
		/// <param name="TagName">
		/// A <see cref="System.String"/> indicating which tag to remove
		/// </param>
		/// <returns>
		/// An <see cref="OggTagWriteCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggTagWriteCommandReturn RemoveTag(string TagName)
		{
			// Check that the tag name contains at least one character
			if (TagName.Length<1) { return OggTagWriteCommandReturn.UnknownTag; }
			TagLib.Ogg.XiphComment XC = (TagLib.Ogg.XiphComment) m_TagLibFile.GetTag(TagTypes.Xiph, false);
			if (XC != null)
			{
				// Remove the tag
				XC.RemoveField(TagName);
				// Copy the XC instance into our file (might need to clear the Xiph block first, but we'll see)
				XC.CopyTo(m_TagLibFile.Tag, true);
				// Save
				m_TagLibFile.Save();
				return OggTagWriteCommandReturn.Success;
			}
			else
			{
				// Either there isn't a Xiph comment block or something went wrong
				return OggTagWriteCommandReturn.Error;
			}
			
		}
		/// <summary>
		/// Remove a tag from the file.
		/// This command writes to disk.
		/// </summary>
		/// <param name="TagName">
		/// An <see cref="OggTag"/> indicating which tag to remove
		/// </param>
		/// <returns>
		/// An <see cref="OggTagWriteCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggTagWriteCommandReturn RemoveTag(OggTag Tag)
		{
			return this.RemoveTag(Tag.Name);	
		}
		
		/// <summary>
		/// Strip all tags from the file.
		/// This command writes to disk.
		/// </summary>
		/// <returns>
		/// An <see cref="OggTagWriteCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggTagWriteCommandReturn RemoveAllTags()
		{
			// Dead simple (in theory)
			m_TagLibFile.RemoveTags(TagTypes.AllTags);
			m_TagLibFile.Save();
			// Done?
			return OggTagWriteCommandReturn.Success;
		}
		
		/// <summary>
		/// Get the next segment of Ogg data decoded into PCM format
		/// </summary>
		/// <param name="SegmentLength">
		/// A <see cref="System.Int32"/> indicating the number of bytes of data to request. 
		/// Defaults to 4096 if set to 0. 
		/// Use the ReturnValue property of the result to discover how many bytes are actually returned
		/// </param>
		/// <returns>
		/// Am <see cref="OggBufferSegment"/> containing the returned data
		/// </returns>
		public OggBufferSegment GetBufferSegment(int SegmentLength)
		{
			if (SegmentLength<=0) { SegmentLength = _SEGMENTLENGTH; }	// If segment length is invalid, use default segment length
			OggBufferSegment retVal; // Declare the buffer segment structure
			retVal.BufferLength = SegmentLength;
			retVal.Buffer = new Byte[retVal.BufferLength];	// Init buffer
			retVal.ReturnValue = m_CSVorbisFile.read(retVal.Buffer, retVal.BufferLength, _BIGENDIANREADMODE, _WORDREADMODE, _SGNEDREADMODE, null);
			retVal.RateHz = m_TagLibFile.Properties.AudioSampleRate; //m_Info[0].rate;
			return retVal;
		}
		
		/// <summary>
		/// Reset the OggFile (reload from disk).
		/// Useful if tags have changed externally, or to reset the internal position pointer to replay the file from the beginning
		/// SeekToTime(0) is the prefered method of moving the internal pointer to the beginning however however
		/// </summary>		
		public void ResetFile()
		{
			try
			{
				m_CSVorbisFile = null;
				m_CSVorbisFile = new VorbisFile(m_Filename);	// No point reloading anything else 'cos it shouldn't have changed	
				m_TagLibFile = null;
				m_TagLibFile = TagLib.File.Create(m_Filename);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to reload OggFile [" + m_Filename + "]", ex);	
			}
		}
		
		/// <summary>
		/// Attempt to change the internal position pointer to a new value within the file
		/// </summary>
		/// <param name="Seconds">
		/// A <see cref="System.Single"/> indicating what time to seek to
		/// </param>
		/// <returns>
		/// An <see cref="OggPlayerCommandReturn"/> indicating the result of the operation
		/// </returns>
		public OggPlayerCommandReturn SeekToTime(float Seconds)
		{
			if(!(m_CSVorbisFile.seekable())) { return OggPlayerCommandReturn.OperationNotValid; }
			if(Seconds>m_CSVorbisFile.time_total(-1)) { return OggPlayerCommandReturn.ValueOutOfRange; }
			if(Seconds<0) { return OggPlayerCommandReturn.ValueOutOfRange; }
			try 
			{ 
				if(m_CSVorbisFile.time_seek(Seconds)!=0) { return OggPlayerCommandReturn.Error; } 
			} 
			catch(Exception ex)
			{
				if (typeof(IndexOutOfRangeException)==ex.GetType()) { return OggPlayerCommandReturn.ValueOutOfRange; }
				return OggPlayerCommandReturn.Error;
			}
			return OggPlayerCommandReturn.Success;
		}
		
		/// <summary>
		/// Return current position of the internal pointer in seconds
		/// </summary>
		/// <returns>
		/// A <see cref="System.Single"/> indicating the position of the internal pointer in seconds
		/// </returns>
		public float GetTime()
		{
			return m_CSVorbisFile.time_tell();
		}
		
		#region IDisposable implementation
		public void Dispose ()
		{
			m_TagLibFile.Dispose();
			m_TagLibFile = null;
			m_CSVorbisFile.Dispose();
			m_CSVorbisFile = null;
		}
		
		#endregion
	}

	/// <summary>
	/// Exception raised when the specified file cannot be read.
	/// </summary>
	public class OggFileReadException : Exception
	{
		private string m_Filename;
		
		/// <summary>
		/// The filename of the file that the exception refers to
		/// </summary>
		public string Filename { get { return m_Filename; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Msg">
		/// A <see cref="System.String"/> containing the message to the user
		/// </param>
		/// <param name="FName">
		/// A <see cref="System.String"/> containing the filename that the exception refers to
		/// </param>
		public OggFileReadException(string Msg, string FName) : base(Msg)
		{
			m_Filename = FName;
		}
	}
	
	/// <summary>
	/// Exception raised when the specified file is corrupt
	/// </summary>
	public class OggFileCorruptException : Exception
	{
		private string m_Filename;
		private string m_Section;
		
		/// <summary>
		/// The filename of the file that the exception refers to
		/// </summary>
		public string Filename { get { return m_Filename; } }
		/// <summary>
		/// The section of the file which was corrupt (e.g. 'Tags' or 'Data' etc)
		/// </summary>
		public string Section { get { return m_Section; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="Msg">
		/// A <see cref="System.String"/> containing the message to the user
		/// </param>
		/// <param name="Filename">
		/// A <see cref="System.String"/> containing the filename that the exception refers to
		/// </param>
		/// <param name="Section">
		/// A <see cref="System.String"/> containing the name of the section of the file that was corrupt
		/// </param>
		public OggFileCorruptException(string Msg, string Filename, string Section) : base (Msg)
		{
			m_Filename = Filename;
			m_Section = Section;
		}
	}
}
