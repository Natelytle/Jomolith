using System.Collections.Generic;
using Godot;
using Godot.Collections;
using Jomolith.Towers.Domain.Enums;

namespace Jomolith.Towers.Factory.Shapes;

public static class SurfaceGenerator
{
    /// <summary>
    /// Adds a surface made of quads to the SurfaceTool, supporting curves.
    /// </summary>
    /// <param name="st">The SurfaceTool to add the surface to.</param>
    /// /// <param name="indices">The index buffer.</param>
    /// <param name="latestVertIndex">The latest vert index, ref variable so that it can update in the method.</param>
    /// <param name="uvAxis">Vertex shader data that tells the shader which UV Axis (i.e. physical side, including slopes) this vertex belongs to.</param>
    /// <param name="surfaceId">Vertex shader data that tells the shader which actual surface ID (always xp, xn, etc) this side belongs to (for textures).</param>
    /// <param name="xAxis">The direction the X axis of this surface should point.</param>
    /// <param name="yAxis">The direction the Y axis of this surface should point.</param>
    /// <param name="segmentsX">How many quad segments the X axis comprises.</param>
    /// <param name="segmentsY">How many quad segments the Y axis comprises.</param>
    /// <param name="curveX">Whether the X axis should curve (e.g. a cylinder).</param>
    /// <param name="curveY">Whether the Y axis should curve (when paired with <see cref="curveX"/>, makes a ball).</param>
    public static void BuildQuad(
        SurfaceTool st,
        ref int latestVertIndex,
        UvAxis uvAxis,
        SurfaceId surfaceId,
        Vector3 xAxis, Vector3 yAxis,
        int segmentsX, int segmentsY,
        bool curveX, bool curveY
    )
    {
        Vector3 zAxis = xAxis.Cross(yAxis);

        // Gotta figure out this math
        // I think this is related to where the side position is relative to the origin? For curving reasons
        Vector3 origin = -(xAxis + yAxis + zAxis) * 0.5f;

        // Construct and add the vertices
        for (int x = 0; x < segmentsX; x++)
        {
            float tX = (float)x / (segmentsX - 1);

            for (int y = 0; y < segmentsY; y++)
            {
                float tY = (float)y / (segmentsY - 1);

                Vector2 uv = new Vector2(tX, 1f - tY);
                Vector3 pos = origin + xAxis * tX + yAxis * tY;
                Vector3 uncurvedNormal = -zAxis; // The normal vector before curving

                if (uvAxisIsSlope(uvAxis))
                {
                    uncurvedNormal = (yAxis - zAxis).Normalized();
                    pos += zAxis * tY;
                }

                Vector3 normal = uncurvedNormal;
                Vector3 tangent;

                if (curveX && curveY)
                {
                    // To curve along both x and y axes, all we need to do is normalize position.
                    pos = pos.Normalized();
                    normal = pos;
                    pos *= 0.5f;
                }
                else if (curveX)
                {
                    // To curve along a single axis, we simply do whatever we were doing in 3D, but in 2D
                    // This weird dot product magic generates a 2d plane that
                    float localX = pos.Dot(xAxis);
                    float localZ = pos.Dot(zAxis);
                    Vector2 normalized2D = new Vector2(localX, localZ).Normalized() * 0.5f; // This is the plane to normalize against

                    pos = xAxis * normalized2D.X
                        + yAxis * tY
                        - yAxis * 0.5f
                        + zAxis * normalized2D.Y;

                    normal = xAxis * normalized2D.X
                           + zAxis * normalized2D.Y;
                }
                else if (curveY)
                {
                    float localY = pos.Dot(yAxis);
                    float localZ = pos.Dot(zAxis);
                    Vector2 normalized2D = new Vector2(localY, localZ).Normalized() * 0.5f;

                    pos = xAxis * tX
                        - xAxis * 0.5f
                        + yAxis * normalized2D.X
                        + zAxis * normalized2D.Y;

                    normal = yAxis * normalized2D.X
                           + zAxis * normalized2D.Y;
                }

                tangent = rotateByAxisDifference(-xAxis, uncurvedNormal, normal);

                st.SetNormal(normal);
                st.SetTangent(new Plane(tangent, 1));
                st.SetUV(uv);
                st.SetCustom(0, new Color((float)uvAxis, (float)surfaceId, 0f, 0f));
                st.AddVertex(pos);
            }
        }

        int indexOffset(int x, int y) => x * segmentsY + y;

        for (int x = 0; x < segmentsX - 1; x++)
        {
            for (int y = 0; y < segmentsY - 1; y++)
            {
                // Triangle 1
                st.AddIndex(latestVertIndex + indexOffset(x + 1, y + 0));
                st.AddIndex(latestVertIndex + indexOffset(x + 0, y + 1));
                st.AddIndex(latestVertIndex + indexOffset(x + 0, y + 0));

                // Triangle 2
                st.AddIndex(latestVertIndex + indexOffset(x + 0, y + 1));
                st.AddIndex(latestVertIndex + indexOffset(x + 1, y + 0));
                st.AddIndex(latestVertIndex + indexOffset(x + 1, y + 1));
            }
        }

        latestVertIndex += segmentsX * segmentsY;
    }

