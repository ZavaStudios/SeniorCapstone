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
	public class RoomCubes
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

		public const int CORNER_WIDTH = 8;
		public const int CORNER_LENGTH = 8;
		private int ROOM_HEIGHT;

		private Cube.CubeType[,,] tlCorner;
		private Cube.CubeType[,,] trCorner;
		private Cube.CubeType[,,] blCorner;
		private Cube.CubeType[,,] brCorner;
		private LinkedList<Cube.CubeType>[,] lSide;
		private LinkedList<Cube.CubeType>[,] rSide;
		private LinkedList<Cube.CubeType>[,] tSide;
		private LinkedList<Cube.CubeType>[,] bSide;

		private int Width { get; set; }
		private int Height { get; set; }

		/// <summary>
		/// Constructs a new instance of the RoomCubes data structure, given the dimensions of
		/// the room the cubes are in.
		/// </summary>
		/// <param name="roomWidth">Width of the room</param>
		/// <param name="roomHeight">Height of the room</param>
		/// <param name="ceilingHeight">Height of the ceiling</param>
		public RoomCubes(int roomWidth, int roomHeight, int doorCode, int ceilingHeight)
		{
			ROOM_HEIGHT = ceilingHeight;
			Width = roomWidth;
			Height = roomHeight;

			// Code to indicate which type of initialization we're performing
			int initCode = 0;

			// If the room is small enough that our corners would fill it up, we
			// gain nothing from our system, so just use one corner as the whole
			// room:
			if (Width <= CORNER_WIDTH * 2 && Height <= CORNER_LENGTH * 2)
			{
				tlCorner = new Cube.CubeType[Width, ROOM_HEIGHT, Height];
				trCorner = new Cube.CubeType[0,0,0];
				blCorner = new Cube.CubeType[0,0,0];
				brCorner = new Cube.CubeType[0,0,0];
				tSide = new LinkedList<Cube.CubeType>[0,0];
				bSide = new LinkedList<Cube.CubeType>[0,0];
				lSide = new LinkedList<Cube.CubeType>[0,0];
				rSide = new LinkedList<Cube.CubeType>[0,0];
				initCode = 1;
			}
			// If we're too thin, merge top and bottom, but keep other sides:
			else if (Width <= CORNER_WIDTH * 2)
			{
				tlCorner = new Cube.CubeType[Width, ROOM_HEIGHT, CORNER_LENGTH];
				trCorner = new Cube.CubeType[0,0,0];
				blCorner = new Cube.CubeType[Width, ROOM_HEIGHT, CORNER_LENGTH];
				brCorner = new Cube.CubeType[0,0,0];
				tSide = new LinkedList<Cube.CubeType>[0,0];
				bSide = new LinkedList<Cube.CubeType>[0,0];
				lSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT,Height - (CORNER_LENGTH * 2)];
				rSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT,Height - (CORNER_LENGTH * 2)];
				initCode = 2;
			}
			// If were too short, merge left and right, but keep other sides:
			else if (Height <= CORNER_LENGTH * 2)
			{
				tlCorner = new Cube.CubeType[CORNER_WIDTH, ROOM_HEIGHT, Height];
				blCorner = new Cube.CubeType[0,0,0];
				trCorner = new Cube.CubeType[CORNER_WIDTH, ROOM_HEIGHT, Height];
				brCorner = new Cube.CubeType[0,0,0];
				tSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT,Width - (CORNER_WIDTH * 2)];
				bSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT,Width - (CORNER_WIDTH * 2)];
				lSide = new LinkedList<Cube.CubeType>[0,0];
				rSide = new LinkedList<Cube.CubeType>[0,0];
				initCode = 3;
			}
			// Otherwise, we're all good to do our usual thing:
			else
			{
				tlCorner = new Cube.CubeType[CORNER_WIDTH, ROOM_HEIGHT, CORNER_LENGTH];
				trCorner = new Cube.CubeType[CORNER_WIDTH, ROOM_HEIGHT, CORNER_LENGTH];
				blCorner = new Cube.CubeType[CORNER_WIDTH, ROOM_HEIGHT, CORNER_LENGTH];
				brCorner = new Cube.CubeType[CORNER_WIDTH, ROOM_HEIGHT, CORNER_LENGTH];
				tSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT, Width - (CORNER_WIDTH * 2)];
				bSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT, Width - (CORNER_WIDTH * 2)];
				lSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT, Height - (CORNER_LENGTH * 2)];
				rSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT, Height - (CORNER_LENGTH * 2)];
				initCode = 0;
			}

			// Fill in linkedLists into sides:
			// left:
			for (int z = 0; z < lSide.GetLength(0); z++)
			{
				for (int y = 0; y < lSide.GetLength(1); y++)
				{
					lSide[z,y] = new LinkedList<Cube.CubeType>();
				}
			}
			// right:
			for (int z = 0; z < rSide.GetLength(0); z++)
			{
				for (int y = 0; y < rSide.GetLength(1); y++)
				{
					rSide[z,y] = new LinkedList<Cube.CubeType>();
				}
			}
			// top:
			for (int z = 0; z < tSide.GetLength(0); z++)
			{
				for (int x = 0; x < tSide.GetLength(1); x++)
				{
					tSide[z,x] = new LinkedList<Cube.CubeType>();
				}
			}
			// bottom:
			for (int z = 0; z < bSide.GetLength(0); z++)
			{
				for (int x = 0; x < bSide.GetLength(1); x++)
				{
					bSide[z,x] = new LinkedList<Cube.CubeType>();
				}
			}
			
			// Initialize some cube values!
			InitializeCubes(doorCode, initCode);
		}

		/// <summary>
		/// Forwards our init request to the appropriate helper function, based on
		/// which case of initialization we are in.
		/// 
		/// Init codes are as follows:
		/// 	0 -- Standard case (4 corners, 4 walls)
		/// 	1 -- Smallest case (1 corner, nothing else)
		/// 	2 -- Thin case (Top and Bottom merge, Left and Right sides exist)
		/// 	3 -- Short case (Left and Right merge, Top and Bottom sides exist)
		/// </summary>
		/// <param name="doorCode">Door code.</param>
		/// <param name="initCode">Init code.</param>
		public void InitializeCubes(int doorCode, int initCode)
		{
			switch(initCode)
			{
			case 0:
				InitializeCubes0(doorCode);
				break;
			case 1:
				InitializeCubes1(doorCode);
				break;
			case 2:
				InitializeCubes2(doorCode);
				break;
			case 3:
			default:
				InitializeCubes3(doorCode);
				break;
			}
		}

		/// <summary>
		/// Determines initial setup of the cubes in the room, assuming we are in
		/// the case where only one corner (top-right) is being used to store cubes.
		/// </summary>
		/// <param name="doorCode">Door code.</param>
		private void InitializeCubes1(int doorCode)
		{
			// TODO: something reasonable. For now, just don't place anything
			for (int z = 0; z < trCorner.GetLength(1); z++)
			{
				for (int x = 0; x < trCorner.GetLength(0); x++)
				{
					for (int y = 0; y < trCorner.GetLength(2); y++)
					{
						trCorner[x,z,y] = Cube.CubeType.Air;
					}
				}
			}
		}

		/// <summary>
		/// Determines initial setup of the cubes in the room, assuming we are in
		/// the case where the Top and Bottom are merged into one corner (top-right
		/// and bottom-right, respectively), and Left and Right sides exist.
		/// </summary>
		/// <param name="doorCode">Door code.</param>
		private void InitializeCubes2(int doorCode)
		{
			// TODO: anything. I'm actually pretty sure we guarantee this case never happens for now,
			// so I'm willing to let this code hang out for now
		}

		/// <summary>
		/// Determines initial setup of the cubes in the room, assuming we are in
		/// the case where the Left and Right are merged into one corner (top-left
		/// and top-right, respectively), and Top and Bottom sides exist.
		/// </summary>
		/// <param name="doorCode">Door code.</param>
		private void InitializeCubes3(int doorCode)
		{
			// TODO: anything. I'm actually pretty sure we guarantee this case never happens for now,
			// so I'm willing to let this code hang out for now
		}

		/// <summary>
		/// Determines the initial setup of cubes in the room, assuming we are in our
		/// standard case (4 corners, 4 sides).
		/// </summary>
		/// <param name="doorCode">Door code.</param>
		private void InitializeCubes0(int doorCode)
		{
			// Randomizer:
			Random r = new Random();

			// Sides:
				// left:
			int[,] noise = PerlinNoise.GenerateNoise128();
			for (int z = 0; z < lSide.GetLength(0); z++)
			{
				for (int y = 0; y < lSide.GetLength(1); y++)
				{
					// Don't place if it's in the doorway:
					int modX = 0;
					int modY = y + tlCorner.GetLength(2);
					if ((doorCode & RogueRoom.LEFT_DOOR_MASK) != 0)
						if (inLeftCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.UP_DOOR_MASK) != 0)
						if (inUpCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.RIGHT_DOOR_MASK) != 0)
						if (inRightCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.DOWN_DOOR_MASK) != 0)
						if (inDownCorridor(modX, modY))
							continue;

					// TODO: better indexing. We could average nearby values or something.
					int zIndex = (int)(((float)z / (float)lSide.GetLength(0)) * 127.0f);
					int yIndex = (int)(((float)y / (float)lSide.GetLength(1)) * 127.0f);
					int depth = (int)((float)noise[zIndex,yIndex] * 0.01f * (float)CORNER_WIDTH);
					// HACK: for now, perlin noise is still busted. We don't want to get more than
					// our corner sizes (or bad things happen), so clamp the value:
					depth = (depth > CORNER_WIDTH) ? CORNER_WIDTH : depth;
					for (int x = 0; x < depth; x++)
					{
						lSide[z,y].AddLast(Cube.CubeType.Silver);
					}
				}
			}
				// right:
			noise = PerlinNoise.GenerateNoise128();
			for (int z = 0; z < rSide.GetLength(0); z++)
			{
				for (int y = 0; y < rSide.GetLength(1); y++)
				{
					// Don't place if it's in the doorway:
					int modX = tlCorner.GetLength(0) + tSide.GetLength(1) + trCorner.GetLength(0);
					int modY = y + tlCorner.GetLength(2);
					if ((doorCode & RogueRoom.LEFT_DOOR_MASK) != 0)
						if (inLeftCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.UP_DOOR_MASK) != 0)
						if (inUpCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.RIGHT_DOOR_MASK) != 0)
						if (inRightCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.DOWN_DOOR_MASK) != 0)
						if (inDownCorridor(modX, modY))
							continue;

					// TODO: better indexing. We could average nearby values or something.
					int zIndex = (int)(((float)z / (float)rSide.GetLength(0)) * 127.0f);
					int yIndex = (int)(((float)y / (float)rSide.GetLength(1)) * 127.0f);
					int depth = (int)((float)noise[zIndex,yIndex] * 0.01f * (float)CORNER_WIDTH);
					// HACK: for now, perlin noise is still busted. We don't want to get more than
					// our corner sizes (or bad things happen), so clamp the value:
					depth = (depth > CORNER_WIDTH) ? CORNER_WIDTH : depth;
					for (int x = 0; x < depth; x++)
					{
						rSide[z,y].AddLast(Cube.CubeType.Stone);
					}
				}
			}
				// top:
			noise = PerlinNoise.GenerateNoise128();
			for (int z = 0; z < tSide.GetLength(0); z++)
			{
				for (int x = 0; x < tSide.GetLength(1); x++)
				{
					// Don't place if it's in the doorway:
					int modX = x + tlCorner.GetLength(0);
					int modY = 0;
					if ((doorCode & RogueRoom.LEFT_DOOR_MASK) != 0)
						if (inLeftCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.UP_DOOR_MASK) != 0)
						if (inUpCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.RIGHT_DOOR_MASK) != 0)
						if (inRightCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.DOWN_DOOR_MASK) != 0)
						if (inDownCorridor(modX, modY))
							continue;
					
					// TODO: better indexing. We could average nearby values or something.
					int zIndex = (int)(((float)z / (float)tSide.GetLength(0)) * 127.0f);
					int xIndex = (int)(((float)x / (float)tSide.GetLength(1)) * 127.0f);
					int depth = (int)((float)noise[zIndex,xIndex] * 0.01f * (float)CORNER_WIDTH);
					// HACK: for now, perlin noise is still busted. We don't want to get more than
					// our corner sizes (or bad things happen), so clamp the value:
					depth = (depth > CORNER_LENGTH) ? CORNER_LENGTH : depth;
					for (int y = 0; y < depth; y++)
					{
						tSide[z,x].AddLast(Cube.CubeType.Stone);
					}
				}
			}
			// bottom:
			for (int z = 0; z < bSide.GetLength(0); z++)
			{
				for (int x = 0; x < bSide.GetLength(1); x++)
				{
					// Don't place if it's in the doorway:
					int modX = x + tlCorner.GetLength(0);
					int modY = tlCorner.GetLength(2) + lSide.GetLength(1) + blCorner.GetLength(2);
					if ((doorCode & RogueRoom.LEFT_DOOR_MASK) != 0)
						if (inLeftCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.UP_DOOR_MASK) != 0)
						if (inUpCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.RIGHT_DOOR_MASK) != 0)
						if (inRightCorridor(modX, modY))
							continue;
					if ((doorCode & RogueRoom.DOWN_DOOR_MASK) != 0)
						if (inDownCorridor(modX, modY))
							continue;

					// TODO: better indexing. We could average nearby values or something.
					int zIndex = (int)(((float)z / (float)bSide.GetLength(0)) * 127.0f);
					int xIndex = (int)(((float)x / (float)bSide.GetLength(1)) * 127.0f);
					int depth = (int)((float)noise[zIndex,xIndex] * 0.01f * (float)CORNER_WIDTH);
					// HACK: for now, perlin noise is still busted. We don't want to get more than
					// our corner sizes (or bad things happen), so clamp the value:
					depth = (depth > CORNER_LENGTH) ? CORNER_LENGTH : depth;
					for (int y = 0; y < depth; y++)
					{
						bSide[z,x].AddLast(Cube.CubeType.Stone);
					}
				}
			}

			// Corners:
				// top-left:
			for (int z = 0; z < tlCorner.GetLength(1); z++)
			{
				// Quadrants: Only one is interesting
				int quadX = lSide[z, 0].Count;
				int quadY = tSide[z, 0].Count;
				// Quadrants 1 and 2 (merged for convenience)
				for (int x = 0; x < CORNER_WIDTH; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						tlCorner[x,z,y] = Cube.CubeType.Iron;
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < CORNER_LENGTH; y++)
					{
						tlCorner[x,z,y] = Cube.CubeType.Gold;
					}
				}
				// Quadrant 4
				for (int x = quadX; x < CORNER_WIDTH; x++)
				{
					for (int y = quadY; y < CORNER_LENGTH; y++)
					{
						// TODO
						tlCorner[x,z,y] = Cube.CubeType.Air;
					}
				}
			}

				// top-right:
			for (int z = 0; z < trCorner.GetLength(1); z++)
			{
				// Quadrants: Only one is interesting
				int quadX = rSide[z, 0].Count;
				int quadY = tSide[z, tSide.GetLength(1)-1].Count;
				// Quadrants 1 and 2 (merged for convenience)
				for (int x = 0; x < CORNER_WIDTH; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						trCorner[CORNER_WIDTH-1-x,z,y] = Cube.CubeType.Iron;
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < CORNER_LENGTH; y++)
					{
						trCorner[CORNER_WIDTH-1-x,z,y] = Cube.CubeType.Gold;
					}
				}
				// Quadrant 4
				for (int x = quadX; x < CORNER_WIDTH; x++)
				{
					for (int y = quadY; y < CORNER_LENGTH; y++)
					{
						// TODO
						trCorner[CORNER_WIDTH-1-x,z,y] = Cube.CubeType.Air;
					}
				}
			}
				// bottom-left:
			for (int z = 0; z < blCorner.GetLength(1); z++)
			{
				// Quadrants: Only one is interesting
				int quadX = lSide[z, lSide.GetLength(1)-1].Count;
				int quadY = bSide[z, 0].Count;
				// Quadrants 1 and 2 (merged for convenience)
				for (int x = 0; x < CORNER_WIDTH; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						blCorner[x,z,CORNER_LENGTH-1-y] = Cube.CubeType.Iron;
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < CORNER_LENGTH; y++)
					{
						blCorner[x,z,CORNER_LENGTH-1-y] = Cube.CubeType.Gold;
					}
				}
				// Quadrant 4
				for (int x = quadX; x < CORNER_WIDTH; x++)
				{
					for (int y = quadY; y < CORNER_LENGTH; y++)
					{
						// TODO
						blCorner[x,z,CORNER_LENGTH-1-y] = Cube.CubeType.Air;
					}
				}
			}
				// bottom-right:
			for (int z = 0; z < brCorner.GetLength(1); z++)
			{
				// Quadrants: Only one is interesting
				int quadX = rSide[z, rSide.GetLength(1)-1].Count;
				int quadY = bSide[z, bSide.GetLength(1)-1].Count;
				// Quadrants 1 and 2 (merged for convenience)
				for (int x = 0; x < CORNER_WIDTH; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						brCorner[CORNER_WIDTH-1-x,z,CORNER_LENGTH-1-y] = Cube.CubeType.Iron;
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < CORNER_LENGTH; y++)
					{
						brCorner[CORNER_WIDTH-1-x,z,CORNER_LENGTH-1-y] = Cube.CubeType.Gold;
					}
				}
				// Quadrant 4
				for (int x = quadX; x < CORNER_WIDTH; x++)
				{
					for (int y = quadY; y < CORNER_LENGTH; y++)
					{
						// TODO
						brCorner[CORNER_WIDTH-1-x,z,CORNER_LENGTH-1-y] = Cube.CubeType.Air;
					}
				}
			}
		}

		private bool inLeftCorridor(int x, int y)
		{
			int corMinPos = (Height - RogueDungeon.CORRIDOR_WIDTH) / 2;
			int corMaxPos = (Height + RogueDungeon.CORRIDOR_WIDTH) / 2;

			return ((y < corMaxPos) && (y >= corMinPos) && (x <= Width / 2));
		}

		private bool inUpCorridor(int x, int y)
		{
			int corMinPos = (Width - RogueDungeon.CORRIDOR_WIDTH) / 2;
			int corMaxPos = (Width + RogueDungeon.CORRIDOR_WIDTH) / 2;
			
			return ((x < corMaxPos) && (x >= corMinPos) && (y <= Height / 2));
		}

		private bool inRightCorridor(int x, int y)
		{
			int corMinPos = (Height - RogueDungeon.CORRIDOR_WIDTH) / 2;
			int corMaxPos = (Height + RogueDungeon.CORRIDOR_WIDTH) / 2;
			
			return ((y < corMaxPos) && (y >= corMinPos) && (x > Width / 2));
		}

		private bool inDownCorridor(int x, int y)
		{
			int corMinPos = (Width - RogueDungeon.CORRIDOR_WIDTH) / 2;
			int corMaxPos = (Width + RogueDungeon.CORRIDOR_WIDTH) / 2;

			return ((x < corMaxPos) && (x >= corMinPos) && (y > Height / 2));
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
			for (int x = 0; x < tlCorner.GetLength(0); x++)
			{
				for (int z = 0; z < tlCorner.GetLength(1); z++)
				{
					for (int y = 0; y < tlCorner.GetLength(2); y++)
					{
						yield return new Cube(tlCorner[x,z,y], x, y, z);
					}
				}
			}
				// top-right:
			for (int x = 0; x < trCorner.GetLength(0); x++)
			{
				for (int z = 0; z < trCorner.GetLength(1); z++)
				{
					for (int y = 0; y < trCorner.GetLength(2); y++)
					{
						yield return new Cube(trCorner[x,z,y],
						                      x + tlCorner.GetLength(0) + tSide.GetLength(1), y, z);
					}
				}
			}
			// bottom-left:
			for (int x = 0; x < blCorner.GetLength(0); x++)
			{
				for (int z = 0; z < blCorner.GetLength(1); z++)
				{
					for (int y = 0; y < blCorner.GetLength(2); y++)
					{
						yield return new Cube(blCorner[x,z,y],
						                      x, y + tlCorner.GetLength(2) + lSide.GetLength(1), z);
					}
				}
			}
				// bottom-right:
			for (int x = 0; x < brCorner.GetLength(0); x++)
			{
				for (int z = 0; z < brCorner.GetLength(1); z++)
				{
					for (int y = 0; y < brCorner.GetLength(2); y++)
					{
						yield return new Cube(brCorner[x,z,y],
						                      x + tlCorner.GetLength(0) + tSide.GetLength(1),
						                      y + tlCorner.GetLength(2) + lSide.GetLength(1), z);
					}
				}
			}

			// Sides:
				// left:
			for (int z = 0; z < lSide.GetLength(0); z++)
			{
				for (int y = 0; y < lSide.GetLength(1); y++)
				{
					int x = 0;
					foreach (Cube.CubeType type in lSide[z,y])
					{
						yield return new Cube(type,
						                      x, y + tlCorner.GetLength(2), z);
						x++;
					}
				}
			}
				// right:
			for (int z = 0; z < rSide.GetLength(0); z++)
			{
				for (int y = 0; y < rSide.GetLength(1); y++)
				{
					int x = tlCorner.GetLength(0) + tSide.GetLength(1) + trCorner.GetLength(0) - 1;
					foreach (Cube.CubeType type in rSide[z,y])
					{
						yield return new Cube(type,
						                      x, y + trCorner.GetLength(2), z);
						x--;
					}
				}
			}
				// top:
			for (int z = 0; z < tSide.GetLength(0); z++)
			{
				for (int x = 0; x < tSide.GetLength(1); x++)
				{
					int y = 0;
					foreach (Cube.CubeType type in tSide[z,x])
					{
						yield return new Cube(type,
						                      x + tlCorner.GetLength(0), y, z);
						y++;
					}
				}
			}
				// bottom:
			for (int z = 0; z < bSide.GetLength(0); z++)
			{
				for (int x = 0; x < bSide.GetLength(1); x++)
				{
					int y = tlCorner.GetLength(2) + lSide.GetLength(1) + blCorner.GetLength(2) - 1;
					foreach (Cube.CubeType type in bSide[z,x])
					{
						yield return new Cube(type,
						                      x + blCorner.GetLength(0), y, z);
						y--;
					}
				}
			}
		}
	}
}