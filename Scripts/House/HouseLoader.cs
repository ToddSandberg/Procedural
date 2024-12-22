using Godot;
using System.Collections.Generic;
using System.Text.Json;

public static class HouseLoader
{
	public static void Save(House house, string path) {
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		string jsonAsString = JsonSerializer.Serialize(house);
		file.StoreString(jsonAsString);
	}

	public static List<House> Load() {
		using var dir = DirAccess.Open("res://Data/Houses");
		if (dir != null) {
			List<House> houses = new List<House>();
			dir.ListDirBegin();
			string fileName = dir.GetNext();
			while (fileName != "") {
				if (dir.CurrentIsDir()) {
					GD.Print($"Found directory: {fileName}");
				} else {
					GD.Print($"Found file: {fileName}");
					var file = FileAccess.Open("res://Data/Houses/"+fileName, FileAccess.ModeFlags.Read);
					string content = file.GetAsText();
					GD.Print(content);
					House house = JsonSerializer.Deserialize<House>(content);
					houses.Add(house);
				}

				fileName = dir.GetNext();
			}
		
			GD.Print("Houses loaded: " + houses.Count);
			return houses;
		} else {
			GD.Print("An error occurred when trying to access the path.");
			return new List<House>();
		}
	}
}
