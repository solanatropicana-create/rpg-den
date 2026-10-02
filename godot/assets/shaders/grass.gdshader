// Fantastik Dünya — near-field grass, flowers and reeds (MultiMesh with custom data).
// INSTANCE_CUSTOM.a = 0: blades take the terrain's grass colour at the instance root (ground_color include),
//                         multiplied by INSTANCE_CUSTOM.rgb (small per-clump variation);
// INSTANCE_CUSTOM.a = 1: INSTANCE_CUSTOM.rgb is the absolute tint (reeds, special plants).
// Instances shrink into the ground between fade_start and fade_end (distance to the global fd_cam_pos),
// sway in the wind, and use mostly-up normals so they light like the terrain.
shader_type spatial;
render_mode diffuse_burley, specular_schlick_ggx, cull_disabled, world_vertex_coords;

global uniform vec3 fd_cam_pos;
global uniform vec4 fd_wind;

uniform sampler2D noise_tex : filter_linear_mipmap, repeat_enable;
uniform sampler2D splat_b : filter_linear_mipmap, repeat_disable;
uniform vec4 region = vec4(-600.0, -600.0, 1200.0, 1200.0);
uniform vec3 grass_lush : source_color = vec3(0.40, 0.58, 0.22);
uniform vec3 grass_dry : source_color = vec3(0.62, 0.64, 0.30);
uniform vec3 grass_deep : source_color = vec3(0.27, 0.45, 0.19);
uniform vec3 meadow_col : source_color = vec3(0.58, 0.67, 0.29);
uniform vec3 forest_grass : source_color = vec3(0.30, 0.42, 0.18);

uniform float fade_start = 38.0;
uniform float fade_end = 54.0;
uniform float wind_amount = 0.25;
uniform float sway_height = 0.6;
uniform float up_normal = 0.75;
uniform float base_dark = 0.6;
uniform float tip_light = 1.18;
uniform float backlight = 0.3;

#include "res://assets/shaders/ground_color.gdshaderinc"
#include "res://assets/shaders/fd_grade.gdshaderinc"

varying vec3 v_tint;
varying float v_h;

void vertex() {
	vec3 origin = MODEL_MATRIX[3].xyz;
	float d = distance(origin.xz, fd_cam_pos.xz);
	float s = 1.0 - smoothstep(fade_start, fade_end, d);
	float hgt = VERTEX.y - origin.y;
	v_h = clamp(hgt / sway_height, 0.0, 1.5);
	if (s <= 0.0) {
		VERTEX = origin;
	} else {
		VERTEX.y = origin.y + hgt * s;
		float phase = dot(origin.xz, vec2(0.37, 0.53));
		float t = TIME;
		float sway = sin(t * 1.7 + phase) * 0.55 + sin(t * 3.9 + phase * 2.3) * 0.25 + fd_wind.w * 0.5;
		VERTEX.xz += fd_wind.xy * sway * wind_amount * fd_wind.z * v_h * v_h * s;
	}
	NORMAL = normalize(mix(NORMAL, vec3(0.0, 1.0, 0.0), up_normal));
	if (INSTANCE_CUSTOM.a > 0.5) {
		v_tint = INSTANCE_CUSTOM.rgb;
	} else {
		vec2 uv = (origin.xz - region.xy) / region.zw;
		vec4 sb = textureLod(splat_b, uv, 0.0);
		vec3 g = fd_grass_color(origin.xz, sb, 0.0, 0.5, 0.5);
		g = mix(g, forest_grass, smoothstep(0.2, 0.8, sb.r) * 0.6);
		v_tint = g * INSTANCE_CUSTOM.rgb;
	}
}

void fragment() {
	float mask = smoothstep(0.7, 0.9, COLOR.a);
	// masked parts: RGB is a grey gradient (base → tip); remap so the tip is a bit lighter than the ground
	vec3 shade = mix(vec3(base_dark), vec3(tip_light), smoothstep(0.4, 1.0, COLOR.r));
	vec3 c = mix(COLOR.rgb, v_tint * shade, mask);
	ALBEDO = fd_grade(c);
	ROUGHNESS = fd_wet_rough(0.9);
	SPECULAR = 0.15;
	BACKLIGHT = vec3(backlight) * c;
}
