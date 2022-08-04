
using System;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.Components.PhysicsComponents;
using OpenGL.Game.Math;
using OpenGL.Game.PhysicsEngine.Collider;
using static System.Math;

namespace OpenGL.Game.PhysicsEngine
{
	public static class PhysicsCollisionChecker
	{

		public static bool SphereSphereCollision(PhysicsSphereColliderComponent s1, PhysicsSphereColliderComponent s2, out PhysicsCollision collision)
		{
			float distance = (s1.Transform.Position - s2.Transform.Position).Length();

			float bouncinessFactor = s1.PhysicsObject.Bounciness * s2.PhysicsObject.Bounciness;
			Vector3 normal = (s1.Transform.Position - s2.Transform.Position).Normalize();

			bool collided = distance <= s1.Radius + s2.Radius;

			collision = new PhysicsCollision
			{
				colliderComponentOne = s1,
				colliderComponentTwo = s2,
				CollisionDirection = normal,
				BouncinessFactor = bouncinessFactor,
				DistanceInObject = s1.Radius + s2.Radius - distance,
				CollisionAngle = 180
			};

			return collided;
		}

		public static bool AabbSphereCollision(PhysicsBoxColliderComponent b1, PhysicsSphereColliderComponent s1, out PhysicsCollision collision)
		{

			// Get Box Pos and Rotation (Angle & Axis)
			Vector3 pos = b1.Transform.Position;
			Quaternion q = b1.Transform.Rotation;

			Vector4 axisAngle = q.ToAxisAngle();
			float angle = axisAngle.W;
			Vector3 axis = new Vector3(axisAngle.X, axisAngle.Y, axisAngle.Z);

			// Only when box is rotated, can be skipped otherwise
			// Create Temporary Transform with sphere transform data
			TransformComponent transform = new TransformComponent(Guid.NewGuid())
			{
				Position = s1.Transform.Position,
				Rotation = s1.Transform.Rotation,
				Scale = s1.Transform.Scale
			};

			// Rotate Temporary Transform based on box rotation (Angle & Axis)
			transform.Position = RotationUtils.RotateAroundPoint(transform.Position, pos, Quaternion.FromAngleAxis(-angle, axis));


			// Get new Sphere Position
			float sphereX = transform.Position.X;
			float sphereY = transform.Position.Y;
			float sphereZ = transform.Position.Z;

			float x = Max(b1.MinX, Min(sphereX, b1.MaxX));
			float y = Max(b1.MinY, Min(sphereY, b1.MaxY));
			float z = Max(b1.MinZ, Min(sphereZ, b1.MaxZ));

			// this is the same as isPointInsideSphere
			float distance = (float) Sqrt((x - sphereX) * (x - sphereX) +
			                              (y - sphereY) * (y - sphereY) +
			                              (z - sphereZ) * (z - sphereZ));
		
			float bouncinessFactor = b1.PhysicsObject.Bounciness * s1.PhysicsObject.Bounciness;

			bool collided = distance < s1.Radius;


			Vector3 normal = (new Vector3(x, y, z) - new Vector3(sphereX, sphereY, sphereZ));
			Quaternion rot = Quaternion.FromAngleAxis(angle, axis);
			normal = (rot * normal).Normalize();


			// Fill / Create PhysicsCollision data for the solver
			collision = new PhysicsCollision
			{
				colliderComponentOne = b1,
				colliderComponentTwo = s1,
				CollisionDirection = normal,
				BouncinessFactor = bouncinessFactor,
				DistanceInObject = s1.Radius - distance,
				CollisionAngle = 180
			};
			return collided;
		}

		private static bool IsOnLeft(PhysicsColliderComponent one, PhysicsColliderComponent two)
		{
			float distance = one.PhysicsObject.Position.X - two.PhysicsObject.Position.X;

			return distance < 0;
		}

		public static bool AabbaabbCollision(PhysicsBoxColliderComponent b1, PhysicsBoxColliderComponent b2, out PhysicsCollision collision)
		{

			bool collided = (b1.MinX <= b2.MaxX && b1.MaxX >= b2.MinX) &&
			                (b1.MinY <= b2.MaxY && b1.MaxY >= b2.MinY) &&
			                (b1.MinZ <= b2.MaxZ && b1.MaxZ >= b2.MinZ);

			Vector3 normal = (b1.Transform.Position - b2.Transform.Position).Normalize();
			float bouncinessFactor = b1.PhysicsObject.Bounciness * b2.PhysicsObject.Bounciness;

			collision = new PhysicsCollision
			{
				colliderComponentOne = b1,
				colliderComponentTwo = b2,
				CollisionDirection = normal,
				BouncinessFactor = bouncinessFactor
			};

			return collided;

		}
	}
}
