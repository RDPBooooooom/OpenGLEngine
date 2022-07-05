using System;
using OpenGL;
using OpenGL.Game;
using OpenGL.Game.Shapes;
using OpenGL.Platform;

namespace SAE.OpenGL.P6
{
    public class Cube : GameObject
    {
        private ShaderProgram _mat;

        private int _uvFactor;
        private float _lastTime;
        private float _timeSinceStart;

        public Cube(string name, ShaderProgram mat) : base(name)
        {
            _uvFactor = 1;

            _mat = mat;

            Renderer = new MeshRenderer(_mat,
                GetVao(Shapes.VerticesTextureCube, Shapes.IndicesTextureCube, Shapes.ColorsTextureCube,
                    Shapes.UvTextureCube, _mat));
        }
        
        public Cube(string name, ShaderProgram mat, Texture texture) : base(name)
        {
            _uvFactor = 1;

            _mat = mat;
            _mat["uv_factor"].SetValue(_uvFactor);

            Renderer = new MeshRenderer(_mat, texture,
                GetVao(Shapes.VerticesTextureCube, Shapes.IndicesTextureCube, Shapes.ColorsTextureCube,
                    Shapes.UvTextureCube, _mat));

            Input.Subscribe('m', UvSizeUp);
            Input.Subscribe('n', UvSizeDown);
        }

        public override void Update()
        {
            _timeSinceStart += Time.DeltaTime;
        }

        private void UvSizeUp()
        {
            // Make sure it doesnt happen too often
            if (0.5 > _timeSinceStart - _lastTime) return;
            _lastTime = _timeSinceStart;

            _mat.Use();
            _uvFactor++;
            _mat["uv_factor"].SetValue(_uvFactor);
        }

        private void UvSizeDown()
        {
            // Make sure it doesnt happen too often
            if (0.5 > _timeSinceStart - _lastTime) return;
            _lastTime = _timeSinceStart;

            _mat.Use();
            _uvFactor--;
            _mat["uv_factor"].SetValue(_uvFactor);
        }
    }
}