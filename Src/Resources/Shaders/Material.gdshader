shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;

uniform sampler2D studs_albedo : source_color, filter_linear_mipmap, repeat_enable;
uniform sampler2D studs_normal : hint_normal, filter_linear_mipmap, repeat_enable;
uniform vec3 base_color : source_color = vec3(1.0);
uniform float tile_scale = 1.0;
uniform float normal_strength : hint_range(0.0, 2.0) = 1.0;
uniform vec3 object_size;

varying vec3 local_position;
varying vec3 local_normal;

void vertex() {
	local_position = VERTEX;
	local_normal = NORMAL;
}

int getFirstEqualIndex(vec3 v, float target_value) {
    // Perform component-wise comparison to get a boolean vector
    bvec3 equal_components = equal(v, vec3(target_value));

    // Check for equality with precedence (x first, then y, then z)
    if (equal_components.x) {
        return 0; // x is equal, pick it first
    } else if (equal_components.y) {
        return 1; // x was not equal, but y is
    } else if (equal_components.z) {
        return 2; // neither x nor y were equal, but z is
    }

    // Return a default value if no component matches the target value
    return -1; // Or some other indicator of "not found"
}

void fragment() {
	vec3 abs_normal = abs(local_normal);
	float max_component = max(abs_normal.x, max(abs_normal.y, abs_normal.z));
	
	int idx = getFirstEqualIndex(abs_normal, max_component);
	
	vec3 blend_weights = vec3(0);
	blend_weights[idx] = 1.0;
	
	vec3 studs_per_axis = object_size * tile_scale;
	
	vec3 fractional = fract(studs_per_axis / vec3(2));
	vec3 integer_part = floor(studs_per_axis);
	
	vec2 uv_x = (local_position.yz + fractional.yz) * tile_scale;
	vec2 uv_y = (local_position.xz + fractional.xz) * tile_scale;
	vec2 uv_z = (local_position.xy + fractional.xy) * tile_scale;

	vec3 x_sample = texture(studs_albedo, uv_x).rgb;
	vec3 y_sample = texture(studs_albedo, uv_y).rgb;
	vec3 z_sample = texture(studs_albedo, uv_z).rgb;
	
	vec3 blended_albedo = x_sample * blend_weights.x +
						  y_sample * blend_weights.y +
						  z_sample * blend_weights.z;
		
	ALBEDO = blended_albedo * base_color;

	vec3 x_normal = texture(studs_normal, uv_x).rgb;
	vec3 y_normal = texture(studs_normal, uv_y).rgb;
	vec3 z_normal = texture(studs_normal, uv_z).rgb;
	
	vec3 blended_normal = x_normal * blend_weights.x +
						  y_normal * blend_weights.y +
						  z_normal * blend_weights.z;
	
	NORMAL_MAP = blended_normal;
	NORMAL_MAP_DEPTH = normal_strength;
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
