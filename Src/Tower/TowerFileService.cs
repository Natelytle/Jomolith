using System;
using System.IO;
using Godot;
using Jomolith.Tower.Core;
using Jomolith.Tower.Core.Serialization;

namespace Jomolith.Tower;

public abstract record TowerSource;

public sealed record LocalFileTowerSource(string Path) : TowerSource;

public static class TowerFileService
{
    public static ITowerModel? LoadFromSource(TowerSource source) => source switch
    {
        LocalFileTowerSource s => loadFromPath(s.Path),
        _ => throw new NotSupportedException()
    };

    private static ITowerModel? loadFromPath(string fileName)
    {
        string physicalPath = ProjectSettings.GlobalizePath("user://Towers/" + fileName);

        using StreamReader reader = new StreamReader(physicalPath);

        string json = reader.ReadToEnd();

        return TowerSerializer.FromJson(json);
    }
}
