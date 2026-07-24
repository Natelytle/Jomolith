using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public interface IShapeBuilder
{
    Mesh BuildMesh(PartModel part);
    Shape3D BuildCollisionShape(PartModel part);
}
