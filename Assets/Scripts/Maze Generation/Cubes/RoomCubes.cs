using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGeneration
{
	/// <summary>
	/// Data object used to represent a single cube in the maze.
	/// </summary>
	public struct Cube
	{
		public enum CubeType
		{
			Air, Stone, Iron, Silver, Gold, Platinum, // other ore types...
		}
		
		public CubeType Type { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		
		public Cube(CubeType _type, int _x, int _y, int _z) : this()
		{
			Type = _type;
			X = _x;
			Y = _y;
			Z = _z;
		}
	}

	public interface RoomCubes
	{
		IEnumerable<Cube> EnumerateCubes();
	}

	public interface WallCubes : RoomCubes
	{
		int Width { get; }
		int Height { get; }
		int MaxDepth { get; }
		int GetDepthAt(int x, int y);
		int[] GetRightEdge();
		int[] GetLeftEdge();
	}


}