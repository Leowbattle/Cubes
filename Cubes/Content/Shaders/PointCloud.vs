#version 430 core

layout(location = 0) in vec3 a_position;
layout(location = 1) in vec4 a_colour;

layout(location = 0) uniform mat4 u_mvp;

out VertexOut {
	vec4 colour; 
} vsout;

void main() {
	gl_Position = u_mvp * vec4(a_position, 1);

	vsout.colour = a_colour;
}