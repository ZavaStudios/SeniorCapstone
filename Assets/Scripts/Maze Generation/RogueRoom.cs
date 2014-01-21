using UnityEngine;
using System.Collections;

namespace MazeGeneration
{
	/// <summary>
	/// Represents one room in a RougeDungeon.
	/// Beyond tracking what type the room is, as well as its dimensions, the room
	/// is also responsible for tracking what mineable blocks are stored inside, and
	/// handles spawning / unspawning itself from the Unity environment upon request.
	/// </summary>
	public class RogueRoom
	{
		public const int UP_DOOR_MASK = 0x01;
		public const int DOWN_DOOR_MASK = 0x02;
		public const int LEFT_DOOR_MASK = 0x04;
		public const int RIGHT_DOOR_MASK = 0x08;

		public const int CEILING_HEIGHT = 5;

		public Transform enemy;
		public Transform floor_tile;
		public Transform wall_tile;
		public Transform mine_cube;
		public Transform ore_cube;

		public enum RoomType
		{
			empty, enemy, start, corridor, boss, shop, // TODO: others?
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
			Cubes = new RoomCubes(width, height, Doors, CEILING_HEIGHT);
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

		// TODO: list of enemies in the room. For now, placeholder with just a counter
		public int EnemyCount
		{
			get;
			set;
		}

