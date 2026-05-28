using Godot;
using System;

namespace Jomolith.Utils.Rendering.Meshes;

[Tool]
[GlobalClass]
public abstract partial class PartMesh : ArrayMesh {

	private bool updateRequested;
	internal int ArrayLen;
	internal int ArrayIndexLen;

	public PartMesh() {
		updateRequested = true;
		UpdateMesh();
	}

	protected abstract void GenerateMesh(MeshGenerator meshGen);

	/*
    public override int _GetSurfaceCount() => 1;

    public override uint _SurfaceGetFormat(int _index) => (uint)(
		ArrayFormat.FormatVertex |
		ArrayFormat.FormatNormal |
		ArrayFormat.FormatTangent |
		ArrayFormat.FormatTexUV |
		ArrayFormat.FormatCustom0
	);*/

	public void RequestUpdate() {
		if (updateRequested) return;
		UpdateMesh();
	}

    public void UpdateMesh() {
		MeshGenerator meshGen = new();

		ClearSurfaces();
		GenerateMesh(meshGen);
		meshGen.Commit(this);

		updateRequested = false;
	}


	#region UpdateMesh triggers
	// All of these overrides cause the mesh to automatically update if necessary
	// A technique oberserved from godot's own scene/resources/3d/primitive_meshes.cpp

    public override int _SurfaceGetArrayLen(int index) {
		if (updateRequested) UpdateMesh();
        return base._SurfaceGetArrayLen(index);
    }

	public override int _SurfaceGetArrayIndexLen(int index) {
		if (updateRequested) UpdateMesh();
        return base._SurfaceGetArrayIndexLen(index);
    }

    public override int _GetSurfaceCount() {
		if (updateRequested) UpdateMesh();
		return base._GetSurfaceCount();
	}

	public override Godot.Collections.Array _SurfaceGetArrays(int index) {
		if (updateRequested) UpdateMesh();
		return base._SurfaceGetArrays(index);
	}

    public override Aabb _GetAabb() {
		if (updateRequested) UpdateMesh();
        return base._GetAabb();
    }

    public override Rid _GetRid() {
		if (updateRequested) UpdateMesh();
        return base._GetRid();
    }

	#endregion
}
