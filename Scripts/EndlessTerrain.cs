using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class EndlessTerrain : Node {

	[Export]
	public const float maxViewDst = 450;
	[Export]
	public MeshInstance3D viewer;
	[Export]
	public MapGenerator mapGenerator;

	// Trying to get a way to update in the editor
	[Export]
	public int _rerender;
	private int lastRerender = 0;

	public static Vector3 viewerPosition;
	int chunkSize;
	int chunksVisibleInViewDst;

	Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
	List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GD.Print("Ready called in endless terrain");
		chunkSize = MapGenerator.mapChunkSize - 1;
		chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (lastRerender != _rerender) {
			lastRerender = _rerender;
			GD.Print("Rerendering");
			viewerPosition = new Vector3(viewer.Position.X, viewer.Position.Y, viewer.Position.Z);
			UpdateVisibleChunks();
		}
	}

	void UpdateVisibleChunks() {
		GD.Print("Updating visible chunks");
		GD.Print(terrainChunksVisibleLastUpdate.Count);
		foreach (TerrainChunk chunk in terrainChunksVisibleLastUpdate) {
			chunk.SetVisible(false);
			// Dont need to do this unless trying to compeletely reset
			chunk.Delete();
		}
		terrainChunksVisibleLastUpdate.Clear();

		int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.X / chunkSize);
		int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.Y / chunkSize);

		GD.Print("Current chunk x:" + currentChunkCoordX);
		GD.Print("Current chunk y:" + currentChunkCoordY);

		for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++) {
			for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++) {
				Vector2 viewedChunk = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
				GD.Print(terrainChunkDictionary);

				if (terrainChunkDictionary.ContainsKey(viewedChunk)) {
					terrainChunkDictionary[viewedChunk].Update(viewerPosition);
					if (terrainChunkDictionary[viewedChunk].IsVisible()) {
						terrainChunksVisibleLastUpdate.Add(terrainChunkDictionary[viewedChunk]);
					}
				} else {
					terrainChunkDictionary.Add(viewedChunk, new TerrainChunk(viewedChunk, chunkSize, this, mapGenerator));
				}
			}
		}
	}

	public class TerrainChunk {
		MeshInstance3D meshObject;
		Vector2 position;

		public TerrainChunk(Vector2 coord, int size, Node parent, MapGenerator mapGenerator) {
			position = coord * size;
			GD.Print("Position:" + position);
			GD.Print(parent);

			Vector3 positionV3 = new Vector3(position.X, 0, position.Y);
			meshObject = new MeshInstance3D();
			meshObject.Name = "MapChunk"+position.X+","+position.Y;
			
			parent.AddChild(meshObject);
			meshObject.Owner = parent.Owner;
			
			mapGenerator.DrawMap(meshObject, position);
			meshObject.Position = positionV3;
			meshObject.Scale = Vector3.One; // TODO might need to divide by 10
			meshObject.CreateTrimeshCollision();
			SetVisible(true);
		}

		public void Update(Vector3 viewerPosition) {
			// TODO check distance of person to 
			float distance = meshObject.Position.DistanceTo(viewerPosition);
			GD.Print("distance:" + distance);
			bool visible = distance <= maxViewDst;
			SetVisible(visible);
		}

		public void SetVisible(bool visible) {
			meshObject.Visible = visible;
		}

		public bool IsVisible() {
			return meshObject.Visible;
		}

		public void Delete() {
			meshObject.QueueFree();
		}
	}
}