		/// <summary>
		/// Assigns enemies to this room based on the assigned enemyScore.
		/// Short version of how this works: various enemy types are worth differing
		/// numbers of points. E.g., a spider may be worth 1 while a skeleton is worth 5.
		/// This function randomly determines which type of enemies to spawn in the room
		/// based on how many points are assigned to the room.
		/// </summary>
		/// <param name="enemyScore">Number of enemy points allocated to this room</param>
		public void AssignEnemies(int enemyScore)
		{
			// For now, just assign one "enemy" per enemy score:
			EnemyCount = enemyScore;
		}

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
		public void LoadRoom(int gridPosX, int gridPosY, int totalHeight,
		                     int totalWidth, int corridorWidth)
		{
			// Spawn main walls.
			// TODO: spawn doors, spawn ores, and more(s)!
			
			float wallHeight = 0.0f;
			float bufferWidth = corridorWidth;
			Vector3 center = new Vector3(((float)totalWidth + bufferWidth) * ((float)gridPosX + 0.5f),
			                             0.0f,
			                             ((float)totalHeight + bufferWidth)  * ((float)gridPosY + 0.5f));

			float ceilingHeight = CEILING_HEIGHT;

			floor_tile.transform.localScale = new Vector3((float)totalWidth + bufferWidth,
			                                              0.0f,
			                                              (float)totalHeight + bufferWidth);
			MonoBehaviour.Instantiate(floor_tile, center, Quaternion.identity);
			MonoBehaviour.Instantiate(floor_tile,
			                          center + new Vector3(0.0f, ceilingHeight, 0.0f),
			                          Quaternion.AngleAxis (180.0f, Vector3.forward));

			// LEFT
			//	if there is no door here:
			if ((Doors & LEFT_DOOR_MASK) == 0)
			{
				InstantiateWall(Height,
				                ceilingHeight,
				                (center + new Vector3(-(float)Width / 2.0f, wallHeight, 0.0f)),
							    Quaternion.AngleAxis(90.0f, Vector3.up));
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Height - corridorWidth) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-Width * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + corridorWidth) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up));
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-Width * 0.5f,
				                      			      wallHeight,
				                      				  -(wallLength + corridorWidth) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up));

				// Corridors
				wallLength = ((float)totalWidth + bufferWidth - (float)Width) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  corridorWidth * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up));
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  -corridorWidth * 0.5f)),
				                Quaternion.identity);
			}
			
			// TOP
			//	if there is no door here:
			if ((Doors & UP_DOOR_MASK) == 0)
			{
				InstantiateWall(Width,
				                ceilingHeight,
				                (center + new Vector3(0.0f, wallHeight, -(float)Height / 2.0f)),
				                Quaternion.identity);
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Width - corridorWidth) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3((wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  -Height * 0.5f)),
				                Quaternion.identity);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  -Height * 0.5f)),
				                Quaternion.identity);

				// Corridors
				wallLength = ((float)totalHeight + bufferWidth - (float)Height) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + Height) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up));
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + Height) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up));
			}

			// RIGHT
			//	if there is no door here:
			if ((Doors & RIGHT_DOOR_MASK) == 0)
			{
				InstantiateWall(Height,
				                ceilingHeight,
				                (center + new Vector3((float)Width / 2.0f, wallHeight, 0.0f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up));
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Height - corridorWidth) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(Width * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + corridorWidth) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up));
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(Width * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + corridorWidth) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up));

				// Corridors
				wallLength = ((float)totalWidth + bufferWidth - (float)Width) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3((wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  corridorWidth * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up));
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3((wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  -corridorWidth * 0.5f)),
				                Quaternion.identity);
			}
			
			// BOTTOM
			//	if there is no door here:
			if ((Doors & DOWN_DOOR_MASK) == 0)
			{
				InstantiateWall(Width,
				                ceilingHeight,
				                (center + new Vector3(0.0f, wallHeight, (float)Height / 2.0f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up));
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Width - corridorWidth) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3((wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  Height * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up));
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  Height * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up));

				// Corridors
				wallLength = ((float)totalHeight + bufferWidth - (float)Height) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + Height) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up));
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + Height) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up));
			}

			// CUBES
			Vector3 cubeStart = center - 
								new Vector3((float)Width * 0.5f, 0.0f, (float)Height * 0.5f) +
								new Vector3(0.5f, 0.5f, 0.5f);
			foreach (RoomCubes.Cube cube in Cubes.EnumerateCubes())
			{
				if (cube.Type != RoomCubes.Cube.CubeType.Air)
					InstantiateCube(cube, cubeStart);
			}

			// ENEMIES / ETC.
			if (Type == RoomType.enemy)
			{
				// TODO: this won't be how enemies work long term, but for now just spawn it from count
				for (int i = 0; i < EnemyCount; i++)
				{
					InstantiateEnemy(center);
				}
			}
		}

		/// <summary>
		/// Spawns a wall in Unity given a position, width, height, and angle.
		/// </summary>
		/// <param name="wallWidth">Horizontal length of the wall.</param>
		/// <param name="ceilingHeight">Vertical height of the wall.</param>
		/// <param name="position">Position of the wall (center of the quad).</param>
		/// <param name="angle">Angle about which to rotate the wall.</param>
		private void InstantiateWall(float wallWidth, float ceilingHeight, Vector3 position, Quaternion angle)
		{
			wall_tile.transform.localScale = new Vector3(wallWidth,
			                                             ceilingHeight,
			                                             1.0f);
			MonoBehaviour.Instantiate(wall_tile,
			                          position + new Vector3(0.0f, ceilingHeight * 0.5f, 0.0f),
			            			  angle);
		}

		/// <summary>
		/// Spawns a cube in Unity given a Cube object (holding the type of cube, and (x,y,z)
		/// coordinate position of the cube in the room) and a vector denoting where (0,0,0)
		/// lies in Unity space.
		/// </summary>
		/// <param name="cube">Cube object you would like to spawn.</param>
		/// <param name="cubeStart">Offset of (0,0,0) in Unity space.</param>
		private void InstantiateCube(RoomCubes.Cube cube, Vector3 cubeStart)
		{
			MonoBehaviour.Instantiate(ore_cube,
			                          (new Vector3(cube.X, cube.Z, cube.Y) + cubeStart),
			                          Quaternion.identity);
		}
		
		/// <summary>
		/// Places an enemy in the room at the given floor position. Assumes
		/// that the Y-coordinate is zero, and will lift the enemy up to
		/// accomodate for that.
		/// </summary>
		/// <param name="position">Floor position to place enemy at.</param>
		private void InstantiateEnemy(Vector3 position)
		{
			MonoBehaviour.Instantiate(enemy,
			                          position + new Vector3(0.0f, enemy.collider.bounds.center.y, 0.0f),
			                          Quaternion.identity);
		}

		/// <summary>
		/// Returns center coordinates of the floor of this room.
		/// </summary>
		/// <returns>The center.</returns>
		/// <param name="gridPosX">X position of this room in the dungeon map.</param>
		/// <param name="gridPosY">Y position of this room in the dungeon map.</param>
		/// <param name="totalHeight">Maximum height of a room.</param>
		/// <param name="totalWidth">Maximum width of a room.</param>
		/// <param name="corridorWidth">Width of a corridor.</param>
		public Vector3 GetCenter(int gridPosX, int gridPosY, int totalHeight,
		                         int totalWidth, int corridorWidth)
		{
			float bufferWidth = corridorWidth;
			return new Vector3(((float)totalWidth + bufferWidth) * ((float)gridPosX + 0.5f),
	                             0.0f,
			                     ((float)totalHeight + bufferWidth)  * ((float)gridPosY + 0.5f));
		}
	}
}