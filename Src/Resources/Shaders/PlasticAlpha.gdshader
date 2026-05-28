shader_type spatial;
render_mode blend_mix, depth_draw_never, cull_back, diffuse_burley, specular_schlick_ggx;

#define AIH_ALPHA_ENABLED
#include "AIHMaterial.gdshaderinc"