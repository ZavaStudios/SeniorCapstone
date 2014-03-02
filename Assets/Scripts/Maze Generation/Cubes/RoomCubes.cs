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

		public CubeTracker Parent { get; set; }
		public CubeType Type { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		
		public Cube(CubeTracker _parent, CubeType _type, int _x, int _y, int _z) : this()
		{
			Parent = _parent;
			Type = _type;
			X = _x;
			Y = _y;
			Z = _z;
		}
	}

	public interface CubeTracker
	{
		IEnumerable<Cube> DestroyCube(Cube c);
	}

	public abstract class RoomCubes : CubeTracker
	{
		public abstract IEnumerable<Cube> EnumerateCubes();
		public abstract IEnumerable<Cube> DestroyCube(Cube c);

		// Placeholder thresholds:
		protected const float STONE_FREQ    = 0.80f;
		protected const float IRON_FREQ     = 0.10f;
		protected const float SILVER_FREQ   = 0.05f;
		protected const float GOLD_FREQ     = 0.005f;
		protected const float PLATINUM_FREQ = 0.005f;
		private System.Random r = new System.Random();

		/// <summary>
		/// Retruns a type of cube for placement in the maze.
		/// 
		/// Long term, this theoretically takes various parameters to define behavior. For now,
		/// it just returns rarer materials at a much lower frequency.
		/// </summary>
		/// <returns>Cube type to be placed in the maze.</returns>
		protected Cube.CubeType GetCubeType()
		{
			/*
			if (r.NextDouble() <= STONE_FREQ)
				return Cube.CubeType.Stone;
			if (r.NextDouble() - STONE_FREQ <= IRON_FREQ)
				return Cube.CubeType.Iron;
			if (r.NextDouble() - STONE_FREQ - IRON_FREQ <= SILVER_FREQ)
				return Cube.CubeType.Silver;
			if (r.NextDouble() - STONE_FREQ - IRON_FREQ - SILVER_FREQ <= GOLD_FREQ)
				return Cube.CubeType.Gold;
			if (r.NextDouble() - STONE_FREQ - IRON_FREQ - SILVER_FREQ - GOLD_FREQ <= PLATINUM_FREQ)
				return Cube.CubeType.Platinum;
				*/
				
			// None succeeded: return stone
			return Cube.CubeType.Stone;
		}
	}

	public abstract class WallCubes : RoomCubes
	{
		public abstract int Width { get; }
		public abstract int Height { get; }
		public abstract int MaxDepth { get; }
		public abstract int GetDepthAt(int x, int y);
		public abstract int[] GetRightEdge();
		public abstract int[] GetLeftEdge();
	}


}