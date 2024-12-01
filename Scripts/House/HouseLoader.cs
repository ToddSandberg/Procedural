using Godot;
using System.Text.Json;

public static class HouseLoader
{
	public static void Save(House house, string path) {
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		string jsonAsString = JsonSerializer.Serialize(house);
		file.StoreString(jsonAsString);
	}
}
