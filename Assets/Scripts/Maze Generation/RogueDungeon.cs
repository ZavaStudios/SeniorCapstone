using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MazeGeneration
{
    /// <summary>
    /// Represents a dungeon map, in the style of Rogue.
    /// </summary>
    class RogueDungeon
    {
        // Approximate number of slots in the dungeon to be filled with enemy rooms,
        // with 1.0f being 100% of the rooms, and 0.0f being (probably) none of them.
		private const float ENEMY_ROOM_DENSITY = 0.5f;

        // Values deciding how large rooms can be. Specifically,
		// main min/max values describe what the sizes of any room
		// will be constrained to, and other values describe what
		// specific room types are constrained to.
		//
        // Also, note that MIN is inclusive, MAX is exclusive
		public const int CORRIDOR_WIDTH = 16;
		public const int ROOM_CORNER_WIDTH = 8;
		public const int ROOM_CORNER_DEPTH = 8;
		
		public const int ROOM_WIDTH = (ROOM_CORNER_WIDTH * 2) + CORRIDOR_WIDTH + (4 * 2);
		public const int ROOM_DEPTH = (ROOM_CORNER_DEPTH * 2) + CORRIDOR_WIDTH + (4 * 2);
		public const int BOSS_ROOM_WIDTH = (ROOM_CORNER_WIDTH * 2) + CORRIDOR_WIDTH + (10 * 2);
		public const int BOSS_ROOM_DEPTH = (ROOM_CORNER_DEPTH * 2) + CORRIDOR_WIDTH + (10 * 2);
		
		public const int MAX_ROOM_WIDTH = BOSS_ROOM_WIDTH;
		public const int MAX_ROOM_DEPTH = BOSS_ROOM_DEPTH;
		public const int MAX_ROOM_HEIGHT = 5;

		/*
		public const int MIN_ROOM_WIDTH = CORRIDOR_WIDTH;
		public const int MAX_ROOM_WIDTH = CORRIDOR_WIDTH * 4;
		public const int MIN_ROOM_DEPTH = CORRIDOR_WIDTH;
		public const int MAX_ROOM_DEPTH = CORRIDOR_WIDTH * 4;
		public const int MAX_ROOM_HEIGHT = 5;

		// Enemy Rooms
		public const int MIN_ENEMY_ROOM_WIDTH = CORRIDOR_WIDTH * 2;
		public const int MAX_ENEMY_ROOM_WIDTH = CORRIDOR_WIDTH * 2 + (CORRIDOR_WIDTH / 2);
		public const int MIN_ENEMY_ROOM_DEPTH = CORRIDOR_WIDTH * 2;
		public const int MAX_ENEMY_ROOM_DEPTH = CORRIDOR_WIDTH * 2 + (CORRIDOR_WIDTH / 2);
		// Starting / Shop Room
		public const int MIN_SHOP_ROOM_WIDTH = MIN_ENEMY_ROOM_WIDTH;
		public const int MAX_SHOP_ROOM_WIDTH = MAX_ENEMY_ROOM_WIDTH;
		public const int MIN_SHOP_ROOM_DEPTH = MIN_ENEMY_ROOM_DEPTH;
		public const int MAX_SHOP_ROOM_DEPTH = MAX_ENEMY_ROOM_DEPTH;
		// Boss Room
		public const int MIN_BOSS_ROOM_WIDTH = CORRIDOR_WIDTH * 3;
		public const int MAX_BOSS_ROOM_WIDTH = MAX_ROOM_WIDTH;
		public const int MIN_BOSS_ROOM_DEPTH = CORRIDOR_WIDTH * 3;
		public const int MAX_BOSS_ROOM_DEPTH = MAX_ROOM_DEPTH;
		*/

		public RogueRoom[,] Map { get; private set; }

        // Cache the start room
        private int startRoomX;
        private int startRoomY;

		/// <summary>
        /// Generates a new RogueDungeon of the specified width and height.
        /// In this case, "width" and "height" mean the number of potential
        /// rooms horizontally or vertically, respectively.
        /// </summary>
        /// <param name="width">Maximum number of rooms tall the map can be</param>
        /// <param name="height">Maximum number of rooms wide the map can be</param>
        public RogueDungeon(int width, int height)
        {
            // Build a maze, which gives us door values
            bool[,] boolMap = Maze.GenerateMaze(width, height);

			// Use the maze to fill in our map
			int newWidth = boolMap.GetLength(0) - 2;
			int newHeight = boolMap.GetLength(1) - 2;
            Map = new RogueRoom[newWidth, newHeight];

			// Assign initial placements:
			int bossX = 0, bossY = 0;
			int keyX = 0, keyY = 0;
            startRoomX = 0;
            startRoomY = 0;
			for (int x = 1; x < boolMap.GetLength(0); x += 2)
			{
				for (int y = 1; y < boolMap.GetLength(1); y += 2)
				{
					int dirCode = 0;
					dirCode |= (boolMap[x-1,y] ? 0x1 : 0x0);
					dirCode |= (boolMap[x+1,y] ? 0x2 : 0x0);
					dirCode |= (boolMap[x,y-1] ? 0x4 : 0x0);
					dirCode |= (boolMap[x,y+1] ? 0x8 : 0x0);

					if (dirCode == 0x1)
					{
						bossX = x;
						bossY = y;
						startRoomX = x-2;
						startRoomY = y;
					}
					else if (dirCode == 0x2)
					{
						bossX = x;
						bossY = y;
						startRoomX = x+2;
						startRoomY = y;
					}
					else if (dirCode == 0x4)
					{
						bossX = x;
						bossY = y;
						startRoomX = x;
						startRoomY = y-2;
					}
					else if (dirCode == 0x8)
					{
						bossX = x;
						bossY = y;
						startRoomX = x;
						startRoomY = y+2;
					}
				}
			}
			// Find key room:
			Maze.FindFarthest(boolMap, bossX, bossY, out keyX, out keyY);

			// Account for coordinate offset:
			bossX -= 1;
			bossY -= 1;
			startRoomX -= 1;
			startRoomY -= 1;
			keyX -= 1;
			keyY -= 1;
			
            // Instantiate between rooms:
            for (int x = 0; x < newWidth; x += 2)
            {
                for (int y = 0; y < newHeight; y += 2)
                {
                    // CorridorFork room by default
                    int roomWidth = CORRIDOR_WIDTH;
                    int roomDepth = CORRIDOR_WIDTH;
					RogueRoom.RoomType type = RogueRoom.RoomType.corridorFork;

					// If the x,y coordinate was one of our set aside vectors, use that type:
					if (x == startRoomX && y == startRoomY)
					{
						roomWidth = ROOM_WIDTH;//r.Next(MIN_SHOP_ROOM_WIDTH, MAX_SHOP_ROOM_WIDTH-1);
						roomDepth = ROOM_DEPTH;//r.Next (MIN_SHOP_ROOM_DEPTH, MAX_SHOP_ROOM_DEPTH-1);
						type = RogueRoom.RoomType.start;
					}
					else if (x == bossX && y == bossY)
					{
						roomWidth = BOSS_ROOM_WIDTH;//r.Next(MIN_BOSS_ROOM_WIDTH, MAX_BOSS_ROOM_WIDTH-1);
						roomDepth = BOSS_ROOM_DEPTH;//r.Next (MIN_BOSS_ROOM_DEPTH, MAX_BOSS_ROOM_DEPTH-1);
						type = RogueRoom.RoomType.boss;
					}
					else if (x == keyX && y == keyY)
					{
						roomWidth = ROOM_WIDTH;
						roomDepth = ROOM_DEPTH;
						type = RogueRoom.RoomType.keyRoom;
					}
					// TODO: others?

                    // If we decide to place a room here, adjust the width
                    // and height accordingly
                    else if (Maze.rnd.NextDouble() < ENEMY_ROOM_DENSITY)
                    {
						roomWidth = ROOM_WIDTH;//r.Next(MIN_ENEMY_ROOM_WIDTH, MAX_ENEMY_ROOM_WIDTH-1);
						roomDepth = ROOM_DEPTH;//r.Next(MIN_ENEMY_ROOM_DEPTH, MAX_ENEMY_ROOM_DEPTH-1);
						type = RogueRoom.RoomType.enemy;

						// TODO: handle enemy score distribution
                    }

					// We need rooms to be nice values to accomodate corridor widths & cubes.
					// Short version: we need an equal number of blocks on the left and right
					// of every door in the maze, so width-corridor_width as well as
					// height-corridor_width must be even.
					// Adjust accordingly:
					/*
					if ((roomWidth - CORRIDOR_WIDTH) % 2 == 1)
						roomWidth++;
					if ((roomDepth - CORRIDOR_WIDTH) % 2 == 1)
						roomDepth++;
						*/

                    // Determine what the door code should be, by checking the map:
                    int mapCoordX = x + 1;
                    int mapCoordY = y + 1;
                    int doorCode = 0x0;

                    // NOTE: the below operations are safe, because the Maze class guarantees
                    // that the map will have a buffer of false values around where the rooms are.
                    // Left:
                    if (boolMap[mapCoordX - 1, mapCoordY])
						doorCode |= RogueRoom.LEFT_DOOR_MASK;

                    // Right:
                    if (boolMap[mapCoordX + 1, mapCoordY])
						doorCode |= RogueRoom.RIGHT_DOOR_MASK;

                    // Up:
                    if (boolMap[mapCoordX, mapCoordY - 1])
						doorCode |= RogueRoom.UP_DOOR_MASK;

                    // Down:
                    if (boolMap[mapCoordX, mapCoordY + 1])
						doorCode |= RogueRoom.DOWN_DOOR_MASK;

                    // Create the room, and insert it into the map:
					RogueRoom newRoom;
					switch (type)
					{
					case RogueRoom.RoomType.start:
					case RogueRoom.RoomType.shop:
					case RogueRoom.RoomType.empty:
                    case RogueRoom.RoomType.enemy:
                        newRoom = new GeneralRoom(roomWidth, roomDepth, MAX_ROOM_HEIGHT, x, y, doorCode);
                        break;
					case RogueRoom.RoomType.boss:
					case RogueRoom.RoomType.keyRoom:
						newRoom = new GeneralRoom(roomWidth, roomDepth, MAX_ROOM_HEIGHT, x, y, doorCode);
                        SkeletonKingAI.bossRoom = (GeneralRoom)newRoom;
                        ZombiePrinceAI.bossRoom = (GeneralRoom)newRoom;
                        SpiderQueenAI.bossRoom = (GeneralRoom)newRoom;
						break;
					case RogueRoom.RoomType.corridorFork:
					default:
						newRoom = new CorridorBranchRoom(roomWidth, MAX_ROOM_HEIGHT, x, y, doorCode);
						break;
					// This case can't happen:
					//case RogueRoom.RoomType.corridor:
					}
                    newRoom.Type = type;

					// Initialize enemy list to a GeneralRoom. The GeneralRoom handles when it isn't an
                    // enemy room, so we won't worry about that.
					// TODO: smarter distribution of enemy points. For now, just give same value to each room.
					if (newRoom is GeneralRoom)
						((GeneralRoom)newRoom).AssignEnemies(5);
					
                    Map[x, y] = newRoom;
                }
            }

			// Instantiate corridors:
			// Left-Right:
			for (int x = 1; x < newWidth; x += 2)
			{
				for (int y = 0; y < newHeight; y += 2)
				{
					// Determine door codes, and attach relationships:
					int mapCoordX = x + 1;
					int mapCoordY = y + 1;
					int doorCode = 0x0;
					RogueRoom lftNbr = null,
							  rgtNbr = null;
					
					int corridorWidth = CORRIDOR_WIDTH;
					int corridorDepth = CORRIDOR_WIDTH;
					CorridorRoom corridor;
					
					// Confirm this corridor is actually in the maze:
					if (boolMap[mapCoordX, mapCoordY])
					{
						doorCode |= RogueRoom.LEFT_DOOR_MASK;
						lftNbr = Map[x-1, y];
						doorCode |= RogueRoom.RIGHT_DOOR_MASK;
						rgtNbr = Map[x+1,y];
						
						// Determine correct width for this corridor:
						corridorWidth = MAX_ROOM_WIDTH +
										((MAX_ROOM_WIDTH - lftNbr.Width) / 2) +
										((MAX_ROOM_WIDTH - rgtNbr.Width) / 2);
						
						// Instantiate & hookup neighbors:
						corridor = new CorridorRoom(corridorWidth, corridorDepth, MAX_ROOM_HEIGHT, x, y, doorCode);
						lftNbr.RightNeighbor = corridor;
						rgtNbr.LeftNeighbor = corridor;
						corridor.LeftNeighbor = lftNbr;
						corridor.RightNeighbor = rgtNbr;

						// Initialize cubes:
						corridor.InitializeCubes();

						// Put it in the map:
						Map[x,y] = corridor;
					}
					else
					{
						Map[x,y] = null;
					}
				}
			}

				// Up-Down:
			for (int x = 0; x < newWidth; x += 2)
			{
				for (int y = 1; y < newHeight; y += 2)
				{
					// Determine door codes, and attach relationships:
					int mapCoordX = x + 1;
					int mapCoordY = y + 1;
					int doorCode = 0x0;
					RogueRoom upNbr  = null,
							  dwnNbr = null;
					
					int corridorWidth = CORRIDOR_WIDTH;
					int corridorDepth = CORRIDOR_WIDTH;
					CorridorRoom corridor;
					
					// Confirm this corridor is actually in the maze:
					if (boolMap[mapCoordX, mapCoordY])
					{
						doorCode |= RogueRoom.UP_DOOR_MASK;
						upNbr = Map[x, y-1];
						doorCode |= RogueRoom.DOWN_DOOR_MASK;
						dwnNbr = Map[x,y+1];
						
						// Determine correct width for this corridor:
						corridorDepth = MAX_ROOM_DEPTH +
										((MAX_ROOM_DEPTH - upNbr.Depth) / 2) +
										((MAX_ROOM_DEPTH - dwnNbr.Depth) / 2);
						
						// Instantiate & hookup neighbors:
						corridor = new CorridorRoom(corridorWidth, corridorDepth, MAX_ROOM_HEIGHT, x, y, doorCode);
						upNbr.DownNeighbor = corridor;
						dwnNbr.UpNeighbor = corridor;
						corridor.UpNeighbor = upNbr;
						corridor.DownNeighbor = dwnNbr;

						// Initialize cubes:
						corridor.InitializeCubes();

						// Put it in the map:
						Map[x,y] = corridor;
					}
					else
					{
						Map[x,y] = null;
					}
				}
			}

			// Initialize main rooms' cubes:
			for (int x = 0; x < newWidth; x += 2)
			{
				for (int y = 0; y < newHeight; y += 2)
				{
					Map[x,y].InitializeCubes();
				}
			}
		}

        public RogueRoom GetStartRoom()
        {
            return Map[startRoomX, startRoomY];
        }

        public void Update()
        {
            foreach (RogueRoom room in Map)
                if (room != null)
                    room.Update();
        }
        
		public IEnumerable<RogueRoom> EnumerateRooms()
		{
			foreach (RogueRoom room in Map)
				if (room != null)
					yield return room;
		}

        /// <summary>
        /// Given a position, return the room containing that position.
        /// Note: may give a faulty answer if the point doesn't actually lie
        /// inside the maze.
        /// </summary>
        /// <param name="position">Position to search for.</param>
        /// <returns>Room position lies in.</returns>
        public RogueRoom GetCurrentRoom(float x, float y)
        {
            int roomX = (int)(x / MAX_ROOM_WIDTH);
            int roomY = (int)(y / MAX_ROOM_DEPTH);

            RogueRoom ckRm = Map[roomX, roomY];
            Vector3 roomCenter = ckRm.GetCenter(MAX_ROOM_WIDTH, MAX_ROOM_DEPTH);
            
            float dx = roomCenter.x - x;
            float dy = roomCenter.z - y;

            int xAdj = Math.Sign(dx) * (Math.Abs(dx) <= (ckRm.Width * 0.5f) ? 0 : 1);
            int yAdj = Math.Sign(dy) * (Math.Abs(dy) <= (ckRm.Depth * 0.5f) ? 0 : 1);

            if (roomX + xAdj < 0 || roomX + xAdj > Map.GetLength(0) ||
                roomY + yAdj < 0 || roomY + yAdj > Map.GetLength(1))
                return null;
            return Map[roomX + xAdj, roomY + yAdj];
        }

        /// <summary>
        /// Creates a boolean grid view for the entire map, which is, at the very
        /// least, useful for printing out a picture for the time being.
        /// </summary>
        /// <returns>
        /// Boolean map, where false indicates a wall, and true indicates an open
        /// path in the maze.
        /// </returns>
        public bool[,] getGrid()
        {
            // First, to size the grid, we must find the max width/height in each col/row:
            int[] maxWidths = new int[Map.GetLength(0)];
            for (int i = 0; i < Map.GetLength(0); i++)
                maxWidths[i] = -1;

            int[] maxHeights = new int[Map.GetLength(1)];
            for (int i = 0; i < Map.GetLength(1); i++)
                maxHeights[i] = -1;

            for (int x = 0; x < Map.GetLength(0); x++)
            {
                for (int y = 0; y < Map.GetLength(1); y++)
                {
                    RogueRoom room = Map[x, y];
                    // Note: we pad lengths by 2 so we can ensure some walls between rooms
                    maxWidths[x] = (maxWidths[x] < room.Width + 2) ? room.Width + 2 : maxWidths[x];
                    maxHeights[y] = (maxHeights[y] < room.Depth + 2) ? room.Depth + 2 : maxHeights[y];
                }
            }

            // Next, sum these components together:
            int gridWidth = 0;
            int gridHeight = 0;
            for (int x = 0; x < maxWidths.Length; x++)
                gridWidth += maxWidths[x];
            for (int y = 0; y < maxHeights.Length; y++)
                gridHeight += maxHeights[y];

            // Now we finally build our actual grid!
            bool[,] grid = new bool[gridWidth, gridHeight];
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid[x, y] = false;
                }
            }

            int xOffset = 1;
            for (int x = 0; x < maxWidths.Length; x++)
            {
                int yOffset = 1;
                for (int y = 0; y < maxHeights.Length; y++)
                {
                    RogueRoom room = Map[x, y];

                    // First, fill out grid square based on room size
                    for (int x0 = xOffset; x0 < xOffset + room.Width; x0++)
                    {
                        for (int y0 = yOffset; y0 < yOffset + room.Depth; y0++)
                        {
                            grid[x0, y0] = true;
                        }
                    }

                    // Next, extend doors to the outer limits:
                    // NOTE: right now, we're doing a dumb way of things. That is,
                    // the rooms are all affixed to the top-left of their row/col,
                    // and doors will extend from that pixel as well. So, we don't
                    // need to check anything other than right and down doors, and
                    // extend them to the end
                    /*
                    if ((room.Doors & Room.LEFT_DOOR_MASK) != 0)
                    {
                        for (int x0 = xOffset; x0 < xOffset + (maxWidths[x] / 2); x0++)
                            grid[x0, yOffset] = true;
                    }
                     */
					if ((room.DoorCode & RogueRoom.RIGHT_DOOR_MASK) != 0)
                    {
                        for (int x0 = xOffset; x0 < xOffset + maxWidths[x]; x0++)
                            grid[x0, yOffset] = true;
                    }
                    /*
                    if ((room.Doors & Room.UP_DOOR_MASK) != 0)
                    {
                        for (int y0 = yOffset; y0 < yOffset + (maxHeights[y] / 2); y0++)
                            grid[xOffset, y0] = true;
                    }
                     */
					if ((room.DoorCode & RogueRoom.DOWN_DOOR_MASK) != 0)
                    {
                        for (int y0 = yOffset; y0 < yOffset + maxHeights[y]; y0++)
                            grid[xOffset, y0] = true;
                    }

                    // Finally, update offsets:
                    yOffset += maxHeights[y];
                }
                xOffset += maxWidths[x];
            }

            return grid;
        }
    }
}
