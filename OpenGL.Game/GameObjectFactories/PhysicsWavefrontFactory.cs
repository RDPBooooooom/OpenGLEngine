using System;
using OpenGL.Game.Components.PhysicsComponents;
using OpenGL.Game.PhysicsEngine;
using OpenGL.Game.PhysicsEngine.Collider;

namespace OpenGL.Game.GameObjectFactories
{
    public class PhysicsWavefrontFactory : WavefrontFactory
    {
        private float _radius;
        private float _mass;

        public PhysicsWavefrontFactory(string filePath, string fileName, float colliderRadius, float mass) : base(filePath, fileName)
        {
            this._radius = colliderRadius;
            _mass = mass;
        }

        public override Guid Create(ShaderProgram mat, Texture texture)
        {
            Guid id = base.Create(mat, texture);
            
            Game.Instance.AddComponent(new PhysicsSphereColliderComponent(id, _radius));
            Game.Instance.AddComponent(new PhysicsObject(id, _mass));

            return id;
        }
    }
}