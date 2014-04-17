using System;
using UnityEngine;

namespace MazeGeneration
{
    /// <summary>
    /// Generates a grid of smooth noise for you to use for whatever. :D
	/// 
	/// Quick note: most of this code is scraped straight out of an old project of mine (Aric). This
    /// code has no pretense of being optimized - that can come later. For now, this is just a proof
    /// of concept.
	/// </summary>
	public class PerlinNoise
    {
        private static int[,] NOISE;
		private static int FREQ;
		private static float PERSISTENCE = .69f;
		private static int OVERLAY_CNT = 3;
        
        /// <summary>
		/// Returns a 128x128 array with values ranging from 0 to 100. We recommend
		/// using the values as percentages, and index into the array based on scaled
		/// results, but YMMV.
		/// </summary>
		/// <param name="seedArray">Array describing seeded values for the generation
		/// function. Null by default.</param>
		/// <returns>Grid of noise.</returns>
		public static int[,] GenerateNoise128()
		{
            int WIDTH = 128;
            int HEIGHT = 128;
			int[,,] overlays = new int[WIDTH,HEIGHT,OVERLAY_CNT];
			FREQ = 64;

			// Build overlays:
			for (int i = 0; i < OVERLAY_CNT; i++)
			{
				NOISE = new int[WIDTH/FREQ + 2, HEIGHT/FREQ + 2];

				// Build this level's noise:
				for (int x = 0; x < NOISE.GetLength(0) - 1; x++)
				{
					for (int y = 0; y < NOISE.GetLength(1) - 1; y++)
					{
						NOISE[x,y] = Maze.rnd.Next (-50, 51);
					}
				}

				// Build this level's overlay:
				for (int x = 0; x < WIDTH; x++)
				{
					for (int y = 0; y < HEIGHT; y++)
					{
						int a = NOISE[x / FREQ, y / FREQ];
						int b = NOISE[(x / FREQ) + 1, y / FREQ];
						int c = NOISE[x / FREQ, (y / FREQ) + 1];
						int d = NOISE[(x / FREQ) + 1, (y / FREQ) + 1];

						overlays[x,y,i] = interp_2((float)(x % FREQ) / (float)FREQ,
						         				   (float)(y % FREQ) / (float)FREQ,
						         				   a,b,c,d);
					}
				}
			}

			// Merge overlays:
			int[,] toRet = new int[WIDTH,HEIGHT];
			for (int x = 0; x < WIDTH; x++)
			{
				for (int y = 0; y < HEIGHT; y++)
				{
					float h = 0.0f;
					float curPersistence = 1.0f;
					for (int i = 0; i < OVERLAY_CNT; i++)
					{
						h += (float)overlays[x,y,i]*curPersistence;
						curPersistence *= PERSISTENCE;
					}

					toRet[x,y] = (int)h + 50;
				}
			}

			return toRet;
		}

		private static int interp_2(float x, float y, float a, float b, float c, float d)
		{
			float i1 = interp(x, a, b);
			float i2 = interp(x, c, d);
			return (int)interp(y, i1, i2);
		}

		private static float interp(float t, float a, float b)
		{
			return (a * (1.0f - t)) + (b * t);
		}

        /*
        /// <summary>
        /// Generates a grid of random noise, smoothed a bit at a local level. Not
        /// great for generating a large grid of random noise, but at a very small
        /// resolution, this should do an alright job.
        /// </summary>
        /// <param name="width">Width of the noise to generate</param>
        /// <param name="height">Height of the noise to generate</param>
        /// <returns>2D integer array varying between 0 to 100 (think percentages)</returns>
        public static int[,] GenerateNoise(int width, int height)
        {
            int[,] pregen = new int[width, height];
            int[,] toRet  = new int[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    pregen[x, y] = Maze.rnd.Next(101);
            // Smooth it:
            // Fenceposts:
            // Corners:
            toRet[0,0]              = (pregen[0,0] + pregen[1,0] +
                                       pregen[0,1] + pregen[1,1]) / 4;
            toRet[width-1,0]        = (pregen[width-1,0] + pregen[width-2,0] +
                                       pregen[width-1,1] + pregen[width-2,1]) / 4;
            toRet[0,height-1]       = (pregen[0,height-1] + pregen[1,height-1] +
                                       pregen[0,height-2] + pregen[1,height-2]) / 4;
            toRet[width-1,height-1] = (pregen[width-1,height-1] + pregen[width-2,height-1] +
                                       pregen[width-1,height-2] + pregen[width-2,height-2]) / 4;
            for (int x = 1; x < width-1; x++)
            {
                // TOP:
                toRet[x,0] = (pregen[x-1, 0] + pregen[x,0] + pregen[x+1,0] +
                              pregen[x-1, 1] + pregen[x,1] + pregen[x+1,1]) / 6;
                // BOT:
                toRet[x,height-1] = (pregen[x-1,height-1] + pregen[x,height-1] + pregen[x+1,height-1] +
                                     pregen[x-1,height-2] + pregen[x,height-2] + pregen[x+1,height-2]) / 6;
            }
            for (int y = 1; y < height-1; y++)
            {
                // LEFT:
                toRet[0,y] = (pregen[0,y-1] + pregen[1,y-1] +
                              pregen[0,y]   + pregen[1,y] +
                              pregen[0,y+1] + pregen[1,y+1]) / 6;
                // RIGHT:
                toRet[width-1,y] = (pregen[width-1,y-1] + pregen[width-2,y-1] +
                                    pregen[width-1,y]   + pregen[width-2,y] +
                                    pregen[width-1,y+1] + pregen[width-2,y+1]) / 6;
            }

            // Rest:
            for (int x = 1; x < width - 1; x++)
            {
                for (int y = 1; y < height - 1; y++)
                {
                    toRet[x,y] = (pregen[x-1,y-1] + pregen[x,y-1] + pregen[x+1,y-1] +
                                  pregen[x-1,y]   + pregen[x,y]   + pregen[x+1,y]   +
                                  pregen[x-1,y+1] + pregen[x,y+1] + pregen[x+1,y+1]) / 9;
                }
            }

            return toRet;
        }
         */
	}
}

