using UnityEngine;
using System;

namespace MazeGeneration
{
	// TODO
	public class CorridorRoom : RogueRoom
	{
		public CorridorRoom (int width, int depth, int height, int x, int y, int doorCode)
		{
			Width = width;
			Depth = depth;
			Height = height;
			GridX = x;
			GridY = y;
			DoorCode = doorCode;

			// TODO
		}

		/// <summary>
		/// This room type has to override the typical GetCenter routine, because the
		/// center of a corridor gets shifted based on the "off-ness" of its neighbors.
		/// </summary>
		public override Vector3 GetCenter(int maxWidth, int maxDepth)
		{
			Vector3 center = base.GetCenter(maxWidth, maxDepth);

			// Left & Right adjustment:
			if ((DoorCode & RogueRoom.LEFT_DOOR_MASK) != 0)
			{
				center.x = (LeftNeighbor.GetCenter(maxWidth, maxDepth).x + (LeftNeighbor.Width / 2.0f) +
				            RightNeighbor.GetCenter(maxWidth, maxDepth).x - (RightNeighbor.Width / 2.0f)) / 2.0f;
			}

			// Up and Down adjustment:
			else // if ((DoorCode & RogueRoom.UP_DOOR_MASK) != 0)
			{
				center.z = (UpNeighbor.GetCenter(maxWidth, maxDepth).z + (UpNeighbor.Depth / 2.0f) +
				            DownNeighbor.GetCenter(maxWidth, maxDepth).z - (DownNeighbor.Depth / 2.0f)) / 2.0f;
			}

			return center;
		}

		public override void InitializeCubes ()
		{
			if ((DoorCode & RogueRoom.LEFT_DOOR_MASK) != 0)
				Cubes = new LRCorridorCubes(Width, Depth, Height);
			else // ((DoorCode & RogueRoom.UP_DOOR_MASK) != 0)
				Cubes = new UDCorridorCubes(Width, Depth, Height);
		}
	}
}

