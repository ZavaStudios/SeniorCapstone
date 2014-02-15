using System;

namespace MazeGeneration
{
	public class CorridorBranchRoom : RogueRoom
	{
		public CorridorBranchRoom (int width, int depth, int height, int x, int y, int doorCode)
		{
			Width = width;
			Depth = depth;
			Height = height;
			GridX = x;
			GridY = y;
			DoorCode = doorCode;
			
			// TODO
		}

		public override void InitializeCubes ()
		{
			// TODO
		}
	}
}

