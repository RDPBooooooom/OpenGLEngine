#version 450 core
layout (location = 0) in vec3 in_position;
layout (location = 1) in vec3 in_color;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

out vec3 vertexColor;

void main()
{
// note that we read the multiplication from right to left
    gl_Position = projection * view * model * vec4(in_position, 1.0);
    vertexColor = in_color;
}