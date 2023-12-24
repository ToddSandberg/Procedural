using Godot;
using System;

// https://www.youtube.com/watch?v=WP-Bm65Q-1Y&list=PLrMEhC9sAD1zprGu_lphl3cQSS3uFIXA9&index=5
public static class Noise {
	public static float[,] GenerateNoiseMap(int mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		float[,] noiseMap = new float[mapSize, mapSize];

		// Erosion
		FastNoiseLite noise = new FastNoiseLite();
		noise.FractalLacunarity = lacunarity;
		noise.FractalOctaves = octaves;
		noise.Seed = seed;
		noise.Offset = new Vector3(offset.X, 0, offset.Y);
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfSize = mapSize / 2f;

		for (int y = 0; y < mapSize; y++) {
			for (int x=0; x < mapSize; x++) {
				float amplitude = 1;
				float noiseHeight = 0;

				float sampleX = (x-halfSize) / scale;
				float sampleY = (y-halfSize) / scale;

				float perlinValue = 1 - Mathf.Abs(noise.GetNoise2D(sampleX, sampleY));
				perlinValue *= perlinValue;
				noiseHeight += perlinValue * amplitude;

				amplitude *= persistance;

				if (noiseHeight > maxNoiseHeight) {
					maxNoiseHeight = noiseHeight;
				} else if (noiseHeight < minNoiseHeight) {
					minNoiseHeight = noiseHeight;
				}

				noiseMap[x, y] = noiseHeight;
			}
		}

		// Dont completely understand this but basically we made the range -1 to 1 above
		// and this is meant to change it back to 0 to 1
		for (int y = 0; y < mapSize; y++) {
			for (int x=0; x < mapSize; x++) {
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}

	public static float[,] GenerateMountainRangeMap(int mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
		float[,] noiseMap = new float[mapSize, mapSize];

		// Erosion
		FastNoiseLite noise = new FastNoiseLite();
		/*noise.FractalLacunarity = lacunarity;
		noise.FractalOctaves = octaves;*/
		noise.Seed = seed;
		noise.Offset = new Vector3(offset.X, 0, offset.Y);
		noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular;

		if (scale <= 0) {
			scale = 0.0001f;
		}

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfSize = mapSize / 2f;

		for (int y = 0; y < mapSize; y++) {
			for (int x=0; x < mapSize; x++) {

				float sampleX = (x-halfSize) / scale;
				float sampleY = (y-halfSize) / scale;

				// TODO what is the *2-1 supposed  to be doing, its causing some weird stuff with cellular noise
				float noiseValue = noise.GetNoise2D(sampleX, sampleY);
				float cellularValue = (noiseValue*2) + 1;
				//noiseHeight += cellularNosie * amplitude;

				if (cellularValue > maxNoiseHeight) {
					maxNoiseHeight = cellularValue;
				} else if (cellularValue < minNoiseHeight) {
					minNoiseHeight = cellularValue;
				}

				noiseMap[x, y] = cellularValue;
			}
		}

		// Dont completely understand this but basically we made the range -1 to 1 above
		// and this is meant to change it back to 0 to 1
		for (int y = 0; y < mapSize; y++) {
			for (int x=0; x < mapSize; x++) {
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
			}
		}

		return noiseMap;
	}
}

