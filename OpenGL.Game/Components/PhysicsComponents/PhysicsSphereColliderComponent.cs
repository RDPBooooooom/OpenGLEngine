using System;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.PhysicsEngine;
using OpenGL.Game.PhysicsEngine.Collider;

namespace OpenGL.Game.Components.PhysicsComponents
{
	public class PhysicsSphereColliderComponent : PhysicsColliderComponent
	{
		private float _radius;
		public float Radius { get => _radius; set => _radius = value; }

		public TransformComponent Transform => PhysicsObject.Transform;

		/// <summary>
		/// Check if a Collison between this and the given collider has happened
		/// </summary>
		/// <param name="colliderComponent">Collider to check with</param>
		public override void CheckCollision(PhysicsSphereColliderComponent colliderComponent)
		{
			if (PhysicsCollisionChecker.SphereSphereCollision(this, colliderComponent, out PhysicsCollision c))
			{
				OnCollision(c);
			};
		}

		/// <summary>
		/// Check if a Collison between this and the given collider has happened
		/// </summary>
		/// <param name="colliderComponent">Collider to check with</param>
		public override void CheckCollision(PhysicsBoxColliderComponent colliderComponent)
		{
			if (PhysicsCollisionChecker.AabbSphereCollision(colliderComponent, this, out PhysicsCollision c))
			{
				c.SwapColliders();
				OnCollision(c);
			};
		}

		/// <summary>
		/// Solve a collision between this and the given collider
		/// </summary>
		/// <param name="collidedWith"></param>
		/// <param name="args"></param>
		protected override void SolveCollision(object collidedWith, EventArgs args)
		{
			CollidedEventArgs collidedArgs = (CollidedEventArgs)args;
			if (collidedArgs.ShouldSolve)
			{
				PhysicsCollision data = collidedArgs.Collision;
				PhysicsCollisionSolver.SolveSphereCollision(data);
			}
		}

		public PhysicsSphereColliderComponent(Guid owner, float radius) : base(owner)
		{
			_radius = radius;
		}
	}
}

