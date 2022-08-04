using OpenGL.Game.Components.PhysicsComponents;
using OpenGL.Game.Math;
using OpenGL.Game.PhysicsEngine.Collider;
using OpenGL.Game.Utils;

namespace OpenGL.Game.PhysicsEngine
{
    public static class PhysicsCollisionSolver
    {
        public static void SolveSphereCollision(PhysicsCollision collisionData)
        {
            PhysicsColliderComponent colliderComponent = collisionData.colliderComponentOne;

            if (!colliderComponent.PhysicsObject.IsStatic)
            {
                if (collisionData.colliderComponentTwo.GetType() == typeof(PhysicsSphereColliderComponent))
                {
                    UpdateToPositionAtCollision(colliderComponent, collisionData.CollisionDirection,
                        collisionData.DistanceInObject);

                    colliderComponent.PhysicsObject.Velocity =
                        RotationUtils.FromQNoDeg(Quaternion.FromAngleAxis(collisionData.CollisionAngle,
                            collisionData.CollisionDirection)) * colliderComponent.PhysicsObject.Velocity;
                    colliderComponent.PhysicsObject.Velocity *= -collisionData.BouncinessFactor;

                    UpdateRotation(collisionData);
                }
                else if (collisionData.colliderComponentTwo.GetType() == typeof(PhysicsBoxColliderComponent))
                {
                    if (CheckRolling(collisionData))
                    {
                        UpdateToPositionAtCollision(colliderComponent, collisionData.CollisionDirection,
                            collisionData.DistanceInObject);

                        UpdateRotation(collisionData);

                        Vector3 velocity = new Vector3(colliderComponent.PhysicsObject.Velocity.X,
                            colliderComponent.PhysicsObject.Velocity.Y * -collisionData.BouncinessFactor,
                            colliderComponent.PhysicsObject.Velocity.Z);

                        colliderComponent.PhysicsObject.Velocity = velocity;
                    }
                    else
                    {
                        UpdateToPositionAtCollision(colliderComponent, collisionData.CollisionDirection,
                            collisionData.DistanceInObject);

                        colliderComponent.PhysicsObject.Velocity =
                            RotationUtils.FromQNoDeg(Quaternion.FromAngleAxis(collisionData.CollisionAngle,
                                collisionData.CollisionDirection)) * colliderComponent.PhysicsObject.Velocity;
                        colliderComponent.PhysicsObject.Velocity *= -collisionData.BouncinessFactor;

                        UpdateRotation(collisionData);
                    }
                }
            }
        }

        private static void UpdateToPositionAtCollision(PhysicsColliderComponent colliderComponent,
            Vector3 collisionDirection, float distanceInObject)
        {
            colliderComponent.PhysicsObject.Position += -collisionDirection * distanceInObject;
        }

        private static void UpdateRotation(PhysicsCollision collisionData)
        {
            PhysicsColliderComponent colliderComponent = collisionData.colliderComponentOne;
            Vector3 rotation = new Vector3(colliderComponent.PhysicsObject.Velocity.Z * 100, 0,
                colliderComponent.PhysicsObject.Velocity.X * -100);

            colliderComponent.PhysicsObject.RotationSpeed = rotation;
        }

        private static bool CheckRolling(PhysicsCollision collisionData)
        {
            return false; // Currently not working correctly => Therefor disabled
            return collisionData.colliderComponentTwo.PhysicsObject.IsStatic &&
                   collisionData.colliderComponentOne.PhysicsObject.Velocity.Y < 0.2 &&
                   IsOnTop(collisionData.colliderComponentOne, collisionData.colliderComponentTwo);
        }

        private static bool IsOnTop(PhysicsColliderComponent one, PhysicsColliderComponent two)
        {
            float distance = one.PhysicsObject.Position.Y - two.PhysicsObject.Position.Y;

            return distance > 0;
        }

        public static void SolveBoxCollision(PhysicsCollision collisionData)
        {
        }
    }
}