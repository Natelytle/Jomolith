
const int AXIS_X = 0;
const int AXIS_Y = 1;
const int AXIS_Z = 2;
const int AXIS_HYP_ZY = 3;
const int AXIS_HYP_XY = 4;

const int SURFACE_XP = 0;
const int SURFACE_XN = 1;
const int SURFACE_YP = 2;
const int SURFACE_YN = 3;
const int SURFACE_ZP = 4;
const int SURFACE_ZN = 5;

const float hypoMagic = sqrt(2);

uniform bool surfaceVariant0IsSmooth = true;
uniform float detailFadeStart = 64;
uniform float detailFadeEnd = 128;

uniform sampler2DArray texturesAlbedo : source_color, filter_linear_mipmap, repeat_enable;
uniform vec3 textureAlbedoBase : source_color = vec3(1);
uniform vec3 simpleAlbedo : source_color = vec3(1);
uniform float albedoScale = 1.0;

uniform sampler2DArray texturesNormal : hint_roughness_normal, filter_linear_mipmap, repeat_enable;
uniform float normalScale : hint_range(-16.0, 16.0) = 1;
uniform vec2 uvScale = vec2(1,1);
uniform float specular : hint_range(0.0, 1.0, 0.01);

varying flat float surfaceVariant;
varying flat uint completelySimple;

float hypotenuseScale(vec2 ao) {
	return length(ao) * hypoMagic;
}

ivec2 unpackSurfaceData(float packed) {
	int packedI = floatBitsToInt(packed);
	int uvAxis = packedI & 0x0F;
	int surfaceID = (packedI >> 4) & 0x0F;

	return ivec2(uvAxis, surfaceID);
}

int unpackSurfaceVariant(float packed, int surfaceID) {
	int packedI = floatBitsToInt(packed);
	return (packedI >> (surfaceID * 5)) & 0x1F;
}

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
			vertUVScale = vec2(objectSize.x, hypotenuseScale(objectSize.zy));
			break;
		case AXIS_HYP_XY:
			vertUVScale = vec2(objectSize.z, hypotenuseScale(objectSize.xy));
			break;
	}
	return vertUVScale;
}

// sRGB color conversion stolen from StandardMaterial3D lol
vec3 sRGBConvert(vec3 color) {
	return mix(
				pow((color + vec3(0.055)) * (1.0 / (1.0 + 0.055)), vec3(2.4)),
				color * (1.0 / 12.92),
				lessThan(color, vec3(0.04045)));
}

void vertex() {
	// Vertex processing
	vec4 vertexView = MODELVIEW_MATRIX * vec4(VERTEX, 1.0);
	POSITION = PROJECTION_MATRIX * vertexView;

	// Convert from sRGB to linear color space if necessary
	if (!OUTPUT_IS_SRGB) {
		COLOR.rgb = sRGBConvert(COLOR.rgb);
	}

	// Determine the size of this instance
	vec3 objectSize = vec3(
		length(MODEL_MATRIX[0]),
		length(MODEL_MATRIX[1]),
		length(MODEL_MATRIX[2])
	);
	vec3 objectPos = MODEL_MATRIX[3].xyz; // Can't use NODE_POSITION_WORLD because it won't work for multimeshes
	float objectRadius = length(objectSize);
	float objectDistance = -dot(objectPos - CAMERA_POSITION_WORLD, CAMERA_DIRECTION_WORLD);

	// Determine if this object should be "completely simple"
	// In other words, if it's far away enough from the camera, it shouldn't render the part with extra details
	// This will override detail fading
	completelySimple = uint(0);
	if (objectDistance >= detailFadeEnd + objectRadius) {
		completelySimple = uint(1);
	}

	// Cool texture mapping stuff
	// This will be skipped if the object is completely simple to save computation time
	//	(which should have no bearing on appearance since completely-simple vertices don't texutre-map)
	if (completelySimple != uint(1)) {
		// Determine the axis this vertex belongs to
		ivec2 surfaceData = unpackSurfaceData(CUSTOM0.x);
		int uvAxis = surfaceData.x;
		int surfaceID = surfaceData.y;

		// Get the surface variation of this vertex
		int surfaceVariantI = unpackSurfaceVariant(INSTANCE_CUSTOM.x, surfaceID);
		surfaceVariant = float(surfaceVariantI);

		// If the surface variation = 0, and is configured to be smooth, we can mark this vertex completely simple
		if (surfaceVariantI == 0 && surfaceVariant0IsSmooth) {
			completelySimple = uint(1);
		}

		// Scale vertex UVs based on the size of the axis it belongs to
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
	vec3 final_albedo = vec3(1.0);

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
		vec3 uv3 = vec3(UV, surfaceVariant);

		// Albedo map
		vec3 albedoTex = texture(texturesAlbedo, uv3).rgb;
		albedoTex /= textureAlbedoBase;
		albedoTex *= albedoScale;
		albedoTex *= COLOR.rgb; // Apply vertex color

		final_albedo = albedoTex;

		// Normal map
		NORMAL_MAP = texture(texturesNormal, uv3).rgb;
	}

	// Outputs
	ALBEDO = mix(final_albedo.rgb, COLOR.rgb * simpleAlbedo, detailFadeT);
	#ifdef AIH_ALPHA_ENABLED
		// Put a "#define AIH_ALPHA_ENABLED" in your shader to enable transparency
		ALPHA = COLOR.a;
	#endif
	NORMAL_MAP_DEPTH = normalScale * (1.0 - detailFadeT);
	SPECULAR = specular;
}