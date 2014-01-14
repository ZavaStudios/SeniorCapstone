using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MazeGeneration
{
    /// <summary>
    /// Represents a dungeon map, in the style of Rogue.
    /// </summary>
    class RogueDungeon
    {
        // Approximate number of slots in the dungeon to be filled with rooms,
        // with 1.0f being 100% of the rooms, and 0.0f being (probably) none
        // of them.
        private const float ROOM_DENSITY = 0.5f;

        // Values deciding how large a standard room can be. These can be
        // tweaked, but the desirable characteristics are:
        //      1) The rooms are noticebaly large than hallways
        //      2) The rooms are not absurdly huge (for a wide number of reasons)
        // Also, note that MIN is inclusive, MAX is exclusive
        public const int MIN_ROOM_WIDTH = 3;
        public const int MAX_ROOM_WIDTH = 10;
        public const int MIN_ROOM_HEIGHT = 3;
        public const int MAX_ROOM_HEIGHT = 10;

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
            Map = new RogueRoom[width, height];
            Random r = new Random();
			bool placedFirstRoom = false;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Corridors have unit width
                    int roomWidth = 1;
                    int roomHeight = 1;
                    RogueRoom.RoomType type = RogueRoom.RoomType.corridor;

                    // If we decide to place a room here, adjust the width
                    // and height accordingly
                    if (r.NextDouble() < ROOM_DENSITY)
                    {
                        roomWidth = r.Next(MIN_ROOM_WIDTH, MAX_ROOM_WIDTH);
                        roomHeight = r.Next(MIN_ROOM_HEIGHT, MAX_ROOM_HEIGHT);

                        // Determine room type at random:
                        if (!placedFirstRoom)
                        {
                            type = RogueRoom.RoomType.start;
                            placedFirstRoom = true;
                        }
                        else if (r.NextDouble() < 0.5)
                            type = RogueRoom.RoomType.enemy;
                        else
                            type = RogueRoom.RoomType.empty;
                    }

                    // Determine what the door code should be, by checking the map:
                    int mapCoordX = (2 * x) + 1;
                    int mapCoordY = (2 * y) + 1;
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
                    RogueRoom newRoom = new RogueRoom(roomWidth, roomHeight, doorCode);
                    newRoom.Type = type;
					
                    Map[x, y] = newRoom;
                }
            }
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
                    maxHeights[y] = (maxHeights[y] < room.Height + 2) ? room.Height + 2 : maxHeights[y];
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
                        for (int y0 = yOffset; y0 < yOffset + room.Height; y0++)
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
                    if ((room.Doors & RogueRoom.RIGHT_DOOR_MASK) != 0)
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
                    if ((room.Doors & RogueRoom.DOWN_DOOR_MASK) != 0)
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

        public RogueRoom[,] Map
        {
            get;
            private set;
        }
    }
}
