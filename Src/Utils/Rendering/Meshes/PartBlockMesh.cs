using Godot;

namespace Jomolith.Utils.Rendering.Meshes;

[Tool]
[GlobalClass]
public partial class PartBlockMesh : PartMesh {

	public enum BlockMeshType {
		Block, Cylinder, Sphere
	}

	private BlockMeshType meshType;
	[Export] public BlockMeshType MeshType {
		get => meshType;
		set {
			meshType = value;
			RequestUpdate();
		}
	}

	private int roundSegments = 7;
	[Export] public int RoundSegments {
		get => roundSegments;
		set {
			roundSegments = value;
			RequestUpdate();
		}
	}

	private bool IsCylindrical => meshType != BlockMeshType.Block;
	private bool IsSphere => meshType == BlockMeshType.Sphere;
	private int SegsY => IsCylindrical ? roundSegments : 2;
	private int SegsX => IsSphere ? roundSegments : 2;

	protected override void GenerateMesh(MeshGenerator meshGen) {

		// Top, Bottom
		meshGen.BuildQuad(MeshGenerator.UvAxis.YAxis, MeshGenerator.SurfaceId.Yp, Vector3.Right, Vector3.Back, SegsX, SegsY, IsSphere, IsCylindrical);
		meshGen.BuildQuad(MeshGenerator.UvAxis.YAxis, MeshGenerator.SurfaceId.Yn, Vector3.Right, Vector3.Forward, SegsX, SegsY, IsSphere, IsCylindrical);

		// Back, Front
		meshGen.BuildQuad(MeshGenerator.UvAxis.ZAxis, MeshGenerator.SurfaceId.Zp, Vector3.Left, Vector3.Up, SegsX, SegsY, IsSphere, IsCylindrical);
		meshGen.BuildQuad(MeshGenerator.UvAxis.ZAxis, MeshGenerator.SurfaceId.Zn, Vector3.Right, Vector3.Up, SegsX, SegsY, IsSphere, IsCylindrical);

		// Right, Left
		if (meshType == BlockMeshType.Cylinder) {
			meshGen.BuildCircle(MeshGenerator.UvAxis.XAxis, MeshGenerator.SurfaceId.Xp, Vector3.Back, Vector3.Up, RoundSegments);
			meshGen.BuildCircle(MeshGenerator.UvAxis.XAxis, MeshGenerator.SurfaceId.Xn, Vector3.Forward, Vector3.Up, RoundSegments);
		} else {
			meshGen.BuildQuad(MeshGenerator.UvAxis.XAxis, MeshGenerator.SurfaceId.Xp, Vector3.Back, Vector3.Up, SegsX, SegsY, IsSphere, IsCylindrical);
			meshGen.BuildQuad(MeshGenerator.UvAxis.XAxis, MeshGenerator.SurfaceId.Xn, Vector3.Forward, Vector3.Up, SegsX, SegsY, IsSphere, IsCylindrical);
		}
	}
}
