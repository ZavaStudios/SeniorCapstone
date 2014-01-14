using UnityEngine;
using System.Collections;
using MazeGeneration;

public class Main : MonoBehaviour
{
    private const int WIDTH  = 5;
    private const int HEIGHT = 5;

    private const float TILE_SCALAR = 4.0f;
    private const float CEILING_HEIGHT = 5.0f;
    private const float LIGHT_DISTANCE = 2.0f;
    private const float ORE_DISTRIBUTION = 0.2f;

	public Transform floor_tile;
	public Transform wall_tile;
	public Transform mine_cube;
	public Transform ore_cube;

	public Transform player;
	public Transform enemy;

	// Use this for initialization
	void Start ()
    {
        RogueDungeon dungeon = new RogueDungeon(WIDTH, HEIGHT);

        // Build appropriate scalars
        float dungeon_width  = WIDTH  * TILE_SCALAR * (RogueDungeon.MAX_ROOM_WIDTH  + 2);
        float dungeon_height = HEIGHT * TILE_SCALAR * (RogueDungeon.MAX_ROOM_HEIGHT + 2);

		/*
        floor_tile.transform.localScale = new Vector3(dungeon_width, 1.0f, dungeon_height);
        Instantiate(floor_tile,
                    new Vector3(0.0f, 0.0f, 0.0f),
                    Quaternion.identity);
        Instantiate(floor_tile,
                    new Vector3(0.0f, CEILING_HEIGHT, 0.0f),
                    Quaternion.AngleAxis(180, Vector3.forward));
		*/

        // build walls
        Rect room_bounds = new Rect(TILE_SCALAR - (dungeon_width  / 2.0f),
                                    TILE_SCALAR - (dungeon_height / 2.0f),
                                    RogueDungeon.MAX_ROOM_WIDTH  * TILE_SCALAR,
                                    RogueDungeon.MAX_ROOM_HEIGHT * TILE_SCALAR);

        for (int roomX = 0; roomX < WIDTH; roomX++)
        {
            for (int roomY = 0; roomY < HEIGHT; roomY++)
            {
				RogueRoom room = dungeon.Map[roomX, roomY];
				// TODO: FIX!
				room.floor_tile = floor_tile;
				room.wall_tile = wall_tile;
				room.mine_cube = mine_cube;
				room.ore_cube = ore_cube;

				room.LoadRoom(TILE_SCALAR, roomX, roomY,
				              RogueDungeon.MAX_ROOM_WIDTH, RogueDungeon.MAX_ROOM_HEIGHT,
				              RogueDungeon.CORRIDOR_WIDTH);
				/*
                Vector3 center = new Vector3(room_bounds.center.x, 0.0f, room_bounds.center.y);

                // Draw walls, leaving space for doors where necessary
                int doorCode = room.Doors;
                Debug.Log("DOOR CODE: " + roomX + ", " + roomY + " -- " + doorCode);
                float roomWidth  = room.Width  * TILE_SCALAR;
                float roomHeight = room.Height * TILE_SCALAR;
                // UP
                if ((doorCode & RogueRoom.UP_DOOR_MASK) != 0)
                {
                    InstantiateDoor(center, roomWidth, roomHeight, RogueRoom.UP_DOOR_MASK);
                    InstantiateCorridor(center, roomWidth, roomHeight, RogueRoom.UP_DOOR_MASK);
                }
                else
                    InstantiateWall(center, roomWidth, roomHeight, RogueRoom.UP_DOOR_MASK);
                // DOWN
                if ((doorCode & RogueRoom.DOWN_DOOR_MASK) != 0)
                {
                    InstantiateDoor(center, roomWidth, roomHeight, RogueRoom.DOWN_DOOR_MASK);
                    InstantiateCorridor(center, roomWidth, roomHeight, RogueRoom.DOWN_DOOR_MASK);
                }
                else
                    InstantiateWall(center, roomWidth, roomHeight, RogueRoom.DOWN_DOOR_MASK);
                // LEFT
                if ((doorCode & RogueRoom.LEFT_DOOR_MASK) != 0)
                {
                    InstantiateDoor(center, roomWidth, roomHeight, RogueRoom.LEFT_DOOR_MASK);
                    InstantiateCorridor(center, roomWidth, roomHeight, RogueRoom.LEFT_DOOR_MASK);
                }
                else
                    InstantiateWall(center, roomWidth, roomHeight, RogueRoom.LEFT_DOOR_MASK);
                // RIGHT
                if ((doorCode & RogueRoom.RIGHT_DOOR_MASK) != 0)
                {
                    InstantiateDoor(center, roomWidth, roomHeight, RogueRoom.RIGHT_DOOR_MASK);
                    InstantiateCorridor(center, roomWidth, roomHeight, RogueRoom.RIGHT_DOOR_MASK);
                }
                else
                    InstantiateWall(center, roomWidth, roomHeight, RogueRoom.RIGHT_DOOR_MASK);

				// One last thing: add some cubes!
                if (room.Type != RogueRoom.RoomType.corridor)
                    InstantiateCubes(center, roomWidth, roomHeight, doorCode);

				// Also an enemy for shits and giggles
				if (room.Type == RogueRoom.RoomType.enemy)
				{
					Instantiate (enemy,
					             center + new Vector3(0, 100.0f, 0),
					             Quaternion.identity);
					Debug.Log("Enemey!");
				}
				else if (room.Type == RogueRoom.RoomType.start)
				{
					player.transform.position = center + new Vector3(0, 10, 0);
				}

                // Move to the next row
                room_bounds.y += (RogueDungeon.MAX_ROOM_HEIGHT + 1) * TILE_SCALAR;
                room_bounds.height = RogueDungeon.MAX_ROOM_HEIGHT * TILE_SCALAR;
                */
            }

			/*
            // Reset the row
            room_bounds.y = TILE_SCALAR - (dungeon_height / 2.0f);
            room_bounds.height = RogueDungeon.MAX_ROOM_HEIGHT * TILE_SCALAR;

            // Move to the next column
            room_bounds.x += (RogueDungeon.MAX_ROOM_WIDTH + 1) * TILE_SCALAR;
            room_bounds.width = RogueDungeon.MAX_ROOM_WIDTH * TILE_SCALAR;
            */
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
