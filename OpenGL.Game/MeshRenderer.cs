using System;
using OpenGL;

namespace OpenGL.Game
{
    public class MeshRenderer
    {
        #region Properties

        public ShaderProgram Material { get; set; }

        public Texture Texture { get; set; }

        public VAO Geometry { get; set; }

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

        public void Render(Matrix4 model, Matrix4 view, Matrix4 projection)
        {
            Geometry.Program.Use();
            Material["projection"].SetValue(projection);
            Material["view"].SetValue(view);
            Material["model"].SetValue(model);
            Geometry.Draw();
        }

        #endregion
    }
}