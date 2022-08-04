using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using OpenGL.Game.Components;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.Components.Interfaces;
using OpenGL.Game.PhysicsEngine;

namespace OpenGL.Game
{
    /// <summary>
    /// Game class represents the Scene and holds all objects that should be rendered. 
    /// Manages the light sources in the scene and returns only the max number of supported point lights to the shader. Currently hardcoded as 6.
    /// This class is a singleton and can be accessed over <see cref="Instance"/>
    /// </summary>
    public class Game
    {
        #region "Singleton"

        private static Game instance;

        /// <summary>
        /// Returns the current game instance. If no instance exists a new one is created.
        /// </summary>
        public static Game Instance
        {
            get => instance ?? (instance = new Game());
            protected set => instance = value;
        }

        private List<ComponentStartDelegate> _startDelegates;

        #endregion

        private const int MaxPointLights = 6;

        #region Properties

        /// <summary>
        /// List of all components that are currently active. Components of objects can be found by using <see cref="FindComponent{T}"/> 
        /// </summary>
        public LinkedList<BaseComponent> ComponentList { get; protected set; }

        /// <summary>
        /// List of all <see cref="PointLightComponent"/> that are currently in the Scene and should be considered for light calculations
        /// </summary>
        public LinkedList<PointLightComponent> LightList { get; protected set; }

        /// <summary>
        /// List of all IUpdatable that should be updated per frame. Usually also a <see cref="BaseComponent"/>
        /// </summary>
        public LinkedList<IUpdatable> Updatables { get; protected set; }

        /// <summary>
        /// List of all objects that should be rendered per frame
        /// </summary>
        public LinkedList<IRenderable> Renderables { get; protected set; }

        /// <summary>
        /// Current Method to get the Projection <see cref="Matrix4"/> from
        /// </summary>
        public ProjectionDelegate CurrentProjection { get; set; }

        /// <summary>
        /// Camera to use as main camera
        /// </summary>
        public Camera CurrentCamera { get; set; }

        /// <summary>
        /// Set and get the current <see cref="Skybox"/> to render
        /// </summary>
        public Skybox Skybox { get; set; }

        /// <summary>
        /// set and get the current <see cref="DirectionalLight"/> to use
        /// </summary>
        public DirectionalLight CurrentDirLight { get; set; }

        /// <summary>
        /// Current physics world that handles all Physics calculations
        /// </summary>
        public PhysicsWorld World { get; set; }

        #endregion

        #region delegate

        public delegate Matrix4 ProjectionDelegate();

        public delegate void ComponentStartDelegate();

        #endregion

        #region Constructor

        protected Game()
        {
            Renderables = new LinkedList<IRenderable>();
            Updatables = new LinkedList<IUpdatable>();
            LightList = new LinkedList<PointLightComponent>();
            ComponentList = new LinkedList<BaseComponent>();
            _startDelegates = new List<ComponentStartDelegate>();
            World = PhysicsWorld.Instance;
            World.Gravity = new Vector3(0, -1, 0);
        }

        #endregion

        /// <summary>
        /// Removes a <see cref="BaseComponent"/> from the scene
        /// </summary>
        /// <param name="component">Component to remove</param>
        private void RemoveComponent(BaseComponent component)
        {
            ComponentList.Remove(component);
            if (component is IRenderable renderable) Renderables.Remove(renderable);
            if (component is IUpdatable updatable) Updatables.Remove(updatable);
            if (component is PointLightComponent plc) LightList.Remove(plc);
        }
        
        /// <summary>
        /// Gets the closest point lights to the transform. Returns at max the amount of <see cref="MaxPointLights"/>
        /// </summary>
        /// <param name="transform">Transform to get the nearest <see cref="PointLightComponent"/> of.</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        private PointLightComponent[] GetClosestPointLights(TransformComponent transform)
        {
            Dictionary<PointLightComponent, float> closest = new Dictionary<PointLightComponent, float>();

            foreach (PointLightComponent component in LightList)
            {
                float distance = (component.Transform.Position - transform.Position).Length();

                closest.Add(component, distance);
            }

            List<KeyValuePair<PointLightComponent, float>> list = closest.ToList();

            list.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

            return list.GetRange(0, System.Math.Min(list.Count, MaxPointLights)).Select(kvp => kvp.Key).ToArray();
        }

