using System;
using System.Collections.Generic;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.Shapes;
using OpenGL.Game.Utils;

namespace OpenGL.Game.GameObjectFactories
{
    public class CubeGameObjectFactory : GameObjectFactory
    {
        public override Guid Create(ShaderProgram mat, Texture texture)
        {
            Guid id = Guid.NewGuid();

            VAO vao = VaoUtil.GetVao(Shape.VerticesTextureCube, Shape.IndicesTextureCube, Shape.VerticesNormalsTextureCube, Shape.ColorsTextureCube,
                Shape.UvTextureCube, mat);
            
            Game.Instance.AddComponent(new TransformComponent(id));
            Game.Instance.AddComponent(new RenderComponent(id, new List<MeshRenderer>(){ new MeshRenderer(mat, texture, vao)}));

            return id;
        }
    }
}