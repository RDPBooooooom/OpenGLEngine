using System.Collections.Generic;

namespace OpenGL.Game.WavefrontParser
{
    public class ObjObject
    {
        public string Name { get; private set; }

        // Contains all the data (SubMeshes included)
        public ObjData Data { get; set; }
        public List<ObjData> SubMeshData { get; protected set; }

        public ObjObject(string name)
        {
            SubMeshData = new List<ObjData>();
            Data = new ObjData();
            Name = name;
        }

        public void AddSubMesh(ObjData submesh)
        {
            SubMeshData.Add(submesh);
        }
    }

    public class ObjData
    {
        public readonly List<Vector3> Vertices = new List<Vector3>();
        public readonly List<uint> Indices = new List<uint>();
        public readonly List<Vector3> VertexNormals = new List<Vector3>();
        public readonly List<Vector2> Uvs = new List<Vector2>();
        public ObjMaterial Material;
    }

    public class SubMeshObjData : ObjData
    {
        private readonly Dictionary<uint, uint> _indexLookup = new Dictionary<uint, uint>();
        private readonly Dictionary<int, Vector2> _uvLookup = new Dictionary<int, Vector2>();
        private readonly Dictionary<int, Vector3> _vertexNormalsLookup = new Dictionary<int, Vector3>();

        public void AddData(uint[] tempIndices, int[] tempUvs, int[] tempVertexNormals, ObjData currentData)
        {
            AddIndices(tempIndices, currentData);
            AddUvs(tempUvs, currentData);
            AddVertexNormal(tempVertexNormals, currentData);
        }

        private void AddIndices(uint[] toAdd, ObjData toAddFrom)
        {
            foreach (uint u in toAdd)
            {
                if (!_indexLookup.ContainsKey(u))
                {
                    _indexLookup.Add(u, (uint)_indexLookup.Count);
                    Vertices.Add(toAddFrom.Vertices[(int)u]);
                }
            }
            
            // First Triangle of face
            Indices.Add(_indexLookup[toAdd[0]]);
            Indices.Add(_indexLookup[toAdd[2]]);
            Indices.Add(_indexLookup[toAdd[1]]);
            // Second Triangle of face
            Indices.Add(_indexLookup[toAdd[0]]);
            Indices.Add(_indexLookup[toAdd[3]]);
            Indices.Add(_indexLookup[toAdd[2]]);
        }
        
        private void AddUvs(int[] toAdd, ObjData toAddFrom)
        {
            foreach (int u in toAdd)
            {
                if (!_uvLookup.ContainsKey(u))
                {
                    _uvLookup.Add(u, toAddFrom.Uvs[u]);
                }
                
                Uvs.Add(_uvLookup[u]);
            }
        }
        
        private void AddVertexNormal(int[] toAdd, ObjData toAddFrom)
        {
            foreach (int u in toAdd)
            {
                if (!_vertexNormalsLookup.ContainsKey(u))
                {
                    _vertexNormalsLookup.Add(u, toAddFrom.VertexNormals[u]);
                }
                
                VertexNormals.Add(_vertexNormalsLookup[u]);
            }
        }
    }

    public class ObjMaterial
    {
        public string Name;
        public Vector3 Color;
        public Texture Texture;
    }
}