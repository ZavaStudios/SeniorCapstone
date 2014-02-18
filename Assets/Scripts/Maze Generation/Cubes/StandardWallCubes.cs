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
			return Cubes[x,y].Count;
		}
		
		public IEnumerable<Cube> EnumerateCubes()
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					/*
					int z = 0;
					foreach (Cube.CubeType type in Cubes[x,y])
					{
						yield return new Cube(type, x, y, z);
						z++;
					}
					*/

					yield return new Cube(Cubes[x,y].Last.Value, x, y, Cubes[x,y].Count-1);
				}
			}
		}
	}
}