    public static void BuildCircle(
        SurfaceTool st,
        ref int latestVertIndex,
        UvAxis uvAxis,
        SurfaceId surfaceId,
        Vector3 xAxis, Vector3 zAxis,
        int segments
    )
    {
        Vector3 yAxis = xAxis.Cross(zAxis);

        st.SetNormal(yAxis);
        st.SetTangent(new Plane(-xAxis, 1f));
        st.SetUV(Vector2.One * 0.5f);
        st.SetCustom(0, new Color((float)uvAxis, (float)surfaceId, 0, 0));
        st.AddVertex(yAxis * 0.5f);

        void pushVertex(float localX, float localZ)
        {
            Vector2 uv = (new Vector2(localX, localZ) + Vector2.One) * 0.5f;

            // No need to set other information since it's all the same.
            st.SetUV(uv);
            st.AddVertex((xAxis * localX + yAxis + zAxis * localZ) * 0.5f);
        }

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);
            Vector2 localPos = new Vector2(t * 2f - 1f, 1f).Normalized();

            pushVertex(localPos.X, localPos.Y);
            pushVertex(localPos.Y, -localPos.X);
            pushVertex(-localPos.X, -localPos.Y);
            pushVertex(-localPos.Y, localPos.X);
        }

        for (int i = 0; i < segments - 1; i++)
        {
            st.AddIndex(latestVertIndex);
            st.AddIndex(latestVertIndex + (i + 0) * 4 + 1);
            st.AddIndex(latestVertIndex + (i + 1) * 4 + 1);

            st.AddIndex(latestVertIndex);
            st.AddIndex(latestVertIndex + (i + 0) * 4 + 2);
            st.AddIndex(latestVertIndex + (i + 1) * 4 + 2);

            st.AddIndex(latestVertIndex);
            st.AddIndex(latestVertIndex + (i + 0) * 4 + 3);
            st.AddIndex(latestVertIndex + (i + 1) * 4 + 3);

            st.AddIndex(latestVertIndex);
            st.AddIndex(latestVertIndex + (i + 0) * 4 + 4);
            st.AddIndex(latestVertIndex + (i + 1) * 4 + 4);
        }

        latestVertIndex += segments * 4 + 1;
    }

    public static void BuildTriangle(
        SurfaceTool st,
        ref int latestVertIndex,
        UvAxis uvAxis,
        SurfaceId surfaceId,
        Vector3 xAxis,
        Vector3 yAxis,
        bool leftHanded
    )
    {
        Vector3 zAxis = xAxis.Cross(yAxis);
        Vector3 origin = -(xAxis + yAxis + zAxis) * 0.5f;

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                if (x == 1 && y == 1) continue;

                float tX = x;
                float tY = y;

                if (y == 1 && !leftHanded)
                    tX += 1f;

                Vector2 uv = new(1f - tX, tY);
                Vector3 pos = origin + (xAxis * tX) + (yAxis * tY);
                Vector3 normal = -zAxis;

                if (uvAxisIsSlope(uvAxis))
                {
                    normal = (yAxis - zAxis).Normalized();
                    pos += zAxis * tY;
                }

                st.SetNormal(normal);
                st.SetTangent(new Plane(xAxis, 1f));
                st.SetUV(uv);
                st.SetCustom(0, new Color((float)uvAxis, (float)surfaceId, 0, 0));
                st.AddVertex(pos);
            }
        }

        st.AddIndex(latestVertIndex + 2);
        st.AddIndex(latestVertIndex + 1);
        st.AddIndex(latestVertIndex);

        latestVertIndex += 3;
    }

    private static bool uvAxisIsSlope(UvAxis axis) => (int)axis > 2;

    private static Vector3 rotateByAxisDifference(Vector3 target, Vector3 from, Vector3 to) {
        Vector3 cross = from.Cross(to);
        // If the cross product is about zero, that means there is no rotation difference
        // Simply return the target vector, unrotated
        if (cross.IsZeroApprox()) return target;

        Vector3 rotAxis = cross.Normalized();

        float dot = from.Dot(to);
        float angle = float.Acos(float.Clamp(dot, -1f, 1f));

        // Turn the axis-angle pair into a basis which we can then use to actually rotate the vector
        Basis rotation = new(rotAxis, angle);
        return target * rotation;
    }
}
