varying vec3 lightVec;
varying vec3 eyeVec;
varying vec2 texCoord;
uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform float invRadius;

void main (void)
{
					 
    gl_FragColor = texture2D(colorMap, gl_TexCoord[0].st);
}