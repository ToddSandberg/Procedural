using Godot;
using System;
using System.Collections.Generic;

public enum Gender {
	MALE,
	FEMALE
}

public class Character {
	public string firstName { get; set; }
	public string lastName { get; set; }
	public Race race { get; set; }
	public Gender gender { get; set; }
	public List<PersonalityTrait> personalityTraits { get; set; }
	public BackstoryDetails backstoryDetails { get; set; }
	public Job job { get; set; }
	public int age;

	// Stats
	public int dexterity { get; set; }
	public int strength { get; set; }
	public int constitution { get; set; }
	public int intelligence { get; set; }
	public int wisdom { get; set; }
	public int charisma { get; set; }
}
