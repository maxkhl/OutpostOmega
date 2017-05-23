#version 120
				
// in eye-space/camera space
varying vec3 vertexNormal;
varying vec3 n;  // vertex normal
varying vec3 VV; // vertex position
varying vec3 lightPosition;
varying vec3 eyeVec;

uniform vec3 tangent;
uniform vec3 bitangent;

varying vec3 vertexPosition_objectspace;

void main()
{
	gl_TexCoord[0] =  gl_MultiTexCoord0;  // output base UV coordinates
	gl_TexCoord[1] =  gl_MultiTexCoord1;  // output base UV coordinates
	gl_TexCoord[2] =  gl_MultiTexCoord2;  // output base UV coordinates
	gl_TexCoord[3] =  gl_MultiTexCoord3;  // output base UV coordinates
	gl_TexCoord[4] =  gl_MultiTexCoord4;  // output base UV coordinates
	gl_TexCoord[5] =  gl_MultiTexCoord5;  // output base UV coordinates

    vertexPosition_objectspace = gl_Vertex.xyz;

	// transform into eye-space
	vertexNormal = n = normalize (gl_NormalMatrix * gl_Normal);
	vec4 vertexPosition = gl_ModelViewMatrix * gl_Vertex;
	VV = vec3(vertexPosition);
	lightPosition = (gl_LightSource[0].position - vertexPosition).xyz;
	eyeVec = -normalize(vertexPosition).xyz;

	gl_Position = ftransform();
}	