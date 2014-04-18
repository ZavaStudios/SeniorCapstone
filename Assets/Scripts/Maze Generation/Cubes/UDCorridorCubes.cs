using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
    /// <summary>
    /// Handles cube placement in a corridor with doors in the up and down directions.
    /// </summary>
	public class UDCorridorCubes : RoomCubes
	{
        // Walls of cubes for the left and right hand sides of the room.
		public StandardWallCubes LeftWall { get; private set; }
		public StandardWallCubes RightWall { get; private set; }
		
        // Width (floor coordinates) of corridor
		private int Width { get; set; }
        // Depth (floor coordinates) of corridor
		private int Depth { get; set; }
        // Height (floor to ceiling) of corridor
		private int Height { get; set; }
		
        /// <summary>
        /// Creates a fully rendered instance of cubes for a Corridor.
        /// </summary>
        /// <param name="width">Width (floor coordinates) of corridor.</param>
        /// <param name="depth">Depth (floor coordinates) of corridor.</param>
        /// <param name="height">Height (floor to ceiling) of corridor.</param>
		public UDCorridorCubes (int width, int depth, int height)
		{
			LeftWall = new StandardWallCubes(depth, height, width / 2, 1);
			RightWall = new StandardWallCubes(depth, height, width / 2, 1);
			
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
			foreach (Cube c in LeftWall.EnumerateCubes())
				yield return new Cube(this, c.Type, c.Z, c.Y, c.X);
			foreach (Cube c in RightWall.EnumerateCubes())
				yield return new Cube(this, c.Type, Width - c.Z - 1, c.Y, c.X);
		}

        /// <summary>
        /// Removes a cube from the corridor, and yields any cubes that are
        /// uncovered as a consequence.
        /// </summary>
        /// <param name="c">Cube to be destroyed.</param>
        /// <returns>Cubes that have been uncovered by destroying c.</returns>
		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
			if (c.X < Width / 2)
			{
				int tmp = c.Z;
				c.Z = c.X;
				c.X = tmp;
				foreach (Cube uncovered in LeftWall.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      uncovered.Z, uncovered.Y, uncovered.X);
			}
			else
			{
				int tmp = c.Z;
				c.Z = Width - 1 - c.X;
				c.X = tmp;
				foreach (Cube uncovered in RightWall.DestroyCube(c))
					yield return new Cube(this, uncovered.Type,
					                      Width - 1 - uncovered.Z, uncovered.Y, uncovered.X);
			}
		}
	}
}