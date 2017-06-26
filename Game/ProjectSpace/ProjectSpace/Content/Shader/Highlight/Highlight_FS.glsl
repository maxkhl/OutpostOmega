#version 120

uniform vec4 Color;
uniform float Transparency;

void main (void)
{	
	gl_FragColor = Color * Transparency; //0.24;
}