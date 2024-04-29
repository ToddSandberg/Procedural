using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class CharacterLoader : Node
{
	public override void _Ready()
	{
		NPCGenerator npcGenerator = new NPCGenerator();
		Character character = npcGenerator.GenerateNPC(new AgeRange(0, 60));
		Save(character, "res://Data/exampleCharacter.json");
	}

	public void Save(Character character, string path) {
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		string jsonAsString = JsonSerializer.Serialize(character);
		file.StoreString(jsonAsString);
	}

	public Character Load(string path) {
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string content = file.GetAsText();
		GD.Print(content);
		Character character = JsonSerializer.Deserialize<Character>(content);
		return character;
	}

	public Character CreateExampleCharacter() {
		Character character = new Character();
		character.firstName = "Joe";
		character.lastName = "Shmoe";
		Race race = new Race();
		Origin origin = new Origin();
		origin.name = "Human";
		race.AddOrigins(origin, 1);
		character.race = race;
		character.gender = Gender.MALE;
		List<PersonalityTrait> personalityTraits = new List<PersonalityTrait>();
		PersonalityTrait personalityTrait = new PersonalityTrait();
		personalityTrait.name = "Shy";
		personalityTraits.Add(personalityTrait);
		character.personalityTraits = personalityTraits;
		character.job = new Job() { name = "Farmer" };
		return character;
	}
}
