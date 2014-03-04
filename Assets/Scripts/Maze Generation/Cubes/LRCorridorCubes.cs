using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	public class LRCorridorCubes : RoomCubes
	{
		public StandardWallCubes UpWall { get; private set; }
		public StandardWallCubes DownWall { get; private set; }
		
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

		public override IEnumerable<Cube> EnumerateCubes()
		{
			foreach (Cube c in UpWall.EnumerateCubes())
				yield return c;
			foreach (Cube c in DownWall.EnumerateCubes())
				yield return new Cube(this, c.Type, c.X, c.Y, Depth - c.Z - 1);
		}

		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
			if (c.Z < Depth / 2)
				foreach (Cube uncovered in UpWall.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      uncovered.X, uncovered.Y, uncovered.Z);
			else
			{
				c.Z = Depth - 1 - c.Z;
				foreach (Cube uncovered in DownWall.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      uncovered.X, uncovered.Y, Depth - 1 - uncovered.Z);
			}
		}
	}
}

