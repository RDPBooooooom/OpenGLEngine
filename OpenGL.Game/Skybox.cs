using OpenGL.Game.Utils;

namespace OpenGL.Game
{
    /// <summary>
    /// Represents a Skybox. There should only be one Skybox per Scene. This functionality is not contained in this class and needs to be handled by the Scene / Game Class.
    /// </summary>
    public class Skybox
    {
        #region Private Fields

        private readonly Vector3[] _skyboxVertices =
        {
            //   Coordinates
            new Vector3(-1.0f, -1.0f, 1.0f), //     7--------6
            new Vector3(1.0f, -1.0f, 1.0f), //      /|       /|
            new Vector3(1.0f, -1.0f, -1.0f), //    4--------5 |
            new Vector3(-1.0f, -1.0f, -1.0f), //     | |      | |
            new Vector3(-1.0f, 1.0f, 1.0f), //       | 3------|-2
            new Vector3(1.0f, 1.0f, 1.0f), //             |/       |/
            new Vector3(1.0f, 1.0f, -1.0f), //            0--------1
            new Vector3(-1.0f, 1.0f, -1.0f)
        };

        private readonly uint[] _skyboxIndices =
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

        #endregion

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skyboxTexture"> <see cref="CubeMapTexture"/> to render as Skybox</param>
        /// <param name="mat"><see cref="ShaderProgram"/> to render the skybox</param>
        public Skybox(CubeMapTexture skyboxTexture, ShaderProgram mat)
        {
            _skyboxTexture = skyboxTexture;
            _mat = mat;
            _vao = VaoUtil.GetVao(_skyboxVertices, _skyboxIndices, null, mat);
        }

        /// <summary>
        /// Renders the Skybox and set's all the needed Parameters for the Shader.
        /// </summary>
        /// <param name="view">View Matrix of the current <see cref="Camera"/></param>
        /// <param name="projection">Current Projection Matrix</param>
        public void Render(Matrix4 view, Matrix4 projection)
        {
            Gl.DepthFunc(DepthFunction.Lequal);
            _vao.Program.Use();
            Gl.ActiveTexture(0);
            Gl.BindTexture(_skyboxTexture.TextureTarget, _skyboxTexture.TextureID);
            _mat["skybox"]?.SetValue(0);

            _mat["projection"].SetValue(projection);
            _mat["view"].SetValue(view);

            _vao.Draw();
            Gl.DepthFunc(DepthFunction.Less);
        }
    }
}