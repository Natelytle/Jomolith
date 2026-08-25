
const int AXIS_X = 0;
const int AXIS_Y = 1;
const int AXIS_Z = 2;
const int AXIS_HYP_ZY = 3;
const int AXIS_HYP_XY = 4;

const float sqrt2 = sqrt(2);

uniform bool surfaceVariant0IsSmooth = true;
uniform float detailFadeStart = 64;
uniform float detailFadeEnd = 128;

uniform sampler2DArray texturesAlbedo : source_color, filter_linear_mipmap, repeat_enable;
uniform vec3 textureAlbedoBase : source_color = vec3(1);
uniform vec3 albedoTint : source_color = vec3(1);
uniform float opacity = 1.0f;
uniform float albedoScale = 1.0;

uniform sampler2DArray texturesNormal : hint_roughness_normal, filter_linear_mipmap, repeat_enable;
uniform float normalScale : hint_range(-16.0, 16.0) = 1;
uniform vec2 uvScale = vec2(1,1);
uniform float specular : hint_range(0.0, 1.0, 0.01);

uniform ivec3 xyzSurfaceVariantPos;
uniform ivec3 xyzSurfaceVariantNeg;

varying flat float surfaceVariant;
varying flat uint completelySimple;

vec2 getVertUVScale(vec3 objectSize, int vertexAxis) {
    vec2 vertUVScale = vec2(1);
    switch(vertexAxis) {
        case AXIS_X:
            vertUVScale = objectSize.zy;
            break;
        case AXIS_Y:
            vertUVScale = objectSize.xz;
            break;
        case AXIS_Z:
            vertUVScale = objectSize.xy;
            break;
        case AXIS_HYP_ZY:
            vertUVScale = vec2(objectSize.x, length(objectSize.zy) * sqrt2);
            break;
        case AXIS_HYP_XY:
            vertUVScale = vec2(objectSize.z, length(objectSize.zy) * sqrt2);
            break;
    }
    return vertUVScale;
}

void vertex() {
    vec4 vertexView = MODELVIEW_MATRIX * vec4(VERTEX, 1.0);
    POSITION = PROJECTION_MATRIX * vertexView;

    vec3 objectSize = vec3(
        length(MODEL_MATRIX[0]),
        length(MODEL_MATRIX[1]),
        length(MODEL_MATRIX[2])
    );

    vec3 objectPos = MODEL_MATRIX[3].xyz;

    float objectRadius = length(objectSize);
    float objectDistance = dot(objectPos - CAMERA_POSITION_WORLD, CAMERA_DIRECTION_WORLD);

    completelySimple = uint(0);
    if (objectDistance >= detailFadeEnd + objectRadius) {
        completelySimple = uint(1);
    }

    if (completelySimple != uint(1)) {
        int uvAxis = int(CUSTOM0.x);
        int surfaceId = int(CUSTOM0.y);

        int surfaceVariantI = 0;

        // If the surface variant is positive
        if (surfaceId % 2 == 0) {
            surfaceVariantI = xyzSurfaceVariantPos[surfaceId / 2];
        }
        else {
            surfaceVariantI = xyzSurfaceVariantNeg[(surfaceId - 1) / 2];
        }

        surfaceVariant = float(surfaceVariantI);

        vec2 vertUVScale = getVertUVScale(objectSize, uvAxis);
        UV *= vertUVScale * uvScale;
        UV2 *= vertUVScale * uvScale;
    }
}

// Gets the *linear* distance of the fragment away from the camera
float getFragZ(mat4 invProjMatrix, float fragCoordZ) {
    vec4 depthVec = invProjMatrix * vec4(0.0, 0.0, fragCoordZ, 1.0);
    return -(depthVec.z / depthVec.w);
}

void fragment() {
    float fragZ = getFragZ(INV_PROJECTION_MATRIX, FRAGCOORD.z);
    vec3 final_albedo = albedoTint;

    // Determine the detail fading
    // Works similarly to basic linear fog, but instead of mixing fog, it mixes the simple albedo (downscales the normal map)
    // This lets us skip texture mapping for fragments that are too far away to save on computation time
    float detailFadeT;
    if (completelySimple == uint(1)) {
        detailFadeT = 1.0;
    } else {
        detailFadeT = clamp((fragZ - detailFadeStart) / (detailFadeEnd - detailFadeStart), 0.0, 1.0);
    }

    // Texture mapping
    // Only done if detail fading isn't 100%
    if (detailFadeT < 1.0) {
        float checker = mod(floor(UV.x) + floor(UV.y), 2.0);
        final_albedo = mix(vec3(0.2), vec3(0.8), checker);

        vec3 uv3 = vec3(UV, surfaceVariant);

        // Albedo map
        vec3 albedoTex = texture(texturesAlbedo, uv3).rgb;
        albedoTex /= textureAlbedoBase;
        albedoTex *= albedoScale;
        albedoTex *= albedoTint;

        final_albedo = albedoTex;

        // Normal map
        NORMAL_MAP = texture(texturesNormal, uv3).rgb;
    }

    // Outputs
    ALBEDO = mix(final_albedo, albedoTint, detailFadeT);
	#ifdef ALPHA_ENABLED
    	ALPHA = opacity;
	#endif
    NORMAL_MAP_DEPTH = normalScale * (1.0 - detailFadeT);
    SPECULAR = specular;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
