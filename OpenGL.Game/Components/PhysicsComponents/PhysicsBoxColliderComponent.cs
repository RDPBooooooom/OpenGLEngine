using System;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.PhysicsEngine;
using OpenGL.Game.PhysicsEngine.Collider;

namespace OpenGL.Game.Components.PhysicsComponents
{
	public class PhysicsBoxColliderComponent : PhysicsColliderComponent
	{
		private Vector3 _boxSize;

		public TransformComponent Transform { get; set; }
		
		public float MaxX { get { return Transform.Position.X + _boxSize.X / 2; } }
		public float MinX { get { return Transform.Position.X - _boxSize.X / 2; } }
		public float MaxY { get { return Transform.Position.Y + _boxSize.Y / 2; } }
		public float MinY { get { return Transform.Position.Y - _boxSize.Y / 2; } }
		public float MaxZ { get { return Transform.Position.Z + _boxSize.Z / 2; } }
		public float MinZ { get { return Transform.Position.Z - _boxSize.Z / 2; } }

		public Vector3 BoxSize { get => _boxSize; set => _boxSize = value; }

		public override void Start()
		{
			base.Start();
			Transform = Game.Instance.FindComponent<TransformComponent>(Owner);
		}

		public override void CheckCollision(PhysicsSphereColliderComponent colliderComponent)
		{
			if (PhysicsCollisionChecker.AabbSphereCollision(this, colliderComponent, out PhysicsCollision c))
			{
				OnCollision(c);
			};
		}

		public override void CheckCollision(PhysicsBoxColliderComponent colliderComponent)
		{
			if (PhysicsCollisionChecker.AabbaabbCollision(this, colliderComponent, out PhysicsCollision c))
			{
				OnCollision(c);
			};
		}

		protected override void SolveCollision(object colliededWith, EventArgs args)
		{
			CollidedEventArgs collidedArgs = (CollidedEventArgs)args;
			if (collidedArgs.ShouldSolve)
			{
				PhysicsCollision data = collidedArgs.Collision;
				PhysicsCollisionSolver.SolveBoxCollision(data);
			}
		}

		public PhysicsBoxColliderComponent(Guid owner, Vector3 boxSize) : base(owner)
		{
			_boxSize = boxSize;
		}
	}
}

