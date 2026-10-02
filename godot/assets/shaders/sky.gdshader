// Fantastik Dünya — stylised sky: time-of-day gradient, sun/moon disks and glow, soft clouds, stars.
// All parameters are driven by FD.World.DayNight (no TIME use, so radiance updates incrementally).
shader_type sky;

uniform vec3 zenith_color : source_color = vec3(0.23, 0.47, 0.78);
uniform vec3 horizon_color : source_color = vec3(0.66, 0.78, 0.89);
uniform vec3 horizon_sun_color : source_color = vec3(0.95, 0.80, 0.60);
uniform vec3 ground_color : source_color = vec3(0.45, 0.50, 0.52);
uniform vec3 sun_dir = vec3(0.3, 0.7, 0.3);
uniform vec3 sun_color : source_color = vec3(1.0, 0.95, 0.85);
uniform float sun_glow = 1.0;
uniform float sun_disk = 1.0;
uniform vec3 moon_dir = vec3(-0.3, -0.7, -0.3);
uniform float moon_amount = 0.0;
uniform float star_amount = 0.0;
uniform float sky_time = 0.0;          // seconds, drives cloud drift and twinkle
uniform float cloud_cover = 0.42;
uniform vec3 cloud_lit : source_color = vec3(1.0, 0.98, 0.95);
uniform vec3 cloud_dark : source_color = vec3(0.62, 0.66, 0.74);
uniform float cloud_alpha = 0.85;
uniform sampler2D cloud_noise : filter_linear_mipmap, repeat_enable;

float hash13(vec3 p) {
	p = fract(p * 0.1031);
	p += dot(p, p.zyx + 31.32);
	return fract((p.x + p.y) * p.z);
}

vec3 stars(vec3 d) {
	vec3 p = d * 200.0;
	vec3 cell = floor(p);
	float r = hash13(cell);
	if (r < 0.9965) return vec3(0.0);
	vec3 jitter = vec3(hash13(cell + 7.1), hash13(cell + 13.7), hash13(cell + 29.3)) * 0.8 + 0.1;
	vec3 sp = normalize(cell + jitter);
	float ang = 1.0 - dot(d, sp);
	float size = 9.0e-7 + 2.6e-6 * hash13(cell + 3.3);
	float s = smoothstep(size, 0.0, ang);
	float tw = 0.75 + 0.25 * sin(sky_time * (1.5 + 3.0 * hash13(cell + 1.7)) + r * 60.0);
	vec3 tint = mix(vec3(0.75, 0.85, 1.0), vec3(1.0, 0.92, 0.8), hash13(cell + 5.5));
	return tint * s * tw * (0.6 + 2.2 * (r - 0.9965) / 0.0035);
}

float clouds(vec3 d, out float shade) {
	shade = 0.0;
	if (d.y <= 0.01) return 0.0;
	vec2 uv = d.xz / (d.y + 0.12) * 0.28 + vec2(sky_time * 0.0022, sky_time * 0.0009);
	float n = texture(cloud_noise, uv).r * 0.6 + texture(cloud_noise, uv * 2.3 + 0.37).r * 0.28
			+ texture(cloud_noise, uv * 5.1 + 0.71).r * 0.12;
	float c = smoothstep(1.0 - cloud_cover, 1.0 - cloud_cover + 0.28, n);
	shade = smoothstep(0.0, 0.35, n - (1.0 - cloud_cover)) ;
	return c * smoothstep(0.01, 0.2, d.y);
}

void sky() {
	vec3 d = normalize(EYEDIR);
	float h = d.y;
	vec3 sd = normalize(sun_dir);
	float cs = dot(d, sd);

	// gradient, warmer toward the sun's azimuth near the horizon
	float az = 0.5 + 0.5 * dot(normalize(d.xz + vec2(1e-5)), normalize(sd.xz + vec2(1e-5)));
	vec3 hor = mix(horizon_color, horizon_sun_color, pow(az, 4.0) * (1.0 - smoothstep(0.0, 0.5, h)));
	float t = pow(clamp(h, 0.0, 1.0), 0.5);
	vec3 col = mix(hor, zenith_color, t);
	col = mix(col, mix(hor, ground_color, 0.55), smoothstep(0.0, -0.3, h));

	// sun glow + disk
	float g = max(cs, 0.0);
	col += sun_color * sun_glow * (pow(g, 10.0) * 0.28 + pow(g, 90.0) * 0.5);
	if (!AT_CUBEMAP_PASS) {
		col += sun_color * sun_disk * 14.0 * smoothstep(0.99978, 0.99988, cs);
	}

	// night: stars and moon
	if (!AT_CUBEMAP_PASS && star_amount > 0.001 && h > -0.05) {
		col += stars(d) * star_amount * smoothstep(-0.05, 0.25, h);
	}
	vec3 md = normalize(moon_dir);
	float cm = dot(d, md);
	if (moon_amount > 0.001) {
		col += vec3(0.55, 0.62, 0.8) * moon_amount * (pow(max(cm, 0.0), 30.0) * 0.12 + pow(max(cm, 0.0), 400.0) * 0.25);
		if (!AT_CUBEMAP_PASS) {
			float disk = smoothstep(0.99975, 0.99984, cm);
			// cheap "maria": darker blotches on the disk
			vec3 local = d - md;
			float maria = texture(cloud_noise, local.xz * 90.0 + 0.3).r;
			col = mix(col, vec3(0.92, 0.93, 0.88) * (0.75 + 0.35 * maria) * 2.4, disk * moon_amount);
		}
	}

	// clouds (lit by the sun colour, dark on the far side)
	float shade;
	float c = clouds(d, shade);
	if (c > 0.0) {
		vec3 lit = mix(cloud_dark, cloud_lit, clamp(0.35 + 0.65 * (0.5 + 0.5 * cs) - shade * 0.35, 0.0, 1.0));
		lit += sun_color * sun_glow * pow(g, 8.0) * 0.35;
		col = mix(col, lit, c * cloud_alpha);
	}
	COLOR = col;
}
