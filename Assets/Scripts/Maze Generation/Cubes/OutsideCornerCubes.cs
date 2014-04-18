using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	/// <summary>
	/// Decribes cubes held in this room in corners between walls, on
	/// the inside of the room. This class will initialize its own
	/// entries, but to be able to do so, it requires knowledge of the
	/// two walls on either side of the corner. If there is not a wall
	/// on the other side of the corner, this is not the right class
	/// to use.
	/// 
	/// Determines dimensions of the corners based on dimensions of the
	/// neighboring walls. It is assumed that each wall has the same
	/// height - if this constraint is not followed, behavior is not
	/// defined.
	/// </summary>
	public class OutsideCornerCubes : RoomCubes
	{
        // Grid of cubes in the corner
        private ItemBase.tOreType[, ,] Cubes;
		
        // Width (floor coordinates) of the corner
		public int Width
		{
			get { return Cubes.GetLength(0); }
		}
        // Height (floor to ceiling) of the corner
		public int Height
		{
			get { return Cubes.GetLength(2); }
		}
        // Depth (floor coordinates) of the corner
		public int Depth
		{
			get { return Cubes.GetLength(1); }
		}
		
        /// <summary>
        /// Generates all the cubes for the corner.
        /// </summary>
        /// <param name="width">Width (floor coordinates) of the corner.</param>
        /// <param name="depth">Depth (floor coordinates) of the corner.</param>
        /// <param name="left">List of depth values for the wall directly to this corner's left.</param>
        /// <param name="up">List of depth values for the wall directly to this corner's up.</param>
		public OutsideCornerCubes(int width, int depth, int[] left, int[] up)
		{
			int height = up.Length;
            Cubes = new ItemBase.tOreType[width, depth, height];
			
			InitializeCubes(left, up);
		}
		
        /// <summary>
        /// Helper function that actually goes about generating all the cubes.
        /// Parameters skipped, because they match the constructor and this function
        /// is only called from there.
        /// </summary>
		private void InitializeCubes(int[] left, int[] up)
		{
			for (int z = 0; z < Height; z++)
			{
				// Quadrants: Only one is interesting
				int quadX = up[z];
				int quadY = left[z];
				// Quadrant 1
				for (int x = 0; x < quadX; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						Cubes[x,y,z] = GetCubeType();
					}
				}
				// Quadrant 2
				for (int x = quadX; x < Width; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						// TODO
                        Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						// TODO
                        Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
					}
				}
				// Quadrant 4
				for (int x = quadX; x < Width; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						// TODO smarter placement
                        Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
					}
				}
			}
		}

        /// <summary>
        /// Iterates over all the cubes in the corridor, and passes back any
        /// cubes that are visible. Air cubes are skipped, and cubes which
        /// are obscured from the player's view are also not returned.
        /// </summary>
        /// <returns>List of cubes that are visible in this corridor to the player.</returns>
		public override IEnumerable<Cube> EnumerateCubes()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Depth; y++)
					for (int z = 0; z < Height; z++)
						if (IsVisible(x,y,z))
							yield return new Cube(this, Cubes[x,y,z], x, y, z);
		}
		
        /// <summary>
        /// Helper which determines whether the cube at the provided position is
        /// actually visible to the player.
        /// </summary>
        /// <param name="x">X coordinate of the cube in question.</param>
        /// <param name="y">Y coordinate of the cube in question.</param>
        /// <param name="z">Z coordinate of the cube in question.</param>
        /// <returns>True if the input cube is visible. False otherwise.</returns>
		private bool IsVisible(int x, int y, int z)
		{
			// if on the boundary: yes
			if (x == 0 || x == Width-1 ||
			    y == 0 || y == Depth-1 ||
			    z == 0 || z == Height-1)
				return true;
			
			if (Cubes[x-1, y, z] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x+1, y, z] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x, y-1, z] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x, y+1, z] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x, y, z-1] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x, y, z+1] == ItemBase.tOreType.NOT_ORE)
				return true;
			
			return false;
		}

        /// <summary>
        /// Removes a cube from the corridor, and yields any cubes that are
        /// uncovered as a consequence.
        /// </summary>
        /// <param name="c">Cube to be destroyed.</param>
        /// <returns>Cubes that have been uncovered by destroying c.</returns>
		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
			List<Cube> toRet = new List<Cube>();
			
			if (c.X > 0 && !IsVisible(c.X-1, c.Y, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X-1, c.Y, c.Z], c.X-1, c.Y, c.Z));
			if (c.X < Width-1 && !IsVisible(c.X+1, c.Y, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X+1, c.Y, c.Z], c.X+1, c.Y, c.Z));
			if (c.Y > 0 && !IsVisible(c.X, c.Y-1, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y-1, c.Z], c.X, c.Y-1, c.Z));
			if (c.Y < Depth-1 && !IsVisible(c.X, c.Y+1, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y+1, c.Z], c.X, c.Y+1, c.Z));
			if (c.Z > 0 && !IsVisible(c.X, c.Y, c.Z-1))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y, c.Z-1], c.X, c.Y, c.Z-1));
			if (c.Z < Height-1 && !IsVisible(c.X, c.Y, c.Z+1))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y, c.Z+1], c.X, c.Y, c.Z+1));
			
			Cubes[c.X, c.Y, c.Z] = ItemBase.tOreType.NOT_ORE;
			return toRet;
		}
	}
}

