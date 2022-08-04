using System;
using System.Collections.Generic;
using System.Linq;
using OpenGL.Game.Math;
using OpenGL.Platform;

namespace OpenGL.Game.PhysicsEngine
{
	public class PhysicsWorld
	{
		public static PhysicsWorld Instance
		{
			get
			{
				if (instance == null) instance = new PhysicsWorld();
				return	instance;
			}
			private set => instance = value;
		}

		private float _timeScale = 1;

		public float TimeScale { get => _timeScale; set => _timeScale = value; }

		public Vector3 Gravity { get; set; }

		// List of all non static Physic Objects
		private List<PhysicsObject> _dynObjects;
		public List<PhysicsObject> DynObjects { get => _dynObjects; private set => _dynObjects = value; }

		// List of all static Physic Objects
		private List<PhysicsObject> _staticObjects;
		private static PhysicsWorld instance;
		public List<PhysicsObject> StaticObjects { get => _staticObjects; private set => _staticObjects = value; }

		public List<PhysicsObject> PhysicsObjects
		{
			get
			{
				List<PhysicsObject> tempList = new List<PhysicsObject>();
				tempList.AddRange(DynObjects);
				tempList.AddRange(StaticObjects);
				return tempList;
			}
		}
		
		public event EventHandler ObjectAdded;

		public event EventHandler ObjectRemoved;

		protected PhysicsWorld()
		{
			_dynObjects = new List<PhysicsObject>();
			_staticObjects = new List<PhysicsObject>();
		}
		
		/// <summary>
		/// Adds Object to physics world
		/// </summary>
		/// <param name="toAdd"></param>
		public void AddObject(PhysicsObject toAdd)
		{
			if (toAdd.IsStatic)
			{
				_staticObjects.Add(toAdd);
			}
			else
			{
				_dynObjects.Add(toAdd);
			}

			EventHandler handler = ObjectAdded;
			handler?.Invoke(toAdd, EventArgs.Empty);
		}

		/// <summary>
		///  Removes Object from physics world
		/// </summary>
		/// <param name="toRemove"></param>
		public void RemoveObject(PhysicsObject toRemove)
		{
			if (toRemove == null) return;
			if (!PhysicsObjects.Contains(toRemove)) return;

			if (toRemove.IsStatic)
			{
				_staticObjects.Remove(toRemove);
			}
			else
			{
				_dynObjects.Remove(toRemove);
			}

			EventHandler handler = ObjectRemoved;
			handler?.Invoke(toRemove, EventArgs.Empty);
		}

		public void Update()
		{
			Time.TimeScale = _timeScale;
		}

		public void FixedUpdate()
		{
			Step();
			CollisionHandling();
		}

		private void Step()
		{
			float dt = Time.DeltaTime;

			foreach (PhysicsObject phyObject in _dynObjects)
			{
				phyObject.Force += phyObject.Mass * Gravity;

				//Calculate Velocity with force and mass
				phyObject.Velocity += phyObject.Force / phyObject.Mass * dt;

				// Update Position with Velocity
				phyObject.Position += phyObject.Velocity * dt;

				//StepRotation(phyObject);

				phyObject.Force = Vector3.Zero;
			}
		}

		private void StepRotation(PhysicsObject phyObject)
		{
			//modifying the Vector3, based on input multiplied by speed and time
			phyObject.CurrentRotation += phyObject.RotationSpeed * Time.DeltaTime;
			
			//apply the Quaternion.eulerAngles change to the gameObject
			phyObject.Transform.Rotation = RotationUtils.ToQ(phyObject.CurrentRotation);
		}

		private void CollisionHandling()
		{
			List<PhysicsObject> objectsWithCollider = PhysicsObjects.Where(x => x.ColliderComponent != null).ToList();

			for (int i = 0; i < objectsWithCollider.Count; i++)
			{
				PhysicsObject obj1 = objectsWithCollider[i];
				for (int j = i + 1; j < objectsWithCollider.Count; j++)
				{
					PhysicsObject obj2 = objectsWithCollider[j];
					if (!(obj1.IsStatic && obj2.IsStatic)){
						obj1.ColliderComponent.CheckCollision(obj2.ColliderComponent);
					}
				}
			}
		}
	}
}