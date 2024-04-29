using Godot;
using System;
using System.Collections.Generic;

public partial class NPCGenerator
{
	List<string> maleFirstNames = new List<string>();
	List<string> femaleFirstNames = new List<string>();
	List<string> lastNames = new List<string>();
	List<Origin> possibleOrigins = new List<Origin>();
	List<PersonalityTrait> possiblePersonalityTraits = new List<PersonalityTrait>();

	StatCalculator statCalculator;

	public NPCGenerator() {
		NameLoader nameLoader = new NameLoader();
		statCalculator = new StatCalculator();
		maleFirstNames = nameLoader.Load("res://Data/Characters/Names/First/Male");
		femaleFirstNames = nameLoader.Load("res://Data/Characters/Names/First/Female");
		lastNames = nameLoader.Load("res://Data/Characters/Names/Last");
		OriginLoader originLoader = new OriginLoader();
		possibleOrigins = originLoader.Load("res://Data/Characters/RaceOrigins");
		PersonalityTraitLoader personalityTraitLoader = new PersonalityTraitLoader();
		possiblePersonalityTraits = personalityTraitLoader.Load("res://Data/Characters/PersonalityTraits");
	}

	// Used to return an npc birthed from two people
	public Character GenerateNPC(Character mother, Character father, int age, string currentLocation) {
		Random rand = new Random();
		Character npc = new Character();
		npc.gender = GenerateGender(rand);

		// TODO could be influenced by known people/stories from parents
		// TODO eventually maybe have race specific names
		npc.firstName = GenerateFirstName(npc.gender, rand);
		npc.lastName = father.lastName;
		npc.age = age;

		npc.race = new Race();
		npc.race.AddOrigins(mother.race.GetOrigins());
		npc.race.AddOrigins(father.race.GetOrigins());

		List<PersonalityTrait> personalityTraits = new List<PersonalityTrait>
        {
            possiblePersonalityTraits[rand.Next(0, possiblePersonalityTraits.Count)]
        };
		npc.personalityTraits = personalityTraits;

		BackstoryDetails backstory = new BackstoryDetails();
		backstory.fathersName = father.firstName + " " + father.lastName;
		backstory.mothersName = mother.firstName + " " + mother.lastName;
		backstory.birthPlace = currentLocation;
		npc.backstoryDetails = backstory;

		if (age == 0) {
			// Thinking stats will start at one, and as the years progress they will increase/decrease
			Dictionary<StatType, int> stats = new Dictionary<StatType, int>();
			stats.Add(StatType.DEXTERITY, 1);
			stats.Add(StatType.STRENGTH, 1);
			stats.Add(StatType.CONSTITUTION, 1);
			stats.Add(StatType.INTELLIGENCE, 1);
			stats.Add(StatType.WISDOM, 1);
			stats.Add(StatType.CHARISMA, 1);
		} else {
			// TODO figure out how to generate stats based on age
		}

		return npc;
	}

	// Used to return a brand new random npc
	public Character GenerateNPC(AgeRange ageRange) {
		Random rand = new Random();
		Character npc = new Character();
		npc.gender = GenerateGender(rand);
		npc.firstName = GenerateFirstName(npc.gender, rand);
		npc.lastName = lastNames[rand.Next(0, lastNames.Count)];

		npc.age = rand.Next(ageRange.minAge, ageRange.maxAge);

		int randOriginIndex = rand.Next(0, possibleOrigins.Count);
		Origin originToAdd = possibleOrigins[randOriginIndex];
		Race race = new Race();
		race.AddOrigins(originToAdd, 1);
        npc.race = race;

		List<PersonalityTrait> personalityTraits = new List<PersonalityTrait>
        {
            possiblePersonalityTraits[rand.Next(0, possiblePersonalityTraits.Count)]
        };
		npc.personalityTraits = personalityTraits;

		// TODO random backstory

		// TODO figure out how to generate stats based on age
		Dictionary<StatType, int> stats = new Dictionary<StatType, int>();
		stats.Add(StatType.STRENGTH, 1);
		stats.Add(StatType.CONSTITUTION, 1);
		stats.Add(StatType.INTELLIGENCE, 1);
		stats.Add(StatType.WISDOM, 1);
		stats.Add(StatType.CHARISMA, 1);

		int dex = 1;
		for (int i=1;i<=npc.age;i++) {
			dex += statCalculator.GenerateStatIncrease(i, StatType.DEXTERITY);
		}
		stats.Add(StatType.DEXTERITY, dex);

		npc.stats = stats;

		return npc;
	}

	private Gender GenerateGender(Random rand) {
		// For now, 50/50 between two genders
		if (rand.Next(2) == 1) {
			return Gender.MALE;
		} else {
			return Gender.FEMALE;
		}
	}

	private string GenerateFirstName(Gender gender, Random rand) {
		if (gender == Gender.MALE) {
			return maleFirstNames[rand.Next(0, maleFirstNames.Count)];
		} else {
			return femaleFirstNames[rand.Next(0, femaleFirstNames.Count)];
		}
	}
}

public class AgeRange {
	public int minAge;
	public int maxAge;

	public AgeRange(int minAge, int maxAge) {
		this.minAge = minAge;
		this.maxAge = maxAge;
	}
} 