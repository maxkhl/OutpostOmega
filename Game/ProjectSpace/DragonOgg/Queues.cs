// 
//  PushPopInt.cs
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
using System.Collections;

namespace DragonOgg
{

	/// <summary>
	/// A push/pop FIFO array with running total
	/// </summary>
	public class IntegerQueue
	{
		private int m_Total;
		private Queue m_Heap;

		/// <summary>
		/// Total of all integers in the queue
		/// </summary>
		public int Total { get { return m_Total; } }
		/// <summary>
		/// Number of items in the heap
		/// </summary>
		public int Count { get { return m_Heap.Count; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		public IntegerQueue()
		{
			m_Heap = new Queue();
			m_Total = 0;
		}
		
		/// <summary>
		/// Clear all data from the heap
		/// </summary>
		public void Clear()
		{
			m_Heap = new Queue();
			m_Total = 0;
		}
		
		/// <summary>
		/// Add a value to the end of the heap. Returns the new total
		/// </summary>
		/// <param name="PushValue">
		/// The <see cref="System.Int32"/> to add
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/> containing the new total
		/// </returns>
		public int Push(int PushValue)
		{
			m_Total += PushValue;
			m_Heap.Enqueue(PushValue);
			return m_Total;
		}
		
		/// <summary>
		/// Remove the first value from the heap
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int32"/> containing the value extracted
		/// </returns>
		public int Pop()
		{
			if (m_Heap.Count<=0) { return 0; }
			try 
			{ 
				int retVal = (int) m_Heap.Dequeue();
				m_Total -=  retVal;
				return retVal;
			} 
			catch (Exception ex) 
			{ 
				return 0; 
			} 

		}
	}
	
	/// <summary>
	/// A push/pop FIFO array with a running total
	/// </summary>
	public class FloatQueue
	{
		
		private float m_Total;
		private Queue m_Heap;
		
		/// <summary>
		/// Total of all floats in the queue
		/// </summary>
		public float Total { get { return m_Total; } }
		/// <summary>
		/// Number of items in the heap
		/// </summary>
		public int Count { get { return m_Heap.Count; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		public FloatQueue()
		{
			m_Heap = new Queue();
			m_Total = 0;
		}
		
		/// <summary>
		/// Clear all data from the heap
		/// </summary>
		public void Clear()
		{
			m_Heap = new Queue();
			m_Total = 0;
		}
		
		/// <summary>
		/// Add a value to the end of the heap
		/// </summary>
		/// <param name="PushValue">
		/// The <see cref="System.Single"/> to add
		/// </param>
		/// <returns>
		/// A <see cref="System.Single"/> containing the new total
		/// </returns>
		public float Push(float PushValue)
		{
			m_Total += PushValue;
			m_Heap.Enqueue(PushValue);
			return m_Total;
		}
		
		/// <summary>
		/// Remove the first value from the heap
		/// </summary>
		/// <returns>
		/// A <see cref="System.Single"/> containing the value extracted
		/// </returns>
		public float Pop()
		{
			if (m_Heap.Count<=0) { return 0; }
			try 
			{ 
				float retVal = (float) m_Heap.Dequeue();
				m_Total -=  retVal;
				return retVal;
			} 
			catch (Exception ex) 
			{ 
				return 0; 
			}
			
		}
	}
	
	/// <summary>
	/// A push/pop FIFO array with running total
	/// </summary>
	public class LongQueue
	{
		private long m_Total;
		private Queue m_Heap;

		/// <summary>
		/// Total of all longs in the queue
		/// </summary>
		public long Total { get { return m_Total; } }
		/// <summary>
		/// Number of items in the heap
		/// </summary>
		public long Count { get { return m_Heap.Count; } }
		
		/// <summary>
		/// Constructor
		/// </summary>
		public LongQueue()
		{
			m_Heap = new Queue();
			m_Total = 0;
		}
		
		/// <summary>
		/// Clear all data from the heap
		/// </summary>
		public void Clear()
		{
			m_Heap = new Queue();
			m_Total = 0;
		}
		
		/// <summary>
		/// Add a value to the end of the heap. Returns the new total
		/// </summary>
		/// <param name="PushValue">
		/// The <see cref="System.Int64"/> to add
		/// </param>
		/// <returns>
		/// A <see cref="System.Int64"/> containing the new total
		/// </returns>
		public long Push(long PushValue)
		{
			m_Total += PushValue;
			m_Heap.Enqueue(PushValue);
			return m_Total;
		}
		
		/// <summary>
		/// Remove the first value from the heap
		/// </summary>
		/// <returns>
		/// A <see cref="System.Int64"/> containing the value extracted
		/// </returns>
		public long Pop()
		{
			if (m_Heap.Count<=0) { return 0; }
			try 
			{ 
				long retVal = (long) m_Heap.Dequeue();
				m_Total -=  retVal;
				return retVal;
			} 
			catch (Exception ex) 
			{ 
				return 0; 
			}
		}
	}
}
