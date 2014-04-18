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
        // This is the person who is responsible for handling destruction calls.
		public CubeTracker Parent { get; set; }
        // This is the type this cube represents.
        public ItemBase.tOreType Type { get; set; }

        // Coordinate vectors of the cube.
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }

        /// <summary>
        /// Constructs a new Cube struct.
        /// </summary>
        /// <param name="_parent">Parent of this cube</param>
        /// <param name="_type">Type of this cube</param>
        /// <param name="_x">X Coordinate of this cube's position</param>
        /// <param name="_y">Y Coordinate of this cube's position</param>
        /// <param name="_z">Z Coordinate of this cube's position</param>
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

    /// <summary>
    /// Basically just a contract for parents of cubes to inherit so they can
    /// properly handle cube destruction calls.
    /// </summary>
	public interface CubeTracker
	{
        /// <summary>
        /// Removes a cube from the corridor, and yields any cubes that are
        /// uncovered as a consequence.
        /// </summary>
        /// <param name="c">Cube to be destroyed.</param>
        /// <returns>Cubes that have been uncovered by destroying c.</returns>
		IEnumerable<Cube> DestroyCube(Cube c);
	}

    /// <summary>
    /// Generic structure for all the cube holding objects.
    /// </summary>
	public abstract class RoomCubes : CubeTracker
	{
        /// <summary>
        /// Iterates over all the cubes in the corridor, and passes back any
        /// cubes that are visible. Air cubes are skipped, and cubes which
        /// are obscured from the player's view are also not returned.
        /// </summary>
        /// <returns>List of cubes that are visible in this corridor to the player.</returns>
		public abstract IEnumerable<Cube> EnumerateCubes();

        /// <summary>
        /// Removes a cube from the corridor, and yields any cubes that are
        /// uncovered as a consequence.
        /// </summary>
        /// <param name="c">Cube to be destroyed.</param>
        /// <returns>Cubes that have been uncovered by destroying c.</returns>
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

    /// <summary>
    /// Generic structure for cubes that exist on walls. Largely
    /// used so other data structures can expose the GetRight /
    /// LeftEdge functions.
    /// </summary>
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