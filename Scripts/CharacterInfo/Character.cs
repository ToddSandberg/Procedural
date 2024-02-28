using Godot;
using System;
using System.Collections.Generic;

public enum Gender {
	MALE,
	FEMALE
}

public class Character {
	public Race race;
	public Gender gender;
	public List<PersonalityTrait> personalityTraits;
	public List<string> backstoryDetails;
	public Job job;

	// Stats
	public int dexterity;
	public int strength;
	public int constitution;
	public int intelligence;
	public int wisdom;
	public int charisma;
}
