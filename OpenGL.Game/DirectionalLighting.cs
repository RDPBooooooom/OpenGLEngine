namespace OpenGL.Game
{
    /// <summary>
    /// Represents a Directional Light
    /// </summary>
    public class DirectionalLight
    {
        public Vector3 Direction { get; set; }
        /// <summary>
        /// Ambient Color and Intensity
        /// </summary>
        public Vector3 AmbientColor { get; set; }
        /// <summary>
        /// Diffuse Color and Intensity
        /// </summary>
        public Vector3 DiffuseColor { get; set; }
        /// <summary>
        /// Specular Color and Intensity
        /// </summary>
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