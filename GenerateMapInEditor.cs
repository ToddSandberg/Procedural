using Godot;
using System;

[Tool]
public partial class GenerateMapInEditor : EditorScript
{
	// Called when the node enters the scene tree for the first time.
	public override void _Run()
	{
		MapGenerator mapGenerator = GetScene().GetNode<MapGenerator>("MapGenerator");
		mapGenerator.GenerateMap();
	}
}
