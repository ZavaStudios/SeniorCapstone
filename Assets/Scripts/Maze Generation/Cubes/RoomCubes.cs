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
		public CubeTracker Parent { get; set; }
        public ItemBase.tOreType Type { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }

        public Cube(CubeTracker _parent,
                    ItemBase.tOreType _type,
                    int _x, int _y, int _z) : this()
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
        protected ItemBase.tOreType GetCubeType()
		{
			if (r.NextDouble() <= STONE_FREQ)
                return ItemBase.tOreType.Stone;
			if (r.NextDouble() - STONE_FREQ <= IRON_FREQ)
                return ItemBase.tOreType.Bone;
			if (r.NextDouble() - STONE_FREQ - IRON_FREQ <= SILVER_FREQ)
                return ItemBase.tOreType.Iron;
			if (r.NextDouble() - STONE_FREQ - IRON_FREQ - SILVER_FREQ <= GOLD_FREQ)
                return ItemBase.tOreType.Steel;
			if (r.NextDouble() - STONE_FREQ - IRON_FREQ - SILVER_FREQ - GOLD_FREQ <= PLATINUM_FREQ)
                return ItemBase.tOreType.Mithril;
			
	        // TODO: more

			// None succeeded: return stone
			return ItemBase.tOreType.Stone;
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