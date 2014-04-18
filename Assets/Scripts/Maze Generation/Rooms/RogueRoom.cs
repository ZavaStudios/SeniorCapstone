using System;
using System.Collections.Generic;
using UnityEngine;

namespace MazeGeneration
{
    /// <summary>
    /// Template for all rooms in the maze. Defines the base functionality that all rooms need:
    ///     - Template objects to generate (including things this class can't generate, which is somewhat sketchy).
    ///     - Necessary state for handling direction checking, cube allocation, etc.
    ///     - Code for allocating walls and cubes for rooms.
    ///     - Base structure for creating, loading, and unloading the room.
    ///     - Important properties that parents will need to access regardless of type.
    /// </summary>
	public abstract class RogueRoom : CubeTracker
	{
        // Bitmasks used to check Door Codes: that is, used to check which directions
        // this room has doors on
		public const int UP_DOOR_MASK = 0x01;
		public const int DOWN_DOOR_MASK = 0x02;
		public const int LEFT_DOOR_MASK = 0x04;
		public const int RIGHT_DOOR_MASK = 0x08;

		// TODO: something smarter for these transforms. For now, we'll just
		// let them get assigned globally here, and everyone can just share.
		public static Transform skeleton;
		public static Transform zombie;
		public static Transform spider;
		public static Transform floor_tile;
		public static Transform wall_tile;
		public static Transform mine_cube;
		public static Transform ore_cube;
		public static Transform ore2_cube;
		public static Transform ore3_cube;
		public static Transform ore4_cube;
		public static Transform zombieBoss;
		public static Transform skeletonBoss;
		public static Transform spiderBoss;
		public static Transform door;
		public static Transform key;

		// Allocator for getting instantiated cubes for placing in the Unity scene
		public static CubeAllocator allocator;
        // List of cubes that have been allocated to us
		private List<MineableBlock> blocks = new List<MineableBlock>();

		// Cached values so we can spawn new cubes during runtime
		private float _scalar;
		private Vector3 _cubeStart;

        // Stores anything generated in this room so it can be destroyed on unload
        protected GameObject objHolder;
		
        /// <summary>
        /// Enumeration of various types a room may be. This class doesn't directly use this
        /// information, but it is important to subclasses and parents, so it is held here to
        /// be generic.
        /// </summary>
		public enum RoomType
		{
			empty, enemy, start, corridor, boss, shop, corridorFork, keyRoom
		}

		// Basic room properties:
        // ----------------------
        // Width and Depth are the floor dimensions
		public int Width { get;    protected set; }
		public int Depth { get;    protected set; }
        // Height is how tall the room is
		public int Height { get;   protected set; }
        // Bitmask holding which walls of this room have doors
		public int DoorCode { get; protected set; }
        // X and Y position of this room in the overall grid. Effectively
        // a cached value from the parent, so kind of a hack
		public int GridX { get; set; }
		public int GridY { get; set; }
        // Type of this room
		public RoomType Type { get; set; }
        // Data structure holding cubes - in this case, not allocated cubes, but rather
        // the saved cubes structure. TL;DR version: iterate over this to know what to
        // do with cubes allocated from allocator
		public RoomCubes Cubes { get; set; }

		// Pointers to adjascent rooms. Used by subclasses to allow cube generation to
        // be continuous between neighboring rooms.
		public RogueRoom LeftNeighbor { get; set; }
		public RogueRoom RightNeighbor { get; set; }
		public RogueRoom UpNeighbor { get; set; }
		public RogueRoom DownNeighbor { get; set; }

        // Track whether this room is currently loaded:
        protected bool isLoaded = false;

		/// <summary>
		/// Returns center coordinates of the floor of this room.
		/// </summary>
		/// <param name="totalHeight">Maximum height of a room.</param>
        /// <param name="totalWidth">Maximum width of a room.</param>
        /// <returns>The center.</returns>
		public virtual Vector3 GetCenter(int maxWidth, int maxDepth)
		{
			return new Vector3((float)maxWidth * ((float)GridX + 0.5f),
			                   0.0f,
			                   (float)maxDepth * ((float)GridY + 0.5f));
		}

