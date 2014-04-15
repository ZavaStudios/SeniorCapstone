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

		// Type array - return adjacent types
        private ItemBase.tOreType[] genGrid = new ItemBase.tOreType[]
        {
            ItemBase.tOreType.Bone, ItemBase.tOreType.Copper, ItemBase.tOreType.Iron,
            ItemBase.tOreType.Steel, ItemBase.tOreType.Mithril, ItemBase.tOreType.Dragon,
            ItemBase.tOreType.Ethereal
        };

		/// <summary>
		/// Retruns a type of cube for placement in the maze.
		/// 
		/// Long term, this theoretically takes various parameters to define behavior. For now,
		/// it just returns rarer materials at a much lower frequency.
		/// </summary>
		/// <returns>Cube type to be placed in the maze.</returns>
        protected ItemBase.tOreType GetCubeType()
		{
            //Debug.Log("Level: " + LevelHolder.Level);
            int primaryIndex = Math.Min(genGrid.Length - 1, LevelHolder.Level - 1);
            int secondaryIndex = Math.Min(genGrid.Length - 1, primaryIndex + 1);
            if (Maze.rnd.NextDouble() < 0.75)
                return ItemBase.tOreType.Stone;
            else if (Maze.rnd.NextDouble() < 0.9)
                return genGrid[primaryIndex];
            else
                return genGrid[secondaryIndex];
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