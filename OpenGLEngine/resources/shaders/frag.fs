#version 450 core

uniform vec3 color;
uniform sampler2D baseColorMap;

in vec3 vertexColor;
in vec2 vertexUV;

out vec4 fragColor;

void main()
{
   fragColor = vec4(color * vertexColor * texture(baseColorMap, vertexUV).rgb, 1.0f);
}