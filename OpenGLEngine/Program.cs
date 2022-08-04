using System;
using System.Windows.Forms;
using OpenGL;
using OpenGL.Game;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.GameObjectFactories;
using OpenGL.Game.Math;
using OpenGL.Game.Utils;
using OpenGL.Platform;

namespace OpenGLEngine
{
    internal static class Program
    {
        private static int width = 1920;
        private static int height = 1080;

        private static Game game;
        private static Camera camera;

        private const string ResourcesPath = "resources\\";
        private const string TexturePath = ResourcesPath + "textures\\";
        private const string ShaderPath = ResourcesPath + "shaders\\";
        private const string ObjPath = ResourcesPath + "wavefront\\";

        static void Main()
        {
            camera = new Camera
            {
                ScreenWidth = width,
                ScreenHeight = height
            };
            DirectionalLight dirLight = new DirectionalLight(new Vector3(0, 1, -1).Normalize(),
                new Vector3(1, 1, 1) * 0.2f,
                new Vector3(1, 1, 1) * 0.8f, new Vector3(1, 1, 1) * 1f);

            game = Game.Instance;
            game.CurrentProjection = GetProjectionMatrix;
            game.CurrentCamera = camera;
            game.CurrentDirLight = dirLight;

            Time.Init();
            Window.CreateWindow("OpenGLEngine Alpha", width, height);

            // add a reshape callback to update the UI
            Window.OnReshapeCallbacks.Add(OnResize);

            // add a close callback to make sure we dispose of everything properly
            Window.OnCloseCallbacks.Add(OnClose);

            // Enable depth testing to ensure correct z-ordering of our fragments
            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Gl.Enable(EnableCap.TextureCubeMapSeamless);

            #region Textures

            //Load texture files
            Texture crateTexture = new Texture(TexturePath + "crate.jpg");
            Gl.ActiveTexture(0);
            Gl.BindTexture(crateTexture);

            Texture groundTexture = new Texture(TexturePath + "Ground.jpg");
            Gl.ActiveTexture(0);
            Gl.BindTexture(groundTexture);

            Texture brickTexture = new Texture(TexturePath + "brick.jpg");
            Gl.ActiveTexture(0);
            Gl.BindTexture(brickTexture);

            string[] skyboxTexture =
            {
                TexturePath + "skybox\\Left.png",
                TexturePath + "skybox\\Right.png",
                TexturePath + "skybox\\Up.png",
                TexturePath + "skybox\\Down.png",
                TexturePath + "skybox\\Front.png",
                TexturePath + "skybox\\Back.png",
            };
            CubeMapTexture cmt = new CubeMapTexture(skyboxTexture);

            #endregion

            #region Shader

            ShaderProgram skyboxMat =
                new ShaderProgram(ShaderUtil.CreateShader(ShaderPath + "SkyboxVert.vs", ShaderType.VertexShader),
                    ShaderUtil.CreateShader(ShaderPath + "SkyboxFrag.fs", ShaderType.FragmentShader));

            ShaderProgram objMaterial =
                new ShaderProgram(ShaderUtil.CreateShader(ShaderPath + "ObjVert.vs", ShaderType.VertexShader),
                    ShaderUtil.CreateShader(ShaderPath + "ObjFrag.fs", ShaderType.FragmentShader));
            objMaterial["color"]?.SetValue(new Vector3(1, 1, 1));

            ShaderProgram objNoTextureMaterial =
                new ShaderProgram(
                    ShaderUtil.CreateShader(ShaderPath + "\\NoTexture\\ObjVert.vs", ShaderType.VertexShader),
                    ShaderUtil.CreateShader(ShaderPath + "\\NoTexture\\ObjFrag.fs", ShaderType.FragmentShader));
            objMaterial["color"]?.SetValue(new Vector3(1, 1, 1));

            #endregion

            SwapPolygonModeFill();

            //Create game object

            Skybox box = new Skybox(cmt, skyboxMat);
            game.Skybox = box;

            // Adjust Camera over ground level
            camera.Transform.Position = new Vector3(0, -2, 0);

            // Create Ground
            GroundFactory groundFactory = new GroundFactory();
            for (int i = 0; i < 40; i++)
            {
                float zPos = i * 5f - 100;
                for (int j = 0; j < 40; j++)
                {
                    Guid tempGround = groundFactory.Create(objMaterial, groundTexture);
                    game.FindComponent<TransformComponent>(tempGround).Position = new Vector3(j * 5f - 100, 0, zPos);
                }
            }

            // Spawn Bouncing Spheres 
            PhysicsWavefrontFactory phyWavefrontFactory =
                new PhysicsWavefrontFactory(ObjPath + "Sphere\\", "IcoSphere.obj", 1f, 5);
            Guid sphere = phyWavefrontFactory.Create(objNoTextureMaterial, null);
            game.FindComponent<TransformComponent>(sphere).Position = new Vector3(0, 50, -7.5f);

            Guid sphere2 = phyWavefrontFactory.Create(objNoTextureMaterial, null);
            game.FindComponent<TransformComponent>(sphere2).Position = new Vector3(0, 60, -10f);

            CubeGameObjectFactory cubeFactory = new CubeGameObjectFactory();
            // Create cube
            Guid obj2 = cubeFactory.Create(objMaterial, crateTexture);
            game.FindComponent<TransformComponent>(obj2).Position = new Vector3(-10f, 1, -10);

            #region Walls

            // Right Wall
            Vector2 lastPosRight = Vector2.Zero;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Guid obj1 = cubeFactory.Create(objMaterial, brickTexture);
                    lastPosRight = new Vector2(-11.5f - i * 5, 1 + j * 5);
                    game.FindComponent<TransformComponent>(obj1).Position =
                        new Vector3(lastPosRight.X, lastPosRight.Y, 40);
                    game.FindComponent<TransformComponent>(obj1).Scale = new Vector3(5, 5, 5);
                }
            }

