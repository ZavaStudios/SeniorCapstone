using System;

namespace MazeGeneration
{
	public class CorridorBranchRoom : RogueRoom
	{
		public CorridorBranchRoom (int width, int height, int x, int y, int doorCode)
		{
			Width = width;
			Depth = width;
			Height = height;
			GridX = x;
			GridY = y;
			DoorCode = doorCode;
			
			// TODO
		}

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
			// TODO: actually spawn cubes
		}
	}
}

