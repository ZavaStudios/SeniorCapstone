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
		
		public override int Width
		{
			get { return R_Side.Width + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH; }
		}
		public override int Height
		{
			get { return R_Side.Height; }
		}
		public override int MaxDepth
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
		
		public override int GetDepthAt(int x, int y)
		{
			if (x < L_Side.Width)
				return L_Side.GetDepthAt(x, y);
			else if (x < L_Side.Width + RogueDungeon.CORRIDOR_WIDTH)
				return 0;
			else
				return R_Side.GetDepthAt(x - L_Side.Width - RogueDungeon.CORRIDOR_WIDTH, y);
		}

		public override int[] GetRightEdge()
		{
			return R_Side.GetRightEdge();
		}

		public override int[] GetLeftEdge()
		{
			return L_Side.GetLeftEdge();
		}
		
		public override IEnumerable<Cube> EnumerateCubes(int count)
		{
            int remaining = count;

			// L Side
            foreach (Cube c in L_Side.EnumerateCubes(remaining))
            {
                yield return new Cube(this, c.Type, c.X, c.Y, c.Z);
                if (--remaining == 0)
                    yield break;
            }

			// R Side
            foreach (Cube c in R_Side.EnumerateCubes(remaining))
            {
                yield return new Cube(this, c.Type, c.X + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH, c.Y, c.Z);
                if (--remaining == 0)
                    yield break;
            }

			// L Corner
			foreach (Cube c in L_Corner.EnumerateCubes(remaining))
            {
                yield return new Cube(this, c.Type, c.X + L_Side.Width, c.Z, c.Y);
                if (--remaining == 0)
                    yield break;
            }

			// R Corner
			foreach (Cube c in R_Corner.EnumerateCubes(remaining))
            {
                yield return new Cube(this, c.Type, Width - 1 - R_Side.Width - c.Y, c.Z, c.X);
                if (--remaining == 0)
                    yield break;
            }
		}

        public override void ResetEnumeration()
        {
            L_Corner.ResetEnumeration();
            L_Side.ResetEnumeration();
            R_Side.ResetEnumeration();
            R_Corner.ResetEnumeration();
        }

		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
			// L Side
			if (c.X < L_Side.Width)
			{
				foreach (Cube uncovered in L_Side.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      uncovered.X, uncovered.Y, uncovered.Z);
			}
			// L Corner
			else if (c.X < L_Side.Width + L_Corner.Width)
			{
				int tmp = c.Z;
				c.Z = c.Y;
				c.Y = tmp;
				c.X -= L_Side.Width;
				foreach (Cube uncovered in L_Corner.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      uncovered.X + L_Side.Width, uncovered.Z, uncovered.Y);
			}
			// R Corner
			else if (c.X < L_Side.Width + L_Corner.Width + R_Corner.Depth)
			{
				int tmp = c.Z;
				c.Z = c.Y;
				c.Y = Width - 1 - R_Side.Width - c.X;
				c.X = tmp;
				foreach (Cube uncovered in R_Corner.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      Width - 1 - R_Side.Width - uncovered.Y, uncovered.Z, uncovered.X);
			}
			//c.X + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH, c.Y, c.Z
			// R Side
			else
			{
				c.X -= L_Side.Width + RogueDungeon.CORRIDOR_WIDTH;
				foreach (Cube uncovered in R_Side.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      uncovered.X + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH, uncovered.Y, uncovered.Z);
			}
		}
	}
}

