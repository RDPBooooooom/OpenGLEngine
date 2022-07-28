namespace OpenGL.Game
{
    public class DirectionalLight
    {
        public Vector3 Direction { get; set; }
        public Vector3 AmbientColor { get; set; }
        public Vector3 DiffuseColor { get; set; }
        public Vector3 SpecularColor { get; set; }

        public DirectionalLight(Vector3 direction, Vector3 ambientColor, Vector3 diffuseColor, Vector3 specularColor)
        {
            Direction = direction;
            AmbientColor = ambientColor;
            DiffuseColor = diffuseColor;
            SpecularColor = specularColor;
        }
    }
}