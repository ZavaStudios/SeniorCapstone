using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGeneration
{
	/// <summary>
	/// Data object used to represent a single cube in the maze.
	/// </summary>
	public struct Cube
	{
		public enum CubeType
		{
			Air, Stone, Iron, Silver, Gold, Platinum, // other ore types...
		}
		
		public CubeType Type { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
		public int Z { get; set; }
		
		public Cube(CubeType _type, int _x, int _y, int _z) : this()
		{
			Type = _type;
			X = _x;
			Y = _y;
			Z = _z;
		}
	}

	public interface RoomCubes
	{
		IEnumerable<Cube> EnumerateCubes();
	}

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

		/// <summary>
		/// Constructs a new instance of the RoomCubes data structure, given the dimensions of
		/// the room the cubes are in.
		/// </summary>
		/// <param name="roomWidth">Width of the room</param>
		/// <param name="roomHeight">Height of the room</param>
		/// <param name="ceilingHeight">Height of the ceiling</param>
		public StandardRoomCubes(int roomWidth, int roomHeight, int doorCode, int ceilingHeight)
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
					 (WallCubes)new DoorWallCubes(wallDepth, Height, CORNER_DIM, 3);
			R_Wall = (doorCode & RogueRoom.RIGHT_DOOR_MASK) == 0 ?
                     (WallCubes)new StandardWallCubes(wallDepth, Height, CORNER_DIM, 3) :
                     (WallCubes)new DoorWallCubes(wallDepth, Height, CORNER_DIM, 3);
			T_Wall = (doorCode & RogueRoom.UP_DOOR_MASK) == 0 ?
                     (WallCubes)new StandardWallCubes(wallDepth, Height, CORNER_DIM, 3) :
                     (WallCubes)new DoorWallCubes(wallDepth, Height, CORNER_DIM, 3);
			B_Wall = (doorCode & RogueRoom.DOWN_DOOR_MASK) == 0 ?
                     (WallCubes)new StandardWallCubes(wallDepth, Height, CORNER_DIM, 3) :
                     (WallCubes)new DoorWallCubes(wallDepth, Height, CORNER_DIM, 3);
			
			TL_Corner = new InsideCornerCubes(L_Wall, T_Wall);
			BL_Corner = new InsideCornerCubes(B_Wall, L_Wall);
			TR_Corner = new InsideCornerCubes(T_Wall, R_Wall);
			BR_Corner = new InsideCornerCubes(R_Wall, B_Wall);
		}

		/// <summary>
		/// Enumerates all cubes in the data structure. There are no guarantees in what
		/// order the cubes will be returned - just that each cube will appear exactly once.
		/// </summary>
		/// <returns>Enumeration of cubes in hte data structure</returns>
		public IEnumerable<Cube> EnumerateCubes()
		{
			// Corners:
				// top-left:
			foreach (Cube c in TL_Corner.EnumerateCubes())
			{
				// TL_Corner X coordinate is c.X
				//           Y coordinate is c.Z
				//           Z coordinate is c.Y
				yield return new Cube(c.Type, c.X, c.Z, c.Y);
			}
				// top-right:
			foreach (Cube c in TR_Corner.EnumerateCubes())
			{
				// TR_Corner X coordinate is Width - c.Y - 1
				//           Y coordinate is c.Z
				//           Z coordinate is c.X
				yield return new Cube(c.Type, Width - c.Y - 1, c.Z, c.X);
			}

				// bottom-left:
			foreach (Cube c in BL_Corner.EnumerateCubes())
			{
				// BL_Corner X coordinate is c.Y
				//           Y coordinate is c.Z
				//           Z coordinate is Depth - c.X - 1
				yield return new Cube(c.Type, c.Y, c.Z, Depth - c.X - 1);
			}
				// bottom-right:
			foreach (Cube c in BR_Corner.EnumerateCubes())
			{
				// BR_Corner X coordinate is Width - c.X - 1
				//           Y coordinate is c.Z
				//           Z coordinate is Depth - c.Y - 1
				yield return new Cube(c.Type, Width - c.X - 1, c.Z, Depth - c.Y - 1);
			}

			// Sides:
				// left:
			foreach (Cube c in L_Wall.EnumerateCubes())
			{
				// Left wall's X coordinate is equal to c.Z
				//             Y coordinate is equal to c.Y
				//             Z coordinate is equal to Depth - BL_Corner.Width - c.X - 1
				yield return new Cube(c.Type, c.Z, c.Y, Depth - BL_Corner.Width - c.X - 1);
			}
				// right:
			foreach (Cube c in R_Wall.EnumerateCubes())
			{
				// Right wall's X coordinate is equal to Width - c.Z - 1
				//              Y coordinate is equal to c.Y
				//              Z coordinate is equal to c.X + TR_Corner.Width
				yield return new Cube(c.Type, Width - c.Z - 1, c.Y, c.X + TR_Corner.Width);
			}
				// top:
			foreach (Cube c in T_Wall.EnumerateCubes())
			{
				// Top wall's X coordinate is equal to c.X + TL_Corner.Width
				//            Y coordinate is equal to c.Y
				//            Z coordinate is equal to c.Z
				yield return new Cube(c.Type, c.X + TL_Corner.Width, c.Y, c.Z);
			}
				// bottom:
			foreach (Cube c in B_Wall.EnumerateCubes())
			{
				// Bottom wall's X coordinate is equal to Width - BR_Corner.Width - c.X - 1
				//               Y coordinate is equal to c.Y
				//               Z coordinate is equal to Depth - c.Z - 1
				yield return new Cube(c.Type, Width - BR_Corner.Width - c.X - 1, c.Y, Depth - c.Z - 1);
			}
		}
	}

	/// <summary>
	/// Room cube setup for rooms too small for the typical RoomCubes structure.
	/// </summary>
	public class SmallRoomCubes : RoomCubes
	{
		public SmallRoomCubes(int width, int height, int doorCode, int ceilingHeight)
		{
			// TODO
		}

		public IEnumerable<Cube> EnumerateCubes()
		{
			// TODO
			yield return new Cube(Cube.CubeType.Air, 1, 1, 1);
		}
	}

	public interface WallCubes
	{
		int Width { get; }
		int Height { get; }
		int MaxDepth { get; }
		int GetDepthAt(int x, int y);
		IEnumerable<Cube> EnumerateCubes();
	}

	/// <summary>
	/// Describes cubes placed along a wall in a room. Allows the user to
	/// create a wall with arbitrary width and height, as well as a custom
	/// cap on how far out from the wall cubes are allowed to be placed.
	/// </summary>
	public class StandardWallCubes : WallCubes
	{
		private LinkedList<Cube.CubeType>[,] Cubes { get; set; }
		
		public int MaxDepth { get; private set; }
		public int MinDepth { get; private set; }
		public int Width
		{
			get { return Cubes.GetLength(0); }
		}
		public int Height
		{
			get { return Cubes.GetLength(1); }
		}

		public StandardWallCubes(int width, int height, int maxDepth, int minDepth)
		{
			MaxDepth = maxDepth;
			MinDepth = minDepth;
			Cubes = new LinkedList<Cube.CubeType>[width,height];
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					Cubes[x,y] = new LinkedList<Cube.CubeType>();
				}
			}

			InitializeCubes();
		}

		private void InitializeCubes()
		{
			// Randomizer:
			System.Random r = new System.Random();
			
			int[,] noise = PerlinNoise.GenerateNoise128();
			for (int x = 0; x < Cubes.GetLength(0); x++)
			{
				for (int y = 0; y < Cubes.GetLength(1); y++)
				{
					// TODO: better indexing. We could average nearby values or something.
					int xIndex = (int)(((float)x / (float)Cubes.GetLength(0)) * 127.0f);
					int yIndex = (int)(((float)y / (float)Cubes.GetLength(1)) * 127.0f);
					float tmpDepth =(float)noise[xIndex,yIndex] * 0.01f;
					int depth = MinDepth + (int)((float)(MaxDepth - MinDepth) * tmpDepth);
					// HACK: for now, perlin noise is still busted. We don't want to get more than
					// our corner sizes (or bad things happen), so clamp the value:
					depth = (depth > MaxDepth) ? MaxDepth : depth;
					for (int z = 0; z < depth; z++)
					{
						// TODO: generate cube type more nicely
						Cubes[x,y].AddLast(Cube.CubeType.Silver);
					}
				}
			}
		}

		/// <summary>
		/// Returns the depth of the buffer at the specified location.
		/// </summary>
		/// <returns>Depth of the buffer at [x,y].</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public int GetDepthAt(int x, int y)
		{
			// If we have no cubes in our grid (one of the dims is 0), just return 0
			if (Width == 0 || Height == 0)
				return 0;
			return Cubes[x,y].Count;
		}

		public IEnumerable<Cube> EnumerateCubes()
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					int z = 0;
					foreach (Cube.CubeType type in Cubes[x,y])
					{
						yield return new Cube(type, x, y, z);
						z++;
					}
				}
			}
		}
	}

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
	public class InsideCornerCubes
	{
		private WallCubes DownWall;
		private WallCubes RightWall;
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

		public InsideCornerCubes(WallCubes downWall, WallCubes rightWall)
		{
			DownWall = downWall;
			RightWall = rightWall;

			int width = DownWall.MaxDepth;
			int height = DownWall.Height;
			int depth = RightWall.MaxDepth;
			Cubes = new Cube.CubeType[width, depth, height];

			InitializeCubes();
		}

		private void InitializeCubes()
		{
			for (int z = 0; z < Height; z++)
			{
				// Quadrants: Only one is interesting
				int quadX = DownWall.GetDepthAt(DownWall.Width-1, z);
				int quadY = RightWall.GetDepthAt(0, z);
				// Quadrants 1 and 2 (merged for convenience)
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						// TODO: smarter cube type selection
						Cubes[x,y,z] = Cube.CubeType.Iron;
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						Cubes[x,y,z] = Cube.CubeType.Gold;
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