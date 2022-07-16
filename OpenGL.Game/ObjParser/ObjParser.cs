using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenGL.Game.ObjParser
{
    public class ObjParser
    {
        private readonly List<ObjObject> _objList = new List<ObjObject>();
        private ObjObject _current;

        private readonly Dictionary<string, ObjMaterial> _materialLookup = new Dictionary<string, ObjMaterial>();
        

        private void PreParseSetup()
        {
            _objList.Clear();
            _materialLookup.Clear();

            _current = null;
        }

        public GameObject ParseToGameObject(string filepath, string filename, ShaderProgram mat)
        {
            if (!filename.EndsWith(".obj")) throw new IOException("File doesn't have a .obj extension");

            PreParseSetup();
            
            GameObject toReturn;
           try
           {
                IEnumerable<string> lines = GetLines(filepath + filename);

                int lineNumber = LoadData(lines, filepath);
                ProcessData(lines, lineNumber);

                Console.WriteLine("Creating Gameobject");

                toReturn = GameObject.Create(_current, mat);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return toReturn;
        }

        #region Load Methods

        private int LoadData(IEnumerable<string> lines, string filepath)
        {
            int lineNumber = 0;

            foreach (string line in lines)
            {
                Console.WriteLine("Current Line: " + line);
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
                    case "#":
                        break;
                    default:
                        Console.WriteLine("Unknown line in Processing: " + line);
                        break;
                }
            }
            _current.AddSubMesh(subMeshData);
        }
        
        private void HandleUseMaterial(string[] lineParts, ref SubMeshObjData currentSubMeshData)
        {
            if(currentSubMeshData != null) _current.AddSubMesh(currentSubMeshData);

            currentSubMeshData = new SubMeshObjData
            {
                Material = _materialLookup[lineParts[1]]
            };

            Console.WriteLine("Using Material: " + currentSubMeshData.Material.Name);
        }


        private void HandleFace(string[] lineParts, SubMeshObjData currentSubMeshData)
        {
            uint[] tempIndices = new uint[4];
            int[] tempVertexNormals = new int[4];
            int[] tempUvs = new int[4];

            for (int i = 1; i < lineParts.Length; i++)
            {
                string[] data =  lineParts[i].Split('/');
                
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

            currentSubMeshData.AddData(tempIndices, tempUvs, tempVertexNormals, _current.Data);
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
    }
}