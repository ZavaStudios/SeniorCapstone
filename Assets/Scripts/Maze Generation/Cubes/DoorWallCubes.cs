using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	public class DoorWallCubes : WallCubes
	{
		private StandardWallCubes L_Side;
		private OutsideCornerCubes L_Corner;
		private OutsideCornerCubes R_Corner;
		private StandardWallCubes R_Side;
		
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
		public int DoorCode	{ get; private set; }
		
		public DoorWallCubes(int width, int height, int maxDepth, int minDepth,
		                     int doorCode, RoomCubes neighborCubes)
		{
			DoorCode = doorCode;
			int lWidth = (width - RogueDungeon.CORRIDOR_WIDTH) / 2;
			int rWidth = width - lWidth - RogueDungeon.CORRIDOR_WIDTH;
			
			R_Side = new StandardWallCubes(rWidth, height, maxDepth, minDepth);
			L_Side = new StandardWallCubes(lWidth, height, maxDepth, minDepth);

			// We need to know which type of Corridor Cubes to extract - use door code for that!
			if (doorCode == RogueRoom.LEFT_DOOR_MASK)
			{
				LRCorridorCubes corCubes = (LRCorridorCubes)neighborCubes;
				L_Corner = new OutsideCornerCubes(corCubes.DownWall.MaxDepth, L_Side.MaxDepth,
				                                  L_Side.GetRightEdge(), corCubes.DownWall.GetRightEdge());
				R_Corner = new OutsideCornerCubes(R_Side.MaxDepth, corCubes.UpWall.MaxDepth,
				                                  corCubes.UpWall.GetRightEdge(), R_Side.GetLeftEdge());
			}
			else if (doorCode == RogueRoom.RIGHT_DOOR_MASK)
			{
				LRCorridorCubes corCubes = (LRCorridorCubes)neighborCubes;
				L_Corner = new OutsideCornerCubes(corCubes.UpWall.MaxDepth, L_Side.MaxDepth,
				                                  L_Side.GetRightEdge(), corCubes.UpWall.GetLeftEdge());
				R_Corner = new OutsideCornerCubes(R_Side.MaxDepth, corCubes.DownWall.MaxDepth,
				                                  corCubes.DownWall.GetLeftEdge(), R_Side.GetLeftEdge());
			}
			else if (doorCode == RogueRoom.UP_DOOR_MASK)
			{
				UDCorridorCubes corCubes = (UDCorridorCubes)neighborCubes;
				L_Corner = new OutsideCornerCubes(corCubes.LeftWall.MaxDepth, L_Side.MaxDepth,
				                                  L_Side.GetRightEdge(), corCubes.LeftWall.GetRightEdge());
				R_Corner = new OutsideCornerCubes(R_Side.MaxDepth, corCubes.RightWall.MaxDepth,
				                                  corCubes.RightWall.GetRightEdge(), R_Side.GetLeftEdge());
			}
			else if (doorCode == RogueRoom.DOWN_DOOR_MASK)
			{
				UDCorridorCubes corCubes = (UDCorridorCubes)neighborCubes;
				L_Corner = new OutsideCornerCubes(corCubes.RightWall.MaxDepth, L_Side.MaxDepth,
				                                  L_Side.GetRightEdge(), corCubes.RightWall.GetLeftEdge());
				R_Corner = new OutsideCornerCubes(R_Side.MaxDepth, corCubes.LeftWall.MaxDepth,
				                                  corCubes.LeftWall.GetLeftEdge(), R_Side.GetLeftEdge());
			}
			else // ERROR!
			{
				throw new ArgumentException("Door code doesn't work D:");
			}
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

		public int[] GetRightEdge()
		{
			return R_Side.GetRightEdge();
		}

		public int[] GetLeftEdge()
		{
			return L_Side.GetLeftEdge();
		}
		
		public IEnumerable<Cube> EnumerateCubes()
		{
			foreach (Cube c in L_Side.EnumerateCubes())
				yield return c;
			foreach (Cube c in R_Side.EnumerateCubes())
				yield return new Cube(c.Type, c.X + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH, c.Y, c.Z);

			foreach (Cube c in L_Corner.EnumerateCubes())
				yield return new Cube(c.Type, c.X + L_Side.Width, c.Z, c.Y);
			foreach (Cube c in R_Corner.EnumerateCubes())
				yield return new Cube(c.Type, Width - 1 - R_Side.Width - c.Y, c.Z, c.X);
		}
	}
}