            Vector2 lastPosLeft = Vector2.Zero;
            // Left Wall
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Guid obj1 = cubeFactory.Create(objMaterial, brickTexture);
                    lastPosLeft = new Vector2(10.5f + i * 5, 1 + j * 5);
                    game.FindComponent<TransformComponent>(obj1).Position =
                        new Vector3(lastPosLeft.X, lastPosLeft.Y, 40);
                    game.FindComponent<TransformComponent>(obj1).Scale = new Vector3(5, 5, 5);
                }
            }

            // Extend Left
            Vector2 lastExtendLeft = Vector2.Zero;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Guid obj1 = cubeFactory.Create(objMaterial, brickTexture);
                    lastExtendLeft = new Vector2(1 + j * 5, 35 - i * 5);
                    game.FindComponent<TransformComponent>(obj1).Position =
                        new Vector3(lastPosLeft.X + 5, lastExtendLeft.X, lastExtendLeft.Y);
                    game.FindComponent<TransformComponent>(obj1).Scale = new Vector3(5, 5, 5);
                }
            }

            // Extend Right
            Vector2 lastExtendRight = Vector2.Zero;
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Guid obj1 = cubeFactory.Create(objMaterial, brickTexture);
                    lastExtendRight = new Vector2(1 + j * 5, 35 - i * 5);
                    game.FindComponent<TransformComponent>(obj1).Position =
                        new Vector3(lastPosRight.X - 5, lastExtendRight.X, lastExtendRight.Y);
                    game.FindComponent<TransformComponent>(obj1).Scale = new Vector3(5, 5, 5);
                }
            }

            // Close Left
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Guid obj1 = cubeFactory.Create(objMaterial, brickTexture);
                    game.FindComponent<TransformComponent>(obj1).Position =
                        new Vector3(lastPosRight.X + i * 5, 1 + j * 5, lastExtendRight.Y - 5);
                    game.FindComponent<TransformComponent>(obj1).Scale = new Vector3(5, 5, 5);
                }
            }

            // Close Right
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Guid obj1 = cubeFactory.Create(objMaterial, brickTexture);
                    game.FindComponent<TransformComponent>(obj1).Position =
                        new Vector3(lastPosLeft.X - i * 5, 1 + j * 5, lastExtendLeft.Y - 5);
                    game.FindComponent<TransformComponent>(obj1).Scale = new Vector3(5, 5, 5);
                }
            }

            #endregion

            // Spawn Horn
            WavefrontFactory wavefrontFactory = new WavefrontFactory(ObjPath + "Tower\\", "Tower.obj");
            
            #region Towers

            // Left Back
            Guid towerLB = wavefrontFactory.Create(objMaterial, null);
            game.FindComponent<TransformComponent>(towerLB).Position = new Vector3(lastPosLeft.X + 5, 0.5f, 40 + 2.5f);
            game.FindComponent<TransformComponent>(towerLB).Scale = new Vector3(1.1f, 1, 1.1f);

            // Left Front
            Guid towerLF = wavefrontFactory.Create(objMaterial, null);
            game.FindComponent<TransformComponent>(towerLF).Position =
                new Vector3(lastPosLeft.X + 5, 0.5f, lastExtendLeft.Y - 2.5f);
            game.FindComponent<TransformComponent>(towerLF).Scale = new Vector3(1.1f, 1, 1.1f);

            // Right Back
            Guid towerRB = wavefrontFactory.Create(objMaterial, null);
            game.FindComponent<TransformComponent>(towerRB).Position = new Vector3(lastPosRight.X - 5, 0.5f, 40 + 2.5f);
            game.FindComponent<TransformComponent>(towerRB).Scale = new Vector3(1.1f, 1, 1.1f);

            // Right Front
            Guid towerRF = wavefrontFactory.Create(objMaterial, null);
            game.FindComponent<TransformComponent>(towerRF).Position =
                new Vector3(lastPosRight.X - 5, 0.5f, lastExtendRight.Y - 2.5f);
            game.FindComponent<TransformComponent>(towerRF).Scale = new Vector3(1.1f, 1, 1.1f);

            #endregion

            // Gate
            wavefrontFactory.ChangePath(ObjPath + "Gate\\", "Gate.obj");
            Guid gate = wavefrontFactory.Create(objMaterial, null);
            game.FindComponent<TransformComponent>(gate).Position = new Vector3(-0.5f, 0.5f, lastExtendRight.Y);
            game.FindComponent<TransformComponent>(gate).Rotation = RotationUtils.ToQ(new Vector3(0, 180, 0));
            game.FindComponent<TransformComponent>(gate).Scale = new Vector3(1.1f, 1, 1.1f);

            // Spawn Castle
            wavefrontFactory.ChangePath(ObjPath + "Castle\\", "Castle.obj");
            Guid wall = wavefrontFactory.Create(objNoTextureMaterial, null);
            game.FindComponent<TransformComponent>(wall).Position = new Vector3(0, 0.5f, 50);
            game.FindComponent<TransformComponent>(wall).Rotation = RotationUtils.ToQ(new Vector3(0, 180, 0));

            // Church
            wavefrontFactory.ChangePath(ObjPath + "Church\\", "Church.obj");
            Guid church = wavefrontFactory.Create(objMaterial, null);
            game.FindComponent<TransformComponent>(church).Position = new Vector3(-20, 0.6f, 20);
            game.FindComponent<TransformComponent>(church).Rotation = RotationUtils.ToQ(new Vector3(0, 270, 0));

            #region Lights

            Random random = new Random();
            PointLightFactory pointLightFactory = new PointLightFactory();

            Guid lightOne = pointLightFactory.Create(null, null);
            SetRandomLightData(random, lightOne);
            game.FindComponent<TransformComponent>(lightOne).Position = new Vector3(0, 5, 0);

            Guid lightTwo = pointLightFactory.Create(null, null);
            SetRandomLightData(random, lightTwo);
            game.FindComponent<TransformComponent>(lightTwo).Position = new Vector3(45, 5, 30);

            Guid lightThree = pointLightFactory.Create(null, null);
            SetRandomLightData(random, lightThree);
            game.FindComponent<TransformComponent>(lightThree).Position = new Vector3(-45, 5, 30);

            Guid lightFour = pointLightFactory.Create(null, null);
            SetRandomLightData(random, lightFour);
            game.FindComponent<TransformComponent>(lightFour).Position = new Vector3(45, 5, -40);

            Guid lightFive = pointLightFactory.Create(null, null);
            SetRandomLightData(random, lightFive);
            game.FindComponent<TransformComponent>(lightFive).Position = new Vector3(-45, 5, -40);

            #endregion

            // Hook to the escape press event using the OpenGL.UI class library
            Input.Subscribe((char) Keys.Escape, Window.OnClose);

            // Make sure to set up mouse event handlers for the window
            Window.OnMouseCallbacks.Add(global::OpenGL.UI.UserInterface.OnMouseClick);
            Window.OnMouseMoveCallbacks.Add(global::OpenGL.UI.UserInterface.OnMouseMove);

            // Game loop
            while (Window.Open)
            {
                Window.HandleEvents();

                OnPreRenderFrame();

                Input.Update();

                game.Start();
                game.Update();
                game.Render();

                OnPostRenderFrame();

                Time.Update();
            }
        }

        private static void SetRandomLightData(Random random, Guid light)
        {
            PointLightComponent c = game.FindComponent<PointLightComponent>(light);
            LightData data = c.LightData;

            Vector3 ranColor = new Vector3((float) random.NextDouble(), (float) random.NextDouble(),
                (float) random.NextDouble());

            data.AmbientColor = ranColor;
            data.DiffuseColor = ranColor;
            data.SpecularColor = ranColor;
            c.LightData = data;
        }

        #region Transformation

        private static Matrix4 GetProjectionMatrix()
        {
            float fov = 60f;

            float aspectRatio = width / (float) height;
            Matrix4 projection =
                Matrix4.CreatePerspectiveFieldOfView(0.45f, aspectRatio, 0.1f, 1000f);

            return projection;
        }

        #endregion

        #region Callbacks

        private static void SwapPolygonModeFill()
        {
            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }

        private static void OnResize()
        {
            width = Window.Width;
            height = Window.Height;

            camera.ScreenWidth = width;
            camera.ScreenHeight = height;

            global::OpenGL.UI.UserInterface.OnResize(Window.Width, Window.Height);
        }

        private static void OnClose()
        {
            // make sure to dispose of everything
            //global::OpenGL.UI.UserInterface.Dispose();
            //global::OpenGL.UI.BMFont.Dispose();
        }

        private static void OnPreRenderFrame()
        {
            // set up the OpenGL viewport and clear both the color and depth bits
            Gl.Viewport(0, 0, Window.Width, Window.Height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        private static void OnPostRenderFrame()
        {
            // Draw the user interface after everything else
            global::OpenGL.UI.UserInterface.Draw();

            // Swap the back buffer to the front so that the screen displays
            Window.SwapBuffers();
        }

        private static void OnMouseClick(int button, int state, int x, int y)
        {
            // take care of mapping the Glut buttons to the UI enums
            if (!global::OpenGL.UI.UserInterface.OnMouseClick(button + 1, (state == 0 ? 1 : 0), x, y))
            {
                // do other picking code here if necessary
            }
        }

        private static void OnMouseMove(int x, int y)
        {
            if (!global::OpenGL.UI.UserInterface.OnMouseMove(x, y))
            {
                // do other picking code here if necessary
            }
        }

        #endregion
    }
}