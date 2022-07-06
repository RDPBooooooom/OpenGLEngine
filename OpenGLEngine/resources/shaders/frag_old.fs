#version 450 core

uniform vec3 color;

in vec3 vertexColor;

out vec4 fragColor;

void main()
{
   fragColor = vec4(color * vertexColor, 1.0f);
}