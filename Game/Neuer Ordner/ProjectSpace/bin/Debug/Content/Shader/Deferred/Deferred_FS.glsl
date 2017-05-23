varying vec3 lightVec;
varying vec3 eyeVec;
varying vec2 texCoord;
varying vec4 position;
varying vec3 normals;
uniform sampler2D colorMap;
uniform sampler2D normalMap;
uniform bool useDec1;
uniform sampler2D dec1Map;
uniform bool useDec2;
uniform sampler2D dec2Map;
uniform bool useDec3;
uniform sampler2D dec3Map;
uniform bool useDec4;
uniform sampler2D dec4Map;
uniform bool useDec5;
uniform sampler2D dec5Map;
uniform float invRadius;
uniform int UseAlpha;
uniform float SatMin;
uniform float SatMax;
uniform vec4 def_Color;

void main (void)
{
	
	vec4 color = texture2D(colorMap, gl_TexCoord[0].st);

	if(useDec1)
	{
		vec4 dec1color = texture2D(dec1Map, gl_TexCoord[1].st);
		if(dec1color.a > 0.0)
		{
			color = (color * ((dec1color.a - 1.0) * -1.0)) + (dec1color * dec1color.a);
		}
	}

	if(UseAlpha == 1)
	{
		gl_FragData[0] = vec4(color.x, color.y, color.z, color.a);
	}
	else
	{
		gl_FragData[0] = vec4(clamp(color.x,SatMin,SatMax), clamp(color.y,SatMin,SatMax), clamp(color.z,SatMin,SatMax), 1);
	}

	gl_FragData[1] = vec4(position.xyz, 0);
	gl_FragData[2] = vec4(normals.xyz, 0);
	
}