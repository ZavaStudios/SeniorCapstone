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
        /// <summary>
        /// Allows the user to enumerate cubes from the object. Yields no more than
        /// the requested count, except that count must be positive - a value less
        /// than 1 will yield one block assuming one exists to provide.
        /// 
        /// There is no guarantee that this enumeration will provide count cubes -
        /// in fact, it is designed to be called multiple times in succession. For
        /// example, if the object has 160 cubes:
        ///     - The first call for 100 will yield the first 100 cubes
        ///     - The second call for 50 will yield the 101st thru 150th cubes
        ///     - The third call for 20 will yield the final 10 cubes (note: NOT 20!)
        ///     
        /// The enumeration can be reset by calling ResetEnumeration.
        /// </summary>
        /// <param name="count">The maximum number of cubes to yield in this call.</param>
        /// <returns>A list of cubes in the data structure.</returns>
		public abstract IEnumerable<Cube> EnumerateCubes(int count);
        public abstract void ResetEnumeration();
		public abstract IEnumerable<Cube> DestroyCube(Cube c);

		// Placeholder thresholds:
		protected const float STONE_FREQ    = 0.80f;
		protected const float IRON_FREQ     = 0.10f;
		protected const float SILVER_FREQ   = 0.05f;
		protected const float GOLD_FREQ     = 0.005f;
		protected const float PLATINUM_FREQ = 0.005f;

		/// <summary>
		/// Retruns a type of cube for placement in the maze.
		/// 
		/// Long term, this theoretically takes various parameters to define behavior. For now,
		/// it just returns rarer materials at a much lower frequency.
		/// </summary>
		/// <returns>Cube type to be placed in the maze.</returns>
        protected ItemBase.tOreType GetCubeType()
		{
			if (Maze.rnd.NextDouble() <= STONE_FREQ)
                return ItemBase.tOreType.Stone;
            if (Maze.rnd.NextDouble() - STONE_FREQ <= IRON_FREQ)
                return ItemBase.tOreType.Bone;
            if (Maze.rnd.NextDouble() - STONE_FREQ - IRON_FREQ <= SILVER_FREQ)
                return ItemBase.tOreType.Iron;
            if (Maze.rnd.NextDouble() - STONE_FREQ - IRON_FREQ - SILVER_FREQ <= GOLD_FREQ)
                return ItemBase.tOreType.Steel;
            if (Maze.rnd.NextDouble() - STONE_FREQ - IRON_FREQ - SILVER_FREQ - GOLD_FREQ <= PLATINUM_FREQ)
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