using System;
using OpenGL.Game.WavefrontParser;

namespace OpenGL.Game.GameObjectFactories
{
    public class WavefrontFactory : GameObjectFactory
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public ObjParser Parser { get; set; }

        public WavefrontFactory(string filePath, string fileName)
        {
            FileName = fileName;
            FilePath = filePath;

            Parser = new ObjParser();
        }
        
        public override Guid Create(ShaderProgram mat, Texture texture)
        {
            Guid id = Guid.NewGuid();
            Game.Instance.AddComponents(Parser.ParseToGameObject(FilePath, FileName, mat, id));

            return id;
        }

        public void ChangePath(string filePath, string fileName)
        {
            FileName = fileName;
            FilePath = filePath;
        }
    }
}