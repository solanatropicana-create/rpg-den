// Fantastik Dünya — stylised water for the stream ribbon and the pond.
// Depth-tinted (shallow → deep from the depth buffer), fresnel sky reflection via low roughness,
// two scrolling normal maps (flowing along UV.x for the stream, drifting for the pond), soft foam where the
// water meets the banks, transparent.
shader_type spatial;
render_mode blend_mix, depth_draw_always, cull_back, diffuse_burley, specular_schlick_ggx;

uniform vec3 shallow_color : source_color = vec3(0.38, 0.60, 0.55);
uniform vec3 deep_color : source_color = vec3(0.10, 0.28, 0.32);
uniform vec3 foam_color : source_color = vec3(0.88, 0.92, 0.90);
uniform float deep_depth = 1.3;          // metres of water (along the view ray) to reach deep_color
uniform float foam_width = 0.55;
uniform float flow_speed = 0.55;         // m/s along the stream
uniform bool flowing = true;
uniform sampler2D normal_a : hint_normal, filter_linear_mipmap, repeat_enable;
uniform sampler2D normal_b : hint_normal, filter_linear_mipmap, repeat_enable;
uniform sampler2D depth_tex : hint_depth_texture, filter_nearest;
#include "res://assets/shaders/fd_grade.gdshaderinc"

varying vec3 v_world;

void vertex() {
	v_world = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
}

void fragment() {
	vec2 uv1, uv2;
	float t = TIME;
	if (flowing) {
		// UV.x = arc length (m); water flows toward smaller s
		vec2 base = vec2(UV.x, UV.y * 3.0);
		uv1 = (base + vec2(t * flow_speed, 0.0)) / vec2(9.0, 7.0);
		uv2 = (base * vec2(1.0, 1.3) + vec2(t * flow_speed * 1.6, 1.7)) / vec2(4.5, 3.5);
	} else {
		uv1 = v_world.xz / 11.0 + vec2(t * 0.012, t * 0.007);
		uv2 = v_world.xz / 4.3 - vec2(t * 0.02, -t * 0.013);
	}
	vec3 n1 = texture(normal_a, uv1).rgb;
	vec3 n2 = texture(normal_b, uv2).rgb;
	NORMAL_MAP = normalize(mix(n1, n2, 0.45));
	NORMAL_MAP_DEPTH = flowing ? 0.9 : 0.55;

	// water thickness along the view ray from the depth buffer
	float depth_raw = texture(depth_tex, SCREEN_UV).r;
	vec4 ndc = vec4(SCREEN_UV * 2.0 - 1.0, depth_raw, 1.0);
	vec4 view = INV_PROJECTION_MATRIX * ndc;
	view.xyz /= view.w;
	float thickness = max(-view.z - (-VERTEX.z), 0.0);

	float dk = clamp(thickness / deep_depth, 0.0, 1.0);
	vec3 col = mix(shallow_color, deep_color, smoothstep(0.0, 1.0, dk));

	float wob = texture(normal_b, uv2 * 0.7).r;
	float foam = 1.0 - smoothstep(0.0, foam_width * (0.7 + 0.6 * wob), thickness);
	foam *= 0.85;

	ALBEDO = fd_grade(mix(col, foam_color, foam));
	ALPHA = clamp(mix(0.55, 0.93, smoothstep(0.0, 0.7, dk)) + foam * 0.5, 0.0, 1.0);
	ROUGHNESS = mix(0.04, 0.5, foam);
	SPECULAR = 0.55;
	METALLIC = 0.0;
}
