using System;
using System.Collections;
using System.Collections.Generic;
namespace MazeGeneration
{
	/// <summary>
	/// Decribes cubes held in this room in corners between walls, on
	/// the inside of the room. This class will initialize its own
	/// entries, but to be able to do so, it requires knowledge of the
	/// two walls on either side of the corner. If there is not a wall
	/// on the other side of the corner, this is not the right class
	/// to use.
	/// 
	/// Determines dimensions of the corners based on dimensions of the
	/// neighboring walls. It is assumed that each wall has the same
	/// height - if this constraint is not followed, behavior is not
	/// defined.
	/// </summary>
	public class InsideCornerCubes : RoomCubes
	{
        private ItemBase.tOreType[, ,] Cubes;
		
		public int Width
		{
			get { return Cubes.GetLength(0); }
		}
		public int Height
		{
			get { return Cubes.GetLength(2); }
		}
		public int Depth
		{
			get { return Cubes.GetLength(1); }
		}
		
		public InsideCornerCubes(int width, int depth, int[] right, int[] down)
		{
			int height = right.Length;
            Cubes = new ItemBase.tOreType[width, depth, height];
			
			InitializeCubes(right, down);
		}
		
		private void InitializeCubes(int[] right, int[] down)
		{
			for (int z = 0; z < Height; z++)
			{
				// Quadrants: Only one is interesting
				int quadX = down[z];
				int quadY = right[z];
				// Quadrants 1 and 2 (merged for convenience)
				for (int x = 0; x < Width; x++)
				{
					for (int y = 0; y < quadY; y++)
					{
						Cubes[x,y,z] = GetCubeType();
					}
				}
				// Quadrant 3
				for (int x = 0; x < quadX; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						Cubes[x,y,z] = GetCubeType();
					}
				}
				// Quadrant 4
				for (int x = quadX; x < Width; x++)
				{
					for (int y = quadY; y < Depth; y++)
					{
						// TODO
                        Cubes[x, y, z] = ItemBase.tOreType.NOT_ORE;
					}
				}
			}
		}
		
		public override IEnumerable<Cube> EnumerateCubes()
		{
			for (int x = 0; x < Width; x++)
				for (int y = 0; y < Depth; y++)
					for (int z = 0; z < Height; z++)
						if (IsVisible(x,y,z))
							yield return new Cube(this, Cubes[x,y,z], x, y, z);
		}
		
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

		public override IEnumerable<Cube> DestroyCube(Cube c)
		{
            List<Cube> toRet = new List<Cube>();

			if (c.X > 0 && !IsVisible(c.X-1, c.Y, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X-1, c.Y, c.Z], c.X-1, c.Y, c.Z));
			if (c.X < Width-1 && !IsVisible(c.X+1, c.Y, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X+1, c.Y, c.Z], c.X+1, c.Y, c.Z));
			if (c.Y > 0 && !IsVisible(c.X, c.Y-1, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y-1, c.Z], c.X, c.Y-1, c.Z));
			if (c.Y < Depth-1 && !IsVisible(c.X, c.Y+1, c.Z))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y+1, c.Z], c.X, c.Y+1, c.Z));
			if (c.Z > 0 && !IsVisible(c.X, c.Y, c.Z-1))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y, c.Z-1], c.X, c.Y, c.Z-1));
			if (c.Z < Height-1 && !IsVisible(c.X, c.Y, c.Z+1))
				toRet.Add(new Cube(this, Cubes[c.X, c.Y, c.Z+1], c.X, c.Y, c.Z+1));

			Cubes[c.X, c.Y, c.Z] = ItemBase.tOreType.NOT_ORE;
			return toRet;
		}
	}
}

