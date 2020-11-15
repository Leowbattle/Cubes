#version 430 core

in VertexOut {
	vec4 colour; 
} fsin;

layout(location = 0) out vec4 fragColour;

void main() {
	fragColour = fsin.colour;
}