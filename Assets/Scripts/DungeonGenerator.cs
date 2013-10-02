using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MazeGeneration
{
    // Very much like the Maze class, but also inserts rooms scattered about the labyrinth
    class DungeonGenerator
    {
        private class Wall
        {
            public enum Direction
            {
                Left, Up, Right, Down
            }

            public int X
            {
                get;
                set;
            }
            public int Y
            {
                get;
                set;
            }
            public Direction Dir
            {
                get;
                set;
            }

            public Wall(int x, int y, Direction dir)
            {
                X = x;
                Y = y;
                Dir = dir;
            }

            public override string ToString()
            {
                return "(" + X + ", " + Y + ", " + Dir.ToString() + ")";
            }
        }

        public Boolean[,] tiles
        {
            get;
            private set;
        }

        public int width
        {
            get;
            private set;
        }
        public int height
        {
            get;
            private set;
        }

        public DungeonGenerator(int _width, int _height)
        {
            // We must have the maze be odd for the outer walls to work, so we'll add one if necessary.
            width = _width + ((_width % 2 == 1) ? 0 : 1);
            height = _height + ((_height % 2 == 1) ? 0 : 1);

            tiles = new Boolean[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    tiles[x, y] = false;
        }

        // Attempts to generate a maze starting at the given x,y start position. Returns
        // false if the start position is invalid, true otherwise.
        public Boolean GenerateMaze(int startX, int startY)
        {
            // Input sanitation: If the startX startY falls outside legal bounds (either is even, or outside the bounds),
            // adjust appropriately
            if (startX % 2 == 0)
                startX++;
            if (startX <= 0)
                startX = 1;
            if (startX > width - 1)
                startX = width - 2;

            if (startY % 2 == 0)
                startY++;
            if (startY <= 0)
                startY = 1;
            if (startY > height - 1)
                startY = height - 2;

            System.Random r = new System.Random();
            
            // Scatter some rooms about:
            // Room widths can be static, since if they are too large, the roomCount will simply be 0 anyway
            int roomWidth = 19;
            int roomHeight = 19;
            int roomCount = (width * height) / (roomWidth * roomHeight) / 5;
            LinkedList<Vector2> roomCorners = new LinkedList<Vector2>();

            // TODO: Technically, it's possible to cut off part of the maze if someone got really unlucky
            // with their room placement. Should do something to avoid that. It's pretty much impossible, though,
            // so let's not concern ourselves too much with it.
            for (int i = 0; i < roomCount; i++)
            {
                // Given some padding to adjust for potential even increments
                int roomX = r.Next(1, width-roomWidth-2);
                int roomY = r.Next(1, height-roomHeight-2);

                // Adjust so they are odd:
                roomX += (roomX % 2 == 1) ? 1 : 0;
                roomY += (roomY % 2 == 1) ? 1 : 0;
                
                // Confirm our corners don't overlap previously placed rooms:
                while (tiles[roomX, roomY] || tiles[roomX + roomWidth - 1, roomY] ||
                       tiles[roomX, roomY + roomHeight - 1] || tiles[roomX + roomWidth - 1, roomY + roomHeight - 1])
                {
                    roomX = r.Next(1, width - roomWidth - 2);
                    roomY = r.Next(1, height - roomHeight - 2);

                    // Adjust so they are odd:
                    roomX += (roomX % 2 == 1) ? 1 : 0;
                    roomY += (roomY % 2 == 1) ? 1 : 0;
                }

                // Eliminate these tiles:
                for (int x = roomX; x < roomX + roomWidth; x++)
                    for (int y = roomY; y < roomY + roomHeight; y++)
                        tiles[x,y] = true;

                // Track these points so we can finish up the rooms later
                roomCorners.AddFirst(new Vector2(roomX, roomY));
            }

            LinkedList<Wall> WallList = new LinkedList<Wall>();
            tiles[startX, startY] = true;
            foreach (Wall wall in GetAdjacentWalls(startX, startY))
                WallList.AddFirst(wall);

            while (WallList.Count > 0)
            {
                // Pick random wall:
                Wall wall = WallList.ElementAt(r.Next(WallList.Count));
                WallList.Remove(wall);

                // Check next room:
                Vector2 nextTile;
                switch (wall.Dir)
                {
                    case Wall.Direction.Down:
                        nextTile = new Vector2(wall.X, wall.Y - 1);
                        break;

                    case Wall.Direction.Left:
                        nextTile = new Vector2(wall.X - 1, wall.Y);
                        break;

                    case Wall.Direction.Right:
                        nextTile = new Vector2(wall.X + 1, wall.Y);
                        break;

                    case Wall.Direction.Up:
                    default:
                        nextTile = new Vector2(wall.X, wall.Y + 1);
                        break;
                }

                // If the next room was not already opened up, we'll do that now!
                if (!tiles[(int)nextTile.x, (int)nextTile.y])
                {
                    tiles[wall.X, wall.Y] = true;
                    tiles[(int)nextTile.x, (int)nextTile.y] = true;
                    foreach (Wall newWall in GetAdjacentWalls((int)nextTile.x, (int)nextTile.y))
                        WallList.AddFirst(newWall);
                }
            }
            
            // Clean up our rooms:
            foreach (Vector2 roomCorner in roomCorners)
            {
                // Padding to avoid corners:
                int topDoor = r.Next((int)roomCorner.x + 1, (int)roomCorner.x + roomWidth - 1);
                int botDoor = r.Next((int)roomCorner.x + 1, (int)roomCorner.x + roomWidth - 1);
                for (int x = (int)roomCorner.x; x < (int)roomCorner.x + roomWidth; x++)
                {
                    tiles[x, (int)roomCorner.y] = x == botDoor;
                    tiles[x, (int)roomCorner.y + roomHeight - 1] = x == topDoor;
                }
                // Likewise, padding to avoid corners:
                int lftDoor = r.Next((int)roomCorner.y + 1, (int)roomCorner.y + roomHeight - 1);
                int rgtDoor = r.Next((int)roomCorner.y + 1, (int)roomCorner.y + roomHeight - 1);
                for (int y = (int)roomCorner.y; y < (int)roomCorner.y + roomWidth; y++)
                {
                    tiles[(int)roomCorner.x, y] = y == lftDoor;
                    tiles[(int)roomCorner.x + roomWidth - 1, y] = y == rgtDoor;
                }
            }

            return true;
        }

        // Helper method: returns collection of adjacent cells. Useful, since it
        // handles edge cases for you! :D
        private IEnumerable<Wall> GetAdjacentWalls(int x, int y)
        {
            LinkedList<Wall> toRet = new LinkedList<Wall>();

            // Check: if we move over a few slots (through a wall, if you will), will we be on the boundary?
            if (x - 1 > 0)
                toRet.AddFirst(new Wall(x - 1, y, Wall.Direction.Left));
            if (x + 1 < width - 1)
                toRet.AddFirst(new Wall(x + 1, y, Wall.Direction.Right));
            if (y - 1 > 0)
                toRet.AddFirst(new Wall(x, y - 1, Wall.Direction.Down));
            if (y + 1 < height - 1)
                toRet.AddFirst(new Wall(x, y + 1, Wall.Direction.Up));

            return toRet;
        }
    }
}
