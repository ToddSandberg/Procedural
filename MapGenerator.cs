using Godot;
using System;
using System.Diagnostics;

[Tool]
public partial class MapGenerator : Node {

	[Export]
	public int mapSize;
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
	public TerrainType[] regions = new TerrainType[] {
		new TerrainType(0.3f, new Color("#326E9A"), "water"),
		new TerrainType(0.6f, new Color("#2C8F2C"), "land"),
		new TerrainType(0.7f, new Color("#604421"), "rock"),
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
		GenerateMap();
	}

	public override void _Process(double delta)
	{
		if (lastRerender != _rerender) {
			lastRerender = _rerender;
			GenerateMap();
		}
	}

	 public void GenerateMap() {
		float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
		float[,] mountainRangeMap = Noise.GenerateMountainRangeMap(mapSize, seed, mountainRangeNoiseScale, octaves, persistance, lacunarity, offset);
		falloffMap = FalloffGenerator.GenerateFalloffMap(mapSize);

		GD.Print("Generating map");

		// Generate colors based on height
		Image image = Image.Create(mapSize - 1, mapSize - 1, false, Image.Format.Rgb8);
		GD.Print(image.GetSize());
		for (int y = 0; y < mapSize; y++) {
			for (int x = 0; x < mapSize; x++) {
				if (useFalloff) {
					noiseMap[x,y] = Mathf.Clamp(noiseMap[x,y] - falloffMap[x, y], 0, 1);
				}

				if (y < mapSize - 1 && x < mapSize - 1) {
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

		
		MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, mountainRangeMap, mountainPriority, erosionPriority, heightMultiplier);
		MeshInstance3D mesh = GetNode<MeshInstance3D>("MeshInstance3D");
		mesh.Mesh = meshData.CreateMesh();
		image.GenerateMipmaps();
		ImageTexture imageTexture = ImageTexture.CreateFromImage(image);
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
