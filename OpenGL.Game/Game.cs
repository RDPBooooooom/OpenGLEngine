using System;
using System.Collections.Generic;

namespace OpenGL.Game
{
    public class Game
    {
        #region Properties

        public List<GameObject> SceneGraph { get; set; }

        #endregion

        #region Constructor

        public Game()
        {
            SceneGraph = new List<GameObject>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Renders all <see cref="GameObject"/> in the <see cref="SceneGraph"/>
        /// </summary>
        [Obsolete("Old")]
        public void Render()
        {
            //SceneGraph.ForEach(g => g.Renderer.Render());
        }

        /// <summary>
        /// Updates all <see cref="GameObject"/> in the <see cref="SceneGraph"/>
        /// </summary>
        public void Update()
        {
            SceneGraph.ForEach(g => g.Update());
        }

        #endregion
    }
}