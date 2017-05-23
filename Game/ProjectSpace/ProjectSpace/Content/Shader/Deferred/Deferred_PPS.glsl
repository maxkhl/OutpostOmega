#version 120

uniform sampler2D Diffuse; 
uniform sampler2D Position;
uniform sampler2D Normals;
uniform vec3 cameraPosition;

struct Light {
    vec3 Position;
    vec4 Color;
};
const int NR_LIGHTS = 1;
uniform Light Lights[NR_LIGHTS];

void main (void)
{
	vec4 image = texture2D( Diffuse, gl_TexCoord[0].xy );
	vec4 position = texture2D( Position, gl_TexCoord[0].xy );
	vec4 normal = texture2D( Normals, gl_TexCoord[0].xy );

    /*vec3 light = vec3(50,100,50);
    vec3 lightDir = light - position.xyz ;
    
    normal = normalize(normal);
    lightDir = normalize(lightDir);
    
    vec3 eyeDir = normalize(cameraPosition-position.xyz);
    vec3 vHalfVector = normalize(lightDir.xyz+eyeDir);
    
    gl_FragColor = max(dot(normal.xyz,lightDir),0) * image + 
                   pow(max(dot(normal.xyz,vHalfVector),0.0), 100) * 1.5;*/

    gl_FragColor = image;
}