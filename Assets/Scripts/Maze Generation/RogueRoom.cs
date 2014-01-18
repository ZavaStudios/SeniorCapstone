using UnityEngine;
using System.Collections;

namespace MazeGeneration
{
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

		public const int CEILING_HEIGHT = 5;

		public Transform floor_tile;
		public Transform wall_tile;
		public Transform mine_cube;
		public Transform ore_cube;

		public enum RoomType
		{
			empty, enemy, start, corridor,
		}

		public RoomCubes Cubes
		{
			get; private set;
		}
		
		public RogueRoom(int width, int height, int doors)
		{
			Type = RoomType.empty;
			Width = width;
			Height = height;
			Doors = doors;
			Cubes = new RoomCubes(width, height, CEILING_HEIGHT);
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
		public void LoadRoom(float sizeOfBlockUnit, int gridPosX, int gridPosY,
		                     int totalHeight, int totalWidth, int corridorWidth)
		{
			// Spawn main walls.
			// TODO: spawn doors, spawn ores, and more(s)!
			
			float wallHeight = 0.0f;
			float bufferWidth = 1.0f;
			Vector3 center = new Vector3(((float)totalWidth + bufferWidth) * ((float)gridPosX + 0.5f),
			                             0.0f,
			                             ((float)totalHeight + bufferWidth)  * ((float)gridPosY + 0.5f));

			float ceilingHeight = CEILING_HEIGHT * sizeOfBlockUnit;

			floor_tile.transform.localScale = new Vector3((float)totalWidth + bufferWidth,
			                                              0.0f,
			                                              (float)totalHeight + bufferWidth) * sizeOfBlockUnit;
			MonoBehaviour.Instantiate(floor_tile, center * sizeOfBlockUnit, Quaternion.identity);

			// LEFT
			//	if there is no door here:
			if ((Doors & LEFT_DOOR_MASK) == 0)
			{
				InstantiateWall(Height * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-(float)Width / 2.0f, wallHeight, 0.0f)) * sizeOfBlockUnit,
							    Quaternion.AngleAxis(90.0f, Vector3.up));
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Height - corridorWidth) * 0.5f;
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-Width * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + corridorWidth) * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(90.0f, Vector3.up));
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-Width * 0.5f,
				                      			      wallHeight,
				                      				  -(wallLength + corridorWidth) * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(90.0f, Vector3.up));

				// Corridors
				wallLength = ((float)totalWidth + corridorWidth - (float)Width) * 0.5f;
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  corridorWidth * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(180.0f, Vector3.up));
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  -corridorWidth * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.identity);
			}
			
			// TOP
			//	if there is no door here:
			if ((Doors & UP_DOOR_MASK) == 0)
			{
				InstantiateWall(Width * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(0.0f, wallHeight, -(float)Height / 2.0f)) * sizeOfBlockUnit,
				                Quaternion.identity);
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Width - corridorWidth) * 0.5f;
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3((wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  -Height * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.identity);
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  -Height * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.identity);

				// Corridors
				wallLength = ((float)totalHeight + corridorWidth - (float)Height) * 0.5f;
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + Height) * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(270.0f, Vector3.up));
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + Height) * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(90.0f, Vector3.up));
			}

			// RIGHT
			//	if there is no door here:
			if ((Doors & RIGHT_DOOR_MASK) == 0)
			{
				InstantiateWall(Height * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3((float)Width / 2.0f, wallHeight, 0.0f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(270.0f, Vector3.up));
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Height - corridorWidth) * 0.5f;
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(Width * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + corridorWidth) * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(270.0f, Vector3.up));
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(Width * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + corridorWidth) * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(270.0f, Vector3.up));

				// Corridors
				wallLength = ((float)totalWidth + corridorWidth - (float)Width) * 0.5f;
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3((wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  corridorWidth * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(180.0f, Vector3.up));
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3((wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  -corridorWidth * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.identity);
			}
			
			// BOTTOM
			//	if there is no door here:
			if ((Doors & DOWN_DOOR_MASK) == 0)
			{
				InstantiateWall(Width * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(0.0f, wallHeight, (float)Height / 2.0f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(180.0f, Vector3.up));
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Width - corridorWidth) * 0.5f;
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3((wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  Height * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(180.0f, Vector3.up));
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  Height * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(180.0f, Vector3.up));

				// Corridors
				wallLength = ((float)totalHeight + corridorWidth - (float)Height) * 0.5f;
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + Height) * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(270.0f, Vector3.up));
				InstantiateWall(wallLength * sizeOfBlockUnit,
				                ceilingHeight,
				                (center + new Vector3(-corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + Height) * 0.5f)) * sizeOfBlockUnit,
				                Quaternion.AngleAxis(90.0f, Vector3.up));
			}

			// CUBES
			Vector3 cubeStart = center - 
								new Vector3((float)Width * 0.5f, 0.0f, (float)Height * 0.5f) +
								new Vector3(0.5f, 0.5f, 0.5f);
			foreach (RoomCubes.Cube cube in Cubes.EnumerateCubes())
			{
				InstantiateCube(cube, cubeStart, sizeOfBlockUnit);
			}
		}

		private void InstantiateWall(float wallWidth, float ceilingHeight, Vector3 position, Quaternion angle)
		{
			wall_tile.transform.localScale = new Vector3(wallWidth,
			                                             ceilingHeight,
			                                             1.0f);
			MonoBehaviour.Instantiate(wall_tile,
			            			  position,
			            			  angle);
		}

		private void InstantiateCube(RoomCubes.Cube cube, Vector3 cubeStart, float sizeOfBlockUnit)
		{
			MonoBehaviour.Instantiate(ore_cube,
			                          (new Vector3(cube.X, cube.Z, cube.Y) + cubeStart),
			                          Quaternion.identity);
		}
	}
}