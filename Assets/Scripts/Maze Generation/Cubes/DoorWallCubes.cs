using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	public class DoorWallCubes : WallCubes
	{
		private StandardWallCubes R_Side;
		private StandardWallCubes L_Side;
		
		public int Width
		{
			get { return R_Side.Width + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH; }
		}
		public int Height
		{
			get { return R_Side.Height; }
		}
		public int MaxDepth
		{
			get { return R_Side.MaxDepth; }
		}
		public int MinDepth
		{
			get { return R_Side.MinDepth; }
		}
		
		public DoorWallCubes(int width, int height, int maxDepth, int minDepth)
		{
			int lWidth = (width - RogueDungeon.CORRIDOR_WIDTH) / 2;
			int rWidth = width - lWidth - RogueDungeon.CORRIDOR_WIDTH;
			
			R_Side = new StandardWallCubes(rWidth, height, maxDepth, minDepth);
			L_Side = new StandardWallCubes(lWidth, height, maxDepth, minDepth);
		}
		
		public int GetDepthAt(int x, int y)
		{
			if (x < L_Side.Width)
				return L_Side.GetDepthAt(x, y);
			else if (x < L_Side.Width + RogueDungeon.CORRIDOR_WIDTH)
				return 0;
			else
				return R_Side.GetDepthAt(x - L_Side.Width - RogueDungeon.CORRIDOR_WIDTH, y);
		}
		
		public IEnumerable<Cube> EnumerateCubes()
		{
			foreach (Cube c in L_Side.EnumerateCubes())
				yield return c;
			foreach (Cube c in R_Side.EnumerateCubes())
				yield return new Cube(c.Type, c.X + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH, c.Y, c.Z);
		}
	}
}

