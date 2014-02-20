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

			/*
			Cubes = new SmallRoomCubes(Width, Depth, Height, DoorCode,
			                           lftNbr, rgtNbr, upNbr, dwnNbr);
			                           */
			// TODO: actually spawn cubes
		}
	}
}

