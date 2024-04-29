using Godot;

public partial class StatCalculator
{
    Curve2D dexterityCurve = new Curve2D();
    RandomNumberGenerator rnd;

    public StatCalculator() {
        rnd = new RandomNumberGenerator();

        dexterityCurve.AddPoint(new Vector2(1,0));
        dexterityCurve.AddPoint(new Vector2(20,1));
        dexterityCurve.AddPoint(new Vector2(80,-1));
    }

    public int GenerateStatIncrease(int age, StatType statType) {
		if (statType == StatType.DEXTERITY) {
            return GetChange(dexterityCurve.SampleBaked(age).Y);
		} else {
            return 0;
        }
	}

    public int GetChange(float value) {
        if (value >= 0) {
            return (int)Mathf.Ceil(rnd.RandfRange(0, value));
        } else {
            return (int)Mathf.Floor(rnd.RandfRange(0, value));
        }
    }
}