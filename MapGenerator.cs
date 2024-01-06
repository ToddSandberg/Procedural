using Godot;
using System;
using System.Diagnostics;

[Tool]
public partial class MapGenerator : Node {

	public static int mapChunkSize = 241;
	// TODO Cant go higher than 6, should check and throw if it is
	[Export]
	public int levelOfDetail;
	[Export]
	public float noiseScale;
	[Export]
	public int octaves;
	[Export]
	public float persistance;
	[Export]
	public float lacunarity;
	[Export]
	public int seed;
	[Export]
	public Vector2 offset;
	[Export]
	public int heightMultiplier;

	[Export]
	public float mountainRangeNoiseScale;
	[Export]
	public float mountainPriority;
	[Export]
	public float erosionPriority;

	// Trying to get a way to update in the editor
	[Export]
	public int _rerender;
	private int lastRerender = 0;

	// Bleh somehow should make this editable in inspector
	// Also because we arent normalizing the heights, its not going to always be 0 to 1, thus the weird values. Not great but better than having to figure out normalized values for seems for now
	public TerrainType[] regions = new TerrainType[] {
		new TerrainType(0.28f, new Color("#326E9A"), "water"),
		new TerrainType(0.31f, new Color("#2C8F2C"), "land"),
		new TerrainType(0.35f, new Color("#604421"), "rock"),
		new TerrainType(1f, new Color("#FFFFFF"), "snow")
	};

	[Export]
	public bool useFalloff;
	[Export]
	public Node displayNode;

	float[,] falloffMap;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//DrawMapInEditor();
	}

	public override void _Process(double delta)
	{
		if (lastRerender != _rerender) {
			lastRerender = _rerender;
			GD.Print("Rerender!!");
			DrawMapInEditor();
		}
	}

	MapData GenerateMapData(Vector2 localOffset) {
		GD.Print("Map size:"+mapChunkSize);
		float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, localOffset);
		// TODO not being used anymore
		float[,] mountainRangeMap = Noise.GenerateMountainRangeMap(mapChunkSize, seed, mountainRangeNoiseScale, octaves, persistance, lacunarity, localOffset);
		falloffMap = FalloffGenerator.GenerateFalloffMap(mapChunkSize);

		GD.Print("Generating map");

		// Generate colors based on height
		Image image = Image.Create(mapChunkSize - 1, mapChunkSize - 1, false, Image.Format.Rgb8);
		GD.Print(image.GetSize());
		for (int y = 0; y < mapChunkSize; y++) {
			for (int x = 0; x < mapChunkSize; x++) {
				if (useFalloff) {
					noiseMap[x,y] = Mathf.Clamp(noiseMap[x,y] - falloffMap[x, y], 0, 1);
				}

				if (y < mapChunkSize - 1 && x < mapChunkSize - 1) {
					// Get the middle of noiseMap and mountain range
					float currentHeight = MeshGenerator.GetHeight(x, y, noiseMap, mountainRangeMap, mountainPriority, erosionPriority);
					for (int i = 0; i < regions.Length; i++) {
						if (currentHeight <= regions[i].height) {
							image.FillRect(new Rect2I(x, y, 1, 1), regions[i].color);
							break;
						}
					}
				}
			}
		}

		return new MapData(noiseMap, mountainRangeMap, image);
	}

	public void DrawMapInEditor() {
		MapData mapData = GenerateMapData(offset);
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, levelOfDetail, mapData.mountainRangeMap, mountainPriority, erosionPriority, heightMultiplier);
		MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		mesh.Mesh = meshData.CreateMesh();
		mapData.image.GenerateMipmaps();
		ImageTexture imageTexture = ImageTexture.CreateFromImage(mapData.image);
		StandardMaterial3D meshMaterial = new StandardMaterial3D();
		meshMaterial.TextureFilter = BaseMaterial3D.TextureFilterEnum.NearestWithMipmaps;
		meshMaterial.AlbedoTexture = imageTexture;
		mesh.SetSurfaceOverrideMaterial(0, meshMaterial);
		//display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier),);
	}

	public void DrawMap(MeshInstance3D mesh, Vector2 offset) {
		MapData mapData = GenerateMapData(offset);
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.noiseMap, levelOfDetail, mapData.mountainRangeMap, mountainPriority, erosionPriority, heightMultiplier);
		mesh.Mesh = meshData.CreateMesh();
		mapData.image.GenerateMipmaps();
		ImageTexture imageTexture = ImageTexture.CreateFromImage(mapData.image);
		StandardMaterial3D meshMaterial = new StandardMaterial3D();
		meshMaterial.TextureFilter = BaseMaterial3D.TextureFilterEnum.NearestWithMipmaps;
		meshMaterial.AlbedoTexture = imageTexture;
		mesh.SetSurfaceOverrideMaterial(0, meshMaterial);
		//display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier),);
	}
}

public struct TerrainType {
	public float height;
	public Color color;
	public string name;

	public TerrainType(float height, Color color, string name) {
		this.height = height;
		this.color = color;
		this.name = name;
	}
}

public struct MapData {
	public float[,] noiseMap;
	public float[,] mountainRangeMap;
	public Image image;

	public MapData(float[,] noiseMap, float[,] mountainRangeMap, Image image) {
		this.noiseMap = noiseMap;
		this.mountainRangeMap = mountainRangeMap;
		this.image = image;
	}
}
