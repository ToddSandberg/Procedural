using Godot;
using System;

// https://www.youtube.com/watch?v=WP-Bm65Q-1Y&list=PLrMEhC9sAD1zprGu_lphl3cQSS3uFIXA9&index=5
public static class Noise {
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

		// TODO I'm pretty sure this class takes in properties that could reduce the complexity of this code
		FastNoiseLite noise = new FastNoiseLite();
        noise.FractalLacunarity = lacunarity;
        noise.FractalOctaves = octaves;
        noise.Seed = seed;
        noise.Offset = new Vector3(offset.X, 0, offset.Y);

        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for (int y = 0; y < mapHeight; y++) {
            for (int x=0; x < mapWidth; x++) {
                float amplitude = 1;
                float noiseHeight = 0;

                float sampleX = (x-halfWidth) / scale;
                float sampleY = (y-halfHeight) / scale;

                float perlinValue = noise.GetNoise2D(sampleX, sampleY) * 2 - 1;
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
        for (int y = 0; y < mapHeight; y++) {
            for (int x=0; x < mapWidth; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}

