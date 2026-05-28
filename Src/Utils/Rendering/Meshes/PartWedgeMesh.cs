using Godot;
using System;
using System.Collections.Generic;

namespace Jomolith.Utils.Rendering.Meshes;

[Tool]
[GlobalClass]
public partial class PartWedgeMesh : PartMesh {

	public enum WedgeMeshType {
		Wedge, CornerWedge
	}

	private WedgeMeshType meshType;
	[Export] public WedgeMeshType MeshType {
		get => meshType;
		set {
			meshType = value;
			RequestUpdate();
		}
	}

	protected override void GenerateMesh(MeshGenerator meshGen) {

		// Bottom
		meshGen.BuildQuad(MeshGenerator.UvAxis.YAxis, MeshGenerator.SurfaceId.Yn, Vector3.Right, Vector3.Forward, 2, 2, false, false);

		if (meshType == WedgeMeshType.Wedge) {

			// Back, Front (Sloped)
			meshGen.BuildQuad(MeshGenerator.UvAxis.ZAxis, MeshGenerator.SurfaceId.Zp, Vector3.Left, Vector3.Up, 2, 2, false, false);
			meshGen.BuildQuad(MeshGenerator.UvAxis.HypoZYAxis, MeshGenerator.SurfaceId.Zn, Vector3.Right, Vector3.Up, 2, 2, false, false);

			// Right, Left
			meshGen.BuildTriangle(MeshGenerator.UvAxis.XAxis, MeshGenerator.SurfaceId.Xp, Vector3.Back, Vector3.Up, false);
			meshGen.BuildTriangle(MeshGenerator.UvAxis.XAxis, MeshGenerator.SurfaceId.Xn, Vector3.Forward, Vector3.Up, true);

		} else {

			// Back (Sloped), Front (Sloped)
			meshGen.BuildTriangle(MeshGenerator.UvAxis.HypoZYAxis, MeshGenerator.SurfaceId.Zp, Vector3.Left, Vector3.Up, true);
			meshGen.BuildTriangle(MeshGenerator.UvAxis.ZAxis, MeshGenerator.SurfaceId.Zn, Vector3.Right, Vector3.Up, false);

			// Right, Left (Sloped)
			meshGen.BuildTriangle(MeshGenerator.UvAxis.XAxis, MeshGenerator.SurfaceId.Xp, Vector3.Back, Vector3.Up, true);
			meshGen.BuildTriangle(MeshGenerator.UvAxis.HypoXYAxis, MeshGenerator.SurfaceId.Xn, Vector3.Forward, Vector3.Up, false);

		}
	}
}
