using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	public class LRCorridorCubes : RoomCubes
	{
		private StandardWallCubes UpWall { get; set; }
		private StandardWallCubes DownWall { get; set; }
		
		private int Width { get; set; }
		private int Depth { get; set; }
		private int Height { get; set; }

		public LRCorridorCubes (int width, int depth, int height)
		{
			UpWall = new StandardWallCubes(width, height, depth / 2, 1);
			DownWall = new StandardWallCubes(width, height, depth / 2, 1);

			Width = width;
			Height = height;
			Depth = depth;
		}

		public IEnumerable<Cube> EnumerateCubes()
		{
			foreach (Cube c in UpWall.EnumerateCubes())
				yield return c;
			foreach (Cube c in DownWall.EnumerateCubes())
				yield return new Cube(c.Type, Width - c.X - 1, c.Y, Depth - c.Z - 1);
		}
	}
}

