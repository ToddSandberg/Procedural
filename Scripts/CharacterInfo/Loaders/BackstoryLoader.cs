using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class BackstoryLoader
{
	/*public void Save(Character character, string path) {
		var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		string jsonAsString = JsonSerializer.Serialize(character);
		file.StoreString(jsonAsString);
	}*/

	public List<string> Load(string dirPath) {
        List<string> names = new List<string>();
        using var dir = DirAccess.Open(dirPath);
        if (dir != null) {
            dir.ListDirBegin();

            string fileName = dir.GetNext();
            while (fileName != "") {
                GD.Print(dirPath + "/" + fileName);
                var file = FileAccess.Open(dirPath + "/" + fileName, FileAccess.ModeFlags.Read);
                string content = file.GetAsText();
                GD.Print(content);
                names.AddRange(content.Replace(" ", "").Split(",").ToArray<string>());
                fileName = dir.GetNext();
            }
        }

        return names;
	}
}