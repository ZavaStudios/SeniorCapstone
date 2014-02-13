﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		
		public Transform skeleton;
		public Transform zombie;
		public Transform spider;
		public Transform floor_tile;
		public Transform wall_tile;
		public Transform mine_cube;
		public Transform ore_cube;
		public Transform ore2_cube;
		public Transform ore3_cube;
		public Transform ore4_cube;

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

			Cubes = Width > RogueDungeon.CORRIDOR_WIDTH ?
				    (RoomCubes)new StandardRoomCubes(width, height, Doors, CEILING_HEIGHT) :
					(RoomCubes)new SmallRoomCubes(width, height, Doors, CEILING_HEIGHT);
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

		/// <summary>
		/// Stores the types of enemies held in this room.
		/// </summary>
		public List<EnemyGenerator.EnemyType> Enemies
		{
			get;
			private set;
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
			Enemies = EnemyGenerator.generateEnemies(enemyScore);
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
		                     int totalWidth, int corridorWidth, float scalar)
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
			                                              (float)totalHeight + bufferWidth) * scalar;
			MonoBehaviour.Instantiate(floor_tile, center * scalar, Quaternion.identity);
			MonoBehaviour.Instantiate(floor_tile,
			                          (center + new Vector3(0.0f, ceilingHeight, 0.0f)) * scalar,
			                          Quaternion.AngleAxis (180.0f, Vector3.forward));

			// LEFT
			//	if there is no door here:
			if ((Doors & LEFT_DOOR_MASK) == 0)
			{
				InstantiateWall(Height,
				                ceilingHeight,
				                (center + new Vector3(-(float)Width / 2.0f, wallHeight, 0.0f)),
							    Quaternion.AngleAxis(90.0f, Vector3.up),
				                scalar);
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
				                Quaternion.AngleAxis(90.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-Width * 0.5f,
				                      			      wallHeight,
				                      				  -(wallLength + corridorWidth) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up),
				                scalar);

				// Corridors
				wallLength = ((float)totalWidth + bufferWidth - (float)Width) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  corridorWidth * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  -corridorWidth * 0.5f)),
				                Quaternion.identity,
				                scalar);
			}
			
			// TOP
			//	if there is no door here:
			if ((Doors & UP_DOOR_MASK) == 0)
			{
				InstantiateWall(Width,
				                ceilingHeight,
				                (center + new Vector3(0.0f, wallHeight, -(float)Height / 2.0f)),
				                Quaternion.identity,
				                scalar);
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
				                Quaternion.identity,
				                scalar);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  -Height * 0.5f)),
				                Quaternion.identity,
				                scalar);

				// Corridors
				wallLength = ((float)totalHeight + bufferWidth - (float)Height) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + Height) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + Height) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up),
				                scalar);
			}

			// RIGHT
			//	if there is no door here:
			if ((Doors & RIGHT_DOOR_MASK) == 0)
			{
				InstantiateWall(Height,
				                ceilingHeight,
				                (center + new Vector3((float)Width / 2.0f, wallHeight, 0.0f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up),
				                scalar);
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
				                Quaternion.AngleAxis(270.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(Width * 0.5f,
				                      				  wallHeight,
				                      				  -(wallLength + corridorWidth) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up),
				                scalar);

				// Corridors
				wallLength = ((float)totalWidth + bufferWidth - (float)Width) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3((wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  corridorWidth * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3((wallLength + Width) * 0.5f,
				                      				  wallHeight,
				                      				  -corridorWidth * 0.5f)),
				                Quaternion.identity,
				                scalar);
			}
			
			// BOTTOM
			//	if there is no door here:
			if ((Doors & DOWN_DOOR_MASK) == 0)
			{
				InstantiateWall(Width,
				                ceilingHeight,
				                (center + new Vector3(0.0f, wallHeight, (float)Height / 2.0f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up),
				                scalar);
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
				                Quaternion.AngleAxis(180.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-(wallLength + corridorWidth) * 0.5f,
				                      				  wallHeight,
				                      				  Height * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up),
				                scalar);

				// Corridors
				wallLength = ((float)totalHeight + bufferWidth - (float)Height) * 0.5f;
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + Height) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                ceilingHeight,
				                (center + new Vector3(-corridorWidth * 0.5f,
				                      				  wallHeight,
				                      				  (wallLength + Height) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up),
				                scalar);
			}

			// CUBES
			Vector3 cubeStart = center - 
								new Vector3((float)Width * 0.5f, 0.0f, (float)Height * 0.5f) +
								new Vector3(0.5f, 0.5f, 0.5f);

			foreach (Cube cube in Cubes.EnumerateCubes())
			{
				InstantiateCube(cube, cubeStart, scalar);
			}

			// ENEMIES / ETC.
			if (Type == RoomType.enemy)
			{
				System.Random enemyPosGen = new System.Random();
				//Generate a random enemy in the maze based on what the generate enemy function returns.
				//Need to store the enemies into a list to be used if we need to reload the room. 
				foreach(EnemyGenerator.EnemyType enemy in Enemies)
				{
					// TODO: check if collides with ore pillar, if those get added

					//float posX = center.x +
					//			 ((float)((enemyPosGen.NextDouble() - 0.5) * 2.0) *
					//	 		 ((float)(Width * 0.5) - RoomCubes.CORNER_WIDTH));
					//float posY = center.z +
					//			 ((float)((enemyPosGen.NextDouble() - 0.5) * 2.0) *
					//	 		 ((float)(Height * 0.5) - RoomCubes.CORNER_LENGTH));
					//Vector3 enemyPos = new Vector3(posX, 0.2f, posY);
					//InstantiateEnemy(center, enemy, scalar);
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
		/// <param name="scalar">Factor by which to scale 1 block to.<c/param> 
		private void InstantiateWall(float wallWidth, float ceilingHeight,
		                             Vector3 position, Quaternion angle, float scalar)
		{
			wall_tile.transform.localScale = new Vector3(wallWidth,
			                                             ceilingHeight,
			                                             1.0f) * scalar;
			MonoBehaviour.Instantiate(wall_tile,
			                          (position + new Vector3(0.0f, ceilingHeight * 0.5f, 0.0f)) * scalar,
			            			  angle);
		}

		/// <summary>
		/// Spawns a cube in Unity given a Cube object (holding the type of cube, and (x,y,z)
		/// coordinate position of the cube in the room) and a vector denoting where (0,0,0)
		/// lies in Unity space.
		/// </summary>
		/// <param name="cube">Cube object you would like to spawn.</param>
		/// <param name="cubeStart">Offset of (0,0,0) in Unity space.</param>
		/// <param name="scalar">Value to scale 1 block to.</param>
		private void InstantiateCube(Cube cube, Vector3 cubeStart, float scalar)
		{
			Transform toSpawn;
			switch (cube.Type)
			{
			case Cube.CubeType.Stone:
				toSpawn = mine_cube;
				break;
			case Cube.CubeType.Iron:
				toSpawn = ore_cube;
				break;
			case Cube.CubeType.Silver:
				toSpawn = ore2_cube;
				break;
			case Cube.CubeType.Gold:
				toSpawn = ore3_cube;
				break;
			case Cube.CubeType.Platinum:
				toSpawn = ore4_cube;
				break;
			case Cube.CubeType.Air:
			default:
				return;
			}
			toSpawn.transform.localScale = Vector3.one * scalar;
			MonoBehaviour.Instantiate(toSpawn,
			                          (new Vector3(cube.X, cube.Y, cube.Z) + cubeStart) * scalar,
			                          Quaternion.identity);
		}
		
		/// <summary>
		/// Places an enemy in the room at the given floor position. Assumes
		/// that the Y-coordinate is zero, and will lift the enemy up to
		/// accomodate for that.
		/// </summary>
		/// <param name="position">Floor position to place enemy at.</param>
		/// <param name="e">Type of enemy to spawn</param>
	    /// <param name="scalar">Value to scale 1 block size to.</param>
		private void InstantiateEnemy(Vector3 position, EnemyGenerator.EnemyType e, float scalar)
		{
			Transform enemy;
			Vector3 enemy_pos;
			switch(e)
			{
			case EnemyGenerator.EnemyType.skeleton:
				enemy = skeleton;
				enemy_pos = position + new Vector3(0.0f, skeleton.collider.bounds.center.y, 0.0f);
				break;
				
			case EnemyGenerator.EnemyType.spider:
				enemy = spider;
				enemy_pos = position + new Vector3(0.0f, spider.collider.bounds.center.y, 0.0f);
				break;
				
			case EnemyGenerator.EnemyType.zombie:
			default:
				enemy = zombie;
				enemy_pos = position + new Vector3(0.0f, zombie.collider.bounds.center.y, 0.0f);
				break;
			}

			MonoBehaviour.Instantiate(enemy, enemy_pos * scalar, Quaternion.identity);
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