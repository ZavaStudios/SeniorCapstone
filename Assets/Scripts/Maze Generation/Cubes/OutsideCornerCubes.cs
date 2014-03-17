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
        private ItemBase.tOreType[, ,] Cubes;
		
		public int Width
		{
			get { return Cubes.GetLength(0); }
		}
		public int Height
		{
			get { return Cubes.GetLength(2); }
		}
		public int Depth
		{
			get { return Cubes.GetLength(1); }
		}
		
		public OutsideCornerCubes(int width, int depth, int[] left, int[] up)
		{
			int height = up.Length;
            Cubes = new ItemBase.tOreType[width, depth, height];
			
			InitializeCubes(left, up);
		}
		
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
						//Cubes[x,y,z] = Cube.CubeType.Gold;
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

        // Enumeration state:
        private int itr = 0;

		public override IEnumerable<Cube> EnumerateCubes(int count)
		{
            for (; itr < (Width * Height * Depth);)
            {
                int enumZ = itr % Height;
                int enumY = (itr / Height) % Depth;
                int enumX = (itr / (Height * Depth));
                yield return new Cube(this, Cubes[enumX, enumY, enumZ], enumX, enumY, enumZ);
                itr++;
                if (--count == 0)
                    yield break;
            }
		}

        public override void ResetEnumeration()
        {
            itr = 0;
        }

		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
            Cubes[c.X, c.Y, c.Z] = ItemBase.tOreType.NOT_ORE;
			return new List<Cube>();	// Return nothing, since we always show everything
		}
	}
}

