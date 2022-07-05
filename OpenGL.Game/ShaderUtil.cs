using System;
using System.IO;
using System.Reflection;

namespace OpenGL.Game
{
    public static class ShaderUtil
    {
        public static Shader CreateShader(string path, ShaderType type)
        {
            string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(currentPath, path);
            string shaderContent = File.ReadAllText(path);
            return new Shader(shaderContent, type);
        }
    }
}