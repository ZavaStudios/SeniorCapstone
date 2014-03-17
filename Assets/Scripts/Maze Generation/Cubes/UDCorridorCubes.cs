using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	public class UDCorridorCubes : RoomCubes
	{
		public StandardWallCubes LeftWall { get; private set; }
		public StandardWallCubes RightWall { get; private set; }
		
		private int Width { get; set; }
		private int Depth { get; set; }
		private int Height { get; set; }
		
		public UDCorridorCubes (int width, int depth, int height)
		{
			LeftWall = new StandardWallCubes(depth, height, width / 2, 1);
			RightWall = new StandardWallCubes(depth, height, width / 2, 1);
			
			Width = width;
			Height = height;
			Depth = depth;
		}
		
		public override IEnumerable<Cube> EnumerateCubes(int count)
		{
            int remaining = count;

            foreach (Cube c in LeftWall.EnumerateCubes(remaining))
            {
                yield return new Cube(this, c.Type, c.Z, c.Y, c.X);
                if (--remaining == 0)
                    yield break;
            }
            foreach (Cube c in RightWall.EnumerateCubes(remaining))
            {
                yield return new Cube(this, c.Type, Width - c.Z - 1, c.Y, c.X);
                if (--remaining == 0)
                    yield break;
            }
		}

        public override void ResetEnumeration()
        {
            LeftWall.ResetEnumeration();
            RightWall.ResetEnumeration();
        }

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