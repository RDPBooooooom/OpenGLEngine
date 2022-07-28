using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using OpenGL.Game.Components;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.Components.Interfaces;

namespace OpenGL.Game
{
    public class Game
    {
        #region "Singleton"

        private static Game instance;

        public static Game Instance
        {
            get => instance ?? (instance = new Game());
            protected set => instance = value;
        }

        private List<ComponentStartDelegate> StartDelegates;

        #endregion

        private const int MaxPointLights = 6;
        
        #region Properties
        
        public LinkedList<BaseComponent> ComponentList { get; protected set; }
        
        public LinkedList<PointLightComponent> LightList { get; protected set; }
        public LinkedList<IUpdatable> Updatables { get; protected set; }
        public LinkedList<IRenderable> Renderables { get; protected set; }
        public ProjectionDelegate CurrentProjection { get; set; }
        public Camera CurrentCamera { get; set; }

        public Skybox Skybox { get; set; }
        public DirectionalLight CurrentDirLight { get; set; }

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
            StartDelegates = new List<ComponentStartDelegate>();
        }

        #endregion

        private void RemoveComponent(BaseComponent component)
        {
            ComponentList.Remove(component);
            if (component is IRenderable renderable) Renderables.Remove(renderable);
            if (component is IUpdatable updatable) Updatables.Remove(updatable);
            if (component is PointLightComponent plc) LightList.Remove(plc);
        }


        #region Public Methods

        public void AddComponent(BaseComponent c)
        {
            ComponentList.AddLast(c);
            StartDelegates.Add(c.Start);

            if (c is IUpdatable updatable) Updatables.AddLast(updatable);
            if (c is IRenderable renderable) Renderables.AddLast(renderable);
            if (c is PointLightComponent plc) LightList.AddLast(plc);
        }

        public void AddComponents(List<BaseComponent> components)
        {
            components.ForEach(AddComponent);
        }

        public void Remove(Guid id)
        {
            foreach (BaseComponent component in ComponentList.Reverse())
            {
                if (component.Owner.Equals(id)) RemoveComponent(component);
            }
        }

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
        public void Update()
        {
            foreach (IUpdatable component in Updatables)
            {
                component.Update();
            }
        }

        [SuppressMessage("ReSharper.DPA", "DPA0004: Closure object allocation")]
        public void Render()
        {
            Matrix4 view = CurrentCamera.Transform.GetRts();
            Matrix4 projection = CurrentProjection.Invoke();

            foreach (IRenderable component in Renderables)
            {
                component.SetDirectionalLight(CurrentDirLight);
                component.SetPointLights(GetClosestPointLights(component.Transform));
                component.Render(view, projection);
            }

            Gl.DepthFunc(DepthFunction.Lequal);
            Skybox.Render(view, projection);
            Gl.DepthFunc(DepthFunction.Less);
        }

        private PointLightComponent[] GetClosestPointLights(TransformComponent transform)
        {
            Dictionary<PointLightComponent, float> closest = new Dictionary<PointLightComponent, float>();

            foreach (PointLightComponent component in LightList)
            {
                float distance = (component.Transform.Position - transform.Position).Length();
                
                closest.Add(component, distance);
            }

            List<KeyValuePair<PointLightComponent, float>> list = closest.ToList();

            list.Sort((pair1,pair2) => pair1.Value.CompareTo(pair2.Value));

            return list.GetRange(0, System.Math.Min(list.Count, MaxPointLights)).Select(kvp => kvp.Key).ToArray();
        }

        public void Start()
        {
            foreach (ComponentStartDelegate componentStartDelegate in StartDelegates)
            {
                componentStartDelegate.Invoke();
            }

            StartDelegates.Clear();
        }

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