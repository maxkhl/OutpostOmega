#version 120

varying vec2 texCoord;
uniform sampler2D colorMap;
uniform vec2 uShift;

const int gaussRadius = 11;
const float gaussFilter[gaussRadius] = float[gaussRadius](
	0.0402,0.0623,0.0877,0.1120,0.1297,0.1362,0.1297,0.1120,0.0877,0.0623,0.0402
);

void main (void)
{
	vec2 texCoord = gl_TexCoord[0].xy - float(int(gaussRadius/2)) * uShift;
	vec3 color = vec3(0.0, 0.0, 0.0); 
	for (int i=0; i<gaussRadius; ++i) { 
		color += gaussFilter[i] * texture2D(colorMap, texCoord).xyz;
		texCoord += uShift;
	}

	gl_FragColor = (vec4(color,texture2D(colorMap, texCoord)) * 1) * 0.5;
    //gl_FragColor = texture2D(colorMap, gl_TexCoord[0].st);
}