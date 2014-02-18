using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	public class UDCorridorCubes : RoomCubes
	{
		private StandardWallCubes LeftWall { get; set; }
		private StandardWallCubes RightWall { get; set; }
		
		private int Width { get; set; }
		private int Depth { get; set; }
		private int Height { get; set; }
		
		public UDCorridorCubes (int width, int depth, int height)
		{
			LeftWall = new StandardWallCubes(depth, height, width / 3, Math.Min (3, width / 3));
			RightWall = new StandardWallCubes(depth, height, width / 3, Math.Min (3, width / 3));
			
			Width = width;
			Height = height;
			Depth = depth;
		}
		
		public IEnumerable<Cube> EnumerateCubes()
		{
			foreach (Cube c in LeftWall.EnumerateCubes())
				yield return new Cube(c.Type, c.Z, c.Y, c.X);
			foreach (Cube c in RightWall.EnumerateCubes())
				yield return new Cube(c.Type, Width - c.Z, c.Y, Depth - c.X);
		}
	}
}