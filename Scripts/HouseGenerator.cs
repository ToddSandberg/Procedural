using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class HouseGenerator : Node
{
	[Export]
	public EndlessTerrain terrain;
	[Export]
	public NPCManager npcManager;

	// Just for deleting on rerender for now
	List<Node3D> houses = new List<Node3D>();

	bool initialLoadCompleted = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		// Just an example of how it would work, in reality it should be called by some game manager
		if (!initialLoadCompleted && !terrain.loading) {
			//GenerateTown();
			List<House> houses = HouseLoader.Load();
			initialLoadCompleted = true;
		}
	}

	public void GenerateTown() {
		House startingHouse = new House();
		GenerateHouse(startingHouse);

		// TODO create town name generator
		string townName = "Temp town name";
		startingHouse.town = townName;

		List<Character> family = npcManager.GenerateFamily(townName);
		foreach (Character familyMember in family) {
			startingHouse.household.Add(familyMember.id);
		}

		SaveHouse(startingHouse);
		npcManager.SaveNpcs();
	}

	void SaveHouse(House house) {
		GD.Print("Saving to res://Data/Houses/"+house.town+"_"+house.id+".json");
		HouseLoader.Save(house, "res://Data/Houses/"+house.town+"_"+house.id+".json");
	}

	void GenerateHouse(House house) {
		GD.Print("Generating house");
		float maxViewDst = SettingsDefaults.MAX_VIEW_DISTANCE_DEFAULT;
		int chunkSize = terrain.chunkSize;

		// 1. for now, get a random x and y between -480 and 480
		int randX = (int)(GD.Randi() % (maxViewDst * 2)) - (int)maxViewDst;
		int randY = (int)(GD.Randi() % (maxViewDst * 2)) - (int)maxViewDst;

		bool validHeightFound = false;
		int currentTries = 0;
	

		while(!validHeightFound && currentTries < 50) {
			// 2. figure out which chunk this is in
			int randChunkCoordX = Mathf.RoundToInt(randX / chunkSize);
			int randChunkCoordY = Mathf.RoundToInt(randY / chunkSize);
			EndlessTerrain.TerrainChunk currentChunk = terrain.terrainChunkDictionary[new Vector2(randChunkCoordX, randChunkCoordY)];
			// 3. get height of position in chunk
			// Mod by chunksize to get relative x,y coordinates within the chunk
			int xPositionInChunk = Math.Abs(randX%chunkSize);
			int yPositionInChunk = Math.Abs(randY%chunkSize);
			float heightMapHeight = currentChunk.GetHeight(xPositionInChunk, yPositionInChunk);
			if (heightMapHeight > 0.32f && heightMapHeight < 0.37f) {
				GD.Print("Valid height found");
				validHeightFound = true;
			} else {
				GD.Print("inValid height found: " + heightMapHeight);
				currentTries++;

				randX = (int)(GD.Randi() % (maxViewDst * 2)) - (int)maxViewDst;
				randY = (int)(GD.Randi() % (maxViewDst * 2)) - (int)maxViewDst;
			}
		}

		if (!validHeightFound) {
			GD.Print("Valid height not found");
		}

		// 4. instantiate house object
		var houseObj = GD.Load<PackedScene>("res://Scenes/House.tscn");
		Node3D instance = (Node3D) houseObj.Instantiate();
		this.AddChild(instance);
		instance.Owner = this.Owner;
		instance.Scale = Vector3.One;
		instance.Position = new Vector3(randX, 1000, randY);

		// Get height by raycast
		RayCast3D rayCast = (RayCast3D) instance.GetNode("RayCast3D");
		rayCast.ForceRaycastUpdate();
		float height = 0;
		if (rayCast.IsColliding()) {
			height = rayCast.GetCollisionPoint().Y;
		} else {
			GD.PrintErr("House raycast is not colliding, height will be 0");
		}
		instance.Position = new Vector3(randX, height + 0.5f, randY);
		house.positionx = randX;
		house.positiony = height + 0.5f;
		house.positionz = randY;

		// 5. add to dictionary
		houses.Add(instance);
		GD.Print("Generated house at " + randX + ", " + height + ", " + randY);
	}
}
