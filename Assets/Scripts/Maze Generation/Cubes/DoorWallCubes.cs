using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
    /// <summary>
    /// Generates cubes for the walls of a general room where there is
    /// a door on that wall. Basically a single wall split in two by
    /// two corners pointing towards the neighboring corridor.
    /// </summary>
	public class DoorWallCubes : WallCubes
	{
        // All the sub-structures used to make up the door wall.
		private StandardWallCubes L_Side;
		private OutsideCornerCubes L_Corner;
		private OutsideCornerCubes R_Corner;
		private StandardWallCubes R_Side;
		
        // Width (floor coordinates) of the wall
		public override int Width
		{
			get { return R_Side.Width + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH; }
		}
        // Height (floor to ceiling) of the wall
		public override int Height
		{
			get { return R_Side.Height; }
		}
        // Maximum depth the wall can poke out
		public override int MaxDepth
		{
			get { return R_Side.MaxDepth; }
		}
        // Minimum depth the wall can poke out
		public int MinDepth
		{
			get { return R_Side.MinDepth; }
		}
        // Bitmask representing which direction the door is facing
		public int DoorCode	{ get; private set; }
		
        /// <summary>
        /// Generates cubes along our wall with a door in it.
        /// </summary>
        /// <param name="width">Width (floor coordinates) of the wall</param>
        /// <param name="height">Height (floor to ceiling) of the wall</param>
        /// <param name="maxDepth">Maximum depth the wall can poke out</param>
        /// <param name="minDepth">Minimum depth the wall can poke out</param>
        /// <param name="doorCode">Direction the door is facing</param>
        /// <param name="neighborCubes">Reference to the RoomCubes structure of the neighbor
        /// connected by this door.</param>
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
		
        /// <summary>
        /// Get's the depth of the wall at the specified position. These coordinates are
        /// along the wall - not along the floor.
        /// </summary>
        /// <param name="x">Desired row of interest.</param>
        /// <param name="y">Desired column of interest.</param>
        /// <returns>Width of the selected stack of cubes.</returns>
		public override int GetDepthAt(int x, int y)
		{
			if (x < L_Side.Width)
				return L_Side.GetDepthAt(x, y);
			else if (x < L_Side.Width + RogueDungeon.CORRIDOR_WIDTH)
				return 0;
			else
				return R_Side.GetDepthAt(x - L_Side.Width - RogueDungeon.CORRIDOR_WIDTH, y);
		}

        /// <summary>
        /// Returns a list of depth values constituting the right edge of this wall.
        /// </summary>
        /// <returns>Depth values of the right edge of the wall.</returns>
		public override int[] GetRightEdge()
		{
			return R_Side.GetRightEdge();
		}

        /// <summary>
        /// Returns a list of depth values constituting the left edge of this wall.
        /// </summary>
        /// <returns>Depth values of the left edge of the wall.</returns>
		public override int[] GetLeftEdge()
		{
			return L_Side.GetLeftEdge();
		}

        /// <summary>
        /// Iterates over all the cubes in the corridor, and passes back any
        /// cubes that are visible. Air cubes are skipped, and cubes which
        /// are obscured from the player's view are also not returned.
        /// </summary>
        /// <returns>List of cubes that are visible in this corridor to the player.</returns>
		public override IEnumerable<Cube> EnumerateCubes()
		{
			// L Side
			foreach (Cube c in L_Side.EnumerateCubes())
				yield return new Cube(this, c.Type, c.X, c.Y, c.Z);

			// R Side
			foreach (Cube c in R_Side.EnumerateCubes())
				yield return new Cube(this, c.Type, c.X + L_Side.Width + RogueDungeon.CORRIDOR_WIDTH, c.Y, c.Z);

			// L Corner
			foreach (Cube c in L_Corner.EnumerateCubes())
				yield return new Cube(this, c.Type, c.X + L_Side.Width, c.Z, c.Y);

			// R Corner
			foreach (Cube c in R_Corner.EnumerateCubes())
				yield return new Cube(this, c.Type, Width - 1 - R_Side.Width - c.Y, c.Z, c.X);
		}

        /// <summary>
        /// Removes a cube from the corridor, and yields any cubes that are
        /// uncovered as a consequence.
        /// </summary>
        /// <param name="c">Cube to be destroyed.</param>
        /// <returns>Cubes that have been uncovered by destroying c.</returns>
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

