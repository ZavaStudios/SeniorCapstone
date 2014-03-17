using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGeneration
{
	/// <summary>
	/// Describes cubes placed along a wall in a room. Allows the user to
	/// create a wall with arbitrary width and height, as well as a custom
	/// cap on how far out from the wall cubes are allowed to be placed.
	/// </summary>
	public class StandardWallCubes : WallCubes, CubeTracker
	{
        private ItemBase.tOreType[, ,] Cubes { get; set; }
		private int _maxDepth;

		public override int MaxDepth { get { return _maxDepth; }}
		public int MinDepth { get; private set; }
		public override int Width
		{
			get { return Cubes.GetLength(0); }
		}
		public override int Height
		{
			get { return Cubes.GetLength(1); }
		}
		
		public StandardWallCubes(int width, int height, int maxDepth, int minDepth)
		{
			_maxDepth = maxDepth;
			MinDepth = minDepth;
            Cubes = new ItemBase.tOreType[width, height, MaxDepth];
			for (int x = 0; x < width; x++)
				for (int y = 0; y < height; y++)
					for (int z = 0; z < MaxDepth; z++)
                        Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
			
			InitializeCubes();
		}
		
		private void InitializeCubes()
		{
            int[,] noise = PerlinNoise.GenerateNoise128();
			for (int x = 0; x < Cubes.GetLength(0); x++)
			{
				for (int y = 0; y < Cubes.GetLength(1); y++)
				{
					// TODO: better indexing. We could average nearby values or something.
					int xIndex = (int)(((float)x / (float)Cubes.GetLength(0)) *
                                       ((float)(noise.GetLength(0)-1)));
					int yIndex = (int)(((float)y / (float)Cubes.GetLength(1)) * 
                                       ((float)(noise.GetLength(1)-1)));

                    float tmpDepth =(float)noise[xIndex,yIndex] * 0.01f;
					int depth = MinDepth + (int)((float)(MaxDepth - MinDepth) * tmpDepth);
					// HACK: for now, perlin noise is still busted. We don't want to get more than
					// our corner sizes (or bad things happen), so clamp the value:
					depth = (depth > MaxDepth) ? MaxDepth : depth;
					// HACK: I think somehow I may be getting lower than min? w/e
					depth = (depth < MinDepth) ? MinDepth : depth;
					for (int z = 0; z < depth; z++)
					{
						Cubes[x,y,z] = GetCubeType();
					}
				}
			}
		}
		
		/// <summary>
		/// Returns the depth of the buffer at the specified location.
		/// </summary>
		/// <returns>Depth of the buffer at [x,y].</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public override int GetDepthAt(int x, int y)
		{
			// If we have no cubes in our grid (one of the dims is 0), just return 0
			if (Width == 0 || Height == 0)
				return 0;
			if (Cubes == null)
				return -1;

			for (int z = MaxDepth - 1; z > 0; z--)
                if (Cubes[x, y, z] != ItemBase.tOreType.NOT_ORE)
					return z + 1;

			// If all the blocks were air, then we don't have any blocks in this section:
			return 0;
		}

		/// <summary>
		/// Returns an array of depths representing the right-most edge of this wall.
		/// </summary>
		/// <returns>Depths along the right-most edge.</returns>
		public override int[] GetRightEdge()
		{
			int[] toRet = new int[Height];
			for (int y = 0; y < Height; y++)
				toRet[y] = GetDepthAt(Width-1, y);
			return toRet;
		}

		/// <summary>
		/// Returns an array of depths representing the left-most edge of this wall.
		/// </summary>
		/// <returns>Depths along the left-most edge.</returns>
		public override int[] GetLeftEdge()
		{
			int[] toRet = new int[Height];
			for (int y = 0; y < Height; y++)
				toRet[y] = GetDepthAt(0, y);
			return toRet;
		}
		
        // Enumeration state:
        private int enumEdgeY = 0;
        private int enumEdgeZ = 0;
        private int enumX = 1;
        private int enumY = 0;
        private int enumZ = 0;

		public override IEnumerable<Cube> EnumerateCubes(int count)
		{
			// Fencepost: draw left and right edges always
			for (; enumEdgeY < Height; enumEdgeY++)
			{
				for (; enumEdgeZ < MaxDepth; enumEdgeZ++)
				{
					yield return new Cube(this, Cubes[0,enumEdgeY,enumEdgeZ], 0, enumEdgeY, enumEdgeZ);
                    if (--count == 0)
                        yield break;

					yield return new Cube(this, Cubes[Width-1,enumEdgeY,enumEdgeZ], Width-1, enumEdgeY, enumEdgeZ);
                    if (--count == 0)
                        yield break;
				}
			}

			// Return remainder only if they are uncovered somewhere:
            for (; enumX < Width - 1; enumX++)
            {
                for (; enumY < Height; enumY++)
                {
                    for (; enumZ < MaxDepth; enumZ++)
                    {
                        if (IsUncovered(enumX, enumY, enumZ))
                        {
                            yield return new Cube(this, Cubes[enumX, enumY, enumZ], enumX, enumY, enumZ);
                            if (--count == 0)
                                yield break;
                        }
                    }
                }
            }
		}

        public override void ResetEnumeration()
        {
            enumEdgeY = 0;
            enumEdgeZ = 0;
            enumX = 1;
            enumY = 0;
            enumZ = 0;
        }

		private bool IsUncovered(int x, int y, int z)
		{
			return ((z == MaxDepth-1) ||
                    ((x > 0) && (Cubes[x - 1, y, z] == ItemBase.tOreType.NOT_ORE)) ||
                    ((x < Width - 1) && (Cubes[x + 1, y, z] == ItemBase.tOreType.NOT_ORE)) ||
                    ((y > 0) && (Cubes[x, y - 1, z] == ItemBase.tOreType.NOT_ORE)) ||
                    ((y < Height - 1) && (Cubes[x, y + 1, z] == ItemBase.tOreType.NOT_ORE)) ||
                    ((z > 0) && (Cubes[x, y, z - 1] == ItemBase.tOreType.NOT_ORE)) ||
                    ((Cubes[x, y, z + 1] == ItemBase.tOreType.NOT_ORE)));
		}

		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
			List<Cube> toRet = new List<Cube>();
			// If we are on one of the edges, don't check neighbors in the plane:
			// we know that those are already displayed
			if (c.X == 0)
			{
				if (!IsUncovered(c.X+1, c.Y, c.Z))
				    toRet.Add(new Cube(this, Cubes[c.X+1, c.Y, c.Z], c.X+1, c.Y, c.Z));
			}
			else if (c.X == Width-1)
			{
				if (!IsUncovered(c.X-1, c.Y, c.Z))
					toRet.Add(new Cube(this, Cubes[c.X-1, c.Y, c.Z], c.X-1, c.Y, c.Z));
			}
			else
			{
				if (!IsUncovered(c.X-1, c.Y, c.Z))
					toRet.Add(new Cube(this, Cubes[c.X-1, c.Y, c.Z], c.X-1, c.Y, c.Z));
				if (!IsUncovered(c.X+1, c.Y, c.Z))
					toRet.Add(new Cube(this, Cubes[c.X+1, c.Y, c.Z], c.X+1, c.Y, c.Z));

				if (c.Y > 0 && !IsUncovered(c.X, c.Y-1, c.Z))
					toRet.Add(new Cube(this, Cubes[c.X, c.Y-1, c.Z], c.X, c.Y-1, c.Z));
				if (c.Y < Height-1 && !IsUncovered(c.X, c.Y+1, c.Z))
					toRet.Add(new Cube(this, Cubes[c.X, c.Y+1, c.Z], c.X, c.Y+1, c.Z));

				if (c.Z > 0 && !IsUncovered(c.X, c.Y, c.Z-1))
					toRet.Add(new Cube(this, Cubes[c.X, c.Y, c.Z-1], c.X, c.Y, c.Z-1));
				if (c.Z < MaxDepth-1 && !IsUncovered(c.X, c.Y, c.Z+1))
					toRet.Add(new Cube(this, Cubes[c.X, c.Y, c.Z+1], c.X, c.Y, c.Z+1));
			}

            Cubes[c.X, c.Y, c.Z] = ItemBase.tOreType.NOT_ORE;
			return toRet;
		}
	}
}

