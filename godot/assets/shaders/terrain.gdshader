// Fantastik Dünya — terrain.
// Ground cover comes from 1 m/texel splat maps baked by Heightfield.Bake():
//   splat_a: R dirt/road, G field soil, B rockiness, A sand/bank
//   splat_b: R forest floor, G meadow, B furrow angle (0..1 → 0..π), A cavity (0.5 neutral, <0.5 crease)
//   splat_c: R crop index/4 (nearest), G worn/trampled, B wet (stream/pond edge)
// Layers are painted bottom→top (grass, meadow, forest floor, field, sand, dirt, earth/rock) with noisy,
// sharpened edges so roads and fields read crisply at every LOD. Colours are sRGB (source_color).
shader_type spatial;
render_mode diffuse_burley, specular_schlick_ggx;

uniform sampler2D splat_a : filter_linear_mipmap, repeat_disable;
uniform sampler2D splat_b : filter_linear_mipmap, repeat_disable;
uniform sampler2D splat_c : filter_nearest, repeat_disable;
uniform sampler2D noise_tex : filter_linear_mipmap, repeat_enable;
uniform sampler2D detail_normal : hint_normal, filter_linear_mipmap, repeat_enable;
uniform float detail_normal_strength = 0.45;
uniform vec4 region = vec4(-600.0, -600.0, 1200.0, 1200.0);

group_uniforms grass;
uniform vec3 grass_lush : source_color = vec3(0.40, 0.58, 0.22);
uniform vec3 grass_dry : source_color = vec3(0.62, 0.64, 0.30);
uniform vec3 grass_deep : source_color = vec3(0.27, 0.45, 0.19);
uniform vec3 meadow_col : source_color = vec3(0.58, 0.67, 0.29);
group_uniforms forest;
uniform vec3 forest_floor : source_color = vec3(0.25, 0.30, 0.15);
uniform vec3 forest_litter : source_color = vec3(0.40, 0.32, 0.19);
group_uniforms ground;
uniform vec3 dirt_col : source_color = vec3(0.64, 0.51, 0.34);
uniform vec3 dirt_dark : source_color = vec3(0.49, 0.37, 0.24);
uniform vec3 soil_col : source_color = vec3(0.45, 0.31, 0.19);
uniform vec3 soil_dark : source_color = vec3(0.30, 0.20, 0.12);
uniform vec3 earth_col : source_color = vec3(0.47, 0.37, 0.25);
uniform vec3 rock_col : source_color = vec3(0.56, 0.54, 0.50);
uniform vec3 rock_dark : source_color = vec3(0.39, 0.37, 0.35);
uniform vec3 sand_col : source_color = vec3(0.70, 0.63, 0.47);
uniform vec3 sand_wet : source_color = vec3(0.40, 0.37, 0.30);
group_uniforms crops;
uniform vec3 crop_wheat : source_color = vec3(0.80, 0.68, 0.34);
uniform vec3 crop_barley : source_color = vec3(0.70, 0.70, 0.40);
uniform vec3 crop_veg : source_color = vec3(0.33, 0.52, 0.22);
uniform vec3 crop_flax : source_color = vec3(0.48, 0.58, 0.55);
group_uniforms;
uniform float furrow_spacing = 0.9;
uniform float detail_far = 240.0;

// Distance LOD crossfade (per fragment, dithered, opaque): this LOD is kept while lod_min <= d < lod_max,
// d = horizontal distance to the global fd_cam_pos. Neighbouring LODs use complementary dither so exactly one
// covers every pixel in the overlap band — no transparency pass, no see-through, no washed-out rectangles.
global uniform vec3 fd_cam_pos;
uniform float lod_min = 0.0;
uniform float lod_max = 100000.0;
uniform float lod_fade = 12.0;
varying vec3 v_world;
varying vec3 v_wnormal;

#include "res://assets/shaders/ground_color.gdshaderinc"
#include "res://assets/shaders/fd_grade.gdshaderinc"

void vertex() {
	v_world = (MODEL_MATRIX * vec4(VERTEX, 1.0)).xyz;
	v_wnormal = normalize((MODEL_MATRIX * vec4(NORMAL, 0.0)).xyz);
}

float nz(vec2 p) { return texture(noise_tex, p).r; }

// sharpen a soft weight into a noisy-edged mask
float edge(float w, float n, float soft) { return smoothstep(0.5 - soft, 0.5 + soft, w + n); }

