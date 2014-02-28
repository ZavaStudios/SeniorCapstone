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
	public class InsideCornerCubes : RoomCubes
	{
		private Cube.CubeType[,,] Cubes;
		
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
		
		public InsideCornerCubes(int width, int depth, int[] right, int[] down)
		{
			int height = right.Length;
			Cubes = new Cube.CubeType[width, depth, height];
			
			InitializeCubes(right, down);
		}
		
		private void InitializeCubes(int[] right, int[] down)
		{
			for (int z = 0; z < Height; z++)
			{
				// Quadrants: Only one is interesting
				int quadX = down[z];
				int quadY = right[z];
				// Quadrants 1 and 2 (merged for convenience)
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						Cubes[x,y,z] = GetCubeType();
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						Cubes[x,y,z] = GetCubeType();
					}
				}
				// Quadrant 4
				for (int x = quadX; x < Width; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						// TODO
						Cubes[x,y,z] = Cube.CubeType.Air;
					}
				}
			}
		}
		
		public override IEnumerable<Cube> EnumerateCubes()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Depth; y++)
					for (int z = 0; z < Height; z++)
						yield return new Cube(this, Cubes[x,y,z], x, y, z);
		}

		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
			Cubes[c.X, c.Y, c.Z] = Cube.CubeType.Air;
			return new List<Cube>(); // Return empty list, since we always show everything
			/*
			if (c.X > 0)
				yield return new Cube(this, Cubes[c.X-1, c.Y, c.Z], c.X-1, c.Y, c.Z);
			if (c.X < Width)
				yield return new Cube(this, Cubes[c.X+1, c.Y, c.Z], c.X-1, c.Y, c.Z);
			if (c.Y > 0)
				yield return new Cube(this, Cubes[c.X, c.Y-1, c.Z], c.X, c.Y-1, c.Z);
			if (c.Y < Depth)
				yield return new Cube(this, Cubes[c.X, c.Y+1, c.Z], c.X, c.Y+1, c.Z);
			if (c.Z > 0)
				yield return new Cube(this, Cubes[c.X, c.Y, c.Z-1], c.X, c.Y, c.Z-1);
			if (c.Z < Height)
				yield return new Cube(this, Cubes[c.X, c.Y, c.Z+1], c.X, c.Y, c.Z+1);
				*/
		}
	}
}

