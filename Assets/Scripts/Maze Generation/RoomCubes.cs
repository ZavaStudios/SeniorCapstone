using UnityEngine;
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
				Air, Stone, Iron, // other ore types...
			}

			public CubeType Type { get; set; }
			public int X { get; set; }
			public int Y { get; set; }
			public int Z { get; set; }

			/*
			public Cube(CubeType _type, int _x, int _y, int _z)
			{
				Type = _type;
				X = _x;
				Y = _y;
				Z = _z;
			}
			*/
		}

		private const int CORNER_WIDTH = 5;
		private const int CORNER_LENGTH = 5;
		private const int ROOM_HEIGHT = RogueRoom.CEILING_HEIGHT;

		private Cube.CubeType[,,] tlCorner;
		private Cube.CubeType[,,] trCorner;
		private Cube.CubeType[,,] blCorner;
		private Cube.CubeType[,,] brCorner;
		private LinkedList<Cube.CubeType>[,] lSide;
		private LinkedList<Cube.CubeType>[,] rSide;
		private LinkedList<Cube.CubeType>[,] tSide;
		private LinkedList<Cube.CubeType>[,] bSide;

		public RoomCubes(int roomWidth, int roomHeight)
		{
			// If the room is small enough that our corners would fill it up, we
			// gain nothing from our system, so just use one corner as the whole
			// room:
			if (roomWidth <= CORNER_WIDTH * 2 && roomHeight <= CORNER_LENGTH * 2)
			{
				tlCorner = new Cube.CubeType[roomWidth, ROOM_HEIGHT, roomHeight];
				trCorner = new Cube.CubeType[0,0,0];
				blCorner = new Cube.CubeType[0,0,0];
				brCorner = new Cube.CubeType[0,0,0];
				tSide = new LinkedList<Cube.CubeType>[0,0];
				bSide = new LinkedList<Cube.CubeType>[0,0];
				lSide = new LinkedList<Cube.CubeType>[0,0];
				rSide = new LinkedList<Cube.CubeType>[0,0];
			}
			// If we're too thin, merge top and bottom, but keep other sides:
			else if (roomWidth <= CORNER_WIDTH * 2)
			{
				tlCorner = new Cube.CubeType[roomWidth, ROOM_HEIGHT, CORNER_LENGTH];
				trCorner = new Cube.CubeType[0,0,0];
				blCorner = new Cube.CubeType[roomWidth, ROOM_HEIGHT, CORNER_LENGTH];
				brCorner = new Cube.CubeType[0,0,0];
				tSide = new LinkedList<Cube.CubeType>[0,0];
				bSide = new LinkedList<Cube.CubeType>[0,0];
				lSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT,roomHeight - (CORNER_LENGTH * 2)];
				rSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT,roomHeight - (CORNER_LENGTH * 2)];
			}
			// If were too short, merge left and right, but keep other sides:
			else if (roomHeight <= CORNER_LENGTH * 2)
			{
				tlCorner = new Cube.CubeType[CORNER_WIDTH, ROOM_HEIGHT, roomHeight];
				blCorner = new Cube.CubeType[0,0,0];
				trCorner = new Cube.CubeType[CORNER_WIDTH, ROOM_HEIGHT, roomHeight];
				brCorner = new Cube.CubeType[0,0,0];
				tSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT,roomWidth - (CORNER_WIDTH * 2)];
				bSide = new LinkedList<Cube.CubeType>[ROOM_HEIGHT,roomWidth - (CORNER_WIDTH * 2)];
				lSide = new LinkedList<Cube.CubeType>[0,0];
				rSide = new LinkedList<Cube.CubeType>[0,0];
			}
			// Otherwise, we're all good to do our usual thing:
			else
			{
				tlCorner = new Cube.CubeType[roomWidth, ROOM_HEIGHT, roomHeight];
				trCorner = new Cube.CubeType[0,0,0];
				blCorner = new Cube.CubeType[0,0,0];
				brCorner = new Cube.CubeType[0,0,0];
				tSide = new LinkedList<Cube.CubeType>[0,0];
				bSide = new LinkedList<Cube.CubeType>[0,0];
				lSide = new LinkedList<Cube.CubeType>[0,0];
				rSide = new LinkedList<Cube.CubeType>[0,0];
			}
		}
	}
}