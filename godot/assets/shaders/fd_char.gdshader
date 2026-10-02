// Fantastik Dünya — characters (skinned or not). Vertex colour A selects a colour slot (shared spec):
//   0 fixed · 0.25 skin · 0.5 cloth 1 (shirt/tunic) · 0.75 cloth 2 (trousers/skirt) · 1.0 hair
// RGB is a grey shade multiplied by the slot colour. Slot colours are per-instance (linear RGB):
//   SetInstanceShaderParameter("skin_color" | "cloth1_color" | "cloth2_color" | "hair_color", Color)
shader_type spatial;
render_mode diffuse_burley, specular_schlick_ggx;

instance uniform vec3 skin_color = vec3(0.62, 0.36, 0.24);
instance uniform vec3 cloth1_color = vec3(0.40, 0.12, 0.08);
instance uniform vec3 cloth2_color = vec3(0.16, 0.13, 0.10);
instance uniform vec3 hair_color = vec3(0.10, 0.05, 0.02);

uniform float roughness : hint_range(0.0, 1.0) = 0.85;
#include "res://assets/shaders/fd_grade.gdshaderinc"

void fragment() {
	int slot = int(round(COLOR.a * 4.0));
	vec3 t = vec3(1.0);
	if (slot == 1) t = skin_color;
	else if (slot == 2) t = cloth1_color;
	else if (slot == 3) t = cloth2_color;
	else if (slot == 4) t = hair_color;
	ALBEDO = fd_grade(COLOR.rgb * t);
	ROUGHNESS = fd_wet_rough(roughness);
	SPECULAR = slot == 1 ? 0.35 : 0.25;
}
