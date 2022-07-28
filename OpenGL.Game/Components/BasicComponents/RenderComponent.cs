using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenGL.Game.Components.Interfaces;

namespace OpenGL.Game.Components.BasicComponents
{
    public class RenderComponent : BaseComponent, IRenderable
    {
        private Game _game;
        private Matrix4 _currentModel;
        private DirectionalLight _dirLight;
        private PointLightComponent[] _pointLights;

        public List<MeshRenderer> MeshRenderers { get; protected set; }

        public TransformComponent Transform { get; private set; }

        public RenderComponent(Guid owner, List<MeshRenderer> renderers) : base(owner)
        {
            MeshRenderers = renderers;
        }

        public override void Start()
        {
            _game = Game.Instance;
            Transform = _game.FindComponent<TransformComponent>(Owner);
        }

        public void SetDirectionalLight(DirectionalLight dirLight)
        {
            _dirLight = dirLight;
        }

        public void SetPointLights(PointLightComponent[] pointLights)
        {
            _pointLights = pointLights;
        }

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public void Render(Matrix4 view, Matrix4 projection)
        {
            _currentModel = Transform.GetTrs();
            MeshRenderers.ForEach(r =>
                r.Render(_currentModel, view, projection, _game.CurrentCamera, _dirLight, _pointLights));
        }
    }
}