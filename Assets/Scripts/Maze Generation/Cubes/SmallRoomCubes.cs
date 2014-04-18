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

    /// <summary>
    /// Represents the data structure for the small rooms that aren't rooms, but really
    /// extensions of corridors.
    /// </summary>
	public class SmallRoomCubes : RoomCubes
	{
        // Width (floor coordinates) of the small room
		private int Width { get; set; }
        // Depth (floor coordinates) of the small room
		private int Depth { get; set; }
        // Height (floor to ceiling) of the small room
		private int Height { get; set; }
        
        // 3D grid of the cubes in the small room
		private ItemBase.tOreType[,,] Cubes;

        /// <summary>
        /// Instantiates all the cubes for the small room.
        /// </summary>
        /// <param name="width">Width (floor coordinates) of the wall.</param>
        /// <param name="depth">Depth (floor coordinates) of the wall.</param>
        /// <param name="height">Height (floor to ceiling) of the wall.</param>
        /// <param name="doorCode">Bitmask representing all the directions doors exist on the small room.</param>
        /// <param name="lftNbrUp">Array of depths for the upper portion of the left neighbor.
        /// May be null, but only if there is no left neighbor.</param>
        /// <param name="lftNbrDwn">Array of depths for the lower portion of the left neighbor.
        /// May be null, but only if there is no left neighbor.</param>
        /// <param name="rgtNbrUp">Array of depths for the upper portion of the right neighbor.
        /// May be null, but only if there is no right neighbor.</param>
        /// <param name="rgtNbrDwn">Array of depths for the lower portion of the right neighbor.
        /// May be null, but only if there is no right neighbor.</param>
        /// <param name="upNbrLft">Array of depths for the left portion of the up neighbor.
        /// May be null, but only if there is no up neighbor.</param>
        /// <param name="upNbrRgt">Array of depths for the right portion of the up neighbor.
        /// May be null, but only if there is no up neighbor.</param>
        /// <param name="dwnNbrLft">Array of depths for the left portion of the down neighbor.
        /// May be null, but only if there is no down neighbor.</param>
        /// <param name="dwnNbrRgt">Array of depths for the right portion of the down neighbor.
        /// May be null, but only if there is no down neighbor.</param>
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

        /// <summary>
        /// Helper function for initializing the cubes. I'll skip over the parameters, because they are the
        /// same as for the constructor, and this function is only used there.
        /// </summary>
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

        /// <summary>
        /// Iterates over all the cubes in the corridor, and passes back any
        /// cubes that are visible. Air cubes are skipped, and cubes which
        /// are obscured from the player's view are also not returned.
        /// </summary>
        /// <returns>List of cubes that are visible in this corridor to the player.</returns>
		public override IEnumerable<Cube> EnumerateCubes()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Depth; y++)
					for (int z = 0; z < Height; z++)
						if (IsVisible(x,y,z))
							yield return new Cube(this, Cubes[x,y,z], x, z, y);
		}

        /// <summary>
        /// Helper function for determining whether a cube at the given position is visible.
        /// </summary>
        /// <param name="x">X coordinate of the cube in question.</param>
        /// <param name="y">Y coordinate of the cube in question.</param>
        /// <param name="z">Z coordinate of the cube in question.</param>
        /// <returns></returns>
		private bool IsVisible(int x, int y, int z)
		{
			// if on the boundary: yes
			if (x == 0 || x == Width-1 ||
			    y == 0 || y == Depth-1 ||
			    z == 0 || z == Height-1)
				return true;

			if (Cubes[x-1, y, z] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x+1, y, z] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x, y-1, z] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x, y+1, z] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x, y, z-1] == ItemBase.tOreType.NOT_ORE ||
			    Cubes[x, y, z+1] == ItemBase.tOreType.NOT_ORE)
				return true;

			return false;
		}

        /// <summary>
        /// Removes a cube from the corridor, and yields any cubes that are
        /// uncovered as a consequence.
        /// </summary>
        /// <param name="c">Cube to be destroyed.</param>
        /// <returns>Cubes that have been uncovered by destroying c.</returns>
		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
			int tmp = c.Y;
			c.Y = c.Z;
			c.Z = tmp;
			List<Cube> toRet = new List<Cube>();
			
			if (c.X > 0 && !IsVisible(c.X-1, c.Y, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X-1, c.Y, c.Z], c.X-1, c.Z, c.Y));
			if (c.X < Width-1 && !IsVisible(c.X+1, c.Y, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X+1, c.Y, c.Z], c.X+1, c.Z, c.Y));
			if (c.Y > 0 && !IsVisible(c.X, c.Y-1, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y-1, c.Z], c.X, c.Z, c.Y-1));
			if (c.Y < Depth-1 && !IsVisible(c.X, c.Y+1, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y+1, c.Z], c.X, c.Z, c.Y+1));
			if (c.Z > 0 && !IsVisible(c.X, c.Y, c.Z-1))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y, c.Z-1], c.X, c.Z-1, c.Y));
			if (c.Z < Height-1 && !IsVisible(c.X, c.Y, c.Z+1))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y, c.Z+1], c.X, c.Z+1, c.Y));
			
			Cubes[c.X, c.Y, c.Z] = ItemBase.tOreType.NOT_ORE;
			return toRet;
		}
	}
}

