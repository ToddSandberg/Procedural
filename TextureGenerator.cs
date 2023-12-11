using Godot;
using System;

public static class TextureGenerator {
    public static ImageTexture TextureFromColorMap(Color[] colorMap, int width, int height) {
        /*Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;*/
		return new ImageTexture();
    }

    public static ImageTexture TextureFromHeightMap(float[,] heightMap) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

		Image image = new Image();
		image.Resize(width, height);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
               colorMap[y * width + x] = new Color("#000000").Lerp(new Color("#FFFFFF"), heightMap[x, y]);
            }
        }
        
		ImageTexture imageTexture = ImageTexture.CreateFromImage(image);
		return imageTexture;
    }
}

