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
		private ItemBase.tOreType[,,] Cubes;

		public SmallRoomCubes(int width, int depth, int height, int doorCode,
		                      int[] lftNbrUp, int[] lftNbrDwn, int[] rgtNbrUp, int[] rgtNbrDwn,
		                      int[] upNbrLft, int[] upNbrRgt, int[] dwnNbrLft, int[] dwnNbrRgt)
		{
			Width = width;
			Depth = depth;
			Height = height;
            Cubes = new ItemBase.tOreType[width, depth, height];

			InitializeCubes(lftNbrUp, lftNbrDwn, rgtNbrUp, rgtNbrDwn, upNbrLft, upNbrRgt, dwnNbrLft, dwnNbrRgt);
		}

		private void InitializeCubes(int[] lftNbrUp, int[] lftNbrDwn, int[] rgtNbrUp, int[] rgtNbrDwn,
		                             int[] upNbrLft, int[] upNbrRgt, int[] dwnNbrLft, int[] dwnNbrRgt)
		{
			// Initial values:
			for (int z = 0; z < Height; z++)
				for (int x = 0; x < Width; x++)
					for (int y = 0; y < Depth; y++)
						Cubes[x,y,z] = GetCubeType();

			// "punch out" air blocks:
			for (int z = 0; z < Height; z++)
			{
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < Depth; y++)
					{
						/*
						// Top half:
						if (y <= Depth / 2)
							if (upNbrLft != null && upNbrRgt != null)
								if (x >= upNbrLft[z] && x <= Width - 1 - upNbrRgt[z])
									Cubes[x,y,z] = Cube.CubeType.Air;

						// Bottom half:
						if (y >= Depth / 2)
							if (dwnNbrLft != null && dwnNbrRgt != null)
								if (x >= dwnNbrLft[z] && x <= Width - 1 - dwnNbrRgt[z])
									Cubes[x,y,z] = Cube.CubeType.Air;

						// Left half:
						if (x <= Width / 2)
							if (lftNbrUp != null && lftNbrDwn != null)
								if (y >= lftNbrUp[z] && y <= Depth - 1 - lftNbrDwn[z])
									Cubes[x,y,z] = Cube.CubeType.Air;

						// Right half:
						if (x >= Width / 2)
							if (rgtNbrUp != null && rgtNbrDwn != null)
								if (y >= rgtNbrUp[z] && y <= Depth - 1 - rgtNbrDwn[z])
									Cubes[x,y,z] = Cube.CubeType.Air;
									*/

						// Quadrant 1:
						if ((x <= y) && ((Width-x-1) >= y))
						{
							// If neighbor doesn't exist, just skip
							if (lftNbrUp == null || lftNbrDwn == null)
								continue;

							int w1 = lftNbrUp[z];
							int w2 = lftNbrDwn[z];
							if (y >= w1 && y <= (Depth - w2 - 1))
                                Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
						}
						// Quadrant 2:
						if ((x >= y) && ((Width-x-1) >= y))
						{
							// If neighbor doesn't exist, just skip
							if (upNbrLft == null || upNbrRgt == null)
								continue;

							int w1 = upNbrLft[z];
							int w2 = upNbrRgt[z];
							if (x >= w1 && x <= (Depth - w2 - 1))
                                Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
						}
						// Quadrant 3:
						if ((x >= y) && ((Width-x-1) <= y))
						{
							// If neighbor doesn't exist, just skip
							if (rgtNbrUp == null || rgtNbrDwn == null)
								continue;

							int w1 = rgtNbrUp[z];
							int w2 = rgtNbrDwn[z];
							if (y >= w1 && y <= (Depth - w2 - 1))
                                Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
						}
						// Quadrant 4:
						if ((x <= y) && ((Width-x-1) <= y))
						{
							// If neighbor doesn't exist, just skip
							if (dwnNbrLft == null || dwnNbrRgt == null)
								continue;

							int w1 = dwnNbrLft[z];
							int w2 = dwnNbrRgt[z];
							if (x >= w1 && x <= (Depth - w2 - 1))
                                Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
						}
					}
				}
			}
		}

        // Enumeration state:
        private int enumX = 0;
        private int enumY = 0;
        private int enumZ = 0;

		public override IEnumerable<Cube> EnumerateCubes(int count)
		{
            for (; enumX < Width; enumX++)
            {
                for (; enumY < Depth; enumY++)
                {
                    for (; enumZ < Height; enumZ++)
                    {
                        yield return new Cube(this, Cubes[enumX, enumY, enumZ], enumX, enumZ, enumY);
                        if (--count == 0)
                            yield break;
                    }
                }
            }
		}

        public override void ResetEnumeration()
        {
            enumX = 0;
            enumY = 0;
            enumZ = 0;
        }

		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
            Cubes[c.X, c.Z, c.Y] = ItemBase.tOreType.NOT_ORE;
			return new List<Cube>(); // Return empty list, since we currently show everything
		}
	}
}

