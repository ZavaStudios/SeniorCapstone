using UnityEngine;
using System.Collections;
using MazeGeneration;

public class Main : MonoBehaviour
{
    private int WIDTH  = 3;
    private int HEIGHT = 3;

	private const float CUBE_SCALAR = 1.0f;
    private const float TILE_SCALAR = 1.0f;
    private const float CEILING_HEIGHT = 6.0f;
    private const float LIGHT_DISTANCE = 2.0f;
    private const float ORE_DISTRIBUTION = 0.2f;

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

	public Transform player;
	public Transform zombie;
	public Transform skeleton;
	public Transform spider;
	public Transform zombieBoss;
	public Transform skeletonBoss;
	public Transform spiderBoss;
	public Transform healthBar;

    private RogueDungeon dungeon;

    // position tracking for loading/unloading rooms
    private int gridX;
	private int gridY;

	// Use this for initialization
	void Start ()
    {
		WIDTH = LevelHolder.Level + 2;
		HEIGHT = LevelHolder.Level + 2;
        dungeon = new RogueDungeon(WIDTH, HEIGHT);

		// TODO: FIX!
		//room.enemy = enemy;
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

		ZombieWeapon.Player = player;
		EnemyStaff.player = player;
		UnitEnemy.player = player;
		UnitEnemy.healthBarStatic = healthBar;
		BossUnit.playercc = player.GetComponent<CharacterController>();
        BossUnit.endGamePortal = endGamePortal;

        DoorScript.player = player;
		KeyPickup.player = player;
		PortalScript.player = player;

        // Load start room and put player there:
        RogueRoom start = dungeon.GetStartRoom();
        start.LoadRoom(RogueDungeon.MAX_ROOM_WIDTH, RogueDungeon.MAX_ROOM_DEPTH,
                       RogueDungeon.CORRIDOR_WIDTH, CUBE_SCALAR);
        start.LoadNeighbors(RogueDungeon.MAX_ROOM_WIDTH, RogueDungeon.MAX_ROOM_DEPTH,
                            RogueDungeon.CORRIDOR_WIDTH, CUBE_SCALAR, null);
        player.transform.position = (start.GetCenter(RogueDungeon.MAX_ROOM_WIDTH,
                                                     RogueDungeon.MAX_ROOM_DEPTH) +
                                                     new Vector3(0.0f, 1.5f, 0.0f)) * CUBE_SCALAR;

		Vector3 playerPos = player.transform.position / CUBE_SCALAR;
		gridX = (int)playerPos.x / RogueDungeon.MAX_ROOM_WIDTH;
  		gridY = (int)playerPos.z / RogueDungeon.MAX_ROOM_DEPTH;
	}

	// Update is called once per frame
    void Update()
    {
		Vector3 playerPos = player.transform.position / CUBE_SCALAR;
        int newGridX = (int)playerPos.x / RogueDungeon.MAX_ROOM_WIDTH;
        int newGridY = (int)playerPos.z / RogueDungeon.MAX_ROOM_DEPTH;

        if (newGridX != gridX || newGridY != gridY)
        {
            dungeon.Map[gridX, gridY].UnloadNeighbors(dungeon.Map[newGridX, newGridY]);
            dungeon.Map[newGridX, newGridY].LoadNeighbors(RogueDungeon.MAX_ROOM_WIDTH,
                                                          RogueDungeon.MAX_ROOM_DEPTH,
                                                          RogueDungeon.CORRIDOR_WIDTH,
                                                          CUBE_SCALAR,
                                                          dungeon.Map[gridX, gridY]);

            gridX = newGridX;
            gridY = newGridY;
        }
	}
}