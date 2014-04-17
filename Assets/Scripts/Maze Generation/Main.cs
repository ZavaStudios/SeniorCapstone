using UnityEngine;
using System.Collections;
using MazeGeneration;

/// <summary>
/// Primary hub class for the main game. Basically does two things:
/// initializes and stores the dungeon, and holds prefab templates
/// and instances of objects other classes need to be aware of (e.g.
/// the player).
/// </summary>
public class Main : MonoBehaviour
{
    // Holds what the current width and height of the room is, in terms of
    // primary rooms (that is, ignoring corridors)
    private int WIDTH  = 3;
    private int HEIGHT = 3;

    // Constant used to determine how much bigger (in world coordinates) we want
    // our game to be. You'll notice this is 1.0 right now - yeah, turns out we
    // decided not to scale things up. It's still an option, though, so it stays
    // to avoid expensive refactoring for no reason.
	private const float CUBE_SCALAR = 1.0f;

    // Texture sequence for cracking ore cubes.
    public Texture2D[] crackTextures;
    // Texture lists for various material types.
    public Material[] weaponMaterials;

    // Various prefabs we generate in the game. These are assigned to various
    // static variables throughout the rest of the system so those systems can, in
    // turn, generate objects.
	public Transform door;
	public Transform key;
	public Transform floor_tile;
	public Transform wall_tile;
	public Transform mine_cube;
	public Transform ore_cube;
	public Transform ore2_cube;
	public Transform ore3_cube;
	public Transform ore4_cube;
    public Transform endGamePortal;

    // More of those cool prefab things. These are the player and
    // enemy related things. The player is intended to not be a
    // prefab, but an instance, however.
	public Transform player;
	public Transform zombie;
	public Transform skeleton;
	public Transform spider;
	public Transform zombieBoss;
	public Transform skeletonBoss;
	public Transform spiderBoss;
	public Transform healthBar;

    // Instance of a cube allocator that the dungeon uses to generate
    // cube objects, and move them into position for the dungeon's rooms.
	public CubeAllocator allocator;

    // It's the dungeon! This has all those room type things we want to
    // spawn in the maze.
    private RogueDungeon dungeon;

    // position tracking for loading/unloading rooms
    private int gridX;
	private int gridY;

	void Start ()
    {
		WIDTH = LevelHolder.Level + 2;
		HEIGHT = LevelHolder.Level + 2;
        dungeon = new RogueDungeon(WIDTH, HEIGHT);

		RogueRoom.skeleton = skeleton;
		RogueRoom.spider = spider;
		RogueRoom.zombie = zombie;
		RogueRoom.floor_tile = floor_tile;
		RogueRoom.wall_tile = wall_tile;
		RogueRoom.mine_cube = mine_cube;
		RogueRoom.ore_cube = ore_cube;
		RogueRoom.ore2_cube = ore2_cube;
		RogueRoom.ore3_cube = ore3_cube;
		RogueRoom.ore4_cube = ore4_cube;
		RogueRoom.door = door;
		RogueRoom.zombieBoss = zombieBoss;
		RogueRoom.skeletonBoss = skeletonBoss;
		RogueRoom.spiderBoss = spiderBoss;
        RogueRoom.key = key;
		RogueRoom.allocator = allocator;

		ZombieWeapon.Player = player;
		EnemyStaff.player = player;
		UnitEnemy.player = player;
		UnitEnemy.healthBarStatic = healthBar;
		BossUnit.playercc = player.GetComponent<CharacterController>();
        BossUnit.endGamePortal = endGamePortal;
        MineableBlock.crackTextures = crackTextures;
        ItemTextureSwitcher.materials = weaponMaterials;

        DoorScript.player = player;
		KeyPickup.player = player;
		PortalScript.player = player;

		// Load all static elements:
		foreach (RogueRoom room in dungeon.Map)
			if (room != null)
				room.StaticLoad(RogueDungeon.MAX_ROOM_WIDTH, RogueDungeon.MAX_ROOM_DEPTH,
				                RogueDungeon.CORRIDOR_WIDTH, CUBE_SCALAR);

        // Load start room and put player there:
        RogueRoom start = dungeon.GetStartRoom();
        start.DynamicLoad();
        start.LoadNeighbors(null);
        player.transform.position = (start.GetCenter(RogueDungeon.MAX_ROOM_WIDTH,
                                                     RogueDungeon.MAX_ROOM_DEPTH) +
                                                     new Vector3(0.0f, 1.5f, 0.0f)) * CUBE_SCALAR;

		Vector3 playerPos = player.transform.position / CUBE_SCALAR;
		gridX = (int)playerPos.x / RogueDungeon.MAX_ROOM_WIDTH;
  		gridY = (int)playerPos.z / RogueDungeon.MAX_ROOM_DEPTH;
	}

	/// <summary>
	/// The main object is tracking the player's position in order to trigger
    /// dynamic loading and unloading of rooms. That happens here.
	/// </summary>
    void Update()
    {
		Vector3 playerPos = player.transform.position / CUBE_SCALAR;
        int newGridX = (int)playerPos.x / RogueDungeon.MAX_ROOM_WIDTH;
        int newGridY = (int)playerPos.z / RogueDungeon.MAX_ROOM_DEPTH;

        if (newGridX != gridX || newGridY != gridY)
        {
            dungeon.Map[gridX, gridY].UnloadNeighbors(dungeon.Map[newGridX, newGridY]);
            dungeon.Map[newGridX, newGridY].LoadNeighbors(dungeon.Map[gridX, gridY]);

            gridX = newGridX;
            gridY = newGridY;
        }
	}
}