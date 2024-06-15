using Godot;
using System.Collections.Generic;

public partial class NPCManager : Node
{
	List<Character> npcs = new List<Character>();

	CharacterLoader characterLoader;
	NPCGenerator npcGenerator;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		npcGenerator = new NPCGenerator();
		characterLoader = new CharacterLoader();
		// This class should
		// TODO this is just for testing
		Character father = npcGenerator.GenerateNPC(new AgeRange(20, 40), Gender.MALE);
		Character mother = npcGenerator.GenerateNPC(new AgeRange(20, 40), Gender.FEMALE);
		Character child = npcGenerator.GenerateChild(mother, father, 3, "Asgard");
		father.children.Add(child.id);
		mother.children.Add(child.id);
		npcs.Add(father);
		npcs.Add(mother);
		npcs.Add(child);
		// 1. fetch all existing npcs on load
		// TODO what do if there are no existing to load
		// 2. Generate relationship map

		// TODO saving just for testing
		SaveNpcs();
	}

	public void SaveNpcs() {
		foreach (Character npc in npcs) {
			GD.Print("Saving to res://Data/NPCs/"+npc.firstName+"_"+npc.lastName+".json");
			characterLoader.Save(npc, "res://Data/NPCs/"+npc.firstName+"_"+npc.lastName+".json");
		}
	}
}