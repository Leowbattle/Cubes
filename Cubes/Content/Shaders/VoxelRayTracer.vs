#version 430 core

layout(location = 0) in vec3 a_position;

layout(location = 0) uniform mat4 u_mvp;
layout(location = 1) uniform mat4 u_modelMatrix;

out VertexOut {
	vec3 position; 
} vsout;

void main() {
	gl_Position = u_mvp * vec4(a_position, 1);
	vsout.position = (u_modelMatrix * vec4(a_position, 1)).xyz;
}