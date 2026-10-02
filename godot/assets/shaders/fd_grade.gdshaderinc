// Fantastik Dünya — Tur 1 D: the shared colour grade of every world material (art direction: Valheim's gloom on Kenshi's
// land). Applied to the albedo (linear) of vertex-colour props, trees, characters, terrain, grass and water, so the models
// stay as painted and the whole palette is pulled down here:
//   fd_sat     saturation ×   (style frame: 0.60)
//   fd_bright  brightness ×   (0.88)
//   fd_dirt    bright surfaces mixed toward earth-grey (0.30, 0.27, 0.22) by brightness × fd_dirt (0.55)
//   fd_wet     0 dry … 1 soaked (rain): darker albedo; the including shader lowers its roughness with fd_wet too
// Globals are declared in project.godot [shader_globals] and driven by FD.World.DayNight / Weather.
global uniform float fd_sat;
global uniform float fd_bright;
global uniform float fd_dirt;
global uniform float fd_wet;

vec3 fd_rgb2hsv(vec3 c) {
	vec4 K = vec4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	vec4 p = mix(vec4(c.bg, K.wz), vec4(c.gb, K.xy), step(c.b, c.g));
	vec4 q = mix(vec4(p.xyw, c.r), vec4(c.r, p.yzx), step(p.x, c.r));
	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return vec3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

vec3 fd_hsv2rgb(vec3 c) {
	vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 fd_grade(vec3 c) {
	vec3 hsv = fd_rgb2hsv(max(c, vec3(0.0)));
	hsv.y *= fd_sat;
	hsv.z *= fd_bright;
	vec3 g = fd_hsv2rgb(hsv);
	g = mix(g, vec3(0.30, 0.27, 0.22), clamp(hsv.z * fd_dirt, 0.0, 1.0));
	return g * (1.0 - 0.28 * fd_wet);
}

float fd_wet_rough(float r) { return mix(r, min(r, 0.42), fd_wet); }
