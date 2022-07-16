using System;
using System.Collections.Generic;
using OpenGL;
using OpenGL.Game.ObjParser;

namespace OpenGL.Game
{
    public class GameObject
    {
        #region Properties

        public string Name { get; set; }

        public List<MeshRenderer> MeshRenderers { get; protected set; }
        public Transform Transform { get; set; }

        #endregion

        #region Constructor

        public GameObject(string name, MeshRenderer renderer) : this(name, new List<MeshRenderer>(){renderer})
        {
            
        }

        public GameObject(string name, List<MeshRenderer> renderers)
        {
            Name = name;
            MeshRenderers = renderers;

            Initialize();
        }

        public GameObject(string name, ShaderProgram material, VAO geometry, Texture texture) : this(name, new MeshRenderer(material, texture, geometry))
        {
        }

        public GameObject(string name, ShaderProgram material, VAO geometry) : this(name, material, geometry, null)
        {
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            if (Transform == null) Transform = new Transform();
        }

        public virtual void Update()
        {
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            MeshRenderers.ForEach(r => r.Render(Transform.GetTrs(), view, projection ));
        }

        #endregion

        #region Static Methods

        public static VAO GetVao(Vector3[] vertices, uint[] indices, Vector3[] colors, ShaderProgram mat)
        {
            return GetVao(vertices, indices, colors, null, mat);
        }

        public static VAO GetVao(Vector3[] vertices, uint[] indices, Vector3[] colors, Vector2[] uv, ShaderProgram mat)

        {
            List<IGenericVBO> vbos = new List<IGenericVBO>
            {
                new GenericVAO.GenericVBO<Vector3>(new VBO<Vector3>(vertices), "in_position"),
                new GenericVAO.GenericVBO<uint>(new VBO<uint>(indices,
                    BufferTarget.ElementArrayBuffer,
                    BufferUsageHint.StaticRead))
            };

            if (uv != null && mat["uv"] != null)
                vbos.Add(new GenericVAO.GenericVBO<Vector2>(new VBO<Vector2>(uv), "uv"));
            if(colors != null && mat["in_color"] != null)
                vbos.Add(new GenericVAO.GenericVBO<Vector3>(new VBO<Vector3>(colors), "in_color"));

            return new VAO(mat, vbos.ToArray());
        }

        public static GameObject Create(ObjObject obj, ShaderProgram mat)
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();

            

            foreach (ObjData objData in obj.SubMeshData)
            {
                Vector3[] colors = new Vector3[objData.Vertices.Count];
                for (int i = 0; i < objData.Vertices.Count; i++)
                {
                    colors[i] = objData.Material.Color;
                }
                
                VAO vao = GetVao(objData.Vertices.ToArray(), objData.Indices.ToArray(), colors,
                    objData.Uvs.ToArray(), mat);

                MeshRenderer r = new MeshRenderer(mat, objData.Material.Texture, vao);
                renderers.Add(r);
            }

            return new GameObject(obj.Name, renderers);
        }

        #endregion
    }
}