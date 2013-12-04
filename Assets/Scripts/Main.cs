using UnityEngine;
using System.Collections;
using MazeGeneration;

public class Main : MonoBehaviour
{
    private const int WIDTH  = 5;
    private const int HEIGHT = 5;

    private const float TILE_SCALAR = 10.0f;
    private const float CEILING_HEIGHT = 10.0f;

	public Transform player;
	public Transform enemy;
    public Transform floor_tile;
    public Transform wall_tile;
	public Transform mine_cube;

	// Use this for initialization
	void Start ()
    {
        RogueDungeon dungeon = new RogueDungeon(WIDTH, HEIGHT);

        // Build appropriate scalars
        float dungeon_width  = WIDTH  * TILE_SCALAR * (RogueDungeon.MAX_ROOM_WIDTH  + 2);
        float dungeon_height = HEIGHT * TILE_SCALAR * (RogueDungeon.MAX_ROOM_HEIGHT + 2);

        floor_tile.transform.localScale = new Vector3(dungeon_width, 1.0f, dungeon_height);
        Instantiate(floor_tile,
                    new Vector3(0.0f, 0.0f, 0.0f),
                    Quaternion.identity);

        // build walls
        Rect room_bounds = new Rect(TILE_SCALAR - (dungeon_width  / 2.0f),
                                    TILE_SCALAR - (dungeon_height / 2.0f),
                                    RogueDungeon.MAX_ROOM_WIDTH  * TILE_SCALAR,
                                    RogueDungeon.MAX_ROOM_HEIGHT * TILE_SCALAR);
        for (int roomX = 0; roomX < WIDTH; roomX++)
        {
            for (int roomY = 0; roomY < HEIGHT; roomY++)
            {
				RogueDungeon.Room room = dungeon.Map[roomX, roomY];
                Vector3 center = new Vector3(room_bounds.center.x, 0.0f, room_bounds.center.y);

                // Draw walls, leaving space for doors where necessary
                int doorCode = room.Doors;
                Debug.Log("DOOR CODE: " + roomX + ", " + roomY + " -- " + doorCode);
                float roomWidth  = room.Width  * TILE_SCALAR;
                float roomHeight = room.Height * TILE_SCALAR;
                // UP
                if ((doorCode & RogueDungeon.Room.UP_DOOR_MASK) != 0)
                {
                    InstantiateDoor(center, roomWidth, roomHeight, RogueDungeon.Room.UP_DOOR_MASK);
                    InstantiateCorridor(center, roomWidth, roomHeight, RogueDungeon.Room.UP_DOOR_MASK);
                }
                else
                    InstantiateWall(center, roomWidth, roomHeight, RogueDungeon.Room.UP_DOOR_MASK);
                // DOWN
                if ((doorCode & RogueDungeon.Room.DOWN_DOOR_MASK) != 0)
                {
                    InstantiateDoor(center, roomWidth, roomHeight, RogueDungeon.Room.DOWN_DOOR_MASK);
                    InstantiateCorridor(center, roomWidth, roomHeight, RogueDungeon.Room.DOWN_DOOR_MASK);
                }
                else
                    InstantiateWall(center, roomWidth, roomHeight, RogueDungeon.Room.DOWN_DOOR_MASK);
                // LEFT
                if ((doorCode & RogueDungeon.Room.LEFT_DOOR_MASK) != 0)
                {
                    InstantiateDoor(center, roomWidth, roomHeight, RogueDungeon.Room.LEFT_DOOR_MASK);
                    InstantiateCorridor(center, roomWidth, roomHeight, RogueDungeon.Room.LEFT_DOOR_MASK);
                }
                else
                    InstantiateWall(center, roomWidth, roomHeight, RogueDungeon.Room.LEFT_DOOR_MASK);
                // RIGHT
                if ((doorCode & RogueDungeon.Room.RIGHT_DOOR_MASK) != 0)
                {
                    InstantiateDoor(center, roomWidth, roomHeight, RogueDungeon.Room.RIGHT_DOOR_MASK);
                    InstantiateCorridor(center, roomWidth, roomHeight, RogueDungeon.Room.RIGHT_DOOR_MASK);
                }
                else
                    InstantiateWall(center, roomWidth, roomHeight, RogueDungeon.Room.RIGHT_DOOR_MASK);

				// One last thing: add some cubes!
				for (int x_0 = 0; x_0 < 3; x_0++)
				{
					for (int y_0 = 0; y_0 < 3; y_0++)
					{
						Instantiate (mine_cube,
				             		center + new Vector3 (x_0, 0.5f, y_0),
				             		Quaternion.identity);
					}
				}
				// Also an enemy for shits and giggles
				if (room.Type == RogueDungeon.Room.RoomType.enemy)
				{
					Instantiate (enemy,
					             center + new Vector3(0, 100.0f, 0),
					             Quaternion.identity);
					Debug.Log("Enemey!");
				}
				else if (room.Type == RogueDungeon.Room.RoomType.start)
				{
					player.transform.position = center + new Vector3(0, 10, 0);
				}

                // Move to the next row
                room_bounds.y += (RogueDungeon.MAX_ROOM_HEIGHT + 1) * TILE_SCALAR;
                room_bounds.height = RogueDungeon.MAX_ROOM_HEIGHT * TILE_SCALAR;
            }

            // Reset the row
            room_bounds.y = TILE_SCALAR - (dungeon_height / 2.0f);
            room_bounds.height = RogueDungeon.MAX_ROOM_HEIGHT * TILE_SCALAR;

            // Move to the next column
            room_bounds.x += (RogueDungeon.MAX_ROOM_WIDTH + 1) * TILE_SCALAR;
            room_bounds.width = RogueDungeon.MAX_ROOM_WIDTH * TILE_SCALAR;
        }
	}

