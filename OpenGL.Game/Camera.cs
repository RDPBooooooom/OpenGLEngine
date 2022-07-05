using System;
using System.Diagnostics;
using OpenGL.Platform;

namespace OpenGL.Game
{
    public class Camera
    {
        #region Properties

        public Transform Transform { get; protected set; }
        public float MoveStepDistance { get; set; } = 2;
        public float RotationStepAngle { get; set; } = 60;

        #endregion

        #region Constructor

        public Camera()
        {
            Transform = new Transform();

            InitActions();
        }

        #endregion

        #region Private Methods

        private void Move(Vector3 toMove)
        {
            Transform.Position += toMove;
        }

        private void Rotate(Vector3 rotation)
        {
            Transform.Rotation += rotation;
        }

        private void InitActions()
        {
            CreateRepeatInput('w', MoveForward);
            CreateRepeatInput('s', MoveBack);
            CreateRepeatInput('a', MoveLeft);
            CreateRepeatInput('d', MoveRight);

            CreateRepeatInput('q', RotateLeft);
            CreateRepeatInput('e', RotateRight);

            CreateRepeatInput('y', MoveUp);
            CreateRepeatInput('c', MoveDown);

            Input.MouseMove = new Event(Mouse);
        }

        public static void CreateRepeatInput(char key, Event.RepeatEvent method)
        {
            Event @event = new Event(method);
            Input.Subscribe(key, @event);
        }

        #endregion

        #region Public Methods

        public void Mouse(int lx, int ly, int x, int y)
        {
            Rotate(new Vector3(RotationStepAngle * (ly - y), 0f, 0f) * Time.DeltaTime);
            //Rotate(new Vector3(0f, RotationStepAngle * (lx - x), 0f) * Time.DeltaTime);
        }

        public void MoveForward(float dt)
        {
            Move(Transform.GetForward() * MoveStepDistance * dt);
        }

        public void MoveBack(float dt)
        {
            Move(Transform.GetForward() * -1 * MoveStepDistance * dt);
        }

        public void MoveLeft(float dt)
        {
            Move(Transform.GetRight() * -1 * MoveStepDistance * dt);
        }

        public void MoveRight(float dt)
        {
            Move(Transform.GetRight() * MoveStepDistance * dt);
        }

        public void MoveUp(float dt)
        {
            Move(Transform.GetUp() * MoveStepDistance * dt);
        }

        public void MoveDown(float dt)
        {
            Move(Transform.GetUp() * -1 * MoveStepDistance * dt);
        }

        public void RotateLeft(float dt)
        {
            Rotate(new Vector3(0f, RotationStepAngle, 0f) * dt);
        }

        public void RotateRight(float dt)
        {
            Rotate(new Vector3(0f, -RotationStepAngle, 0f) * dt);
        }

        public Matrix4 GetRts()
        {
            return Transform.GetRts();
        }

        #endregion
    }
}