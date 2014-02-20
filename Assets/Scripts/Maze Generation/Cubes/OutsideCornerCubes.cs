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
		private WallCubes UpWall;
		private WallCubes LeftWall;
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
		
		public OutsideCornerCubes(WallCubes upWall, WallCubes leftWall)
		{
			UpWall = upWall;
			LeftWall = leftWall;
			
			int width = UpWall.MaxDepth;
			int height = UpWall.Height;
			int depth = LeftWall.MaxDepth;
			Cubes = new Cube.CubeType[width, depth, height];
			
			InitializeCubes();
		}
		
		private void InitializeCubes()
		{
			for (int z = 0; z < Height; z++)
			{
				// Quadrants: Only one is interesting
				int quadX = UpWall.GetDepthAt(0, z);
				int quadY = LeftWall.GetDepthAt(LeftWall.Width-1, z);
				// Quadrant 1
				for (int x = 0; x < quadX; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						// TODO: smarter cube type selection
						// TODO
						//Cubes[x,y,z] = Cube.CubeType.Air;
						Cubes[x,y,z] = Cube.CubeType.Iron;
					}
				}
				// Quadrant 2
				for (int x = quadX; x < Width; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						// TODO
						Cubes[x,y,z] = Cube.CubeType.Air;
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						// TODO
						Cubes[x,y,z] = Cube.CubeType.Air;
						//Cubes[x,y,z] = Cube.CubeType.Gold;
					}
				}
				// Quadrant 4
				for (int x = quadX; x < Width; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						// TODO smarter placement
						Cubes[x,y,z] = Cube.CubeType.Air;
					}
				}
			}
		}
		
		public IEnumerable<Cube> EnumerateCubes()
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Depth; y++)
				{
					for (int z = 0; z < Height; z++)
					{
						yield return new Cube(Cubes[x,y,z], x, y, z);
					}
				}
			}
		}
	}
}

