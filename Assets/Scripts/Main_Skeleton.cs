using UnityEngine;
using System.Collections;
using MazeGeneration;

public class Main_Skeleton : MonoBehaviour {
	
	private const int WIDTH = 101;
	private const int HEIGHT = 101;
	
	private const float SCALAR = 3.0f;

    public Transform floor;
    public Transform wall_cube;

	// Use this for initialization
	void Start ()
	{
		DungeonGenerator dgen = new DungeonGenerator(WIDTH, HEIGHT);
		dgen.GenerateMaze(1,1);

        floor.transform.localScale = new Vector3(SCALAR, 1.0f, SCALAR);
        wall_cube.transform.localScale = new Vector3(SCALAR, SCALAR * 3.0f, SCALAR);

		for (int x = 0; x < dgen.width; x++)
		{
			for (int y = 0; y < dgen.height; y++)
			{
                Vector3 pos = new Vector3((x * SCALAR) - ((WIDTH / 2) * SCALAR),
                                              0.0f,
                                              (y * SCALAR) - ((HEIGHT / 2) * SCALAR));
                if (!dgen.tiles[x, y])
                {
                    Instantiate(wall_cube, pos, Quaternion.identity);
                }
                else
                {
                    // init floor
                    Instantiate(floor, pos, Quaternion.identity);
                    //floor.transform.localScale = new Vector3(SCALAR, 1, SCALAR);
                }
			}
		}
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
