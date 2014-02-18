using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	/// <summary>
	/// Room cube setup for rooms too small for the typical RoomCubes structure.
	/// </summary>
	public class SmallRoomCubes : RoomCubes
	{
		public SmallRoomCubes(int width, int height, int doorCode, int ceilingHeight)
		{
			// TODO
		}
		
		public IEnumerable<Cube> EnumerateCubes()
		{
			// TODO
			yield return new Cube(Cube.CubeType.Air, 1, 1, 1);
		}
	}
}

