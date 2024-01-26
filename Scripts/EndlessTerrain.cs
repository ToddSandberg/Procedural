using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class EndlessTerrain : Node {

	[Export]
	public const float maxViewDst = 240;
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

	// Just for deleting on rerender for now
	List<Node3D> objects = new List<Node3D>();

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
			// TODO clear out old houses
			for (int x=0; x < 15; x++) {
				GenerateHouse();
			}
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

	void GenerateHouse() {
		GD.Print("Generating house");
		// 1. for now, get a random x and y between -480 and 480
		int randX = (int)(GD.Randi() % (maxViewDst * 2)) - (int)maxViewDst;
		int randY = (int)(GD.Randi() % (maxViewDst * 2)) -(int) maxViewDst;
		// 2. figure out which chunk this is in
		int randChunkCoordX = Mathf.RoundToInt(randX / chunkSize);
		int randChunkCoordY = Mathf.RoundToInt(randY / chunkSize);
		TerrainChunk currentChunk = terrainChunkDictionary[new Vector2(randChunkCoordX, randChunkCoordY)];
		// 3. get height of position in chunk
		// Mod by chunksize to get relative x,y coordinates within the chunk
		// TODO TBH this still doesnt work perfect, if there is a way to get the exact height of the mesh might be better (maybe raycast?)
		/*int xPositionInChunk = Math.Abs(randX%chunkSize);
		int yPositionInChunk = Math.Abs(randY%chunkSize);
		GD.Print(xPositionInChunk);
		GD.Print(yPositionInChunk);
		GD.Print(currentChunk.GetHeight(xPositionInChunk, yPositionInChunk));*/
		// TODO temporary, trying to mock what we do in terrain generation, this should be extracted
		//float height = MeshGenerator.Squared(currentChunk.GetHeight(xPositionInChunk, yPositionInChunk) * 0.5f) * 50;

		// 4. instantiate house object
		var house = GD.Load<PackedScene>("res://Scenes/House.tscn");
		Node3D instance = (Node3D) house.Instantiate();
		this.AddChild(instance);
		instance.Owner = this.Owner;
		instance.Scale = Vector3.One;
		instance.Position = new Vector3(randX, 1000, randY);

		// Get height by raycast
		RayCast3D rayCast = (RayCast3D) instance.GetNode("RayCast3D");
		rayCast.ForceRaycastUpdate();
		GD.Print(rayCast.IsPhysicsProcessing());
		GD.Print(rayCast.TargetPosition);
		GD.Print(rayCast.CollisionMask);
		GD.Print(rayCast.Enabled);
		float height = 0;
		if (rayCast.IsColliding()) {
			height = rayCast.GetCollisionPoint().Y;
		} else {
			GD.PrintErr("House raycast is not colliding, height will be 0");
		}
		instance.Position = new Vector3(randX, height + 0.5f, randY);

		// 5. add to dictionary
		objects.Add(instance);
		GD.Print("Generated house at " + randX + ", " + height + ", " + randY);
	}

	public class TerrainChunk {
		MeshInstance3D meshObject;
		Vector2 position;
		// Storing mostly for heightmap
		MapData mapData;

		public TerrainChunk(Vector2 coord, int size, Node parent, MapGenerator mapGenerator) {
			position = coord * size;
			GD.Print("Position:" + position);
			GD.Print(parent);

			Vector3 positionV3 = new Vector3(position.X, 0, position.Y);
			meshObject = new MeshInstance3D();
			meshObject.Name = "MapChunk"+position.X+","+position.Y;
			
			parent.AddChild(meshObject);
			meshObject.Owner = parent.Owner;
			
			// Create mesh
			mapData = mapGenerator.DrawMap(meshObject, position);
			meshObject.Position = positionV3;
			meshObject.Scale = Vector3.One; // TODO might need to divide by 10
			meshObject.CreateTrimeshCollision();
			SetVisible(true);

			// Add water
			var scene = GD.Load<PackedScene>("res://Scenes/WaterPlaneScene.tscn");
			Node3D instance = (Node3D) scene.Instantiate();
			meshObject.AddChild(instance);
			instance.Owner = parent.Owner;
			instance.Scale = new Vector3(size, size, size);
			instance.Position = new Vector3(size/2, 95, size/2);
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

		public float GetHeight(int x, int y) {
			return mapData.noiseMap[x,y];
		}
	}
}
