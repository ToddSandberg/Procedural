using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;

public partial class EmpireGenerator : Node
{
	public override void _Ready()
	{
		// TODO generate one 
        int startYear = 0;
        int endYear = 100;
        for (int i = startYear;i < endYear;i++) {
            // TODO For each npc, age up
        }
	}
}