void fragment() {
	{
		float dcam = distance(v_world.xz, fd_cam_pos.xz);
		float fin = lod_min > 0.0 ? smoothstep(lod_min - lod_fade, lod_min, dcam) : 1.0;
		float fout = 1.0 - smoothstep(lod_max - lod_fade, lod_max, dcam);
		float vf = fin * fout;
		if (vf < 0.999) {
			float ign = fract(52.9829189 * fract(dot(FRAGCOORD.xy, vec2(0.06711056, 0.00583715))));
			float t = lod_min > 0.0 && fin < 0.999 ? 1.0 - ign : ign;
			if (t >= vf) discard;
		}
	}
	vec2 wp = v_world.xz;
	vec2 uv = (wp - region.xy) / region.zw;
	vec4 sa = texture(splat_a, uv);
	vec4 sb = texture(splat_b, uv);
	vec4 sc = texture(splat_c, uv);

	float dist = length(v_world - CAMERA_POSITION_WORLD);
	float detail = 1.0 - smoothstep(detail_far * 0.35, detail_far, dist);

	float macro = nz(wp / 420.0);
	float mid = nz(wp / 70.0 + vec2(0.37, 0.11));
	float fine = mix(0.5, nz(wp / 11.0 + vec2(0.71, 0.29)), detail);
	float micro = mix(0.5, nz(wp / 2.7 + vec2(0.13, 0.57)), detail * detail);
	// brush-like streaks: stretched noise gives a painterly grain to the grass
	float streak = mix(0.5, nz(vec2(wp.x / 6.0 + wp.y / 21.0, wp.y / 2.1) + vec2(0.4, 0.8)), detail);

	float slope = 1.0 - clamp(v_wnormal.y, 0.0, 1.0);
	float cav = sb.a;

	// ---- grass & meadow: lush in hollows and moist areas, drier/yellower on crests (shared with grass.gdshader)
	vec3 col = fd_grass_color(wp, sb, slope, fine, streak);
	// slow hue drift: ochre (sun-dried) vs blue-green (shady/moist) patches
	float hue = nz(wp / 160.0 + vec2(0.71, 0.13));
	col = mix(col, col * vec3(1.12, 1.0, 0.78), smoothstep(0.55, 0.8, hue) * 0.55);
	col = mix(col, col * vec3(0.86, 1.0, 1.08), smoothstep(0.45, 0.2, hue) * 0.5);
	// meadow flowers seen from afar: tiny speckles of white/yellow/violet (fade before they alias)
	float fl = smoothstep(0.1, 0.7, sb.g) * (1.0 - smoothstep(0.1, 0.6, sb.r));
	if (fl > 0.01) {
		// only where the 3D flower clumps (GrassField, ~54 m) have faded out, and before they alias
		float fdet = smoothstep(38.0, 55.0, dist) * (1.0 - smoothstep(110.0, 220.0, dist));
		float n1 = texture(noise_tex, wp / 0.8 + vec2(0.23, 0.61)).r;
		float n2 = texture(noise_tex, wp / 23.0 + vec2(0.91, 0.47)).r;
		float speck = smoothstep(0.7, 0.74, n1) * fl * fdet * smoothstep(0.35, 0.6, n2);
		vec3 fcol = n2 > 0.52 ? vec3(0.98, 0.85, 0.35) : (n2 > 0.45 ? vec3(0.95, 0.95, 0.92) : vec3(0.6, 0.42, 0.85));
		col = mix(col, fcol, speck * 0.75);
	}

	// ---- forest floor: dark moss with patches of brown leaf/needle litter
	float fw = smoothstep(0.12, 0.65, sb.r + (mid - 0.5) * 0.35);
	vec3 ff = mix(forest_floor, forest_litter, smoothstep(0.4, 0.7, fine * 0.7 + mid * 0.3));
	ff *= 0.9 + 0.2 * micro;
	col = mix(col, ff, fw);

	// ---- fields: tilled soil with furrows, crops by plot
	float fieldw = edge(sa.g, (fine - 0.5) * 0.18, 0.12);
	if (fieldw > 0.001) {
		float ang = sb.b * 3.14159265;
		vec2 dir = vec2(cos(ang), sin(ang));
		float across = dot(wp, vec2(-dir.y, dir.x));
		float fur = 0.5 + 0.5 * sin(across * 6.2831853 / furrow_spacing);
		// fade the stripes out before they alias (fwidth = metres per pixel across the furrows)
		float aa = 1.0 - smoothstep(0.12, 0.35, fwidth(across) / furrow_spacing);
		float furLod = mix(0.5, fur, aa);
		vec3 soil = mix(soil_dark, soil_col, furLod) * (0.93 + 0.14 * micro);
		int crop = int(round(sc.r * 4.0));
		vec3 fcol = soil;
		if (crop == 1 || crop == 2) {
			vec3 cc = crop == 1 ? crop_wheat : crop_barley;
			cc *= 0.88 + 0.2 * streak;
			fcol = mix(mix(soil, cc, 0.35), cc, smoothstep(0.25, 0.6, furLod) * 0.95 + 0.05);
		} else if (crop == 3) {
			vec3 cc = crop_veg * (0.8 + 0.3 * fine);
			fcol = mix(soil, cc, smoothstep(0.55, 0.85, furLod) * 0.9);
		} else if (crop == 4) {
			vec3 cc = mix(crop_flax, crop_veg, 0.35 + 0.3 * streak);
			fcol = mix(soil, cc, 0.35 + 0.55 * furLod);
		} else {
			// fallow: soil with weedy tufts
			fcol = mix(soil, grass_dry * 0.85, smoothstep(0.45, 0.75, fine * 0.6 + micro * 0.4) * 0.6);
		}
		col = mix(col, fcol, fieldw);
	}

	// ---- sand / gravel banks (wet near water)
	float sw = edge(sa.a, (fine - 0.5) * 0.3 + (micro - 0.5) * 0.12, 0.16);
	vec3 scol = mix(sand_col, sand_wet, smoothstep(0.2, 0.9, sc.b)) * (0.9 + 0.2 * micro);
	col = mix(col, scol, sw);

	// ---- dirt: roads, trails, lanes, yards
	float worn = sc.g;
	col = mix(col, mix(col, dirt_col * 0.9, 0.35), smoothstep(0.1, 0.9, worn) * 0.6);
	float dw = edge(sa.r, (fine - 0.5) * 0.35 + (micro - 0.5) * 0.14, 0.1);
	vec3 dcol = mix(dirt_dark, dirt_col, smoothstep(0.25, 0.75, sa.r * 0.5 + fine * 0.3 + micro * 0.2));
	dcol *= 0.92 + 0.12 * streak;
	col = mix(col, dcol, dw);

	// ---- exposed earth on steep banks, rock on cliffs and rocky ground
	float earthw = smoothstep(0.24, 0.36, slope + (mid - 0.5) * 0.12) * (1.0 - fieldw);
	col = mix(col, earth_col * (0.9 + 0.2 * fine), earthw * 0.85);
	float rk = max(sa.b, smoothstep(0.34, 0.5, slope + (fine - 0.5) * 0.1));
	float rw = edge(rk, (fine - 0.5) * 0.3 + (micro - 0.5) * 0.12, 0.12);
	// rock: faceted light/dark bands plus dark cracks
	float strata = nz(vec2(wp.x / 9.0 + v_world.y / 3.0, wp.y / 9.0 - v_world.y / 5.0));
	vec3 rcol = mix(rock_dark, rock_col, smoothstep(0.3, 0.75, strata * 0.6 + fine * 0.25 + v_wnormal.y * 0.3));
	rcol *= 0.8 + 0.3 * smoothstep(0.2, 0.5, abs(nz(wp / 4.0 + vec2(0.3, 0.9)) - 0.5) * 2.0);
	rcol = mix(rcol, rcol * vec3(0.85, 0.95, 0.8), smoothstep(0.55, 0.8, mid) * 0.5);   // lichen tint
	col = mix(col, rcol, rw);

	// ---- wet darkening, cavity shading
	col *= mix(1.0, 0.75, sc.b * (1.0 - rw));
	col *= mix(0.72, 1.07, smoothstep(0.18, 0.72, cav));

	ALBEDO = fd_grade(col);
	// clumpy micro-relief (grass tufts, pebbles): two scales of a cellular normal map, near the camera only
	vec3 n1 = texture(detail_normal, wp / 3.3).rgb;
	vec3 n2 = texture(detail_normal, wp / 1.15 + vec2(0.31, 0.77)).rgb;
	NORMAL_MAP = normalize(mix(n1, n2, 0.4));
	NORMAL_MAP_DEPTH = detail_normal_strength * detail * (1.0 - 0.6 * dw) * (1.0 - sb.r * 0.3);
	ROUGHNESS = fd_wet_rough(mix(0.95, 0.55, sc.b * 0.8));
	SPECULAR = 0.22;
}
