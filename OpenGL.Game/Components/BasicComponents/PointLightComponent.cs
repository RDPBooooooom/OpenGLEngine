using System;
using System.Diagnostics.CodeAnalysis;

namespace OpenGL.Game.Components.BasicComponents
{
    public class PointLightComponent : BaseComponent
    {

        public TransformComponent Transform { get; set; }

        public LightData LightData { get; set; }

        public PointLightComponent(Guid owner) : base(owner)
        {
        }

        public override void Start()
        {
            Transform = Game.Instance.FindComponent<TransformComponent>(Owner);
        }

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public void AddLightData(ShaderProgram mat, int lightCount)
        {
            mat[$"pointLights[{lightCount}].position"]?.SetValue(Transform.Position);
            mat[$"pointLights[{lightCount}].constant"]?.SetValue(LightData.ConstantFactor);
            mat[$"pointLights[{lightCount}].linear"]?.SetValue(LightData.LinearFactor);
            mat[$"pointLights[{lightCount}].quadratic"]?.SetValue(LightData.QuadraticFactor);
            mat[$"pointLights[{lightCount}].ambient"]?.SetValue(LightData.AmbientColor * LightData.AmbientIntensity);
            mat[$"pointLights[{lightCount}].diffuse"]?.SetValue(LightData.DiffuseColor * LightData.DiffuseIntensity);
            mat[$"pointLights[{lightCount}].specular"]?.SetValue(LightData.SpecularColor * LightData.SpecularIntensity);
        }
    }

    public struct LightData
    {
        public Vector3 AmbientColor { get; set; }
        public float AmbientIntensity { get; set; }

        public Vector3 DiffuseColor { get; set; }
        public float DiffuseIntensity { get; set; }

        public Vector3 SpecularColor { get; set; }
        public float SpecularIntensity { get; set; }

        public float ConstantFactor { get; set; }
        public float LinearFactor { get; set; }
        public float QuadraticFactor { get; set; }
    }
}