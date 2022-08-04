using OpenGL.Game.Components.BasicComponents;

namespace OpenGL.Game
{
    /// <summary>
    /// Takes care of Rendering a Mesh
    /// </summary>
    public class MeshRenderer
    {
        #region Properties

        public ShaderProgram Material { get; private set; }

        public Texture Texture { get; private set; }

        public VAO Geometry { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize a mesh renderer that uses no texture
        /// </summary>
        /// <param name="material"><see cref="ShaderProgram"/> to render the mesh</param>
        /// <param name="geometry"><see cref="VAO"/>Geometry information used to render the mesh</param>
        public MeshRenderer(ShaderProgram material, VAO geometry)
        {
            Material = material;
            Geometry = geometry;
        }

        /// <summary>
        /// Initialize a mesh renderer that uses a texture.
        /// </summary>
        /// <param name="material"><see cref="ShaderProgram"/> to render the mesh</param>
        /// <param name="texture"><see cref="Texture"/> to use</param>
        /// <param name="geometry"><see cref="VAO"/> Geometry information used to render the mesh</param>
        public MeshRenderer(ShaderProgram material, Texture texture, VAO geometry)
        {
            Material = material;
            Texture = texture;
            Geometry = geometry;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Renders the geometry given on the initialization
        /// </summary>
        /// <param name="model">Model <see cref="Matrix4"/> for this Object</param>
        /// <param name="view">View <see cref="Matrix4"/> for the current Camera</param>
        /// <param name="projection">Current Projection <see cref="Matrix4"/></param>
        /// <param name="camera">Camera to base the light calculations on</param>
        /// <param name="dirLight">Directional light to calculate with</param>
        /// <param name="pointLights">Point lights to use for calculation. This should not exceed the maximum the shader is able to handle.</param>
        public void Render(Matrix4 model, Matrix4 view, Matrix4 projection, Camera camera, DirectionalLight dirLight,
            PointLightComponent[] pointLights)
        {
            Geometry.Program.Use();

            if (pointLights != null)
            {
                for (int i = 0; i < pointLights.Length; i++)
                {
                    pointLights[i].AddLightData(Material, i);
                }
            }

            if (Texture != null)
            {
                Gl.ActiveTexture(0);
                Gl.BindTexture(Texture);
                Material["baseColorMap"]?.SetValue(0);
            }

            Material["dirLight.direction"]?.SetValue(dirLight.Direction);
            Material["dirLight.ambient"]?.SetValue(dirLight.AmbientColor);
            Material["dirLight.diffuse"]?.SetValue(dirLight.DiffuseColor);
            Material["dirLight.specular"]?.SetValue(dirLight.SpecularColor);

            Material["material.ambient"]?.SetValue(1f);
            Material["material.diffuse"]?.SetValue(1f);
            Material["material.specular"]?.SetValue(1f);
            Material["material.shininess"]?.SetValue(128f);

            Material["projection"].SetValue(projection);
            Material["view"].SetValue(view);
            Material["model"].SetValue(model);
            Material["tangentToWorld"]?.SetValue(model.Inverse().Transpose());
            Material["viewPos"]?.SetValue(-camera.Transform.Position);
            Geometry.Draw();
        }

        #endregion
    }
}