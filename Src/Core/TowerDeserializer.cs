using System;
using System.IO;
using System.Text.Json;
using Godot;

namespace Jomolith.Core;

public class TowerDeserializer
{
    public static ITowerModel? LoadFromFile(string fileName)
    {
        string virtualPath = "user://Towers/" + fileName;
        string physicalPath = ProjectSettings.GlobalizePath(virtualPath);

        string jsonContents;

        if (!File.Exists(physicalPath))
        {
            Console.WriteLine("Tried reading path that didn't exist: " + physicalPath);
            return null;
        }

        using (StreamReader reader = new StreamReader(physicalPath))
        {
            jsonContents = reader.ReadToEnd();
        }

        TowerDto? towerDto = JsonSerializer.Deserialize<TowerDto>(jsonContents);

        if (towerDto is null)
        {
            Console.WriteLine("Tower did not load successfully: " + physicalPath);
            return null;
        }

        return TowerModel.FromDto(towerDto);
    }
}
