using Godot;
using Godot.Collections;

public static class MeshGenerator {
	
	public static MeshData GenerateTerrainMesh(float[,] heightMap, float[,] mountainRangeMap, float mountainPriority, float erosionPriority, int multiplier) {
		int width = heightMap.GetLength(0);
		int height = heightMap.GetLength(1);
		float topLeftX = (width-1) / -2f;
		float topLeftZ = (height-1) / 2f;

		GD.Print("Width:"+width+" Height:"+height);
		MeshData meshData = new MeshData(width, height);
		int vertexIndex = 0;

		for (int y = 0; y < height - 1; y++) {
			for (int x = 0; x < width - 1; x++) {
				//meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
				//meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);

				if (x == 0 && y == 0) {
					GD.Print("Both x and y are 0!");
				}

				float height1 = GetHeight(x,y,heightMap,mountainRangeMap,mountainPriority,erosionPriority);
				float height2 = GetHeight(x+1,y,heightMap,mountainRangeMap,mountainPriority,erosionPriority);
				float height3 = GetHeight(x,y+1,heightMap,mountainRangeMap,mountainPriority,erosionPriority);
				float height4 = GetHeight(x+1,y+1,heightMap,mountainRangeMap,mountainPriority,erosionPriority);

				// Add at x,y
				meshData.uvs[vertexIndex] = new Vector2(x/(float)(width-1), y/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x, height1 * multiplier, y);
				// Add at x+1,y
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2((x+1)/(float)(width-1), y/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x + 1, height2 * multiplier, y);
				// Add at x,y+1
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2(x/(float)(width-1), (y+1)/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x, height3 * multiplier, y + 1);
				// Add at x,y+1
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2(x/(float)(width-1), (y+1)/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x, height3 * multiplier, y + 1);
				// Add at x+1,y
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2((x+1)/(float)(width-1), y/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x + 1, height2 * multiplier, y);
				// Add at x+1,y+1
				vertexIndex++;
				meshData.uvs[vertexIndex] = new Vector2((x+1)/(float)(width-1), (y+1)/(float)(height - 1));
				meshData.vertices[vertexIndex] = new Vector3(x + 1, height4 * multiplier, (y + 1));

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
		return (heightMap[x,y] * erosionPriority) + (mountainRangeMap[x,y] * mountainPriority);
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