		/// <summary>
		/// Initializes the Cubes property for this room. Functions uniquely for each kind of
		/// room, so there is no default behavior. Generally should be called only once the
		/// neighbors for each room have been appropriately assigned, and always before the
        /// room is loaded.
		/// </summary>
		public abstract void InitializeCubes();

		/// <summary>
		/// Allocates Unity assets for all the things contained in the room. This specific
		/// instance loads walls and cubes, and we recommend subclases call this base version
		/// before performing any custom allocation in addition.
		/// 
		/// Also, scales everything up according to a provided scalar value. Base measurements
		/// should be provided in terms of cube units (e.g. maxWidth = 10 implies 10 cubes is
		/// the maximum width a room can be in the maze), and scalar modifies that value.
		/// </summary>
		/// <param name="maxWidth">Maximum width a room can be in the maze, in cube units.</param>
		/// <param name="maxDepth">Maximum depth a room can be in the maze, in cube units.</param>
		/// <param name="doorWidth">How large the openings to the room need to be. Must be no more than Width or Depth.</param>
		/// <param name="scalar">1 cube -> scalar units in Unity.</param>
		public virtual void StaticLoad(int maxWidth, int maxDepth, int doorWidth, float scalar)
		{
            objHolder = new GameObject("Room_" + GridX + "-" + GridY);
			// Helpful initial values:
			Vector3 center = GetCenter(maxWidth, maxDepth);

			// Spawn floor / ceiling:
			
			//floor_tile.FindChild("Plane").renderer.material.SetTextureScale("_MainTex", new Vector2(Width, Depth));
			Transform ft = (Transform)MonoBehaviour.Instantiate(floor_tile, center * scalar, Quaternion.identity);
			Transform ct = (Transform)MonoBehaviour.Instantiate(floor_tile,
			                          							(center + new Vector3(0.0f, Height, 0.0f)) * scalar,
			                                                    Quaternion.AngleAxis (180.0f, Vector3.forward));
            
            ft.transform.localScale = new Vector3((float)Width, 1.0f, (float)Depth) * scalar;
            ct.transform.localScale = new Vector3((float)Width, 1.0f, (float)Depth) * scalar;

   			ft.FindChild("Plane").renderer.material.SetTextureScale("_MainTex", new Vector2(Width, Depth));
			ct.FindChild("Plane").renderer.material.SetTextureScale("_MainTex", new Vector2(Width, Depth));

            //set the tag for raycast identification.
            ft.FindChild("Plane").tag = "Floor";
            ct.FindChild("Plane").tag = "Ceiling"; 

            //assign parent:
            ft.transform.parent = objHolder.transform;
            ct.transform.parent = objHolder.transform;

			// Instantiate walls:
			float wallHeight = (float)Height / 2.0f;
			// LEFT
			//	if there is no door here:
			if ((DoorCode & LEFT_DOOR_MASK) == 0)
			{
				InstantiateWall(Depth,
				                (center + new Vector3(-(float)Width / 2.0f, wallHeight, 0.0f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up),
				                scalar);
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Depth - doorWidth) * 0.5f;
				InstantiateWall(wallLength,
				                (center + new Vector3(-Width * 0.5f,
				                      wallHeight,
				                      (wallLength + doorWidth) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                (center + new Vector3(-Width * 0.5f,
				                      wallHeight,
				                      -(wallLength + doorWidth) * 0.5f)),
				                Quaternion.AngleAxis(90.0f, Vector3.up),
				                scalar);
			}
			
			// TOP
			//	if there is no door here:
			if ((DoorCode & UP_DOOR_MASK) == 0)
			{
				InstantiateWall(Width,
				                (center + new Vector3(0.0f, wallHeight, -(float)Depth / 2.0f)),
				                Quaternion.identity,
				                scalar);
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Width - doorWidth) * 0.5f;
				InstantiateWall(wallLength,
				                (center + new Vector3((wallLength + doorWidth) * 0.5f,
				                      wallHeight,
				                      -Depth * 0.5f)),
				                Quaternion.identity,
				                scalar);
				InstantiateWall(wallLength,
				                (center + new Vector3(-(wallLength + doorWidth) * 0.5f,
				                      wallHeight,
				                      -Depth * 0.5f)),
				                Quaternion.identity,
				                scalar);
			}
			
			// RIGHT
			//	if there is no door here:
			if ((DoorCode & RIGHT_DOOR_MASK) == 0)
			{
				InstantiateWall(Depth,
				                (center + new Vector3((float)Width / 2.0f, wallHeight, 0.0f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up),
				                scalar);
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Depth - doorWidth) * 0.5f;
				InstantiateWall(wallLength,
				                (center + new Vector3(Width * 0.5f,
				                      wallHeight,
				                      (wallLength + doorWidth) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                (center + new Vector3(Width * 0.5f,
				                      wallHeight,
				                      -(wallLength + doorWidth) * 0.5f)),
				                Quaternion.AngleAxis(270.0f, Vector3.up),
				                scalar);
			}
			
			// BOTTOM
			//	if there is no door here:
			if ((DoorCode & DOWN_DOOR_MASK) == 0)
			{
				InstantiateWall(Width,
				                (center + new Vector3(0.0f, wallHeight, (float)Depth / 2.0f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up),
				                scalar);
			}
			//  if there is a door here:
			else
			{
				// Walls
				float wallLength = ((float)Width - doorWidth) * 0.5f;
				InstantiateWall(wallLength,
				                (center + new Vector3((wallLength + doorWidth) * 0.5f,
				                      wallHeight,
				                      Depth * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up),
				                scalar);
				InstantiateWall(wallLength,
				                (center + new Vector3(-(wallLength + doorWidth) * 0.5f,
				                      wallHeight,
				                      Depth * 0.5f)),
				                Quaternion.AngleAxis(180.0f, Vector3.up),
				                scalar);
			}

			// Instantiate cubes:
			Vector3 cubeStart = center - 
					new Vector3((float)Width * 0.5f, 0.0f, (float)Depth * 0.5f) +
					new Vector3(0.5f, 0.5f, 0.5f);

			_scalar = scalar;
			_cubeStart = cubeStart;
		}

		/// <summary>
		/// Loads anything into the room that needs to be spawned at runtime rather than
		/// in advance. Compare to StaticLoad, which generates objects safe to remain in
		/// the scene perpetually from the start.
		/// </summary>
		public virtual void DynamicLoad()
		{
			if (isLoaded)
				return;
			isLoaded = true;

			if (Cubes != null)
				foreach (Cube cube in Cubes.EnumerateCubes())
					InstantiateCube(cube, _cubeStart, _scalar);
		}

        /// <summary>
        /// Informs this room's neighbors that they need to get themselves loaded into the
        /// scene. Does NOT load this room itself! Also, will not load a room if that room
        /// is already loaded.
        /// 
        /// You may pass in a room you would like not to load, as well, in case your room
        /// is adjacent to a room already loaded (for example).
        /// </summary>
        /// <param name="maxWidth">Maximum width a room can be in the maze, in cube units.</param>
        /// <param name="maxDepth">Maximum depth a room can be in the maze, in cube units.</param>
        /// <param name="doorWidth">How large the openings to the room need to be. Must be no more than Width or Depth.</param>
        /// <param name="scalar">1 cube -> scalar units in Unity.</param>
        public virtual void LoadNeighbors(RogueRoom dontLoad)
        {
            if (LeftNeighbor != null && LeftNeighbor != dontLoad)
                LeftNeighbor.DynamicLoad();
            if (RightNeighbor != null && RightNeighbor != dontLoad)
				RightNeighbor.DynamicLoad();
            if (UpNeighbor != null && UpNeighbor != dontLoad)
				UpNeighbor.DynamicLoad();
            if (DownNeighbor != null && DownNeighbor != dontLoad)
				DownNeighbor.DynamicLoad();
        }

        /// <summary>
        /// Destroys all game objects loaded into Unity inside this room.
        /// </summary>
        public virtual void UnloadRoom()
        {
            if (!isLoaded)
                return;
            isLoaded = false;

			foreach (MineableBlock ct in blocks)
				allocator.ReturnCube(ct);
			blocks.Clear();
        }

        /// <summary>
        /// Informs neighbors of this room to unload themselves. Does NOT unload
        /// this room itself! Also, you may specify one neighbor not to despawn.
        /// </summary>
		/// <param name="dontDespawn">Room you would like to not unload</param>
        public virtual void UnloadNeighbors(RogueRoom dontDespawn)
        {
			if (LeftNeighbor != null && LeftNeighbor != dontDespawn)
                LeftNeighbor.UnloadRoom();
			if (RightNeighbor != null && RightNeighbor != dontDespawn)
                RightNeighbor.UnloadRoom();
			if (UpNeighbor != null && UpNeighbor != dontDespawn)
                UpNeighbor.UnloadRoom();
			if (DownNeighbor != null && DownNeighbor != dontDespawn)
                DownNeighbor.UnloadRoom();
        }

		/// <summary>
		/// Spawns a wall in Unity given a position, width, height, and angle.
		/// </summary>
		/// <param name="wallWidth">Horizontal length of the wall.</param>
		/// <param name="Height">Vertical height of the wall.</param>
		/// <param name="position">Position of the wall (center of the quad).</param>
		/// <param name="angle">Angle about which to rotate the wall.</param>
		/// <param name="scalar">Factor by which to scale 1 block to.<c/param> 
		private void InstantiateWall(float wallWidth, Vector3 position, Quaternion angle, float scalar)
		{
			wall_tile.transform.localScale = new Vector3(wallWidth,
			                                             Height,
			                                             1.0f) * scalar;
			Transform wt = (Transform)MonoBehaviour.Instantiate(wall_tile,
			                                                    position * scalar,
			                                                    angle);
			wt.FindChild("Plane").renderer.material.SetTextureScale("_MainTex", new Vector2(wallWidth, Height));
            wt.transform.parent = objHolder.transform;
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
			if (cube.Type == ItemBase.tOreType.NOT_ORE)
				return;
			
			cube.Parent = this;
			MineableBlock ct = allocator.GetCube(cube);
			ct.transform.position = (new Vector3(cube.X, cube.Y, cube.Z) + cubeStart) * scalar;
			blocks.Add(ct);
		}

        /// <summary>
        /// Destroys a cube stored in this room's Cubes data set. Since this class
        /// doesn't directly hold the cube, we'll pass the cube on to Cubes to handle.
        /// 
        /// This class inherits this structure for consistency in convention for handling
        /// cube destruction. However, since this class will be the one instantiating the
        /// yielded cubes from lower DestroyCube calls, nothing is yielded from this call.
        /// </summary>
        /// <param name="c">Cube to be destroyed.</param>
        /// <returns>Nothing. This class will handle instantiation - don't worry your pretty
        /// little head over it.</returns>
		public IEnumerable<Cube> DestroyCube(Cube c)
		{
			foreach (Cube uncovered in Cubes.DestroyCube(c))
				if (uncovered.Type != ItemBase.tOreType.NOT_ORE)
					InstantiateCube(uncovered, _cubeStart, _scalar);

			// We have no reason to return anything, so don't
			return new List<Cube>();
		}

        /// <summary>
        /// Deallocates the provided instance block, returning it to the allocator for future
        /// use elsewhere in the maze.
        /// 
        /// Also, this function calls DestroyCube on the spawned block's Cube property. No
        /// parent class should be calling Destroy cube if they have a MineableBlock for that
        /// cube instead.
        /// </summary>
        /// <param name="ct">Instantiated block to be deallocated.</param>
		public void DestroyMineableBlock(MineableBlock ct)
		{
			DestroyCube(ct._cube);
			allocator.ReturnCube(ct);
			blocks.Remove(ct);
		}
	}
}

