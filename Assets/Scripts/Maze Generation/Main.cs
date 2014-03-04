using UnityEngine;
using System.Collections;
using MazeGeneration;

public class Main : MonoBehaviour
{
    private const int WIDTH  = 2;
    private const int HEIGHT = 3;

	private const float CUBE_SCALAR = 1.0f;
    private const float TILE_SCALAR = 1.0f;
    private const float CEILING_HEIGHT = 5.0f;
    private const float LIGHT_DISTANCE = 2.0f;
    private const float ORE_DISTRIBUTION = 0.2f;

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

	// Use this for initialization
	void Start ()
    {
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

	/*
    // Helper function for instantiating cubes in the rooms
    private void InstantiateCubes(Vector3 center, float roomWidth, float roomHeight, int doorCode)
    {
        Vector3 botLeft = center - new Vector3(roomWidth / 2.0f, 0.0f, roomHeight / 2.0f);
        Vector3 topRight = center + new Vector3(roomWidth / 2.0f, 0.0f, roomHeight / 2.0f);
        Vector3 cubeOffset = new Vector3(0.5f, 0.0f, 0.5f);
        Vector3 cubeOffsetZ = new Vector3(0.0f, 0.5f, 0.0f);
        
        Vector2 minDoorRange = new Vector2(center.x - TILE_SCALAR / 2.0f, center.z - TILE_SCALAR / 2.0f);
        Vector2 maxDoorRange = new Vector2(center.x + TILE_SCALAR / 2.0f, center.z + TILE_SCALAR / 2.0f);

        int cubesWide = (int)roomWidth;
        int cubesTall = (int)roomHeight;
        int cubesUp = (int)CEILING_HEIGHT;

        for (int z = 0; z < cubesUp; z++)
        {
            for (int x = 0; x < cubesWide; x++)
            {
                Vector3 botPos = botLeft + cubeOffsetZ + cubeOffset + new Vector3(x, z, 0.0f);
                Vector3 topPos = topRight + cubeOffsetZ -cubeOffset - new Vector3(x, -z, 0.0f);
                // Top:
                if ((doorCode & RogueRoom.UP_DOOR_MASK) == 0 ||
                    botPos.x < minDoorRange.x || botPos.x > maxDoorRange.x)
                {
                    Instantiate((Random.value > ORE_DISTRIBUTION) ? mine_cube : ore_cube,
                                botPos,
                                Quaternion.identity);
                }
                // Bot:
                if ((doorCode & RogueRoom.DOWN_DOOR_MASK) == 0 ||
                    topPos.x < minDoorRange.x || topPos.x > maxDoorRange.x)
                {
                    Instantiate((Random.value > ORE_DISTRIBUTION) ? mine_cube : ore_cube,
                                topPos,
                                Quaternion.identity);
                }
            }
            // Trim edges for y placement so we don't double place in corners
            for (int y = 1; y < cubesTall - 1; y++)
            {
                Vector3 lftPos = botLeft + cubeOffsetZ + cubeOffset + new Vector3(0.0f, z, y);
                Vector3 rgtPos = topRight + cubeOffsetZ - cubeOffset - new Vector3(0.0f, -z, y);
                // Left:
                if ((doorCode & RogueRoom.LEFT_DOOR_MASK) == 0 ||
                    lftPos.z < minDoorRange.y || lftPos.z > maxDoorRange.y)
                {
                    Instantiate((Random.value > ORE_DISTRIBUTION) ? mine_cube : ore_cube,
                                lftPos,
                                Quaternion.identity);
                }
                // Right:
                if ((doorCode & RogueRoom.RIGHT_DOOR_MASK) == 0 ||
                    rgtPos.z < minDoorRange.y || rgtPos.z > maxDoorRange.y)
                {
                    Instantiate((Random.value > ORE_DISTRIBUTION) ? mine_cube : ore_cube,
                                rgtPos,
                                Quaternion.identity);
                }
            }
        }
    }
    */

	/*
    // Helper function for instantiating walls of the rooms
    private void InstantiateWall(Vector3 center, float roomWidth, float roomHeight, int door_code)
    {
        Quaternion wall_angle = Quaternion.identity;
        float wallLength = roomWidth;
        float offsetDistanceZ = -roomHeight;
        float offsetDistanceX = 0.0f;

        if (door_code == RogueRoom.DOWN_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(180.0f, Vector3.up);
            offsetDistanceZ = -offsetDistanceZ;
        }
        else if (door_code == RogueRoom.LEFT_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(90.0f, Vector3.up);
            wallLength = roomHeight;
            offsetDistanceZ = 0.0f;
            offsetDistanceX = -roomWidth;
        }
        else if (door_code == RogueRoom.RIGHT_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(270.0f, Vector3.up);
            wallLength = roomHeight;
            offsetDistanceZ = 0.0f;
            offsetDistanceX = roomWidth;
        }
        // else: door mask is presumed to be up, which we set up to already

        // If the wallLength would be 0, don't waste space drawing an invisible wall
        if (wallLength == 0)
            return;

        wall_tile.transform.localScale = new Vector3(wallLength,
                                                     CEILING_HEIGHT,
                                                     1.0f);
        Instantiate(wall_tile,
                    center + new Vector3(offsetDistanceX / 2.0f, CEILING_HEIGHT / 2.0f, offsetDistanceZ / 2.0f),
                    wall_angle);
    }
    */

	/*
	// Helper function for instantiating doors on the rooms
    private void InstantiateDoor(Vector3 center, float roomWidth, float roomHeight, int door_code)
    {
        Quaternion wall_angle = Quaternion.identity;
        float wallLength = (roomWidth - TILE_SCALAR) / 2.0f;
        float offsetDistanceZ1 = -roomHeight;
        float offsetDistanceZ2 = -roomHeight;
        float offsetDistanceX1 = (wallLength + TILE_SCALAR);
        float offsetDistanceX2 = -(wallLength + TILE_SCALAR);

        if (door_code == RogueRoom.DOWN_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(180.0f, Vector3.up);
            offsetDistanceZ1 = -offsetDistanceZ1;
            offsetDistanceZ2 = -offsetDistanceZ2;
        }
        else if (door_code == RogueRoom.LEFT_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(90.0f, Vector3.up);
            wallLength = (roomHeight - TILE_SCALAR) / 2.0f;
            offsetDistanceZ1 = (wallLength + TILE_SCALAR);
            offsetDistanceZ2 = -(wallLength + TILE_SCALAR);
            offsetDistanceX1 = -roomWidth;
            offsetDistanceX2 = -roomWidth;
        }
        else if (door_code == RogueRoom.RIGHT_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(270.0f, Vector3.up);
            wallLength = (roomHeight - TILE_SCALAR) / 2.0f;
            offsetDistanceZ1 = (wallLength + TILE_SCALAR);
            offsetDistanceZ2 = -(wallLength + TILE_SCALAR);
            offsetDistanceX1 = roomWidth;
            offsetDistanceX2 = roomWidth;
        }
        // else: door mask is presumed to be up, which we set up to already

        // If the wallLength would be 0, don't waste space drawing an invisible wall
        if (wallLength == 0)
            return;

        wall_tile.transform.localScale = new Vector3(wallLength,
                                                     CEILING_HEIGHT,
                                                     1.0f);
        Instantiate(wall_tile,
                    center + new Vector3(offsetDistanceX1 / 2.0f, CEILING_HEIGHT / 2.0f, offsetDistanceZ1 / 2.0f),
                    wall_angle);
        Instantiate(wall_tile,
                    center + new Vector3(offsetDistanceX2 / 2.0f, CEILING_HEIGHT / 2.0f, offsetDistanceZ2 / 2.0f),
                    wall_angle);
    }
    */

	/*
    private static int lightId = 0; // Just used to uniquely identify
    private void InstantiateCorridor(Vector3 center, float roomWidth, float roomHeight, int door_code)
    {
        Quaternion wall_angle1 = Quaternion.AngleAxis(270.0f, Vector3.up);
        Quaternion wall_angle2 = Quaternion.AngleAxis(90.0f, Vector3.up);
        float wallLength = ((TILE_SCALAR * (1.0f + RogueDungeon.MAX_ROOM_HEIGHT)) - roomHeight) / 2.0f;
        float offsetDistanceZ1 = -(roomHeight + wallLength);
        float offsetDistanceZ2 = -(roomHeight + wallLength);
        float offsetDistanceX1 = TILE_SCALAR;
        float offsetDistanceX2 = -TILE_SCALAR;
        Vector3 lightVec = new Vector3(0.0f, 0.0f, LIGHT_DISTANCE * TILE_SCALAR);
        Vector3 lightOff = new Vector3(0.0f, CEILING_HEIGHT/2.0f, roomHeight);
        
		if (door_code == RogueRoom.DOWN_DOOR_MASK)
        {
            offsetDistanceZ1 = -offsetDistanceZ1;
            offsetDistanceZ2 = -offsetDistanceZ2;
            lightVec = new Vector3(0.0f, 0.0f, -LIGHT_DISTANCE * TILE_SCALAR);
            lightOff = new Vector3(0.0f, CEILING_HEIGHT / 2.0f, -roomHeight);
        }
        else if (door_code == RogueRoom.LEFT_DOOR_MASK)
        {
            wall_angle1 = Quaternion.AngleAxis(180.0f, Vector3.up);
            wall_angle2 = Quaternion.identity;
            wallLength = ((TILE_SCALAR * (1.0f + RogueDungeon.MAX_ROOM_WIDTH)) - roomWidth) / 2.0f;
            offsetDistanceZ1 = TILE_SCALAR;
            offsetDistanceZ2 = -TILE_SCALAR;
            offsetDistanceX1 = -(roomWidth + wallLength);
            offsetDistanceX2 = -(roomWidth + wallLength);
            lightVec = new Vector3(LIGHT_DISTANCE * TILE_SCALAR, 0.0f, 0.0f);
            lightOff = new Vector3(roomWidth, CEILING_HEIGHT / 2.0f, 0.0f);
        }
        else if (door_code == RogueRoom.RIGHT_DOOR_MASK)
        {
            wall_angle1 = Quaternion.AngleAxis(180.0f, Vector3.up);
            wall_angle2 = Quaternion.identity;
            wallLength = ((TILE_SCALAR * (1.0f + RogueDungeon.MAX_ROOM_WIDTH)) - roomWidth) / 2.0f;
            offsetDistanceZ1 = TILE_SCALAR;
            offsetDistanceZ2 = -TILE_SCALAR;
            offsetDistanceX1 = roomWidth + wallLength;
			offsetDistanceX2 = roomWidth + wallLength;
            lightVec = new Vector3(-LIGHT_DISTANCE * TILE_SCALAR, 0.0f, 0.0f);
            lightOff = new Vector3(-roomWidth, CEILING_HEIGHT / 2.0f, 0.0f);
		}
		// else: door mask is presumed to be up, which we set up to already

        // If the wallLength would be 0, don't waste space drawing an invisible wall
        if (wallLength == 0)
            return;

        wall_tile.transform.localScale = new Vector3(wallLength,
                                                     CEILING_HEIGHT,
                                                     1.0f);
        Instantiate(wall_tile,
                    center + new Vector3(offsetDistanceX1 / 2.0f, CEILING_HEIGHT / 2.0f, offsetDistanceZ1 / 2.0f),
                    wall_angle1);
        Instantiate(wall_tile,
                    center + new Vector3(offsetDistanceX2 / 2.0f, CEILING_HEIGHT / 2.0f, offsetDistanceZ2 / 2.0f),
                    wall_angle2);

        // Spawn some lights:
        // NM, not for now: this seems to be weird?
    }
    */

    // Update is called once per frame
    void Update()
    {
	
	}
}