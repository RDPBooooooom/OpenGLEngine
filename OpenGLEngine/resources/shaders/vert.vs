#version 450 core
layout (location = 0) in vec3 in_position;
layout (location = 1) in vec3 in_color;
layout (location = 2) in vec2 uv;

uniform int uv_factor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 vertexColor;
out vec2 vertexUV;

void main()
{
// note that we read the multiplication from right to left
    gl_Position = projection * view * model * vec4(in_position, 1.0);
    vertexColor = in_color;
    vertexUV = uv * uv_factor;
}