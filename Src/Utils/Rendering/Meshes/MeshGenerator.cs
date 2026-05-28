using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Jomolith.Utils.Rendering.Meshes;

public sealed class MeshGenerator {
	public enum UvAxis : int {
		XAxis = 0,
		YAxis = 1,
		ZAxis = 2,
		HypoZYAxis = 3,
		HypoXYAxis = 4,
	}

	private static bool uvAxisIsSlope(UvAxis axis)
		=> (int)axis > 2;

	public enum SurfaceId : int {
#pragma warning disable CA1069 // Disable "enum has same constant value" warning because it's intentional
		Xp = 0,
		Xn = 1,
		Yp = 2,
		Yn = 3,
		Zp = 4,
		Zn = 5,

		Right = 0,
		Left = 1,
		Top = 2,
		Bottom = 3,
		Back = 4,
		Front = 5
#pragma warning restore CA1069
	}

	public enum SlopeMode {
		Full,
		Left,
		Right
	}

	private bool indicesAdded = false;
	private int latestVertIndex = 0;
	private readonly List<int> indices = new List<int>();
	private readonly SurfaceTool st = new SurfaceTool();

	public MeshGenerator() {
		st.Begin(Mesh.PrimitiveType.Triangles);
		st.SetCustomFormat(0, SurfaceTool.CustomFormat.RgbaFloat);
		st.SetColor(new Color(1f, 1f, 1f, 1f));
	}

	private static Vector3 rotateByAxisDifference(Vector3 target, Vector3 from, Vector3 to) {
		Vector3 cross = from.Cross(to);
		// If the cross product is about zero, that means there is no rotation difference
		// Simply return the target vector, unrotated
		if (cross.IsZeroApprox()) return target;

		Vector3 rotAxis = cross.Normalized();

		float dot = from.Dot(to);
		float angle = MathF.Acos(Math.Clamp(dot, -1f, 1f));

		// Turn the axis-angle pair into a basis which we can then use to actually rotate the vector
		Basis rotation = new(rotAxis, angle);
		return target * rotation;
	}

	private static Color packCustom0(UvAxis uvAxis, SurfaceId surfaceId) {
		int packedI = (int)uvAxis | ((int)surfaceId << 4);
		float r = Unsafe.As<int, float>(ref packedI);

		return new Color(r, 0f, 0f, 0f);
	}

	public void BuildQuad(
		UvAxis uvAxis,
		SurfaceId surfaceId,

		Vector3 xAxis,
		Vector3 yAxis,

		int segmentsX,
		int segmentsY,
		bool curveX,
		bool curveY
	) {

		// Cool maths
		Vector3 zAxis = xAxis.Cross(yAxis);
		Vector3 origin = (-xAxis - yAxis - zAxis) * 0.5f;

		// Construct and add the vertices
		for (int x = 0; x < segmentsX; x++) {
			float tX = (float)x / (float)(segmentsX - 1);

			for (int y = 0; y < segmentsY; y++) {
				float tY = (float)y / (float)(segmentsY - 1);

				Vector2 uv = new(tX, 1f - tY);
				Vector3 pos = origin + (xAxis * tX) + (yAxis * tY);
				Vector3 uncurvedNormal = -zAxis; // The normal vector before curving

				if (uvAxisIsSlope(uvAxis)) {
					uncurvedNormal = (yAxis - zAxis).Normalized();
					pos += zAxis * tY;
				}
				Vector3 normal = uncurvedNormal;
				Vector3 tangent;

				if (curveX && curveY) {
					// To curve along both X and Y axes, all we gotta do is normalize the position
					pos = pos.Normalized();
					normal = pos;
					pos *= 0.5f;

				} else if (curveX) {
					// To curve along a single axis, we simply do whatever we were doing in 3D, but in 2D!!!
					// But we have to make use of YUCKY linear algebra to make this work in world-space

					float localX = pos.Dot(xAxis);
					float localZ = pos.Dot(zAxis);
					Vector2 normalized2D = new Vector2(localX, localZ).Normalized(); // Normalize along this plane

					pos = (xAxis * normalized2D.X) + (yAxis * tY) - (yAxis * 0.5f) + (zAxis * normalized2D.Y);
					normal = (xAxis * normalized2D.X) + (zAxis * normalized2D.Y);

				} else if (curveY) { // FIXME
					// Same jazz as curveX

					float localY = pos.Dot(yAxis);
					float localZ = pos.Dot(zAxis);
					Vector2 normalized2D = new Vector2(localY, localZ).Normalized() * 0.5f;

					pos = (xAxis * tX) - (xAxis * 0.5f) + (yAxis * normalized2D.X) + (zAxis * normalized2D.Y);
					normal = (yAxis * normalized2D.X) + (zAxis * normalized2D.Y);
				}

				// If the normal rotated, the tangent should be rotated
				tangent = rotateByAxisDifference(-xAxis, uncurvedNormal, normal);

				// From here the bitangent can be easily inferred, it's just the cross product of the normal and tangent
				//Vector3 bitangent = normal.Cross(tangent);

				// Pack custom data into a color struct
				Color packedCustom0 = packCustom0(uvAxis, surfaceId);

				st.SetNormal(normal);
				st.SetTangent(new Plane(tangent, 1f));
				st.SetUV(uv);
				st.SetCustom(0, packedCustom0);
				st.AddVertex(pos);
			}
		}

		// Add the indices to form the quads
		// For every vertex, with the exception of vertices at the end of the xAxis and yAxis..
		//	- Get the indices of this vertex and all 3 neighboring vertices
		//	- Push them into the SurfaceTool
		for (int x = 0; x < segmentsX - 1; x++) {
			for (int y = 0; y < segmentsY - 1; y++) {
				// Tri 1
				pushVertIndex(x + 1, y + 0);
				pushVertIndex(x + 0, y + 1);
				pushVertIndex(x + 0, y + 0);

				// Tri 2
				pushVertIndex(x + 0, y + 1);
				pushVertIndex(x + 1, y + 0);
				pushVertIndex(x + 1, y + 1);
			}
		}

		latestVertIndex += segmentsX * segmentsY;
		return;

		void pushVertIndex(int x, int y) => indices.Add(latestVertIndex + (x * segmentsY) + y);
	}

