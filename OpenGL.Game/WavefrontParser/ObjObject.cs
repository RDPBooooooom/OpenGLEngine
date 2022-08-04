using System.Collections.Generic;

namespace OpenGL.Game.WavefrontParser
{
    /// <summary>
    /// Instance of a objObject parsed by the <see cref="ObjParser"/>. If <see cref="Data"/> is empty it is the container object for the data in the submeshes. Those are contained in <see cref="SubMeshData"/>
    /// </summary>
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

        /// <summary>
        /// Merges Submeshes that have no texture. Therefore not needing a submesh.
        /// </summary>
        public void CheckForMerge()
        {
            ObjData firstNoTexture = null;

            SubMeshData.Reverse();

            for (int i = SubMeshData.Count - 1; i >= 0; i--)
            {
                ObjData current = SubMeshData[i];
                if (current.Material.Texture == null)
                {
                    if (firstNoTexture == null)
                    {
                        firstNoTexture = current;
                        continue;
                    }

                    firstNoTexture.Merge(current);
                    SubMeshData.Remove(current);
                }
            }
        }
    }

    /// <summary>
    /// All data contained in an obj File can be stored.
    /// </summary>
    public class ObjData
    {
        public readonly List<Vector3> Vertices = new List<Vector3>();
        public readonly List<uint> Indices = new List<uint>();
        public readonly List<Vector3> VertexColor = new List<Vector3>();
        public readonly List<Vector3> VertexNormals = new List<Vector3>();
        public readonly List<Vector2> Uvs = new List<Vector2>();
        public ObjMaterial Material;

        public void Merge(ObjData toMerge)
        {
            uint currentVertCount = (uint) Vertices.Count;
            Vertices.AddRange(toMerge.Vertices);
            VertexColor.AddRange(toMerge.VertexColor);
            VertexNormals.AddRange(toMerge.VertexNormals);
            Uvs.AddRange(toMerge.Uvs);

            foreach (uint u in toMerge.Indices)
            {
                Indices.Add(u + currentVertCount);
            }

            // Material is no longer valid => Delete
            // TODO: Should probably be saved in a list. But isn't currently used anyway because there is no texture
            // Change in the future
            Material = null;
        }
    }

    /// <summary>
    /// Mainly handles the translation of indices to the new number of vertices.
    /// </summary>
    public class SubMeshObjData : ObjData
    {
        private readonly Dictionary<uint, uint> _indexLookup = new Dictionary<uint, uint>();

        public void AddData(uint[] tempIndices, int[] tempUvs, int[] tempVertexNormals, ObjData currentData,
            SmoothOptions smoothOption)
        {
            for (int i = 0; i < tempIndices.Length; i++)
            {
                uint u = tempIndices[i];

                _indexLookup.Add(u, (uint) _indexLookup.Count);
                Vertices.Add(currentData.Vertices[(int) u]);
                AddVertexColor();
                AddUv(tempUvs[i], currentData);
                AddVertexNormal(tempVertexNormals[i], currentData);
            }

            uint[] newIndices;

            if (tempIndices.Length <= 3)
            {
                newIndices = new uint[]
                {
                    (uint) (Vertices.Count - 3),
                    (uint) (Vertices.Count - 1),
                    (uint) (Vertices.Count - 2)
                };
            }
            else
            {
                newIndices = new uint[]
                {
                    (uint) (Vertices.Count - 3),
                    (uint) (Vertices.Count - 1),
                    (uint) (Vertices.Count - 2),
                    (uint) (Vertices.Count - 3),
                    (uint) (Vertices.Count - 0),
                    (uint) (Vertices.Count - 1)
                };
            }

            AddIndices(newIndices);
            _indexLookup.Clear();
        }

        private void AddVertexColor()
        {
            VertexColor.Add(Material.Color);
        }

        private void AddIndices(uint[] toAdd)
        {
            // First Triangle of face
            Indices.Add(toAdd[0]);
            Indices.Add(toAdd[2]);
            Indices.Add(toAdd[1]);

            if (toAdd.Length <= 3) return;
            // Second Triangle of face
            Indices.Add(toAdd[0]);
            Indices.Add(toAdd[3]);
            Indices.Add(toAdd[2]);
        }

        private void AddUv(int toAdd, ObjData toAddFrom)
        {
            Vector2 uv = toAddFrom.Uvs[toAdd];
            Uvs.Add(uv);
        }

        private void AddVertexNormal(int toAdd, ObjData toAddFrom)
        {
            Vector3 vertexNormal = toAddFrom.VertexNormals[toAdd];
            VertexNormals.Add(vertexNormal);
        }
    }

    public class ObjMaterial
    {
        public string Name;
        public Vector3 Color;
        public Texture Texture;
    }
}