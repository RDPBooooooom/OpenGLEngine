using System;
using OpenGL.Game.Components.BasicComponents;

namespace OpenGL.Game.GameObjectFactories
{
    public class PointLightFactory : GameObjectFactory
    {
        public LightData Data { get; set; } = new LightData()
        {
            AmbientColor = new Vector3(1, 1, 1),
            AmbientIntensity = 0f,
            DiffuseColor = new Vector3(1,1,1),
            DiffuseIntensity = 0.5f,
            SpecularColor = new Vector3(1,1,1),
            SpecularIntensity = 1,
            ConstantFactor = 0.09f,
            LinearFactor = 0.032f,
        };

        public override Guid Create(ShaderProgram mat, Texture texture)
        {
            Guid id = Guid.NewGuid();

            PointLightComponent light = new PointLightComponent(id)
            {
                LightData = Data
            };

            Game.Instance.AddComponent(new TransformComponent(id));
            Game.Instance.AddComponent(light);

            return id;
        }
    }
}