using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	/// <summary>
	/// Describes cubes placed along a wall in a room. Allows the user to
	/// create a wall with arbitrary width and height, as well as a custom
	/// cap on how far out from the wall cubes are allowed to be placed.
	/// </summary>
	public class StandardWallCubes : WallCubes
	{
		private LinkedList<Cube.CubeType>[,] Cubes { get; set; }
		
		public int MaxDepth { get; private set; }
		public int MinDepth { get; private set; }
		public int Width
		{
			get { return Cubes.GetLength(0); }
		}
		public int Height
		{
			get { return Cubes.GetLength(1); }
		}
		
		public StandardWallCubes(int width, int height, int maxDepth, int minDepth)
		{
			MaxDepth = maxDepth;
			MinDepth = minDepth;
			Cubes = new LinkedList<Cube.CubeType>[width,height];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Cubes[x,y] = new LinkedList<Cube.CubeType>();
				}
			}
			
			InitializeCubes();
		}
		
		private void InitializeCubes()
		{
			// Randomizer:
			System.Random r = new System.Random();
			
			int[,] noise = PerlinNoise.GenerateNoise128();
			for (int x = 0; x < Cubes.GetLength(0); x++)
			{
				for (int y = 0; y < Cubes.GetLength(1); y++)
				{
					// TODO: better indexing. We could average nearby values or something.
					int xIndex = (int)(((float)x / (float)Cubes.GetLength(0)) * 127.0f);
					int yIndex = (int)(((float)y / (float)Cubes.GetLength(1)) * 127.0f);
					float tmpDepth =(float)noise[xIndex,yIndex] * 0.01f;
					int depth = MinDepth + (int)((float)(MaxDepth - MinDepth) * tmpDepth);
					// HACK: for now, perlin noise is still busted. We don't want to get more than
					// our corner sizes (or bad things happen), so clamp the value:
					depth = (depth > MaxDepth) ? MaxDepth : depth;
					// HACK: I think somehow I may be getting lower than min? w/e
					depth = (depth < MinDepth) ? MinDepth : depth;
					for (int z = 0; z < depth; z++)
					{
						// TODO: generate cube type more nicely
						Cubes[x,y].AddLast(Cube.CubeType.Silver);
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
		public int GetDepthAt(int x, int y)
		{
			// If we have no cubes in our grid (one of the dims is 0), just return 0
			if (Width == 0 || Height == 0)
				return 0;
			if (Cubes == null)
				return -1;
			return Cubes[x,y].Count;
		}

		/// <summary>
		/// Returns an array of depths representing the right-most edge of this wall.
		/// </summary>
		/// <returns>Depths along the right-most edge.</returns>
		public int[] GetRightEdge()
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
		public int[] GetLeftEdge()
		{
			int[] toRet = new int[Height];
			for (int y = 0; y < Height; y++)
				toRet[y] = GetDepthAt(0, y);
			return toRet;
		}
		
		public IEnumerable<Cube> EnumerateCubes()
		{
			// Fencepost: Enumerate edges of the wall first:
			// Left / Right:
			for (int y = 0; y < Height; y++)
			{
				int z = 0;
				foreach (Cube.CubeType type in Cubes[0,y])
				{
					yield return new Cube(type, 0, y, z);
					z++;
				}
				z = 0;
				foreach (Cube.CubeType type in Cubes[Width-1,y])
				{
					yield return new Cube(type, Width-1, y, z);
					z++;
				}
			}
			// Top / Bottom:
			for (int x = 0; x < Width; x++)
			{
				int z = 0;
				foreach (Cube.CubeType type in Cubes[x,0])
				{
					yield return new Cube(type, x, 0, z);
					z++;
				}
				z = 0;
				foreach (Cube.CubeType type in Cubes[x,Height-1])
				{
					yield return new Cube(type, x, Height-1, z);
					z++;
				}
			}

			// Main loop:
			for (int x = 1; x < Width-1; x++)
			{
				for (int y = 1; y < Height-1; y++)
				{
					// We need to check neighboring lists, as well as this one.
					// We initialize the iterators here:
					LinkedListNode<Cube.CubeType> lftItr = Cubes[x-1,y].First;
					LinkedListNode<Cube.CubeType> rgtItr = Cubes[x+1,y].First;
					LinkedListNode<Cube.CubeType> topItr = Cubes[x,y-1].First;
					LinkedListNode<Cube.CubeType> dwnItr = Cubes[x,y+1].First;
					LinkedListNode<Cube.CubeType> curItr = Cubes[x,y].First;

					int z = 0;
					while(curItr != null)
					{
						// Return this cube if and only if it is not obscured by adjacent cubes
						// This means, to skip it,the following must hold:
						//  1) We have a left, right, up, and down neighbor, each of which is not air
						//  2) This block has another in front of it
						if (!( /*LEFT:*/  (lftItr != null && lftItr.Value != Cube.CubeType.Air) &&
						       /*RIGHT:*/ (rgtItr != null && rgtItr.Value != Cube.CubeType.Air) &&
						       /*UP:*/    (topItr != null && topItr.Value != Cube.CubeType.Air) &&
						       /*DOWN:*/  (dwnItr != null && dwnItr.Value != Cube.CubeType.Air) &&
						       /*FRONT:*/ (curItr.Next != null)))
						{
							yield return new Cube(curItr.Value, x, y, z);
						}

						// Update iterators:
						lftItr = (lftItr != null) ? lftItr.Next : null;
						rgtItr = (rgtItr != null) ? rgtItr.Next : null;
						topItr = (topItr != null) ? topItr.Next : null;
						dwnItr = (dwnItr != null) ? dwnItr.Next : null;
						curItr = curItr.Next;
						z++;
					}
				}
			}
		}
	}
}

