using System;

namespace OpenGL.Game.GameObjectFactories
{
    public abstract class GameObjectFactory
    {
        public abstract Guid Create(ShaderProgram mat, Texture texture);
    }
}