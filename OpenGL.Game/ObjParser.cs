using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace OpenGL.Game
{
    public static class ObjParser
    {
        public static GameObject ParseToGameObject(string filepath)
        {
            if(!filepath.EndsWith(".obj")) throw new IOException("File doesn't have a .obj extension");
            
            string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            filepath = Path.Combine(currentPath, filepath);

            string name;
            
            foreach (string line in File.ReadLines(filepath))
            {
                switch (line[0])
                {
                    case 'o':
                        name = line.Substring(2);
                        break;
                }
                
            }

            Vector3[] vertices;
            Vector3[] indices;

            return null;
        }
    }
}