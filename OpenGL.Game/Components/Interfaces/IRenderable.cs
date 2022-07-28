using OpenGL.Game.Components.BasicComponents;

namespace OpenGL.Game.Components.Interfaces
{
    public interface IRenderable
    {
        TransformComponent Transform { get;}
        
        void Render(Matrix4 view, Matrix4 projection);

        void SetDirectionalLight(DirectionalLight dirLight);

        void SetPointLights(PointLightComponent[] pointLights);
    }
}