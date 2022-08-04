using System;
using OpenGL.Game.PhysicsEngine;
using OpenGL.Game.PhysicsEngine.Collider;

namespace OpenGL.Game.Components.PhysicsComponents
{
	public abstract class PhysicsColliderComponent : BaseComponent
	{
		public event EventHandler Collided;

		protected bool isTrigger;
		public bool IsTrigger { get => isTrigger; set => isTrigger = value; }

		protected PhysicsObject physicsObject;
		public PhysicsObject PhysicsObject { get => physicsObject; protected set => physicsObject = value; }

		public override void Start()
		{
			Collided += SolveCollision;

			Game game = Game.Instance;
		
			PhysicsObject = game.FindComponent<PhysicsObject>(Owner);
			PhysicsObject.ColliderComponent = this;
		}

		public abstract void CheckCollision(PhysicsSphereColliderComponent colliderComponent);
		public abstract void CheckCollision(PhysicsBoxColliderComponent colliderComponent);

		public virtual void CheckCollision(PhysicsColliderComponent colliderComponent)
		{
			if (typeof(PhysicsSphereColliderComponent) == colliderComponent.GetType())
			{
				CheckCollision((PhysicsSphereColliderComponent)colliderComponent);
			}
			else if (typeof(PhysicsBoxColliderComponent) == colliderComponent.GetType())
			{
				CheckCollision((PhysicsBoxColliderComponent)colliderComponent);
			}

		}

		protected abstract void SolveCollision(object colliededWith, EventArgs args);

		protected virtual void OnCollision(PhysicsCollision collision)
		{
			collision.colliderComponentTwo.OnCollisionNotify(collision);
			InvokeCollisionEvent(collision);
		}

		/// <summary>
		/// Notifies this controller that a collision has occured. Collision should not be solved with this event call since the collision is
		/// always "solved" by the collider calling this method. 
		/// </summary>
		/// <param name="collision">Collision data</param>
		public void OnCollisionNotify(PhysicsCollision collision)
		{
			CollidedEventArgs args = new CollidedEventArgs();
			args.ShouldSolve = false;
			args.Collision = collision;
			InvokeCollisionEvent(collision.colliderComponentOne, args);
		}

		/// <summary>
		/// Invokes the collision event and generates the event Arguments
		/// </summary>
		/// <param name="collision">Collision data</param>
		protected void InvokeCollisionEvent(PhysicsCollision collision)
		{
			CollidedEventArgs args = new CollidedEventArgs();
			args.ShouldSolve = !collision.IsTriggerCollision();
			args.Collision = collision;
			InvokeCollisionEvent(collision.colliderComponentTwo, args);
		}

		/// <summary>
		/// Invokes the collision event
		/// </summary>
		/// <param name="collidedWith">Object this collider collided with</param>
		/// <param name="args">Event Arguments</param>
		protected void InvokeCollisionEvent(PhysicsColliderComponent collidedWith, EventArgs args)
		{
			EventHandler handler = Collided;
			handler?.Invoke(collidedWith, args);
		}

		protected PhysicsColliderComponent(Guid owner) : base(owner)
		{
		}
	}
	public class CollidedEventArgs : EventArgs
	{
		public bool ShouldSolve { get; set; }
		public PhysicsCollision Collision { get; set; }
	}
}