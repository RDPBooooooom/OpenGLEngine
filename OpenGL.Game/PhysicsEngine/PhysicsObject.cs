using System;
using OpenGL.Game.Components;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.Components.PhysicsComponents;
using OpenGL.Game.Math;
using OpenGL.Game.PhysicsEngine.Collider;

namespace OpenGL.Game.PhysicsEngine
{
	public class PhysicsObject : BaseComponent
	{

		public TransformComponent Transform { get; private set; }
		public Vector3 Position { get => Transform.Position; set => Transform.Position = value; }

		#region Physics relevant
		private Vector3 velocity;
		public Vector3 Velocity { get => velocity; set => velocity = value; }

		private Vector3 force;
		public Vector3 Force { get => force; set => force = value; }

		private float mass;
		public float Mass { get => mass; private set => mass = value; }

		private float bounciness = 1;
		public float Bounciness { get => bounciness; private set => bounciness = value; }

		private bool isStatic;
		public bool IsStatic
		{
			get => isStatic;
			set => isStatic = value;
		}

		private PhysicsColliderComponent _physicsColliderComponent;
		public PhysicsColliderComponent ColliderComponent { get => _physicsColliderComponent; set => _physicsColliderComponent = value; }
		#endregion

		#region Rotation
		private Vector3 _rotationSpeed;
		public Vector3 RotationSpeed { get => _rotationSpeed; set => _rotationSpeed = value; }
	
		public Vector3 CurrentRotation { get => RotationUtils.FromQ(Transform.Rotation); set => Transform.Rotation = RotationUtils.ToQ(value); }

		#endregion
	
		public override void Start()
		{
			PhysicsWorld.Instance.AddObject(this);
			Transform = Game.Instance.FindComponent<TransformComponent>(Owner);
		
			Force = Vector3.Zero;
		}


		public PhysicsObject(Guid owner, float mass) : base(owner)
		{
			Mass = mass;
		}
	
		~PhysicsObject()
		{
			PhysicsWorld.Instance.RemoveObject(this);
		}
	}
}
