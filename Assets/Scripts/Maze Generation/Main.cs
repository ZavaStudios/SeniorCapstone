using UnityEngine;
using System.Collections;
using MazeGeneration;

public class Main : MonoBehaviour
{
    private int WIDTH  = 3;
    private int HEIGHT = 3;

	private const float CUBE_SCALAR = 1.0f;
    private const float TILE_SCALAR = 1.0f;
    private const float CEILING_HEIGHT = 5.0f;
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

	public Transform player;
	public Transform zombie;
	public Transform skeleton;
	public Transform spider;
	public Transform zombieBoss;
	public Transform skeletonBoss;
	public Transform spiderBoss;

	// Use this for initialization
	void Start ()
    {
		WIDTH = LevelHolder.Level + 2;
		HEIGHT = LevelHolder.Level + 2;
        RogueDungeon dungeon = new RogueDungeon(WIDTH, HEIGHT);

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

		DoorScript.player = player;
		KeyPickup.player = player;
		PortalScript.player = player;

        foreach (RogueRoom room in dungeon.EnumerateRooms())
		{
			room.LoadRoom(RogueDungeon.MAX_ROOM_WIDTH, RogueDungeon.MAX_ROOM_DEPTH,
			              RogueDungeon.CORRIDOR_WIDTH, CUBE_SCALAR);

			if (room.Type == RogueRoom.RoomType.start)
			{
				player.transform.position = (room.GetCenter(RogueDungeon.MAX_ROOM_WIDTH,
				                                            RogueDungeon.MAX_ROOM_DEPTH) +
											new Vector3(0.0f, 1.5f, 0.0f)) * CUBE_SCALAR;
			}
        }
	}

	// Update is called once per frame
    void Update()
    {
	
	}
}