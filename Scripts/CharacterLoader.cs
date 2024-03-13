using Godot;
using System;
using System.Text.Json;

public partial class CharacterLoader : Node
{
	public override void _Ready()
	{
		/*Character character = CreateExampleCharacter();
		Save(character, "res://Data/exampleCharacter.json");*/
		Character character = Load("res://Data/exampleCharacter.json");
		GD.Print("Loaded a character with name: " + character.name);
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
		Race race = new Race();
		race.name = "Human";
		character.race = race;
		character.gender = Gender.MALE;
		PersonalityTrait[] personalityTraits = new PersonalityTrait[1];
		PersonalityTrait personalityTrait = new PersonalityTrait();
		personalityTrait.name = "Shy";
		personalityTraits[0] = personalityTrait;
		string[] backgroundDetails = new string[1];
		backgroundDetails[0] = "Comes from a land far away";
		character.job = new Job() { name = "Farmer" };
		character.dexterity = 10;
		character.strength = 10;
		character.constitution = 10;
		character.intelligence = 10;
		character.wisdom = 10;
		character.charisma = 10;
		return character;
	}
}
