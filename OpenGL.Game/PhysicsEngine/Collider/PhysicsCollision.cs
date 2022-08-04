using OpenGL.Game.Components.PhysicsComponents;

namespace OpenGL.Game.PhysicsEngine.Collider
{
	public struct PhysicsCollision
	{
		public PhysicsColliderComponent colliderComponentOne;
		public PhysicsColliderComponent colliderComponentTwo;
		public Vector3 CollisionDirection;
		public float BouncinessFactor;
		public float DistanceInObject;
		public float CollisionAngle;

		public bool IsTriggerCollision()
		{
			return colliderComponentOne.IsTrigger || colliderComponentTwo.IsTrigger;
		}

		public void SwapColliders() {
			(colliderComponentOne, colliderComponentTwo) = (colliderComponentTwo, colliderComponentOne);
		}
	}
}

