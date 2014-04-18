using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
    /// <summary>
    /// Generates cubes for a corridor oriented such that the doors
    /// are on the left and right sides of the room.
    /// </summary>
	public class LRCorridorCubes : RoomCubes
	{
        // Walls storing cubes for the top and bottom of the corridor, respectively
		public StandardWallCubes UpWall { get; private set; }
		public StandardWallCubes DownWall { get; private set; }
		
        // Width (floor coordinates) of the corridor
        private int Width { get; set; }
        // Depth (floor coordinates) of the corridor
        private int Depth { get; set; }
        // Height (floor to ceiling) of the corridor
		private int Height { get; set; }

        /// <summary>
        /// Generates all the cubes in the corridor.
        /// </summary>
        /// <param name="width">Width (floor coordinates) of corridor.</param>
        /// <param name="depth">Depth (floor coordinates) of corridor.</param>
        /// <param name="height">Height (floor to ceiling) of corridor.</param>
		public LRCorridorCubes (int width, int depth, int height)
		{
			UpWall = new StandardWallCubes(width, height, depth / 2, 1);
			DownWall = new StandardWallCubes(width, height, depth / 2, 1);

			Width = width;
			Height = height;
			Depth = depth;
		}

        /// <summary>
        /// Iterates over all the cubes in the corridor, and passes back any
        /// cubes that are visible. Air cubes are skipped, and cubes which
        /// are obscured from the player's view are also not returned.
        /// </summary>
        /// <returns>List of cubes that are visible in this corridor to the player.</returns>
		public override IEnumerable<Cube> EnumerateCubes()
		{
			foreach (Cube c in UpWall.EnumerateCubes())
				yield return c;
			foreach (Cube c in DownWall.EnumerateCubes())
				yield return new Cube(this, c.Type, c.X, c.Y, Depth - c.Z - 1);
		}

        /// <summary>
        /// Removes a cube from the corridor, and yields any cubes that are
        /// uncovered as a consequence.
        /// </summary>
        /// <param name="c">Cube to be destroyed.</param>
        /// <returns>Cubes that have been uncovered by destroying c.</returns>
		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
			if (c.Z < Depth / 2)
				foreach (Cube uncovered in UpWall.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      uncovered.X, uncovered.Y, uncovered.Z);
			else
			{
				c.Z = Depth - 1 - c.Z;
				foreach (Cube uncovered in DownWall.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      uncovered.X, uncovered.Y, Depth - 1 - uncovered.Z);
			}
		}
	}
}

