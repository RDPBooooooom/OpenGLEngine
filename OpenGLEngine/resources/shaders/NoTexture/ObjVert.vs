#version 450 core
layout (location = 0) in vec3 in_position;
layout (location = 1) in vec2 uv;
layout (location = 2) in vec3 in_color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec2 vertexUV;
out vec3 vertColor;

void main()
{
    gl_Position = projection * view * model * vec4(in_position, 1.0);
    vertexUV = uv;
    vertColor = in_color;
}