using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	// Room cube setup for corridor branches.
	// Setup as follows:
	//  ---------------
	// |\v | | | | | v/|
	// |->\v | 2 | v/<-|
	// |--->\v | v/<---|
	// |----->\v/<- 3 -|
	// |- 1 ->/^\<-----|
	// |--->/^ | ^\<---|
	// |->/^ | 4 | ^\<-|
	// |/^ | | | | | ^\|
	//  ---------------
	// 
	// In case that isn't clear, the idea is that any open doors will
	// "push" their open air towards the center of the cube, but only
	// up to the diagonal-separated region that the door lies in.
	// 
	// For now, the room will start as a solid chunk of cubes, and the
	// doors push air into the system. Long term, there may be some
	// initial air carving that gets put into the room, and the doors
	// will simply cut into that preexisting state.

	public class SmallRoomCubes : RoomCubes
	{
		private int Width { get; set; }
		private int Depth { get; set; }
		private int Height { get; set; }
		private Cube.CubeType[,,] Cubes;
		
		private LRCorridorCubes LeftNeighbor;
		private LRCorridorCubes RightNeighbor;
		private UDCorridorCubes UpNeighbor;
		private UDCorridorCubes DownNeighbor;

		public SmallRoomCubes(int width, int depth, int height, int doorCode,
		                      LRCorridorCubes lftNbr, LRCorridorCubes rgtNbr,
		                      UDCorridorCubes upNbr, UDCorridorCubes dwnNbr)
		{
			Width = width;
			Depth = depth;
			Height = height;
			LeftNeighbor = lftNbr;
			RightNeighbor = rgtNbr;
			UpNeighbor = upNbr;
			DownNeighbor = dwnNbr;
			Cubes = new Cube.CubeType[width, depth, height];

			InitializeCubes();
		}

		private void InitializeCubes()
		{
			// Initial values:
			for (int z = 0; z < Height; z++)
				for (int x = 0; x < Width; x++)
					for (int y = 0; y < Depth; y++)
						Cubes[x,y,z] = Cube.CubeType.Stone; // TODO: smarter

			// "punch out" air blocks:
			for (int z = 0; z < Height; z++)
			{
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Depth; y++)
					{
						// Quadrant 1:
						if ((x <= y) && ((Width-x-1) >= y))
						{
							// If neighbor doesn't exist, just skip
							if (LeftNeighbor == null)
								continue;
							if (LeftNeighbor.UpWall == null)
								continue;

							int w1 = LeftNeighbor.UpWall.GetDepthAt(LeftNeighbor.UpWall.Width-1, z);
							int w2 = LeftNeighbor.DownWall.GetDepthAt(0, z);
							if (y > w1 && y < (Depth - w2 - 1))
								Cubes[x,y,z] = Cube.CubeType.Air;
						}
						// Quadrant 2:
						if ((x >= y) && ((Width-x-1) >= y))
						{
							// If neighbor doesn't exist, just skip
							if (UpNeighbor == null)
								continue;
							if (UpNeighbor.RightWall == null)
								continue;

							int w1 = UpNeighbor.RightWall.GetDepthAt(UpNeighbor.RightWall.Width-1, z);
							int w2 = UpNeighbor.LeftWall.GetDepthAt(0, z);
							if (x > w1 && x < (Depth - w2 - 1))
								Cubes[x,y,z] = Cube.CubeType.Air;
						}
						// Quadrant 3:
						if ((x >= y) && ((Width-x-1) <= y))
						{
							// If neighbor doesn't exist, just skip
							if (RightNeighbor == null)
								continue;
							if (RightNeighbor.DownWall == null)
								continue;

							int w1 = RightNeighbor.DownWall.GetDepthAt(RightNeighbor.DownWall.Width-1, z);
							int w2 = RightNeighbor.UpWall.GetDepthAt(0, z);
							if (y > w1 && y < (Depth - w2 - 1))
								Cubes[x,y,z] = Cube.CubeType.Air;
						}
						// Quadrant 4:
						if ((x <= y) && ((Width-x-1) <= y))
						{
							// If neighbor doesn't exist, just skip
							if (DownNeighbor == null)
								continue;
							if (DownNeighbor.LeftWall == null)
								continue;

							int w1 = DownNeighbor.LeftWall.GetDepthAt(UpNeighbor.LeftWall.Width-1, z);
							int w2 = DownNeighbor.RightWall.GetDepthAt(0, z);
							if (x > w1 && x < (Depth - w2 - 1))
								Cubes[x,y,z] = Cube.CubeType.Air;
						}
					}
				}
			}
		}

		public IEnumerable<Cube> EnumerateCubes()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Depth; y++)
					for (int z = 0; z < Height; z++)
						yield return new Cube(Cubes[x,y,z], x, y, z);
		}
	}
}

