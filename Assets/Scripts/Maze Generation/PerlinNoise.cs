using System;

namespace MazeGeneration
{
	/// <summary>
	/// Generates a grid of smooth noise for you to use for whatever. :D
	/// 
	/// Quick note: most of this code is scraped straight out of an old project of mine. This code has no
	/// pretense of being optimized - that can come later. For now, I just want to get a proof of concept.
	/// </summary>
	public class PerlinNoise
	{
		private static int[,] NOISE;
		private static int FREQ;
		private static float PERSISTENCE = 0.65f;
		private static int OVERLAY_CNT = 7;

		/// <summary>
		/// Returns a 128x128 array with values ranging from 0 to 100. We recommend
		/// using the values as percentages, and index into the array based on scaled
		/// results, but YMMV.
		/// </summary>
		/// <returns>Grid of noise.</returns>
		public static int[,] GenerateNoise128()
		{
			Random rnd = new Random();
			int[,,] overlays = new int[128,128,OVERLAY_CNT];
			FREQ = 64;

			// Build overlays:
			for (int i = 0; i < OVERLAY_CNT; i++)
			{
				NOISE = new int[128/FREQ + 2, 128/FREQ + 2];

				// Build this level's noise:
				for (int x = 0; x < NOISE.GetLength(0) - 1; x++)
				{
					for (int y = 0; y < NOISE.GetLength(1) - 1; y++)
					{
						NOISE[x,y] = rnd.Next (0, 101);
					}
				}

				// Build this level's overlay:
				for (int x = 0; x < 128; x++)
				{
					for (int y = 0; y < 128; y++)
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
			int[,] toRet = new int[128,128];
			for (int x = 0; x < 128; x++)
			{
				for (int y = 0; y < 128; y++)
				{
					float h = 0.0f;
					float curPersistence = 1.0f;
					for (int i = 0; i < OVERLAY_CNT; i++)
					{
						h += (float)overlays[x,y,i]*curPersistence;
						curPersistence *= PERSISTENCE;
					}

					toRet[x,y] = (int)h;
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
	}
}

