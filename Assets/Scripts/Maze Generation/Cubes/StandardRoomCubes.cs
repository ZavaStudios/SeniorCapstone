using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	/// <summary>
	/// Data structure denoting all of the cubes for a single room in the maze.
	/// For external users, this is effectively just an enumerable list: you ask
	/// for a list of the cubes with their x,y,z positions in the room (in cube
	/// coordinates), and the class provides.
	/// 
	/// Internally, the data is stored to minimize wasting space with sparce data.
	/// Further, the class will generate the placement of cubes itself, so users
	/// must simply request the list of cubes for their use, and tell the class
	/// when a cube has been deleted.
	/// </summary>
	public class StandardRoomCubes : RoomCubes
	{
		// Quick aside - how this actually works:
		//-----------------------------------------
		// Okay, so our goal is to store cubes in the room. Easy way to achieve this:
		// giant 3D array for each possible position a cube could take up in the room.
		//  Pros: simple interface, clear where things are, easy to iterate over
		//	Cons: wastes a massive amount of space holding "air" cubes.
		//
		// Space is a big deal in the game, since we are working with a pretty weak
		// piece of hardware. So, to compensate, we will store our blocks as follows:
		//
		//	---------------------------------
		//	|X| OOOOOOOOOOOOOOOOOOOOOOOOO |X|
		//	|-------------------------------|
		//	|O|                           |O|
		//	|O|                           |O|
		//	|-------------------------------|
		//	|X| OOOOOOOOOOOOOOOOOOOOOOOOO |X|
		//	---------------------------------
		//
		// X regions are corners of the room: these are stored as small 3D arrays,
		// taking up a designated corner width. We're talking on the order of 3x3
		// to 5x5 blocks from floor to ceiling.
		//
		// O regions are the remaining space on the sides of the walls. These are
		// 2D arrays of LinkedLists of cubes - that is, at any position on the wall,
		// there is a list of cubes poking out from that position. The list ends
		// when there are no more cubes from that point on at that grid position.
		//
		// Both X and O regions can contain "air" blocks, since the X regions will
		// need a default value for unused spaces in the 3D array, and O regions
		// require a way to designate blocks beyond where a player has mined other
		// blocks.
		//
		//
		// Ultimately, these data structures may be optimized to handle:
		//	* Doors on a wall
		//	* Corridors (are we putting blocks in corridors? probably, but not now)
		//	* Other possible things
		
		private WallCubes L_Wall;
		private WallCubes R_Wall;
		private WallCubes T_Wall;
		private WallCubes B_Wall;
		private InsideCornerCubes TL_Corner;
		private InsideCornerCubes TR_Corner;
		private InsideCornerCubes BL_Corner;
		private InsideCornerCubes BR_Corner;
		
		private int Width { get; set; }
		private int Depth { get; set; }
		private int Height { get; set; }

		public int WallDepth { get { return L_Wall.MaxDepth; } }
		
		/// <summary>
		/// Constructs a new instance of the RoomCubes data structure, given the dimensions of
		/// the room the cubes are in.
		/// </summary>
		/// <param name="roomWidth">Width of the room</param>
		/// <param name="roomHeight">Height of the room</param>
		/// <param name="ceilingHeight">Height of the ceiling</param>
		public StandardRoomCubes(int roomWidth, int roomHeight, int doorCode, int ceilingHeight,
		                         RoomCubes lftNbr, RoomCubes rgtNbr, RoomCubes upNbr, RoomCubes dwnNbr)
		{
			Width = roomWidth;
			Depth = roomHeight;
			Height = ceilingHeight;
			
			// TODO: something smarter than a fixed value
			int CORNER_DIM = 8;
			int wallDepth = (Depth - (CORNER_DIM * 2)) < 0 ? 0 : (Depth - (CORNER_DIM * 2));
			int wallWidth = (Width - (CORNER_DIM * 2)) < 0 ? 0 : (Width - (CORNER_DIM * 2));
			L_Wall = (doorCode & RogueRoom.LEFT_DOOR_MASK) == 0 ?
				(WallCubes)new StandardWallCubes(wallDepth, Height, CORNER_DIM, 3) :
					(WallCubes)new DoorWallCubes(wallDepth, Height, CORNER_DIM, 3,
					                             RogueRoom.LEFT_DOOR_MASK, lftNbr);
			R_Wall = (doorCode & RogueRoom.RIGHT_DOOR_MASK) == 0 ?
				(WallCubes)new StandardWallCubes(wallDepth, Height, CORNER_DIM, 3) :
					(WallCubes)new DoorWallCubes(wallDepth, Height, CORNER_DIM, 3,
					                             RogueRoom.RIGHT_DOOR_MASK, rgtNbr);
			T_Wall = (doorCode & RogueRoom.UP_DOOR_MASK) == 0 ?
				(WallCubes)new StandardWallCubes(wallWidth, Height, CORNER_DIM, 3) :
					(WallCubes)new DoorWallCubes(wallWidth, Height, CORNER_DIM, 3,
					                             RogueRoom.UP_DOOR_MASK, upNbr);
			B_Wall = (doorCode & RogueRoom.DOWN_DOOR_MASK) == 0 ?
				(WallCubes)new StandardWallCubes(wallWidth, Height, CORNER_DIM, 3) :
					(WallCubes)new DoorWallCubes(wallWidth, Height, CORNER_DIM, 3,
					                             RogueRoom.DOWN_DOOR_MASK, dwnNbr);
			
			TL_Corner = new InsideCornerCubes(L_Wall.MaxDepth, T_Wall.MaxDepth,
			                                  T_Wall.GetLeftEdge(), L_Wall.GetRightEdge());
			BL_Corner = new InsideCornerCubes(B_Wall.MaxDepth, L_Wall.MaxDepth,
			                                  L_Wall.GetLeftEdge(), B_Wall.GetRightEdge());
			TR_Corner = new InsideCornerCubes(T_Wall.MaxDepth, R_Wall.MaxDepth,
			                                  R_Wall.GetLeftEdge(), T_Wall.GetRightEdge());
			BR_Corner = new InsideCornerCubes(R_Wall.MaxDepth, B_Wall.MaxDepth,
			                                  B_Wall.GetLeftEdge(), R_Wall.GetRightEdge());
		}
		
		/// <summary>
		/// Enumerates all cubes in the data structure. There are no guarantees in what
		/// order the cubes will be returned - just that each cube will appear exactly once.
		/// </summary>
		/// <returns>Enumeration of cubes in hte data structure</returns>
		public override IEnumerable<Cube> EnumerateCubes()
		{
			// Corners:
			// top-left:
			foreach (Cube c in TL_Corner.EnumerateCubes())
			{
				// TL_Corner X coordinate is c.X
				//           Y coordinate is c.Z
				//           Z coordinate is c.Y
				yield return new Cube(this, c.Type, c.X, c.Z, c.Y);
			}
			// top-right:
			foreach (Cube c in TR_Corner.EnumerateCubes())
			{
				// TR_Corner X coordinate is Width - c.Y - 1
				//           Y coordinate is c.Z
				//           Z coordinate is c.X
				yield return new Cube(this, c.Type, Width - c.Y - 1, c.Z, c.X);
			}
			// bottom-left:
			foreach (Cube c in BL_Corner.EnumerateCubes())
			{
				// BL_Corner X coordinate is c.Y
				//           Y coordinate is c.Z
				//           Z coordinate is Depth - c.X - 1
				yield return new Cube(this, c.Type, c.Y, c.Z, Depth - c.X - 1);
			}
			// bottom-right:
			foreach (Cube c in BR_Corner.EnumerateCubes())
			{
				// BR_Corner X coordinate is Width - c.X - 1
				//           Y coordinate is c.Z
				//           Z coordinate is Depth - c.Y - 1
				yield return new Cube(this, c.Type, Width - c.X - 1, c.Z, Depth - c.Y - 1);
			}

			// Sides:
			// left:
			foreach (Cube c in L_Wall.EnumerateCubes())
			{
				// Left wall's X coordinate is equal to c.Z
				//             Y coordinate is equal to c.Y
				//             Z coordinate is equal to Depth - BL_Corner.Width - c.X - 1
				yield return new Cube(this, c.Type, c.Z, c.Y, Depth - BL_Corner.Width - c.X - 1);
			}
			// right:
			foreach (Cube c in R_Wall.EnumerateCubes())
			{
				// Right wall's X coordinate is equal to Width - c.Z - 1
				//              Y coordinate is equal to c.Y
				//              Z coordinate is equal to c.X + TR_Corner.Width
				yield return new Cube(this, c.Type, Width - c.Z - 1, c.Y, c.X + TR_Corner.Width);
			}
			// top:
			foreach (Cube c in T_Wall.EnumerateCubes())
			{
				// Top wall's X coordinate is equal to c.X + TL_Corner.Width
				//            Y coordinate is equal to c.Y
				//            Z coordinate is equal to c.Z
				yield return new Cube(this, c.Type, c.X + TL_Corner.Width, c.Y, c.Z);
			}
			// bottom:
			foreach (Cube c in B_Wall.EnumerateCubes())
			{
				// Bottom wall's X coordinate is equal to Width - BR_Corner.Width - c.X - 1
				//               Y coordinate is equal to c.Y
				//               Z coordinate is equal to Depth - c.Z - 1
				yield return new Cube(this, c.Type, Width - BR_Corner.Width - c.X - 1, c.Y, Depth - c.Z - 1);
			}
		}

		public override IEnumerable<Cube> DestroyCube (Cube c)
		{
			// Left side:
			if (c.X < L_Wall.MaxDepth)
			{
				// Top:
				if (c.Z < TL_Corner.Depth)
				{
					int tmp = c.Z;
					c.Z = c.Y;
					c.Y = tmp;
					foreach (Cube uncovered in TL_Corner.DestroyCube(c))
						yield return new Cube(this, uncovered.Type,
						                      uncovered.X, uncovered.Z, uncovered.Y);
				}
				// Center:
				else if (c.Z < TL_Corner.Depth + L_Wall.Width)
				{
					int tmp = c.Z;
					c.Z = c.X;
					c.X = Depth - BL_Corner.Width - 1 - tmp;
					foreach (Cube uncovered in L_Wall.DestroyCube(c))
						yield return new Cube(this, uncovered.Type,
						                      uncovered.Z, uncovered.Y, Depth - BL_Corner.Width - 1 - uncovered.X);
				}
				// Bottom:
				else
				{
					int tmp = c.Z;
					c.Z = c.Y;
					c.Y = c.X;
					c.X = Depth - 1 - tmp;
					foreach (Cube uncovered in TR_Corner.DestroyCube(c))
						yield return new Cube(this, uncovered.Type,
						                      uncovered.Y, uncovered.Z, Depth - 1 - uncovered.X);
				}
			}
			// Center:
			else if (c.X < L_Wall.MaxDepth + T_Wall.Width)
			{
				// Top:
				if (c.Z < TL_Corner.Depth)
				{
					c.X -= TL_Corner.Depth;
					foreach (Cube uncovered in T_Wall.DestroyCube(c))
						yield return new Cube(this, uncovered.Type,
						                      uncovered.X + L_Wall.MaxDepth, uncovered.Y, uncovered.Z);
				}
				// Bottom:
				else
				{
					c.X = Width - BR_Corner.Width - 1 - c.X;
					c.Z = Depth - 1 - c.Z;
					foreach (Cube uncovered in B_Wall.DestroyCube(c))
						yield return new Cube(this, uncovered.Type,
						                      Width - BR_Corner.Width - 1 - uncovered.X, uncovered.Y, Depth - 1 - uncovered.Z);
				}
			}
			// Right side:
			else
			{
				// Top:
				if (c.Z < TL_Corner.Depth)
				{
					int tmp = c.Z;
					c.Z = c.Y;
					c.Y = Width - 1 - c.X;
					c.X = tmp;
					foreach (Cube uncovered in TR_Corner.DestroyCube(c))
						yield return new Cube(this, uncovered.Type,
						                      Width - uncovered.Y - 1, uncovered.Z, uncovered.X);
				}
				// Center:
				else if (c.Z < TL_Corner.Depth + L_Wall.Width)
				{
					int tmp = c.Z;
					c.Z = Width - 1 - c.X;
					c.X = tmp - TR_Corner.Width;
					foreach (Cube uncovered in R_Wall.DestroyCube(c))
						yield return new Cube(this, uncovered.Type,
						                      Width - 1 - uncovered.Z, uncovered.Y, TR_Corner.Width + uncovered.X);
				}
				// Bottom:
				else
				{
					int tmp = c.Z;
					c.Z = c.Y;
					c.Y = Depth - 1 - tmp;
					c.X = Width - 1 - c.X;
					foreach (Cube uncovered in BR_Corner.DestroyCube(c))
						yield return new Cube(this, uncovered.Type,
						                      Width - 1 - uncovered.X, uncovered.Z, Depth - 1 - uncovered.Y);
				}
			}
		}
	}
}

