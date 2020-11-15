#version 430 core

in VertexOut {
	vec3 position; 
} fsin;

layout(location = 2) uniform vec3 camPos;
layout(location = 3) uniform vec3 camDir;

layout(location = 0) out vec4 fragColour;

struct Ray {
	vec3 origin;
	vec3 direction;
};

struct Sphere {
	vec3 pos;
	float r;
};

float RaySphere(Ray ray, Sphere sphere) {
	// https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-sphere-intersection

	vec3 L = sphere.pos - ray.origin;
	float tca = dot(L, ray.direction);
	if (tca < 0) {
		return 0;
	}

	float d = sqrt(dot(L, L) - tca * tca);
	if (d < 0) {
		return 0;
	}

	float thc = sqrt(sphere.r * sphere.r - d * d);

	return tca - thc;
}

void main() {
	Ray ray = Ray(camPos, fsin.position - camPos);
    fragColour = vec4(RaySphere(ray, Sphere(vec3(0), 1)));
}