    // Helper function for instantiating walls of the rooms
    private void InstantiateWall(Vector3 center, float roomWidth, float roomHeight, int door_code)
    {
        Quaternion wall_angle = Quaternion.identity;
        float wallLength = roomWidth;
        float offsetDistanceZ = -roomHeight;
        float offsetDistanceX = 0.0f;

        if (door_code == RogueDungeon.Room.DOWN_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(180.0f, Vector3.up);
            offsetDistanceZ = -offsetDistanceZ;
        }
        else if (door_code == RogueDungeon.Room.LEFT_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(90.0f, Vector3.up);
            wallLength = roomHeight;
            offsetDistanceZ = 0.0f;
            offsetDistanceX = -roomWidth;
        }
        else if (door_code == RogueDungeon.Room.RIGHT_DOOR_MASK)
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

    private void InstantiateDoor(Vector3 center, float roomWidth, float roomHeight, int door_code)
    {
        Quaternion wall_angle = Quaternion.identity;
        float wallLength = (roomWidth - TILE_SCALAR) / 2.0f;
        float offsetDistanceZ1 = -roomHeight;
        float offsetDistanceZ2 = -roomHeight;
        float offsetDistanceX1 = (wallLength + TILE_SCALAR);
        float offsetDistanceX2 = -(wallLength + TILE_SCALAR);

        if (door_code == RogueDungeon.Room.DOWN_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(180.0f, Vector3.up);
            offsetDistanceZ1 = -offsetDistanceZ1;
            offsetDistanceZ2 = -offsetDistanceZ2;
        }
        else if (door_code == RogueDungeon.Room.LEFT_DOOR_MASK)
        {
            wall_angle = Quaternion.AngleAxis(90.0f, Vector3.up);
            wallLength = (roomHeight - TILE_SCALAR) / 2.0f;
            offsetDistanceZ1 = (wallLength + TILE_SCALAR);
            offsetDistanceZ2 = -(wallLength + TILE_SCALAR);
            offsetDistanceX1 = -roomWidth;
            offsetDistanceX2 = -roomWidth;
        }
        else if (door_code == RogueDungeon.Room.RIGHT_DOOR_MASK)
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

    private void InstantiateCorridor(Vector3 center, float roomWidth, float roomHeight, int door_code)
    {
        Quaternion wall_angle1 = Quaternion.AngleAxis(270.0f, Vector3.up);
        Quaternion wall_angle2 = Quaternion.AngleAxis(90.0f, Vector3.up);
        float wallLength = ((TILE_SCALAR * (1.0f + RogueDungeon.MAX_ROOM_HEIGHT)) - roomHeight) / 2.0f;
        float offsetDistanceZ1 = -(roomHeight + wallLength);
        float offsetDistanceZ2 = -(roomHeight + wallLength);
        float offsetDistanceX1 = TILE_SCALAR;
		float offsetDistanceX2 = -TILE_SCALAR;
		
		if (door_code == RogueDungeon.Room.DOWN_DOOR_MASK)
        {
            offsetDistanceZ1 = -offsetDistanceZ1;
            offsetDistanceZ2 = -offsetDistanceZ2;
        }
        else if (door_code == RogueDungeon.Room.LEFT_DOOR_MASK)
        {
            wall_angle1 = Quaternion.AngleAxis(180.0f, Vector3.up);
            wall_angle2 = Quaternion.identity;
            wallLength = ((TILE_SCALAR * (1.0f + RogueDungeon.MAX_ROOM_WIDTH)) - roomWidth) / 2.0f;
            offsetDistanceZ1 = TILE_SCALAR;
            offsetDistanceZ2 = -TILE_SCALAR;
            offsetDistanceX1 = -(roomWidth + wallLength);
            offsetDistanceX2 = -(roomWidth + wallLength);
        }
        else if (door_code == RogueDungeon.Room.RIGHT_DOOR_MASK)
        {
            wall_angle1 = Quaternion.AngleAxis(180.0f, Vector3.up);
            wall_angle2 = Quaternion.identity;
            wallLength = ((TILE_SCALAR * (1.0f + RogueDungeon.MAX_ROOM_WIDTH)) - roomWidth) / 2.0f;
            offsetDistanceZ1 = TILE_SCALAR;
            offsetDistanceZ2 = -TILE_SCALAR;
            offsetDistanceX1 = roomWidth + wallLength;
			offsetDistanceX2 = roomWidth + wallLength;
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
    }

    // Update is called once per frame
    void Update()
    {
	
	}
}
