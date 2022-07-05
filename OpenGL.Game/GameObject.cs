using System;
using System.Collections.Generic;
using OpenGL;

namespace OpenGL.Game
{
    public class GameObject
    {
        #region Properties

        public string Name { get; set; }
        public MeshRenderer Renderer { get; set; }
        public Transform Transform { get; set; }

        #endregion

        #region Constructor

        public GameObject(string name, MeshRenderer renderer)
        {
            Name = name;
            Renderer = renderer;

            Initialize();
        }

        protected GameObject(string name)
        {
            Name = name;
            Initialize();
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            if (Transform != null) Transform = new Transform();
        }

        public virtual void Update()
        {
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            Renderer.Render(Transform.GetTrs(), view, projection );
        }

        #endregion

        #region Protected Methods

        protected static VAO GetVao(Vector3[] vertices, uint[] indices, Vector3[] colors, ShaderProgram mat)
        {
            return GetVao(vertices, indices, colors, null, mat);
        }

        protected static VAO GetVao(Vector3[] vertices, uint[] indices, Vector3[] colors, Vector2[] uv, ShaderProgram mat)

        {
            List<IGenericVBO> vbos = new List<IGenericVBO>
            {
                new GenericVAO.GenericVBO<Vector3>(new VBO<Vector3>(vertices), "in_position"),
                new GenericVAO.GenericVBO<Vector3>(new VBO<Vector3>(colors), "in_color"),
                new GenericVAO.GenericVBO<uint>(new VBO<uint>(indices,
                    BufferTarget.ElementArrayBuffer,
                    BufferUsageHint.StaticRead))
            };

            if (uv != null && mat["uv"] != null)
                vbos.Add(new GenericVAO.GenericVBO<Vector2>(new VBO<Vector2>(uv), "uv"));

            return new VAO(mat, vbos.ToArray());
        }

        #endregion
    }
}