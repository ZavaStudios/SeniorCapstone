using UnityEngine;
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
	public class GeneralRoom : RogueRoom
	{
        System.Random enemyPosGen = new System.Random();
        private float _scalar;
        private int _maxWidth;
        private int _maxDepth;

		/// <summary>
		/// Stores the types of enemies held in this room.
		/// </summary>
		public List<EnemyGenerator.EnemyType> Enemies
		{
			get;
			private set;
		}
		
		public GeneralRoom(int width, int depth, int height, int x, int y, int doors)
		{
			Type = RoomType.empty;
			Width = width;
			Depth = depth;
			Height = height;
			DoorCode = doors;
			GridX = x;
			GridY = y;
		}

		/// <summary>
		/// Assigns enemies to this room based on the assigned enemyScore.
		/// Short version of how this works: various enemy types are worth differing
		/// numbers of points. E.g., a spider may be worth 1 while a skeleton is worth 5.
		/// This function randomly determines which type of enemies to spawn in the room
		/// based on how many points are assigned to the room.
        /// 
        /// Note: If this is a boss room, the score is ignored and simply picks a boss to
        /// place into the room randomly. Similarly, if this room is neither an enemy nor
        /// a boss room, this function is completely unused.
		/// </summary>
		/// <param name="enemyScore">Number of enemy points allocated to this room</param>
		public void AssignEnemies(int enemyScore)
		{
			// For now, just assign one "enemy" per enemy score:
            if (Type == RoomType.enemy)
                Enemies = EnemyGenerator.generateEnemies(enemyScore);
            else if (Type == RoomType.boss)
                Enemies = EnemyGenerator.generateBoss();
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
		public override void LoadRoom(int maxWidth, int maxDepth, int doorWidth, float scalar)
		{
            Vector3 center = GetCenter(maxWidth, maxDepth);
            _scalar = scalar;
            _maxDepth = maxDepth;
            _maxWidth = maxWidth;
            base.LoadRoom(_maxWidth, _maxDepth, doorWidth, _scalar);

			// ENEMIES / ETC.
			if (Enemies != null)
			{
				//Generate a random enemy in the maze based on what the generate enemy function returns.
				//Need to store the enemies into a list to be used if we need to reload the room. 
				foreach(EnemyGenerator.EnemyType enemy in Enemies)
				{
					SpawnEnemy(enemy);
				}
			}
			
            if (Type == RoomType.boss)
			{
				switch (DoorCode)
				{
				case RogueRoom.LEFT_DOOR_MASK:
					InstantiateDoor((center + new Vector3(-(float)Width / 2.0f, 0.0f, 0.0f)),
					                Quaternion.AngleAxis(90.0f, Vector3.up),
					                scalar);
					break;
				case RogueRoom.RIGHT_DOOR_MASK:
					InstantiateDoor((center + new Vector3((float)Width / 2.0f, 0.0f, 0.0f)),
					                Quaternion.AngleAxis(270.0f, Vector3.up),
					                scalar);
					break;
				case RogueRoom.UP_DOOR_MASK:
					InstantiateDoor((center + new Vector3(0.0f, 0.0f, -(float)Depth / 2.0f)),
					                Quaternion.identity,
					                scalar);
					break;
				case RogueRoom.DOWN_DOOR_MASK:
				default:
					InstantiateDoor((center + new Vector3(0.0f, 0.0f, (float)Depth / 2.0f)),
					                Quaternion.AngleAxis(180.0f, Vector3.up),
					                scalar);
					break;
				}
			}
			
            if (Type == RoomType.keyRoom)
			{
				InstantiateKey (center + new Vector3(0.0f, Height / 2.0f, 0.0f), scalar);
			}
		}

		private void InstantiateDoor(Vector3 position, Quaternion angle, float scalar)
		{
			Transform dt = (Transform)MonoBehaviour.Instantiate(door, position * scalar, angle);
            dt.transform.parent = objHolder.transform;
		}

        private void InstantiateKey(Vector3 position, float scalar)
        {
            Transform kt = (Transform)MonoBehaviour.Instantiate(key, position * scalar, Quaternion.identity);
            kt.transform.parent = objHolder.transform;
        }

        public Transform SpawnEnemy(EnemyGenerator.EnemyType enemy)
        {
            Vector3 center = GetCenter(_maxWidth, _maxDepth);
            float posX = center.x +
                                 ((float)((enemyPosGen.NextDouble() - 0.5) * 2.0) *
                                 ((float)(Width * 0.5) - (float)((StandardRoomCubes)Cubes).WallDepth));
            float posY = center.z +
                         ((float)((enemyPosGen.NextDouble() - 0.5) * 2.0) *
                         ((float)(Height * 0.5) - (float)((StandardRoomCubes)Cubes).WallDepth));
            Vector3 enemyPos = new Vector3(posX, 0.2f, posY);
            return InstantiateEnemy(enemyPos, enemy, _scalar);
        }

		public override void InitializeCubes ()
		{
			RoomCubes lftCbes = (LeftNeighbor != null)  ? LeftNeighbor.Cubes  : null;
			RoomCubes rgtCbes = (RightNeighbor != null) ? RightNeighbor.Cubes : null;
			RoomCubes upCbes  = (UpNeighbor != null)    ? UpNeighbor.Cubes    : null;
			RoomCubes dwnCbes = (DownNeighbor != null)  ? DownNeighbor.Cubes  : null;
			Cubes = new StandardRoomCubes(Width, Depth, DoorCode, Height,
			                              lftCbes, rgtCbes, upCbes, dwnCbes);
		}
		
		/// <summary>
		/// Places an enemy in the room at the given floor position. Assumes
		/// that the Y-coordinate is zero, and will lift the enemy up to
		/// accomodate for that.
		/// </summary>
		/// <param name="position">Floor position to place enemy at.</param>
		/// <param name="e">Type of enemy to spawn</param>
	    /// <param name="scalar">Value to scale 1 block size to.</param>
		private Transform InstantiateEnemy(Vector3 position, EnemyGenerator.EnemyType e, float scalar)
		{
			Transform enemy = null;
			Vector3 enemy_pos = Vector3.zero;
			switch(e)
			{
			case EnemyGenerator.EnemyType.skeleton:
				enemy = skeleton;
				enemy_pos = position + new Vector3(0.0f, skeleton.GetComponent<CharacterController>().height * skeleton.localScale.y, 0.0f);
				break;
				
			case EnemyGenerator.EnemyType.spider:
				enemy = spider;
				enemy_pos = position + new Vector3(0.0f, spider.GetComponent<CharacterController>().height * spider.localScale.y, 0.0f);
				break;
				
			case EnemyGenerator.EnemyType.zombie:
				enemy = zombie;
				enemy_pos = position + new Vector3(0.0f, zombie.GetComponent<CharacterController>().height * zombie.localScale.y, 0.0f);
				break;
			case EnemyGenerator.EnemyType.skeletonBoss:
				enemy = skeletonBoss;
				enemy_pos = position + new Vector3(0.0f, skeletonBoss.GetComponent<CharacterController>().height * skeletonBoss.localScale.y, 0.0f);
				break;
			case EnemyGenerator.EnemyType.spiderBoss:
				enemy = spiderBoss;
				enemy_pos = position + new Vector3(0.0f, spiderBoss.GetComponent<CharacterController>().height * spiderBoss.localScale.y, 0.0f);
				break;
			case EnemyGenerator.EnemyType.zombieBoss:
				enemy = zombieBoss;
				enemy_pos = position + new Vector3(0.0f, zombieBoss.GetComponent<CharacterController>().height * zombieBoss.localScale.y, 0.0f);
				break; 
			default:
				Debug.Log ("ERROR!");
				break;
			}

			Transform et = (Transform)
                MonoBehaviour.Instantiate(enemy, enemy_pos * scalar, Quaternion.identity);
            et.transform.parent = objHolder.transform;
            return et;
		}
	}
}