using UnityEngine;
using System.Collections;
using MazeGeneration;

public class Main : MonoBehaviour {
	
	private const int WIDTH = 101;
	private const int HEIGHT = 101;
	
	private const float SCALAR = 3.0f;
	
	// Use this for initialization
	void Start ()
	{
		DungeonGenerator dgen = new DungeonGenerator(WIDTH, HEIGHT);
		dgen.GenerateMaze(1,1);
		
		for (int x = 0; x < dgen.width; x++)
		{
			for (int y = 0; y < dgen.height; y++)
			{
				if(!dgen.tiles[x,y])
				{
					Vector3 pos = new Vector3((x * SCALAR) - ((WIDTH / 2) * SCALAR),
											  0,
											  (y * SCALAR) - ((HEIGHT / 2) * SCALAR));
					
					GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
					cube.transform.position = pos;
					cube.transform.localScale = new Vector3(SCALAR, SCALAR * 6, SCALAR);
				}
			}
		}
		
		// init floor
		GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
		floor.transform.position = new Vector3(0, -0.05f * SCALAR, 0);
		floor.transform.localScale = new Vector3(SCALAR * WIDTH, 1, SCALAR * HEIGHT);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
