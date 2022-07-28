using System.Collections.Generic;
using OpenGL.Game.Components;
using OpenGL.Game.Shapes;
using OpenGL.Game.Utils;

namespace OpenGL.Game
{
    public class Skybox
    {
        Vector3[] skyboxVertices =
        {
            //   Coordinates
            new Vector3(-1.0f, -1.0f, 1.0f), //     7--------6
            new Vector3(1.0f, -1.0f, 1.0f),//      /|       /|
            new Vector3(1.0f, -1.0f, -1.0f), //    4--------5 |
            new Vector3(-1.0f, -1.0f, -1.0f), //     | |      | |
            new Vector3(-1.0f, 1.0f, 1.0f),//       | 3------|-2
            new Vector3(1.0f, 1.0f, 1.0f), //             |/       |/
            new Vector3(1.0f, 1.0f, -1.0f),//            0--------1
            new Vector3(-1.0f, 1.0f, -1.0f)
        };

        uint[] skyboxIndices =
        {
            // Right
            1, 2, 6,
            6, 5, 1,
            // Left
            0, 4, 7,
            7, 3, 0,
            // Top
            4, 5, 6,
            6, 7, 4,
            // Bottom
            0, 3, 2,
            2, 1, 0,
            // Back
            0, 1, 5,
            5, 4, 0,
            // Front
            3, 7, 6,
            6, 2, 3
        };

        private CubeMapTexture _skyboxTexture;
        private VAO _vao;
        private ShaderProgram _mat;

        #region Properties

        public CubeMapTexture SkyboxTexture
        {
            get => _skyboxTexture;
            set
            {
                _skyboxTexture.Dispose();
                _skyboxTexture = value;
            }
        }

        #endregion

        public Skybox(CubeMapTexture skyboxTexture, ShaderProgram mat)
        {
            _skyboxTexture = skyboxTexture;
            _mat = mat;
            _vao = VaoUtil.GetVao(skyboxVertices, skyboxIndices, null, mat);
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            _vao.Program.Use();
            Gl.ActiveTexture(0);
            Gl.BindTexture(_skyboxTexture.TextureTarget, _skyboxTexture.TextureID);
            _mat["skybox"]?.SetValue(0);

            _mat["projection"].SetValue(projection);
            _mat["view"].SetValue(view);

            _vao.Draw();
        }
    }
}