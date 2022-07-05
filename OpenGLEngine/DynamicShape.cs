using OpenGL;
using OpenGL.Game;
using OpenGL.Game.Shapes;
using OpenGL.Platform;

namespace SAE.OpenGL.P6
{
    public class DynamicShape : GameObject
    {
        private ShaderProgram _mat;

        private static readonly Vector3[] Colors = new Vector3[]
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f)
        };

        public DynamicShape(string name, ShaderProgram mat) : base(name)
        {
            _mat = mat;

            Renderer = new MeshRenderer(mat, GetVao(Shapes.VerticesCube, Shapes.IndicesCube, Shapes.ColorsCube, _mat));

            RegisterShapeSwap();
        }

        public void RegisterShapeSwap()
        {
            Input.Subscribe('1', SwapCube);
            Input.Subscribe('2', SwapTriangle);
            Input.Subscribe('3', SwapRect);
            Input.Subscribe('4', SwapPyramid);
            Input.Subscribe('5', SwapRealPyramid);
        }

        private void SwapCube()
        {
            Renderer.Geometry = GetVao(Shapes.VerticesCube, Shapes.IndicesCube, Shapes.ColorsCube, _mat);
        }

        private void SwapTriangle()
        {
            Renderer.Geometry = GetVao(Shapes.VerticesTriangle, Shapes.IndicesTriangle, Shapes.ColorsTriangle, _mat);
        }

        private void SwapRect()
        {
            Renderer.Geometry = GetVao(Shapes.VerticesRectangle, Shapes.IndicesRectangle, Shapes.ColorsRectangle, _mat);
        }

        private void SwapPyramid()
        {
            Renderer.Geometry = GetVao(Shapes.VerticesPyramid, Shapes.IndicesPyramid, Shapes.ColorsPyramid, _mat);
        }

        private void SwapRealPyramid()
        {
            Renderer.Geometry = GetVao(Shapes.VerticesRealPyramid, Shapes.IndicesRealPyramid, Shapes.ColorsRealPyramid, _mat);
        }

        
    }
}