using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OpenGL.Game.Components;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.Utils;

namespace OpenGL.Game.WavefrontParser
{
    public class ObjParser
    {
        #region Private Fields

        private readonly List<ObjObject> _objList = new List<ObjObject>();
        private ObjObject _current;

        private readonly Dictionary<string, ObjMaterial> _materialLookup = new Dictionary<string, ObjMaterial>();

        private SmoothOptions _smoothOption = SmoothOptions.SmoothOff;

        #endregion

        /// <summary>
        /// Parses a .obj file to a <see cref="List{T}"/> of <see cref="BaseComponent"/>. They are not added to the current scene yet.
        /// </summary>
        /// <param name="filepath">Path to the file</param>
        /// <param name="filename">Name of the file including the extension</param>
        /// <param name="mat">Shader to use for rendering the object</param>
        /// <param name="objId">Id to identify the object</param>
        /// <returns>List of BaseComponents generated from the file</returns>
        /// <exception cref="IOException">If the file doesn't have a .obj Extension</exception>
        public List<BaseComponent> ParseToGameObject(string filepath, string filename, ShaderProgram mat, Guid objId)
        {
            if (!filename.EndsWith(".obj")) throw new IOException("File doesn't have a .obj extension");

            PreParseSetup();

            List<BaseComponent> baseComponents = new List<BaseComponent>();
            try
            {
                IEnumerable<string> lines = GetLines(filepath + filename);

                int lineNumber = LoadData(lines, filepath);
                ProcessData(lines, lineNumber);

                baseComponents.Add(new RenderComponent(objId, GenerateComponentData(_current, mat)));
                baseComponents.Add(new TransformComponent(objId));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return baseComponents;
        }

        #region Private Methods

        private void PreParseSetup()
        {
            _objList.Clear();
            _materialLookup.Clear();

            _current = null;
        }

        private List<MeshRenderer> GenerateComponentData(ObjObject obj, ShaderProgram mat)
        {
            List<MeshRenderer> renderers = new List<MeshRenderer>();

            obj.CheckForMerge();

            foreach (ObjData objData in obj.SubMeshData)
            {
                VAO vao = VaoUtil.GetVao(objData.Vertices.ToArray(), objData.Indices.ToArray(),
                    objData.VertexNormals.ToArray(), objData.VertexColor.ToArray(),
                    objData.Uvs.ToArray(), mat);

                MeshRenderer r = new MeshRenderer(mat, objData.Material?.Texture, vao);
                renderers.Add(r);
            }

            return renderers;
        }

        #region Load Methods

        private int LoadData(IEnumerable<string> lines, string filepath)
        {
            int lineNumber = 0;

            foreach (string line in lines)
            {
                string[] lineParts = line.Split(' ');

                if (HandleData(lineParts, filepath)) break;
                lineNumber++;
            }

            return lineNumber;
        }

        private bool HandleData(string[] lineParts, string filepath)
        {
            switch (lineParts[0])
            {
                case "#":
                    break;
                case "v":
                    HandleVertex(lineParts);
                    break;
                case "vt":
                    HandleUv(lineParts);
                    break;
                case "vn":
                    HandleVertexNormal(lineParts);
                    break;
                case "mtllib":
                    HandleMaterialFile(filepath, lineParts);
                    break;
                case "f":
                case "usemtl":
                    return true;
                case "o":
                    HandleObject(lineParts[1]);
                    break;
            }

            return false;
        }

        private void HandleVertex(string[] lineParts)
        {
            _current.Data.Vertices.Add(new Vector3(ParseFloat(lineParts[1]), ParseFloat(lineParts[2]),
                ParseFloat(lineParts[3])));
        }

        private void HandleVertexNormal(string[] lineParts)
        {
            _current.Data.VertexNormals.Add(new Vector3(ParseFloat(lineParts[1]), ParseFloat(lineParts[2]),
                ParseFloat(lineParts[3])));
        }

        private void HandleUv(string[] lineParts)
        {
            _current.Data.Uvs.Add(new Vector2(ParseFloat(lineParts[1]), ParseFloat(lineParts[2])));
        }

        private void HandleObject(string objName)
        {
            if (_current != null) _objList.Add(_current);

            _current = new ObjObject(objName);
        }

        #region Material Loading

        private void HandleMaterialFile(string filepath, string[] lineParts)
        {
            ObjMaterial toLoad = null;
            foreach (string line in GetLines(filepath + lineParts[1]))
            {
                string[] matLineParts = line.Split(' ');
                switch (matLineParts[0])
                {
                    case "Kd":
                        Console.WriteLine("KD");
                        HandleColor(toLoad, matLineParts);
                        break;
                    case "map_Kd":
                        Console.WriteLine("KD_MAP");
                        HandleTexture(ref toLoad, matLineParts, filepath);
                        break;
                    case "newmtl":
                        Console.WriteLine("NEW MAT");
                        HandleMaterial(ref toLoad, matLineParts);
                        break;
                    default:
                        Console.WriteLine("Skipped: " + line);
                        break;
                }
            }

            Console.WriteLine("Add Mat if " + toLoad);

            if (toLoad != null) _materialLookup.Add(toLoad.Name, toLoad);

            Console.WriteLine("Finished");
        }

        private void HandleColor(ObjMaterial toLoad, string[] line)
        {
            toLoad.Color = new Vector3(ParseFloat(line[1]), ParseFloat(line[2]),
                ParseFloat(line[3]));

            Console.WriteLine("Loaded color (" + toLoad.Color + ") for " + toLoad.Name);
        }

        private void HandleTexture(ref ObjMaterial toLoad, string[] matLineParts, string pathToTexture)
        {
            toLoad.Texture = new Texture(pathToTexture + matLineParts[1]);
        }

        private void HandleMaterial(ref ObjMaterial toLoad, string[] matLineParts)
        {
            if (toLoad != null) _materialLookup.Add(toLoad.Name, toLoad);

            toLoad = new ObjMaterial
            {
                Name = matLineParts[1]
            };

            Console.WriteLine("Registered Material: " + toLoad.Name);
        }

        #endregion

        #endregion

        #region Process Data

        private void ProcessData(IEnumerable<string> lines, int startLine)
        {
            SubMeshObjData subMeshData = null;
            foreach (string line in lines.Skip(startLine))
            {
                string[] lineParts = line.Split(' ');

                switch (lineParts[0])
                {
                    case "f":
                        HandleFace(lineParts, subMeshData);
                        break;
                    case "usemtl":
                        HandleUseMaterial(lineParts, ref subMeshData);
                        break;
                    case "s":
                        HandleSmooth(lineParts);
                        break;
                    case "#":
                        break;
                    default:
                        Console.WriteLine("Unknown line in Processing: " + line);
                        break;
                }
            }

            _current.AddSubMesh(subMeshData);
        }

        private void HandleSmooth(string[] lineParts)
        {
            switch (lineParts[1])
            {
                case "1":
                    _smoothOption = SmoothOptions.SmoothOne;
                    break;
                case "off":
                    _smoothOption = SmoothOptions.SmoothOff;
                    break;
                default:
                    _smoothOption = SmoothOptions.SmoothOff;
                    break;
            }
        }

        private void HandleUseMaterial(string[] lineParts, ref SubMeshObjData currentSubMeshData)
        {
            if (currentSubMeshData != null) _current.AddSubMesh(currentSubMeshData);

            currentSubMeshData = new SubMeshObjData
            {
                Material = _materialLookup[lineParts[1]]
            };

            Console.WriteLine("Using Material: " + currentSubMeshData.Material.Name);
        }


        private void HandleFace(string[] lineParts, SubMeshObjData currentSubMeshData)
        {
            uint[] tempIndices = new uint[lineParts.Length - 1];
            int[] tempVertexNormals = new int[lineParts.Length - 1];
            int[] tempUvs = new int[lineParts.Length - 1];

            for (int i = 1; i < lineParts.Length; i++)
            {
                string[] data = lineParts[i].Split('/');

                tempIndices[i - 1] = uint.Parse(data[0]) - 1;

                if (data[1] != "")
                {
                    tempUvs[i - 1] = int.Parse(data[1]) - 1;
                }
                else
                {
                    tempUvs = Array.Empty<int>();
                }


                tempVertexNormals[i - 1] = int.Parse(data[2]) - 1;
            }

            currentSubMeshData.AddData(tempIndices, tempUvs, tempVertexNormals, _current.Data, _smoothOption);
        }

        #endregion


        #region Helper Methods

        private IEnumerable<string> GetLines(string path)
        {
            string currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(currentPath ?? throw new InvalidOperationException(), path);

            return File.ReadLines(path);
        }

        private float ParseFloat(string toParse)
        {
            return float.Parse(toParse);
        }

        #endregion

        #endregion
    }

    public enum SmoothOptions
    {
        SmoothOff,
        SmoothOne
    }
}