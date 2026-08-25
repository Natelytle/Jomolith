shader_type spatial;
render_mode blend_mix, depth_draw_never, cull_back, diffuse_burley, specular_schlick_ggx;

#define ALPHA_ENABLED
#include "JomolithMaterial.gdshaderinc"
