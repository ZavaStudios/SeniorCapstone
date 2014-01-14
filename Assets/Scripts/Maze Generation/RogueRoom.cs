﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Represents one room in a RougeDungeon.
/// 
/// TODO: make this potentially more sophisticated. For now, the room
/// is simply going to be variable width / height, of unit sizes
/// (which can then be used dynamically by the system to size precisely
/// as desired).
/// </summary>
public class RogueRoom
{
	public const int UP_DOOR_MASK = 0x01;
	public const int DOWN_DOOR_MASK = 0x02;
	public const int LEFT_DOOR_MASK = 0x04;
	public const int RIGHT_DOOR_MASK = 0x08;

	public const float CEILING_HEIGHT = 5.0f;

	public Transform floor_tile;
	public Transform wall_tile;
	public Transform mine_cube;
	public Transform ore_cube;

	public enum RoomType
	{
		empty, enemy, start, corridor,
	}
	
	public RogueRoom(int width, int height, int doors)
	{
		Type = RoomType.empty;
		Width = width;
		Height = height;
		Doors = doors;
	}
	
	/// <summary>
	/// Represents the width of the room
	/// </summary>
	public int Width
	{
		get;
		set;
	}
	
	/// <summary>
	/// Represents the height of the room
	/// </summary>
	public int Height
	{
		get;
		set;
	}
	
	/// <summary>
	/// Represents which doors are exits from this room. The
	/// specific doors are available through the static bitmask
	/// fields. A value is on when the door exists, and off if
	/// it is not.
	/// </summary>
	public int Doors
	{
		get;
		set;
	}

	/// <summary>
	/// Stores what type of room this object represents. For example,
	/// this room may be the boss room, the starting room, or an
	/// empty corridor.
	/// </summary>
	public RoomType Type
	{
		get;
		set;
	}

	// TODO: store the blocks currently in the room somehow.
	// I'm uncertain what sort of data structure to use for this.

	/// <summary>
	/// Builds game objects to represent this room in Unity, and loads them
	/// into the current game. Expects a scalar size to describe how large
	/// to create these objects - the measurement is how large in Unity units
	/// should one block be considered.
	/// 
	/// Further, because this room is intended to be a part of a larger map,
	/// and corridors will be drawn as "part of the room", we also need the
	/// size of the surrounding box that this room fits into (beyond the width
	/// and height of this room). Also, it needs a notion of what the room's
	/// position is, which is obtained via the center in block-size-coords.
	/// </summary>
	/// <param name="sizeOfBlockUnit">Scalar to describe how large one block is in terms of Unity units</param>
	/// <param name="center">Position of the center of the room in block-lengths</param>
	/// <param name="totalHeight">Height of surrounding space in block-lengths</param>
	/// <param name="totalWidth">Width of surrounding space in block-lengths</param> 
	public void LoadRoom(float sizeOfBlockUnit, Vector2 center, int totalHeight, int totalWidth)
	{
		// Spawn main walls. TODO: handle doors! :o

		// LEFT
		wall_tile.transform.localScale = new Vector3(Height * sizeOfBlockUnit,
		                                             CEILING_HEIGHT,
		                                             1.0f);
		Instantiate(wall_tile,
		            (center + new Vector3(-(float)Width / 2.0f, CEILING_HEIGHT / 2.0f, 0.0f)) * sizeOfBlockUnit,
		            Quaternion.AngleAxis(90.0f, Vector3.up));
		
		// TOP
		wall_tile.transform.localScale = new Vector3(Width * sizeOfBlockUnit,
		                                             CEILING_HEIGHT,
		                                             1.0f);
		Instantiate(wall_tile,
		            (center + new Vector3(0.0f, CEILING_HEIGHT / 2.0f, -(float)Height / 2.0f)) * sizeOfBlockUnit,
		            Quaternion.identity);

		// RIGHT
		wall_tile.transform.localScale = new Vector3(Height * sizeOfBlockUnit,
		                                             CEILING_HEIGHT,
		                                             1.0f);
		Instantiate(wall_tile,
		            (center + new Vector3((float)Width / 2.0f, CEILING_HEIGHT / 2.0f, 0.0f)) * sizeOfBlockUnit,
		            Quaternion.AngleAxis(270.0f, Vector3.up));
		
		// BOTTOM
		wall_tile.transform.localScale = new Vector3(Width * sizeOfBlockUnit,
		                                             CEILING_HEIGHT,
		                                             1.0f);
		Instantiate(wall_tile,
		            (center + new Vector3(0.0f, CEILING_HEIGHT / 2.0f, (float)Height / 2.0f)) * sizeOfBlockUnit,
		            Quaternion.AngleAxis(180.0f, Vector3.up));
	}
}