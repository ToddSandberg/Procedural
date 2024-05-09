using Godot;

public partial class StatCalculator
{
    Curve2D dexterityCurve = new Curve2D();
    Curve2D strengthCurve = new Curve2D();
    Curve2D consitutionCurve = new Curve2D();
    Curve2D intelligenceCurve = new Curve2D();
    Curve2D wisdomCurve = new Curve2D();
    Curve2D charismaCurve = new Curve2D();
    RandomNumberGenerator rnd;

    public StatCalculator() {
        rnd = new RandomNumberGenerator();

        dexterityCurve.AddPoint(new Vector2(1,1));
        dexterityCurve.AddPoint(new Vector2(20,2));
        dexterityCurve.AddPoint(new Vector2(30,-1));
        dexterityCurve.AddPoint(new Vector2(80,-2));
        strengthCurve.AddPoint(new Vector2(1,0));
        strengthCurve.AddPoint(new Vector2(20,1));
        strengthCurve.AddPoint(new Vector2(80,-1));
        consitutionCurve.AddPoint(new Vector2(1,0));
        consitutionCurve.AddPoint(new Vector2(20,1));
        consitutionCurve.AddPoint(new Vector2(80,-1));
        intelligenceCurve.AddPoint(new Vector2(1,0));
        intelligenceCurve.AddPoint(new Vector2(30,1));
        intelligenceCurve.AddPoint(new Vector2(80,0));
        wisdomCurve.AddPoint(new Vector2(1,0));
        wisdomCurve.AddPoint(new Vector2(20,0));
        wisdomCurve.AddPoint(new Vector2(80,1));
        charismaCurve.AddPoint(new Vector2(1,0));
        charismaCurve.AddPoint(new Vector2(30,1));
        charismaCurve.AddPoint(new Vector2(80,0));
    }

    public int GenerateStatIncrease(int age, StatType statType) {
            if (statType == StatType.DEXTERITY) {
                  return GetChange(dexterityCurve.SampleBaked(age).Y);
            } if (statType == StatType.STRENGTH) {
                  return GetChange(strengthCurve.SampleBaked(age).Y);
            } if (statType == StatType.CONSTITUTION) {
                  return GetChange(consitutionCurve.SampleBaked(age).Y);
            } if (statType == StatType.INTELLIGENCE) {
                  return GetChange(intelligenceCurve.SampleBaked(age).Y);
            } if (statType == StatType.WISDOM) {
                  return GetChange(wisdomCurve.SampleBaked(age).Y);
            } if (statType == StatType.CHARISMA) {
                  return GetChange(charismaCurve.SampleBaked(age).Y);
            } else {
                  return 0;
            }
	}

    public int GetChange(float value) {
        if (value >= 0) {
            return (int)Mathf.Round(rnd.RandfRange(0, value));
        } else {
            return (int)Mathf.Round(rnd.RandfRange(0, value));
        }
    }
}