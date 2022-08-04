using System;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.Components.PhysicsComponents;
using OpenGL.Game.PhysicsEngine;
using OpenGL.Game.PhysicsEngine.Collider;

namespace OpenGL.Game.GameObjectFactories
{
    public class GroundFactory : CubeGameObjectFactory
    {
        public override Guid Create(ShaderProgram mat, Texture texture)
        {
            Guid id = base.Create(mat, texture);
            
            Game.Instance.AddComponent(new PhysicsBoxColliderComponent(id, new Vector3(5, 1f, 5)));
            PhysicsObject obj = new PhysicsObject(id, 10)
            {
                IsStatic = true
            };
            Game.Instance.AddComponent(obj);
            Game.Instance.FindComponent<TransformComponent>(id).Scale = new Vector3(5, 1, 5);

            return id;
        }
    }
}