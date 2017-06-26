#version 120

//varying vec3 lightVec;
//varying vec3 eyeVec;
varying vec2 texCoord;
uniform sampler2D colorMap;
//uniform sampler2D normalMap;
//uniform int TextureIndex;
//uniform float invRadius;
uniform float brightness;

void main (void)
{
	vec4 color = texture2D(colorMap, gl_TexCoord[0].st) * brightness;
    gl_FragColor = vec4(color.x, color.y, color.z, 1);
}