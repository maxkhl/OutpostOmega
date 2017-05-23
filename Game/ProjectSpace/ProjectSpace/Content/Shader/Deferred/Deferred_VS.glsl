#version 120
				
varying vec3 normals;
varying vec4 position;

void main()
{
	gl_TexCoord[0] =  gl_MultiTexCoord0;  // output base UV coordinates
	gl_TexCoord[1] =  gl_MultiTexCoord1;  // output base UV coordinates
	gl_TexCoord[2] =  gl_MultiTexCoord2;  // output base UV coordinates
	gl_TexCoord[3] =  gl_MultiTexCoord3;  // output base UV coordinates
	gl_TexCoord[4] =  gl_MultiTexCoord4;  // output base UV coordinates
	gl_TexCoord[5] =  gl_MultiTexCoord5;  // output base UV coordinates

	mat3 worldRotationInverse = transpose(mat3(gl_ModelViewMatrix));

	normals		= normalize(worldRotationInverse * gl_NormalMatrix * gl_Normal);
	//normals = gl_Normal;
	position	= gl_ModelViewProjectionMatrix * gl_Vertex;

	gl_Position	= gl_ModelViewProjectionMatrix * gl_Vertex;

	//gl_Position = ftransform();
}	