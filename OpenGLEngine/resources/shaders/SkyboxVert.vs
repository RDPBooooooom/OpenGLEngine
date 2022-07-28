#version 450 core
layout(location = 0) in vec3 in_position;

uniform mat4 projection;
uniform mat4 view;
    
out vec3 TexCoords;

void main()
{
    mat4 viewTemp = mat4(mat3(view));
    vec4 pos = projection * viewTemp * vec4(in_position, 1.0);
    TexCoords = in_position;
    gl_Position = pos.xyww;
}