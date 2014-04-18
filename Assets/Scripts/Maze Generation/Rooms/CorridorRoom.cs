using UnityEngine;
using System;

namespace MazeGeneration
{
	/// <summary>
	/// Represents a corridor in the room. Generally very close to the base
    /// RogueRoom class, but this specifies that the loaded cubes will be two
    /// walls, based on which directions the doors face.
	/// </summary>
	public class CorridorRoom : RogueRoom
	{
        /// <summary>
        /// Instantiates the corridor. Does not initialize cubes or load the room.
        /// </summary>
        /// <param name="width">Width (floor coordinates) of room.</param>
        /// <param name="depth">Depth (floor coordinates) of room.</param>
        /// <param name="height">Height (floor to ceiling) of room.</param>
        /// <param name="x">X coordinate of this cooridor in the parenting RogueDungeon map.</param>
        /// <param name="y">Y coordinate of this cooridor in the parenting RogueDungeon map.</param>
        /// <param name="doorCode">Bitmask representing which directions this corridor has openings.</param>
		public CorridorRoom (int width, int depth, int height, int x, int y, int doorCode)
		{
			Width = width;
			Depth = depth;
			Height = height;
			GridX = x;
			GridY = y;
			DoorCode = doorCode;
		}

		/// <summary>
		/// This room type has to override the typical GetCenter routine, because the
		/// center of a corridor gets shifted based on the "off-ness" of its neighbors.
        /// </summary>
        /// <param name="totalHeight">Maximum height of a room.</param>
        /// <param name="totalWidth">Maximum width of a room.</param>
        /// <returns>The center.</returns>
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

        /// <summary>
        /// Initializes the cube structure based on which direction the
        /// corridor is facing. This is decided by the doorCode bitmask.
        /// Must be called before loading the room.
        /// 
        /// Because corridors generate their cubes without regard of their
        /// neighbors' structures, this can (and probably will need to be)
        /// called before assigning their neighbors.
        /// </summary>
		public override void InitializeCubes ()
		{
			if ((DoorCode & RogueRoom.LEFT_DOOR_MASK) != 0)
				Cubes = new LRCorridorCubes(Width, Depth, Height);
			else // ((DoorCode & RogueRoom.UP_DOOR_MASK) != 0)
				Cubes = new UDCorridorCubes(Width, Depth, Height);
		}
	}
}

