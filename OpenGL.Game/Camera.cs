using System;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Platform;

namespace OpenGL.Game
{
    /// <summary>
    /// A Basics camera with basic movement. Uses the Keys (w, a, s, d, y, c) and the mouse for movement and viewing direction. Can be turned of by using the optional Parameter in the constructor
    /// </summary>
    public class Camera
    {
        #region Fields

        private float _angleH;
        private float _angleV;

        #endregion

        #region Properties

        /// <summary>
        /// Current Position and Rotation of the camera
        /// </summary>
        public TransformComponent Transform { get; protected set; }

        /// <summary>
        /// Speed factor used for movement. 
        /// </summary>
        public float MoveStepDistance { get; set; } = 2;

        /// <summary>
        /// Speed factor us for camera rotation
        /// </summary>
        public float RotationStepAngle { get; set; } = 0.1f;

        /// <summary>
        /// Speed factor used for the autoscroll feature
        /// </summary>
        public float AutoScrollStepAngle { get; set; } = 1f;

        /// <summary>
        /// Current ScreenWidth. Needs to be updated when changed. 
        /// </summary>
        public float ScreenWidth { get; set; }
        /// <summary>
        /// Curren ScreenHeight. Needs to be updated when changed. 
        /// </summary>
        public float ScreenHeight { get; set; }

        /// <summary>
        /// Pixel Margin to activate the autoscroll feature
        /// </summary>
        public int PixelMargin { get; set; } = 100;

        #endregion

        #region Constructor

        public Camera(bool initActions = true)
        {
            Transform = new TransformComponent(new Guid());

            if (initActions) InitActions();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Moves the transform
        /// </summary>
        /// <param name="toMove">Vector to add to Positio</param>
        private void Move(Vector3 toMove)
        {
            Transform.Position += toMove;
        }

        /// <summary>
        /// Rotation to the Horizontal axis
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        private void RotateHorizontal(float angle)
        {
            _angleH += angle;
        }

        /// <summary>
        /// Rotation on the vertical axis
        /// </summary>
        /// <param name="angle">Angle to rotate</param>
        private void RotateVertical(float angle)
        {
            _angleV += angle;
        }

        /// <summary>
        /// Calculates the percentage of distance traveled
        /// </summary>
        /// <param name="distance">traveled distance</param>
        /// <param name="totalDistance">total distance to travel</param>
        /// <returns>Percent of travelled distance</returns>
        private float GetPercentRotation(float distance, float totalDistance)
        {
            return totalDistance / 100 * distance;
        }

        /// <summary>
        /// Inits the actions
        /// </summary>
        private void InitActions()
        {
            CreateRepeatInput('w', MoveForward);
            CreateRepeatInput('s', MoveBack);
            CreateRepeatInput('a', MoveLeft);
            CreateRepeatInput('d', MoveRight);

            CreateRepeatInput('y', MoveUp);
            CreateRepeatInput('c', MoveDown);

            Input.MouseMove = new Event(Mouse);
        }

        private static void CreateRepeatInput(char key, Event.RepeatEvent method)
        {
            Event @event = new Event(method);
            Input.Subscribe(key, @event);
        }

        private void CheckBorder(int x, int y)
        {
            if (x < 0 + PixelMargin)
            {
                _angleV += AutoScrollStepAngle * Time.DeltaTime;
            }
            else if (x > ScreenWidth - PixelMargin)
            {
                _angleV += -AutoScrollStepAngle * Time.DeltaTime;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Callback on mouse movement
        /// </summary>
        /// <param name="lx">Previouse mouse position on x axis</param>
        /// <param name="ly">Previouse mouse position on y axis</param>
        /// <param name="x">Current mouse position on x axis</param>
        /// <param name="y">Current mouse position on y axis</param>
        public void Mouse(int lx, int ly, int x, int y)
        {
            RotateHorizontal(RotationStepAngle * GetPercentRotation(ly - y, ScreenHeight) * Time.DeltaTime);
            RotateVertical(RotationStepAngle * GetPercentRotation(lx - x, ScreenWidth) * Time.DeltaTime);
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

        /// <summary>
        /// Update the camera Rotation
        /// </summary>
        public void Update()
        {
            CheckBorder(Input.MousePosition.X, Input.MousePosition.Y);
            Vector3 yAxis = Vector3.UnitY;

            // Rotate the view vector by the horizontal angle around the vertical axis
            Vector3 view = new Vector3(0.0f, 0.0f, -1.0f);
            view = Quaternion.FromAngleAxis(_angleV, yAxis) * view;
            view.Normalize();

            // Rotate the view vector by the vertical angle around the horizontal axis
            Vector3 u = yAxis.Cross(view);
            u.Normalize();

            Transform.Rotation = Quaternion.FromAngleAxis(-_angleH, u) * Quaternion.FromAngleAxis(_angleV, yAxis);
        }

        /// <summary>
        /// Get RTS <see cref="Matrix4"/> of the <see cref="Transform"/>
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetRts()
        {
            return Transform.GetRts();
        }

        #endregion
    }
}