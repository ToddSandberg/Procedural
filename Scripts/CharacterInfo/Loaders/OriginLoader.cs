using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class OriginLoader
{
	public void Save(Origin origin, string path) {
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		string jsonAsString = JsonSerializer.Serialize(origin);
		file.StoreString(jsonAsString);
	}

	public List<Origin> Load(string dirPath) {
        List<Origin> origins = new List<Origin>();
        using var dir = DirAccess.Open(dirPath);
        if (dir != null) {
            dir.ListDirBegin();

            string fileName = dir.GetNext();
            while (fileName != "") {
                GD.Print(dirPath + "/" + fileName);
                var file = FileAccess.Open(dirPath + "/" + fileName, FileAccess.ModeFlags.Read);
                string content = file.GetAsText();
                Origin origin = JsonSerializer.Deserialize<Origin>(content);
                origins.Add(origin);
                fileName = dir.GetNext();
            }
        }

        return origins;
	}
}