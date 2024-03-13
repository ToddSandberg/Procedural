using Godot;
using System;
using System.Collections.Generic;

public enum Gender {
	MALE,
	FEMALE
}

public class Character {
	public string name { get; set; }
	public Race race { get; set; }
	public Gender gender { get; set; }
	public List<PersonalityTrait> personalityTraits { get; set; }
	public List<string> backstoryDetails { get; set; }
	public Job job { get; set; }

	// Stats
	public int dexterity { get; set; }
	public int strength { get; set; }
	public int constitution { get; set; }
	public int intelligence { get; set; }
	public int wisdom { get; set; }
	public int charisma { get; set; }
}
