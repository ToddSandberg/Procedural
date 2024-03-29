using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public partial class PersonalityTraitLoader
{
	/*public void Save(Character character, string path) {
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		string jsonAsString = JsonSerializer.Serialize(character);
		file.StoreString(jsonAsString);
	}*/

	public List<PersonalityTrait> Load(string dirPath) {
        List<PersonalityTrait> traits = new List<PersonalityTrait>();
        using var dir = DirAccess.Open(dirPath);
        if (dir != null) {
            dir.ListDirBegin();

            string fileName = dir.GetNext();
            while (fileName != "") {
                var file = FileAccess.Open(dirPath + "/" + fileName, FileAccess.ModeFlags.Read);
                string content = file.GetAsText();
                PersonalityTrait trait = JsonSerializer.Deserialize<PersonalityTrait>(content);
                traits.Add(trait);
                fileName = dir.GetNext();
            }
        }

        return traits;
	}
}