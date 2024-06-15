using Godot;
using System;
using System.Collections.Generic;

public enum Gender {
	MALE,
	FEMALE
}

public enum StatType {
	DEXTERITY,
	STRENGTH,
	CONSTITUTION,
	INTELLIGENCE,
	WISDOM,
	CHARISMA
}

public class Character {
	public string id { get; set; } = System.Guid.NewGuid().ToString();
	public string firstName { get; set; }
	public string lastName { get; set; }
	public Race race { get; set; }
	public Gender gender { get; set; }
	public List<PersonalityTrait> personalityTraits { get; set; }
	public BackstoryDetails backstoryDetails { get; set; }
	public Job job { get; set; }
	public int age { get; set; }
	public string motherId { get; set; }
	public string fatherId { get; set; }
	public List<string> children { get; set; } = new List<string>();

	// Stats
	public Dictionary<StatType, int> stats { get; set; }
}
