using Godot;
using System.Collections.Generic;

[Tool]
public partial class NPCManager : Node {
	List<Character> npcs = new List<Character>();

	CharacterLoader characterLoader;
	NPCGenerator npcGenerator;
	RandomNumberGenerator rnd;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        rnd = new RandomNumberGenerator();
		npcGenerator = new NPCGenerator();
		characterLoader = new CharacterLoader();
		// This class should
		// TODO this is just for testing
		GenerateFamily("Asgard");

		// TODO saving just for testing
		SaveNpcs();
	}

	public void SaveNpcs() {
		foreach (Character npc in npcs) {
			GD.Print("Saving to res://Data/NPCs/"+npc.firstName+"_"+npc.lastName+".json");
			characterLoader.Save(npc, "res://Data/NPCs/"+npc.firstName+"_"+npc.lastName+".json");
		}
	}

	public List<Character> GenerateFamily(string location) {
		List<Character> newNpcs = new List<Character>();
		Character father = npcGenerator.GenerateNPC(new AgeRange(20, 40), Gender.MALE);
		Character mother = npcGenerator.GenerateNPC(new AgeRange(20, 40), Gender.FEMALE);
		int numOfChildren = rnd.RandiRange(0, 4);
		for (int i = 0; i < numOfChildren; i++) {
			int age = rnd.RandiRange(0, 16);
			Character child = npcGenerator.GenerateChild(mother, father, age, location);
			father.children.Add(child.id);
			mother.children.Add(child.id);
			npcs.Add(child);
			newNpcs.Add(child);
		}
		npcs.Add(father);
		newNpcs.Add(father);
		npcs.Add(mother);
		newNpcs.Add(mother);

		return newNpcs;
	}
}