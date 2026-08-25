using System.Collections.Generic;
using Godot;
using Jomolith.Towers.Domain.Enums;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public abstract class ShapeBuilder
{
    protected const int ROUND_SEGMENTS = 7;

    public abstract Mesh BuildMesh(PartModel part);
    public abstract Vector3 MeshScale(PartModel part);
    public abstract Shape3D BuildCollisionShape(PartModel part);
    public abstract float GetVolume(PartModel part);

    protected ArrayMesh BuildBlockMesh(int segmentsX, int segmentsY, bool curveX, bool curveY, bool isCylinder)
    {
        SurfaceTool st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        st.SetCustomFormat(0, SurfaceTool.CustomFormat.RgbaFloat);
        st.SetColor(new Color(1f, 1f, 1f));

        int latestVertIndex = 0;

        // Left and right faces
        if (isCylinder)
        {
            SurfaceGenerator.BuildCircle(st, ref latestVertIndex, UvAxis.XAxis, SurfaceId.Xp, Vector3.Back, Vector3.Up, segmentsY);
            SurfaceGenerator.BuildCircle(st, ref latestVertIndex, UvAxis.XAxis, SurfaceId.Xn, Vector3.Forward, Vector3.Up, segmentsY);
        }
        else
        {
            SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.XAxis, SurfaceId.Xp, Vector3.Back, Vector3.Up, segmentsX, segmentsY, curveX, curveY);
            SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.XAxis, SurfaceId.Xn, Vector3.Forward, Vector3.Up, segmentsX, segmentsY, curveX, curveY);
        }

        // Top and bottom faces
        SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.YAxis, SurfaceId.Yp, Vector3.Right, Vector3.Back, segmentsX, segmentsY, curveX, curveY);
        SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.YAxis, SurfaceId.Yn, Vector3.Right, Vector3.Forward, segmentsX, segmentsY, curveX, curveY);

        // Back and front faces
        SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.ZAxis, SurfaceId.Zp, Vector3.Left, Vector3.Up, segmentsX, segmentsY, curveX, curveY);
        SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.ZAxis, SurfaceId.Zn, Vector3.Right, Vector3.Up, segmentsX, segmentsY, curveX, curveY);

        return st.Commit();
    }

    protected ArrayMesh BuildWedgeMesh(bool corner)
    {
        SurfaceTool st = new SurfaceTool();
        st.Begin(Mesh.PrimitiveType.Triangles);
        st.SetCustomFormat(0, SurfaceTool.CustomFormat.RgbaFloat);
        st.SetColor(new Color(1f, 1f, 1f));

        int latestVertIndex = 0;

        // Bottom
        SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.YAxis, SurfaceId.Yn, Vector3.Right, Vector3.Forward, 2, 2, false, false);

        if (corner)
        {
            // Back (sloped), Front
            SurfaceGenerator.BuildTriangle(st, ref latestVertIndex, UvAxis.HypoZyAxis, SurfaceId.Zp, Vector3.Left, Vector3.Up, true);
            SurfaceGenerator.BuildTriangle(st, ref latestVertIndex, UvAxis.ZAxis, SurfaceId.Zn, Vector3.Right, Vector3.Up, false);

            // Right, Left (sloped)
            SurfaceGenerator.BuildTriangle(st, ref latestVertIndex, UvAxis.XAxis, SurfaceId.Xp, Vector3.Back, Vector3.Up, true);
            SurfaceGenerator.BuildTriangle(st, ref latestVertIndex, UvAxis.HypoXyAxis, SurfaceId.Xn, Vector3.Forward, Vector3.Up, false);
        }
        else
        {
            // Back, Front (sloped)
            SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.ZAxis, SurfaceId.Zp, Vector3.Left, Vector3.Up, 2, 2, false, false);
            SurfaceGenerator.BuildQuad(st, ref latestVertIndex, UvAxis.HypoZyAxis, SurfaceId.Zn, Vector3.Right, Vector3.Up, 2, 2, false, false);

            // Right, Left
            SurfaceGenerator.BuildTriangle(st, ref latestVertIndex, UvAxis.XAxis, SurfaceId.Xp, Vector3.Back, Vector3.Up, false);
            SurfaceGenerator.BuildTriangle(st, ref latestVertIndex, UvAxis.XAxis, SurfaceId.Xn, Vector3.Forward, Vector3.Up, true);
        }

        return st.Commit();
    }
}
