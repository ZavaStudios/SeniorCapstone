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
				center += new Vector3((maxWidth - LeftNeighbor.Width) / 2, 0, 0);
				center -= new Vector3((maxWidth - RightNeighbor.Width) / 2, 0, 0);
			}

			// Up and Down adjustment:
			else // if ((DoorCode & RogueRoom.UP_DOOR_MASK) != 0)
			{
				center += new Vector3(0, 0, (maxDepth - UpNeighbor.Depth) / 2);
				center -= new Vector3(0, 0, (maxDepth - DownNeighbor.Depth) / 2);
			}

			return center;
		}

		public override void InitializeCubes ()
		{
			// TODO
		}
	}
}

