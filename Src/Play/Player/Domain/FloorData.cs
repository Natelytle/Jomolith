using Godot;

namespace Jomolith.Play.Player.Domain;

public readonly record struct FloorData
{
    public Vector3? FloorNormal { get; init; }
    public Vector3? FloorPosition { get; init; }
    public Vector3? FloorVelocity { get; init; }

    public bool FloorFound => FloorNormal is not null;
}