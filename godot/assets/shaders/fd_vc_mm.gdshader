// Fantastik Dünya — vertex-colour material for MultiMesh instances (trees, bushes, rocks, repeated props).
// Same vertex-colour convention as fd_vc.gdshader, but per-instance data comes from MultiMesh custom data
// (MultiMesh.use_custom_data = true): INSTANCE_CUSTOM.rgb = tint (linear), INSTANCE_CUSTOM.a = lit (windows).
//
// Per-instance distance LOD (so one big MultiMesh can hold near and far copies):
//   instance visible while lod_min <= d < lod_max, d = horizontal distance from the global fd_cam_pos to the
//   instance origin; crossfaded with a screen-space dither over lod_fade metres. A "hi" MultiMesh with
//   lod_max = L and a "lo" MultiMesh with lod_min = L crossfade seamlessly (complementary dither).
//   Fully hidden instances are collapsed in the vertex stage (no raster cost). fd_cam_pos is also used in
//   shadow passes, so shadows switch consistently with the visible LOD.
// Wind: vertices sway ∝ ((y − origin.y) / sway_height)² × wind_amount × global fd_wind.z; tint-masked
// vertices (leaves) also flutter.
shader_type spatial;
render_mode diffuse_burley, specular_schlick_ggx, world_vertex_coords;

global uniform float night_light;
global uniform vec3 fd_cam_pos;
global uniform vec4 fd_wind;

uniform float roughness : hint_range(0.0, 1.0) = 0.9;
uniform vec3 window_color : source_color = vec3(1.0, 0.62, 0.28);
uniform float window_energy = 3.0;
uniform float lod_min = 0.0;
uniform float lod_max = 100000.0;
uniform float lod_fade = 8.0;
uniform float wind_amount = 0.0;
uniform float sway_height = 10.0;
uniform float flutter = 0.0;
// subtle translucency for foliage
uniform float leaf_backlight = 0.0;

varying float v_fade;
varying vec3 v_tint;
varying float v_lit;

void vertex() {
	vec3 origin = MODEL_MATRIX[3].xyz;
	float d = distance(origin.xz, fd_cam_pos.xz);
	float fin = lod_min > 0.0 ? smoothstep(lod_min - lod_fade, lod_min, d) : 1.0;
	float fout = 1.0 - smoothstep(lod_max - lod_fade, lod_max, d);
	v_fade = fin * fout;
	v_tint = INSTANCE_CUSTOM.rgb;
	v_lit = INSTANCE_CUSTOM.a;
	if (v_fade <= 0.0) {
		VERTEX = origin;
	} else if (wind_amount > 0.0) {
		float h = max(VERTEX.y - origin.y, 0.0) / sway_height;
		float phase = dot(origin.xz, vec2(0.131, 0.217));
		float t = TIME;
		float sway = sin(t * 1.1 + phase) * 0.6 + sin(t * 2.3 + phase * 1.7) * 0.25 + fd_wind.w * 0.6;
		vec2 dir = fd_wind.xy;
		VERTEX.xz += dir * sway * wind_amount * fd_wind.z * h * h;
		float leaf = smoothstep(0.7, 0.9, COLOR.a);
		VERTEX += flutter * leaf * fd_wind.z * vec3(
			sin(t * 7.0 + VERTEX.x * 1.9 + VERTEX.y * 0.7),
			sin(t * 6.1 + VERTEX.z * 2.3) * 0.5,
			cos(t * 7.7 + VERTEX.z * 1.7 + VERTEX.y * 0.9)) * h;
	}
}

void fragment() {
	if (v_fade < 0.999) {
		float ign = fract(52.9829189 * fract(dot(FRAGCOORD.xy, vec2(0.06711056, 0.00583715))));
		float t = lod_min > 0.0 ? 1.0 - ign : ign;
		if (t >= v_fade) discard;
	}
	float a = COLOR.a;
	float mask = smoothstep(0.7, 0.9, a);
	float glass = 1.0 - smoothstep(0.08, 0.16, abs(a - 0.5));
	vec3 c = COLOR.rgb;
	ALBEDO = mix(c, c * v_tint, mask);
	ROUGHNESS = mix(roughness, 0.25, glass);
	SPECULAR = mix(0.25, 0.55, glass);
	EMISSION = glass * night_light * v_lit * window_color * window_energy;
	BACKLIGHT = vec3(leaf_backlight * mask) * ALBEDO;
}