	public void BuildTriangle(
		UvAxis uvAxis,
		SurfaceId surfaceId,

		Vector3 xAxis,
		Vector3 yAxis,

		bool leftHanded
	) {
		// Cool maths
		Vector3 zAxis = xAxis.Cross(yAxis);
		Vector3 origin = (-xAxis - yAxis - zAxis) * 0.5f;

		for (int x = 0; x < 2; x++) {
			for (int y = 0; y < 2; y++) {

				if (y == 1 && x == 1) continue;

				float tX = x;
				float tY = y;

				if (y == 1 && !leftHanded)
					tX += 1f;

				Vector2 uv = new(1f - tX, tY);
				Vector3 pos = origin + (xAxis * tX) + (yAxis * tY);
				Vector3 normal = -zAxis;
				if (uvAxisIsSlope(uvAxis)) {
					normal = (yAxis - zAxis).Normalized();
					pos += zAxis * tY;
				}

				// Pack custom data into a color struct
				Color packedCustom0 = packCustom0(uvAxis, surfaceId);

				st.SetNormal(normal);
				st.SetTangent(new Plane(xAxis, 1f));
				st.SetUV(uv);
				st.SetCustom(0, packedCustom0);
				st.AddVertex(pos);
			}
		}

		indices.Add(latestVertIndex + 2);
		indices.Add(latestVertIndex + 1);
		indices.Add(latestVertIndex + 0);

		latestVertIndex += 3;
	}

	public void BuildCircle(
		UvAxis uvAxis,
		SurfaceId surfaceId,

		Vector3 xAxis,
		Vector3 zAxis,

		int segments
	) {
		Vector3 yAxis = xAxis.Cross(zAxis);

		// Pack custom data into a color struct
		Color packedCustom0 = packCustom0(uvAxis, surfaceId);

		// Create the center vertex
		st.SetNormal(yAxis);
		st.SetTangent(new Plane(-xAxis, 1f));
		st.SetUV(Vector2.One * 0.5f);
		st.SetCustom(0, packedCustom0);
		st.AddVertex(yAxis * 0.5f);

		for (int i = 0; i < segments; i++) {
			float t = (float)i / (float)(segments - 1);
			Vector2 localPos = new Vector2(t*2f - 1f, 1f).Normalized();

			pushVertex(localPos.X, localPos.Y);
			pushVertex(localPos.Y, -localPos.X);
			pushVertex(-localPos.X, -localPos.Y);
			pushVertex(-localPos.Y, localPos.X);
		}

		for (int i = 0; i < segments - 1; i++) {
			pushTriIndices(i);
		}

		latestVertIndex += segments * 4 + 1;
		return;

		void pushTriIndices(int i) {
			indices.Add(latestVertIndex);
			indices.Add(latestVertIndex + ((i + 0) * 4) + 1);
			indices.Add(latestVertIndex + ((i + 1) * 4) + 1);

			indices.Add(latestVertIndex);
			indices.Add(latestVertIndex + ((i + 0) * 4) + 2);
			indices.Add(latestVertIndex + ((i + 1) * 4) + 2);

			indices.Add(latestVertIndex);
			indices.Add(latestVertIndex + ((i + 0) * 4) + 3);
			indices.Add(latestVertIndex + ((i + 1) * 4) + 3);

			indices.Add(latestVertIndex);
			indices.Add(latestVertIndex + ((i + 0) * 4) + 4);
			indices.Add(latestVertIndex + ((i + 1) * 4) + 4);
		}

		void pushVertex(float localX, float localZ) {
			Vector2 uv = (new Vector2(localX, localZ) + Vector2.One) * 0.5f;

			// No need to set normal and tangent information since it's all the same
			st.SetUV(uv);
			st.AddVertex((xAxis * localX + yAxis + zAxis * localZ) * 0.5f);
		}
	}

	public void Commit(PartMesh mesh) {
		if (!indicesAdded) {
			foreach (int index in indices) st.AddIndex(index);
			indicesAdded = true;
		}
		mesh.ArrayLen = latestVertIndex - 1;
		mesh.ArrayIndexLen = indices.Count;
		//st.OptimizeIndicesForCache();

		st.Commit(mesh);
		//return st.CommitToArrays();
	}
}
