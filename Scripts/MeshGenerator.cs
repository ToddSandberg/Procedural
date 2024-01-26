using Godot;
using Godot.Collections;

public static class MeshGenerator {
	
	public static MeshData GenerateTerrainMesh(float[,] heightMap, int levelOfDetail, float[,] mountainRangeMap, float mountainPriority, float erosionPriority, int multiplier) {
		// Width and height should be the same
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		float topLeftX = (width-1) / -2f;
		float topLeftZ = (height-1) / 2f;

		int meshSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
		int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

		GD.Print("meshSimplificationIncrement:"+meshSimplificationIncrement);
		GD.Print("VerticesPerLine:"+verticesPerLine);
		GD.Print("Width:"+width+" Height:"+height);
		GD.Print("multiplier:"+multiplier);
		MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
		int vertexIndex = 0;

		for (int y = 0; y < height - 1; y += meshSimplificationIncrement) {
			for (int x = 0; x < width - 1; x += meshSimplificationIncrement) {
				//meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
				//meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);

				float height1 = Squared(GetHeight(x,y,heightMap,mountainRangeMap,mountainPriority,erosionPriority));
				float height2 = Squared(GetHeight(x+meshSimplificationIncrement,y,heightMap,mountainRangeMap,mountainPriority,erosionPriority));
				float height3 = Squared(GetHeight(x,y+meshSimplificationIncrement,heightMap,mountainRangeMap,mountainPriority,erosionPriority));
				float height4 = Squared(GetHeight(x+meshSimplificationIncrement,y+meshSimplificationIncrement,heightMap,mountainRangeMap,mountainPriority,erosionPriority));

				// Add at x,y
				meshData.uvs[vertexIndex] = new Vector2(x/(float)(width-1), y/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x, height1 * multiplier, y);
				// Add at x+1,y
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2((x+1)/(float)(width-1), y/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x + meshSimplificationIncrement, height2 * multiplier, y);
				// Add at x,y+1
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2(x/(float)(width-1), (y+1)/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x, height3 * multiplier, y + meshSimplificationIncrement);
				// Add at x,y+1
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2(x/(float)(width-1), (y+1)/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x, height3 * multiplier, y + meshSimplificationIncrement);
				// Add at x+1,y
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2((x+1)/(float)(width-1), y/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x + meshSimplificationIncrement, height2 * multiplier, y);
				// Add at x+1,y+1
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2((x+1)/(float)(width-1), (y+1)/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x + meshSimplificationIncrement, height4 * multiplier, y + meshSimplificationIncrement);

				vertexIndex++;
			}
		}

		/*foreach (var uv in meshData.uvs)
		{
			GD.Print(uv);
		}*/

		GD.Print(vertexIndex);
		return meshData;
	}

	public static float GetHeight(int x, int y, float[,] heightMap, float[,] mountainRangeMap, float mountainPriority, float erosionPriority) {
		return (heightMap[x,y] * erosionPriority) /*+ (mountainRangeMap[x,y] * mountainPriority)*/;
	}

	// TODO there might be a built in function for this, basically just wanted to make higher heights increase faster
	public static float Squared(float num) {
		return ((num*50) * (num*50))/100;
	}
}

public class MeshData {
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uvs;
	public SurfaceTool surfaceTool;

	int triangleIndex;

	public MeshData(int meshWidth, int meshHeight) {
		vertices = new Vector3[(meshWidth - 1)*(meshHeight - 1)*6];
		uvs = new Vector2[(meshWidth - 1)*(meshHeight - 1)*6];
		//uvs = new Vector2[4];
		triangles = new int[(meshWidth - 1)*(meshHeight - 1)*6];
	}

	public void AddTriangle(int a, int b, int c) {
		triangles[triangleIndex] = a;
		triangles[triangleIndex + 1] = b;
		triangles[triangleIndex + 2] = c;
		triangleIndex += 3;
	}

	public ArrayMesh CreateMesh() {
		ArrayMesh mesh = new ArrayMesh();
		//https://docs.godotengine.org/en/3.1/classes/class_arraymesh.html#enum-arraymesh-arraytype
		Array surfaceArray = new Array();
		surfaceArray.Resize((int)ArrayMesh.ArrayType.Max);
		surfaceArray[(int)ArrayMesh.ArrayType.Vertex] = vertices;
		surfaceArray[(int)ArrayMesh.ArrayType.Normal] = vertices;
		surfaceArray[(int)ArrayMesh.ArrayType.TexUV] = uvs;

		mesh.AddSurfaceFromArrays(
			Mesh.PrimitiveType.Triangles,
			surfaceArray
		);
		return mesh;
	}
}
