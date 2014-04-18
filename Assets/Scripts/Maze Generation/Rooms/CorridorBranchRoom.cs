using System;

namespace MazeGeneration
{
    /// <summary>
    /// Represents a small room which merely extends or branches a corridor without
    /// any meaningful content. Generated as a unique case, because its RoomCubes
    /// data structure is unique to accomodate.
    /// </summary>
	public class CorridorBranchRoom : RogueRoom
	{
        /// <summary>
        /// Generates a new corridor branch. Does not load the room or initialize cubes.
        /// </summary>
        /// <param name="width">Width (floor coordinates) of room.</param>
        /// <param name="depth">Depth (floor coordinates) of room.</param>
        /// <param name="height">Height (floor to ceiling) of room.</param>
        /// <param name="x">X coordinate of this cooridor in the parenting RogueDungeon map.</param>
        /// <param name="y">Y coordinate of this cooridor in the parenting RogueDungeon map.</param>
        /// <param name="doorCode">Bitmask representing which directions this corridor has openings.</param>
		public CorridorBranchRoom (int width, int height, int x, int y, int doorCode)
		{
			Width = width;
			Depth = width;
			Height = height;
			GridX = x;
			GridY = y;
			DoorCode = doorCode;
		}

        /// <summary>
        /// Initializes the RoomCubes data structure. Must be called before loading the room,
        /// and after assigning neighbors.
        /// </summary>
		public override void InitializeCubes ()
		{
			LRCorridorCubes lftNbr = (LeftNeighbor != null)  ? (LRCorridorCubes)LeftNeighbor.Cubes  : null;
			LRCorridorCubes rgtNbr = (RightNeighbor != null) ? (LRCorridorCubes)RightNeighbor.Cubes : null;
			UDCorridorCubes upNbr  = (UpNeighbor != null)    ? (UDCorridorCubes)UpNeighbor.Cubes    : null;
			UDCorridorCubes dwnNbr = (DownNeighbor != null)  ? (UDCorridorCubes)DownNeighbor.Cubes  : null;
			
			int[] lftNbrUp  = (lftNbr != null) ? lftNbr.UpWall.GetRightEdge()   : null;
			int[] lftNbrDwn = (lftNbr != null) ? lftNbr.DownWall.GetRightEdge() : null;
			int[] rgtNbrUp  = (rgtNbr != null) ? rgtNbr.UpWall.GetLeftEdge()    : null;
			int[] rgtNbrDwn = (rgtNbr != null) ? rgtNbr.DownWall.GetLeftEdge()  : null;
			int[] upNbrRgt  = (upNbr != null)  ? upNbr.RightWall.GetRightEdge() : null;
			int[] upNbrLft  = (upNbr != null)  ? upNbr.LeftWall.GetRightEdge()  : null;
			int[] dwnNbrRgt = (dwnNbr != null) ? dwnNbr.RightWall.GetLeftEdge() : null;
			int[] dwnNbrLft = (dwnNbr != null) ? dwnNbr.LeftWall.GetLeftEdge()  : null;

			Cubes = new SmallRoomCubes(Width, Depth, Height, DoorCode,
			                           lftNbrUp, lftNbrDwn, rgtNbrUp, rgtNbrDwn,
			                           upNbrLft, upNbrRgt, dwnNbrLft, dwnNbrRgt);
		}
	}
}

