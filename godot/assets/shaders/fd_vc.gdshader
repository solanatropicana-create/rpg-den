// Fantastik Dünya — vertex-colour material for Blender assets on MeshInstance3D (buildings, props).
// Vertex colour (linear RGBA, see docs/art-pipeline.md):
//   A = 0    fixed colour (RGB as painted)
//   A = 1    tint mask: RGB is a grey shade multiplied by `tint` (roof/banner = civ colour, leaves = species colour)
//   A = 0.5  window glass: glows warm at night (global night_light × per-instance `lit`)
// Per-instance: set with GeometryInstance3D.SetInstanceShaderParameter("tint", linear rgb) / ("lit", 0..1).
shader_type spatial;
render_mode diffuse_burley, specular_schlick_ggx;

global uniform float night_light;
#include "res://assets/shaders/fd_grade.gdshaderinc"

instance uniform vec3 tint = vec3(0.434, 0.042, 0.027);   // linear (sRGB #b03a2e)
instance uniform float lit = 1.0;

uniform float roughness : hint_range(0.0, 1.0) = 0.9;
uniform vec3 window_color : source_color = vec3(1.0, 0.62, 0.28);
uniform float window_energy = 3.0;

void fragment() {
	float a = COLOR.a;
	float mask = smoothstep(0.7, 0.9, a);
	float glass = 1.0 - smoothstep(0.08, 0.16, abs(a - 0.5));
	vec3 c = COLOR.rgb;
	ALBEDO = fd_grade(mix(c, c * tint, mask));
	ROUGHNESS = fd_wet_rough(mix(roughness, 0.25, glass));
	SPECULAR = mix(0.3, 0.55, glass);
	EMISSION = glass * night_light * lit * window_color * window_energy;
}
