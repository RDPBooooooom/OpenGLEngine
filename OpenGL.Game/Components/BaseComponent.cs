using System;

namespace OpenGL.Game.Components
{
    public abstract class BaseComponent
    {

        public Guid Owner { get; protected set; }
        
        public abstract void Start();

        protected BaseComponent(Guid owner)
        {
            Owner = owner;
        }
    }
}