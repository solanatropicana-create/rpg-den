// Shared ground colour for terrain.gdshader and grass.gdshader (so grass blades match the ground they grow on).
// The including shader must declare: noise_tex, grass_lush, grass_dry, grass_deep, meadow_col (source_color).

// Grass/meadow colour at world position wp. sb = splat_b sample (forest, meadow, furrow, cavity).
// fine/streak: detail noises in 0..1 (pass 0.5 when not available, e.g. in vertex shaders).
vec3 fd_grass_color(vec2 wp, vec4 sb, float slope, float fine, float streak) {
	float macro = textureLod(noise_tex, wp / 420.0, 0.0).r;
	float mid = textureLod(noise_tex, wp / 70.0 + vec2(0.37, 0.11), 0.0).r;
	float cav = sb.a;
	float moist = clamp((macro - 0.5) * 1.6 + 0.5 + (0.5 - cav) * 1.4 - slope * 0.8, 0.0, 1.0);
	vec3 col = mix(grass_dry, grass_lush, smoothstep(0.2, 0.8, moist));
	col = mix(col, grass_deep, smoothstep(0.45, 0.85, mid) * 0.45);
	col *= 0.86 + 0.28 * mix(fine, streak, 0.55);
	float mw = smoothstep(0.15, 0.8, sb.g) * (0.55 + 0.45 * smoothstep(0.3, 0.7, mid));
	col = mix(col, meadow_col * (0.94 + 0.12 * streak), mw);
	return col;
}
