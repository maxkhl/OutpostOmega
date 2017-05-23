using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OutpostOmega.Drawing
{
    class Shader : IDisposable
    {

        const int AttribTangent = 5; // slot where to pass tangents to VS, not sure which are reserved besides 0

        public int VertexShaderHandle;
        public int FragmentShaderHandle;
        public int ProgramHandle;

        public FileInfo VertexFile { get; private set; }
        public FileInfo FragmentFile { get; private set; }

        protected Shader(FileInfo VertexFile, FileInfo FragmentFile)
        {
            this.VertexFile = VertexFile;
            this.FragmentFile = FragmentFile;
            Load(VertexFile.FullName, FragmentFile.FullName);
        }

        private void Load(string VertexFilePath, string FragmentFilePath)
        {
            string LogInfo;

            // Load&Compile Vertex Shader
            this.ProgramHandle = GL.CreateProgram();

            using (StreamReader sr = new StreamReader(VertexFilePath))
            {
                VertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(VertexShaderHandle, sr.ReadToEnd());
                GL.CompileShader(VertexShaderHandle);
            }

            GL.GetShaderInfoLog(VertexShaderHandle, out LogInfo);
            if (LogInfo.Length > 0 && !LogInfo.Contains("hardware"))
                throw new Exception("Vertex Shader failed!\nLog:\n" + LogInfo);

            // Load&Compile Fragment Shader

            using (StreamReader sr = new StreamReader(FragmentFilePath))
            {
                FragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(FragmentShaderHandle, sr.ReadToEnd());
                GL.CompileShader(FragmentShaderHandle);
            }
            GL.GetShaderInfoLog(FragmentShaderHandle, out LogInfo);

            if (LogInfo.Length > 0 && !LogInfo.Contains("hardware"))
                throw new Exception("Fragment Shader failed!\nLog:\n" + LogInfo);

            // Link the Shaders to a usable Program
            ProgramHandle = GL.CreateProgram();
            GL.AttachShader(ProgramHandle, VertexShaderHandle);
            GL.AttachShader(ProgramHandle, FragmentShaderHandle);

            // must bind the attribute before linking
            GL.BindAttribLocation(ProgramHandle, AttribTangent, "AttributeTangent");

            // link it all together
            GL.LinkProgram(ProgramHandle);

            // flag ShaderObjects for delete when not used anymore
            //GL.DeleteShader(VertexShaderHandle);
            //GL.DeleteShader(FragmentShaderHandle);

            //GL.GetProgram(ProgramObject, GetProgramParameterName.LinkStatus, out temp[0]);
        }

        public void Bind()
        {
            GL.UseProgram(ProgramHandle);
        }

        public void UnBind()
        {
            GL.UseProgram(0);
        }

        public int GetUniformLocation(string Name)
        {
            return GL.GetUniformLocation(this.ProgramHandle, Name);
        }

        public int GetAttribLocation(string Name)
        {
            return GL.GetAttribLocation(this.ProgramHandle, Name);
        }

        public void Dispose()
        {
            if (GL.IsProgram(ProgramHandle))
            {
                GL.DetachShader(ProgramHandle, VertexShaderHandle);
                GL.DetachShader(ProgramHandle, FragmentShaderHandle);
                GL.DeleteProgram(ProgramHandle);
            }

            if (GL.IsShader(VertexShaderHandle))
                GL.DeleteShader(VertexShaderHandle);

            if (GL.IsShader(FragmentShaderHandle))
                GL.DeleteShader(FragmentShaderHandle);

            Tools.OpenGL.CheckError();
        }



        public static List<Shader> LoadedShaders = new List<Shader>();

        public static Shader Load(FileInfo VertexFile, FileInfo FragmentFile)
        {
            var exists = (from shader in LoadedShaders
                          where shader.VertexFile == VertexFile &&
                                shader.FragmentFile == FragmentFile
                          select shader).FirstOrDefault();

            if (exists == null)
                return new Shader(VertexFile, FragmentFile);
            else
                return exists;
        }
    }
}
