using System;
using OpenGL;
using OpenGL.Game.Components.BasicComponents;

namespace OpenGL.Game
{
    public class MeshRenderer
    {
        #region Properties

        public ShaderProgram Material { get; private set; }

        public Texture Texture { get; private set; }

        public VAO Geometry { get; private set; }

        #endregion

        #region Constructor

        public MeshRenderer(ShaderProgram material, VAO geometry)
        {
            Material = material;
            Geometry = geometry;
        }

        public MeshRenderer(ShaderProgram material, Texture texture, VAO geometry)
        {
            Material = material;
            Texture = texture;
            Geometry = geometry;
        }

        #endregion

        #region Public Methods

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
            Material["viewPos"]?.SetValue(camera.Transform.Position);
            Geometry.Draw();
        }

        #endregion
    }
}