        #region Public Methods

        /// <summary>
        /// Adds a <see cref="BaseComponent"/> to the Scene
        /// </summary>
        /// <param name="c">Component to add</param>
        public void AddComponent(BaseComponent c)
        {
            ComponentList.AddLast(c);
            _startDelegates.Add(c.Start);

            if (c is IUpdatable updatable) Updatables.AddLast(updatable);
            if (c is IRenderable renderable) Renderables.AddLast(renderable);
            if (c is PointLightComponent plc) LightList.AddLast(plc);
        }

        /// <summary>
        /// Adds all components in the <see cref="List{T}"/> to the scene
        /// </summary>
        /// <param name="components">List of components to add</param>
        public void AddComponents(List<BaseComponent> components)
        {
            components.ForEach(AddComponent);
        }

        /// <summary>
        /// Removes all <see cref="BaseComponent"/> with the given <see cref="Guid"/> as <see cref="BaseComponent.Owner"/>
        /// </summary>
        /// <param name="id">Id to remove from scene</param>
        public void Remove(Guid id)
        {
            foreach (BaseComponent component in ComponentList.Reverse())
            {
                if (component.Owner.Equals(id)) RemoveComponent(component);
            }
        }

        /// <summary>
        /// Removes a single <see cref="BaseComponent"/> from the scene.
        /// </summary>
        /// <param name="id">Id to remove component from</param>
        /// <typeparam name="T">Type of component to remove</typeparam>
        public void RemoveComponent<T>(Guid id) where T : BaseComponent
        {
            foreach (T component in ComponentList.OfType<T>().Where(c => c.Owner.Equals(id)).Reverse())
            {
                RemoveComponent(component);
            }
        }

        /// <summary>
        /// Updates all <see cref="IUpdatable"/> in the <see cref="Updatables"/>
        /// </summary>
        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public void Update()
        {
            foreach (IUpdatable component in Updatables)
            {
                component.Update();
            }

            World.Update();
            World.FixedUpdate();
            CurrentCamera.Update();
        }

        /// <summary>
        /// Renders all <see cref="IRenderable"/> in <see cref="Renderables"/>. Also renders the <see cref="Skybox"/>
        /// </summary>
        [SuppressMessage("ReSharper.DPA", "DPA0004: Closure object allocation")]
        public void Render()
        {
            Matrix4 view = CurrentCamera.Transform.GetRts();
            Matrix4 projection = CurrentProjection.Invoke();

            Skybox.Render(view, projection);

            foreach (IRenderable component in Renderables)
            {
                component.SetDirectionalLight(CurrentDirLight);
                component.SetPointLights(GetClosestPointLights(component.Transform));
                component.Render(view, projection);
            }
        }

        /// <summary>
        /// Initializes the components that were created last frame
        /// </summary>
        public void Start()
        {
            foreach (ComponentStartDelegate componentStartDelegate in _startDelegates)
            {
                componentStartDelegate.Invoke();
            }

            _startDelegates.Clear();
        }

        /// <summary>
        /// Finds a <see cref="BaseComponent"/> by Type and <see cref="BaseComponent.Owner"/>
        /// </summary>
        /// <param name="id">Id of the owner </param>
        /// <typeparam name="T">Type of component to find</typeparam>
        /// <returns></returns>
        public T FindComponent<T>(Guid id) where T : BaseComponent
        {
            foreach (BaseComponent baseComponent in ComponentList.Where(c => c.GetType() == typeof(T)))
            {
                if (baseComponent.Owner == id) return (T) baseComponent;
            }

            return null;
        }

        #endregion
    }
}