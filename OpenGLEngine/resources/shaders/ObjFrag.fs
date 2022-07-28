#version 450 core

#define NR_POINT_LIGHTS 1 

struct Material {
    float ambient;
    float diffuse;
    float specular;
    float shininess;
}; 
struct DirLight {
    vec3 direction;
  
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};
struct PointLight {    
    vec3 position;
    
    float constant;
    float linear;
    float quadratic;  

    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};  


uniform vec3 color;
uniform sampler2D baseColorMap;

uniform Material material;

uniform vec3 viewPos;
uniform DirLight dirLight;
uniform PointLight pointLights[NR_POINT_LIGHTS];

in vec2 vertexUV;
in vec3 vertexPos;
in vec3 vertexNormalWorldSpace;

out vec4 fragColor;

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir);
vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir);  

void main()
{
    vec3 baseColor = color * texture(baseColorMap, vertexUV).rgb;
    // properties
    vec3 norm = normalize(vertexNormalWorldSpace);
    vec3 viewDir = normalize(viewPos - vertexPos);

    // phase 1: Directional lighting
    vec3 result = CalcDirLight(dirLight, norm, viewDir);
    // phase 2: Point lights
    for(int i = 0; i < NR_POINT_LIGHTS; i++)
        result += CalcPointLight(pointLights[i], norm, vertexPos, viewDir);    
    // phase 3: Spot light
    //result += CalcSpotLight(spotLight, norm, vertexPos, viewDir);    
    
    fragColor = vec4(baseColor * result, 1.0);
}

vec3 ambientReflection(float intensity, float factor, vec3 lightColor){
	return lightColor * intensity * factor;
}

vec3 diffuseReflection(float intensity, float factor, vec3 lightColor, vec3 lightDirection, vec3 normal){
	return lightColor * intensity * factor * clamp(dot(lightDirection, normal), 0.0001f, 1.0f);
}

vec3 specularReflection(float intensity, float factor, vec3 lightColor, float hardness, vec3 viewDirection, vec3 reflectionDirection){
	return lightColor * pow(clamp(dot(viewDirection, reflectionDirection), 0.0001f, 1.0f), hardness) * intensity * factor;
}

vec3 specularReflectionBlinn(float intensity, float factor, vec3 lightColor, float hardness, vec3 h, vec3 normal){
	
	return lightColor * pow(clamp(dot(normal, h), 0.0001f, 1.0f), hardness) * intensity * factor;
}

vec3 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(vec3(0,0,-1));
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 h = normalize(viewDir + -lightDir);
    // combine results
    vec3 ambient  = ambientReflection(1.0f, material.ambient, light.ambient);
    vec3 diffuse  = diffuseReflection(1.0f, material.diffuse, light.diffuse, lightDir, normal);
    vec3 specular = specularReflectionBlinn(1.0f, material.specular, light.specular, material.shininess, h, normal);
    return (ambient + diffuse);
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    if(light.ambient == 0 && light.diffuse == 0 && light.specular == 0) return vec3(0,0,0);

    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance    = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + 
  			     light.quadratic * (distance * distance));    
    // combine results
    vec3 ambient  = light.ambient  * material.diffuse;
    vec3 diffuse  = light.diffuse  * diff * material.diffuse;
    vec3 specular = light.specular * spec * material.specular;
    ambient  *= attenuation;
    diffuse  *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}