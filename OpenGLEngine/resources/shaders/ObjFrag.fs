#version 450 core

uniform vec3 color;
uniform sampler2D baseColorMap;

in vec2 vertexUV;
in vec3 vertColor;

out vec4 fragColor;

void main()
{
   fragColor = vec4(color * vertColor * texture(baseColorMap, vertexUV).rgb, 1.0f);
}