using System.Collections.Generic;

namespace OpenGL.Game.Utils
{
    public class VaoUtil
    {
        public static VAO GetVao(Vector3[] vertices, uint[] indices, Vector3[] colors, ShaderProgram mat)
        {
            return GetVao(vertices, indices, colors, null, mat);
        }

        public static VAO GetVao(Vector3[] vertices, uint[] indices, Vector3[] colors, Vector2[] uv, ShaderProgram mat)
        {
            return GetVao(vertices, indices, null, colors, uv, mat);
        }

        public static VAO GetVao(Vector3[] vertices, uint[] indices, Vector3[] normals, Vector3[] colors, Vector2[] uv, ShaderProgram mat)

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
            if(normals != null && mat["normal"] != null)
                vbos.Add(new GenericVAO.GenericVBO<Vector3>(new VBO<Vector3>(normals), "normal"));

            return new VAO(mat, vbos.ToArray());
        }
    }
}