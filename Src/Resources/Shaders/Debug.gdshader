shader_type spatial;

varying vec3 local_position;
varying vec3 local_normal;
varying vec3 object_size;

void vertex() {
    local_position = VERTEX;
    local_normal = NORMAL;
    
    object_size = vec3(
        length(MODEL_MATRIX[0].xyz),
        length(MODEL_MATRIX[1].xyz),
        length(MODEL_MATRIX[2].xyz)
    );
    
    // Pass it to fragment as color for debugging
    COLOR.rgb = object_size - vec3(1) / 10.0;  // Divide by 10 to keep in 0-1 range
}

void fragment() {
    ALBEDO = COLOR.rgb;  // Show the object_size as color
}

//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
