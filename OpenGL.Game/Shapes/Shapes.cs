namespace OpenGL.Game.Shapes
{
    /// <summary>
    /// Holds static Vertices & Indices for some shapes
    /// </summary>
    public static class Shapes
    {
        #region Pyramid

        public static readonly Vector3[] VerticesPyramid =
        {
            new Vector3(-0.1f, -0.5f, 0.0f), // front_corner
            new Vector3(0.5f, 0.0f, 1.0f), // right_corner
            new Vector3(-0.5f, 0.0f, 1.0f), // left_corner
            new Vector3(0.0f, 0.5f, 0.5f) // top
        };

        public static readonly uint[] IndicesPyramid =
        {
            0, 1, 2,
            //Sides
            0, 2, 3, // left
            1, 3, 2, // back
            0, 3, 1 // right
        };

        public static readonly Vector3[] ColorsPyramid =
        {
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f)
        };

        #endregion

        #region Real pyramid

        public static readonly Vector3[] VerticesRealPyramid =
        {
            new Vector3(-0.5f, -0.5f, 1.0f), // back_left
            new Vector3(-0.5f, -0.5f, 0.0f), // front_left
            new Vector3(0.5f, -0.5f, 0.0f), // front_right
            new Vector3(0.5f, -0.5f, 1.0f), // back_right
            new Vector3(0.0f, 0.5f, 0.5f) // middle_top
        };

        public static readonly uint[] IndicesRealPyramid =
        {
            // BOT
            0, 3, 1,
            1, 3, 2,
            // Front
            1, 4, 2,
            // LEFT
            0, 1, 4,
            // Back
            0, 3, 4,
            // Right
            3, 4, 2
        };

        public static readonly Vector3[] ColorsRealPyramid =
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 0f, 0f),
            new Vector3(1f, 1f, 1f)
        };

        #endregion

        #region Cube

        public static readonly Vector3[] VerticesCube =
        {
            new Vector3(-0.5f, 0.5f, 0.0f), // front_top_Left
            new Vector3(0.5f, 0.5f, 0.0f), //front_top_right
            new Vector3(-0.5f, -0.5f, 0.0f), // front_bot_left
            new Vector3(0.5f, -0.5f, 0.0f), // front_bot_right
            new Vector3(-0.5f, 0.5f, 1.0f), // back_top_left
            new Vector3(0.5f, 0.5f, 1.0f), // back_top_right
            new Vector3(-0.5f, -0.5f, 1.0f), // back_bot_left
            new Vector3(0.5f, -0.5f, 1.0f), // back_bot_right
        };

        public static readonly uint[] IndicesCube =
        {
            // TOP
            0, 4, 1,
            1, 4, 5,
            // Front
            2, 0, 3,
            3, 0, 1,
            // BOT
            6, 2, 7,
            7, 2, 3,
            // Right
            7, 3, 5,
            5, 3, 1,
            // LEFT
            4, 0, 6,
            6, 0, 2,
            // BACK
            4, 6, 5,
            5, 6, 7
        };

        public static readonly Vector3[] ColorsCube =
        {
            //One Side
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            //One Side
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            //One Side
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            //One Side
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            //One Side
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
            //One Side
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
        };

        #endregion

        #region Texture Cube

        public static readonly Vector3[] VerticesTextureCube =
        {
            // front_top_Left 0
            new Vector3(-0.5f, 0.5f, 0.0f), // 0
            new Vector3(-0.5f, 0.5f, 0.0f), // 1
            new Vector3(-0.5f, 0.5f, 0.0f), // 2
            //front_top_right 1
            new Vector3(0.5f, 0.5f, 0.0f), // 3
            new Vector3(0.5f, 0.5f, 0.0f), // 4
            new Vector3(0.5f, 0.5f, 0.0f), // 5
            // front_bot_left 2
            new Vector3(-0.5f, -0.5f, 0.0f), // 6
            new Vector3(-0.5f, -0.5f, 0.0f), // 7
            new Vector3(-0.5f, -0.5f, 0.0f), // 8
            // front_bot_right 3
            new Vector3(0.5f, -0.5f, 0.0f), // 9
            new Vector3(0.5f, -0.5f, 0.0f), // 10
            new Vector3(0.5f, -0.5f, 0.0f), // 11
            // back_top_left 4
            new Vector3(-0.5f, 0.5f, 1.0f), // 12
            new Vector3(-0.5f, 0.5f, 1.0f), // 13
            new Vector3(-0.5f, 0.5f, 1.0f), // 14
            // back_top_right 5
            new Vector3(0.5f, 0.5f, 1.0f), // 15
            new Vector3(0.5f, 0.5f, 1.0f), // 16
            new Vector3(0.5f, 0.5f, 1.0f), // 17
            // back_bot_left 6
            new Vector3(-0.5f, -0.5f, 1.0f), // 18 
            new Vector3(-0.5f, -0.5f, 1.0f), // 19
            new Vector3(-0.5f, -0.5f, 1.0f), // 20
            // back_bot_right 7
            new Vector3(0.5f, -0.5f, 1.0f), // 21 
            new Vector3(0.5f, -0.5f, 1.0f), // 22
            new Vector3(0.5f, -0.5f, 1.0f) // 23
        };

        public static readonly uint[] IndicesTextureCube =
        {
            // Front
            6, 0, 9,
            9, 0, 3,
            // TOP
            1, 12, 4,
            4, 12, 15,
            // BOT
            18, 7, 21,
            21, 7, 10,
            // Right
            22, 11, 16,
            16, 11, 5,
            // LEFT
            13, 2, 19,
            19, 2, 8,
            // BACK
            14, 20, 17,
            17, 20, 23
        };

        public static readonly Vector3[] ColorsTextureCube =
        {
            // front_top_Left 0
            new Vector3(1, 1, 1), // 0
            new Vector3(1, 1, 1), // 1
            new Vector3(1, 1, 1), // 2
            //front_top_right 1
            new Vector3(1, 1, 1), // 3
            new Vector3(1, 1, 1), // 4
            new Vector3(1, 1, 1), // 5
            // front_bot_left 2
            new Vector3(0, 0, 0), // 6
            new Vector3(0, 0, 0), // 7
            new Vector3(0, 0, 0), // 8
            // front_bot_right 3
            new Vector3(0, 0, 0), // 9
            new Vector3(0, 0, 0), // 10
            new Vector3(0, 0, 0), // 11
            // back_top_left 4
            new Vector3(1, 1, 1), // 12
            new Vector3(1, 1, 1), // 13
            new Vector3(1, 1, 1), // 14
            // back_top_right 5
            new Vector3(1, 1, 1), // 15
            new Vector3(1, 1, 1), // 16
            new Vector3(1, 1, 1), // 17
            // back_bot_left 6
            new Vector3(0, 0, 0), // 18 
            new Vector3(0, 0, 0), // 19
            new Vector3(0, 0, 0), // 20
            // back_bot_right 7
            new Vector3(0, 0, 0), // 21 
            new Vector3(0, 0, 0), // 22
            new Vector3(0, 0, 0) // 23
        };

        public static readonly Vector2[] UvTextureCube =
        {
            // front_top_Left 0
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(1f, 1f),
            //front_top_right 1
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(1f, 0f),
            // front_bot_left 2
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, 1f),
            // front_bot_right 3
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0f, 0f),
            // back_top_left 4
            new Vector2(0f, 1f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            // back_top_right 5
            new Vector2(0f, 0f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            // back_bot_left 6
            new Vector2(0f, 0f),
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            // back_bot_right 7
            new Vector2(0f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 0f),
        };

        #endregion

        #region Rectangle

        public static readonly Vector3[] VerticesRectangle =
        {
            new Vector3(-0.5f, 0.5f, 1.0f), // left_top
            new Vector3(-0.5f, -0.5f, 1.0f), // left_down
            new Vector3(0.5f, -0.5f, 1.0f), // right_down
            new Vector3(0.5f, 0.5f, 1.0f), // right_top
        };

        public static readonly uint[] IndicesRectangle =
        {
            0, 3, 1,
            1, 3, 2
        };


        public static readonly Vector3[] ColorsRectangle =
        {
            //One Side
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f),
            new Vector3(1f, 1f, 1f),
        };

        #endregion

        #region Triangle

        public static readonly Vector3[] VerticesTriangle =
        {
            new Vector3(-0.5f, 0.5f, 0.0f), // left_top
            new Vector3(-0.5f, -0.5f, 0.0f), // left_down
            new Vector3(0.5f, 0.5f, 0.0f) // right_top
        };

        public static readonly uint[] IndicesTriangle =
        {
            0, 2, 1
        };

        public static readonly Vector3[] ColorsTriangle =
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, 0f, 1f)
        };

        #endregion
    }
}