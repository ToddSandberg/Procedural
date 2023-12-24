using Godot;
using System;

//https://www.youtube.com/watch?v=WP-Bm65Q-1Y&list=PLrMEhC9sAD1zprGu_lphl3cQSS3uFIXA9&index=5
public partial class MapDisplay : Node
{
	/*[Export]
	public Renderer textureRenderer;*/

	// Generates colors on plane to display noiseMap
	public void DrawTexture(Texture2D texture) {
		/*textureRenderer.sharedMaterial.mainTexture = texture;
		textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);*/
	}

	public void DrawMesh(MeshData meshData, Texture2D texture) {
		GD.Print("Should draw mesh");
		MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		mesh.Mesh = meshData.CreateMesh();
		//textureRenderer.sharedMaterial.SetTexture("_MainTex", texture);
	}
}
