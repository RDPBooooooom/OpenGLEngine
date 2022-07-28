using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenGL;
using OpenGL.Game;
using OpenGL.Game.Components.BasicComponents;
using OpenGL.Game.GameObjectFactories;
using OpenGL.Game.Utils;
using OpenGL.Game.WavefrontParser;
using OpenGL.Platform;

namespace OpenGLEngine
{
    internal static class Program
    {
        private static int width = 800;
        private static int height = 600;

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
            DirectionalLight dirLight = new DirectionalLight(new Vector3(0, 0, -1), new Vector3(1, 1, 1) * 0.2f,
                new Vector3(1, 1, 1) * 2, new Vector3(1, 1, 1) * 4);

            game = Game.Instance;
            game.CurrentProjection = GetProjectionMatrix;
            game.CurrentCamera = camera;
            game.CurrentDirLight = dirLight;

            Time.Init();
            Window.CreateWindow("OpenGLEngine Alpha Alpha", width, height);

            // add a reshape callback to update the UI
            Window.OnReshapeCallbacks.Add(OnResize);

            // add a close callback to make sure we dispose of everything properly
            Window.OnCloseCallbacks.Add(OnClose);

            // Enable depth testing to ensure correct z-ordering of our fragments
            Gl.Enable(EnableCap.DepthTest);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            Gl.Enable(EnableCap.TextureCubeMapSeamless);

            //Load texture files
            Texture crateTexture = new Texture(TexturePath + "crate.jpg");
            Gl.ActiveTexture(0);
            Gl.BindTexture(crateTexture);

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

            ShaderProgram skyboxMat =
                new ShaderProgram(ShaderUtil.CreateShader(ShaderPath + "SkyboxVert.vs", ShaderType.VertexShader),
                    ShaderUtil.CreateShader(ShaderPath + "SkyboxFrag.fs", ShaderType.FragmentShader));

            ShaderProgram objMaterial =
                new ShaderProgram(ShaderUtil.CreateShader(ShaderPath + "ObjVert.vs", ShaderType.VertexShader),
                    ShaderUtil.CreateShader(ShaderPath + "ObjFrag.fs", ShaderType.FragmentShader));
            objMaterial["color"]?.SetValue(new Vector3(1, 1, 1));

            /*ShaderProgram objNoTextureMaterial =
                new ShaderProgram(ShaderUtil.CreateShader(ShaderPath + "\\NoTexture\\ObjVert.vs", ShaderType.VertexShader),
                    ShaderUtil.CreateShader(ShaderPath + "\\NoTexture\\ObjFrag.fs", ShaderType.FragmentShader));
            objMaterial["color"]?.SetValue(new Vector3(1, 1, 1));
            */
            SwapPolygonModeFill();

            //Create game object

            Skybox box = new Skybox(cmt, skyboxMat);
            game.Skybox = box;

            CubeGameObjectFactory cubeFactory = new CubeGameObjectFactory();
            Guid obj1 = cubeFactory.Create(objMaterial, brickTexture);
            Guid obj2 = cubeFactory.Create(objMaterial, crateTexture);

            WavefrontFactory wavefrontFactory = new WavefrontFactory(ObjPath, "cube.obj");
            //Guid objTest1 = wavefrontFactory.Create(objNoTextureMaterial, null);

            wavefrontFactory.ChangePath(ObjPath + "Complex\\", "Head_1.obj");
            //Guid objTest2 = wavefrontFactory.Create(objNoTextureMaterial, null);

            wavefrontFactory.ChangePath(ObjPath + "Plane\\", "Plane.obj");
            Guid objTest3 = wavefrontFactory.Create(objMaterial, null);

            game.FindComponent<TransformComponent>(obj2).Position = new Vector3(0f, 0, -10);
            game.FindComponent<TransformComponent>(obj1).Position = new Vector3(-5f, 0, -10);
            game.FindComponent<TransformComponent>(obj1).Rotation = new Vector3(0, 0, 45);
            game.FindComponent<TransformComponent>(obj1).Scale = new Vector3(2, 2, 2);
            //game.FindComponent<TransformComponent>(objTest2).Position = new Vector3(-5f, 5, -10);
            game.FindComponent<TransformComponent>(objTest3).Position = new Vector3(-20f, 0, -10);

            for (int i = 0; i < 100; i++)
            {
                for (int u = 0; u < 10; u++)
                {
                    Guid id = cubeFactory.Create(objMaterial, crateTexture);
                    game.FindComponent<TransformComponent>(id).Position = new Vector3(u * 2f, i + 2, -10f);
                }
            }

            Random random = new Random();
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    // Add PointLights to scene
                    PointLightFactory pointLightFactory = new PointLightFactory();
                    Guid pointLight = pointLightFactory.Create(null, null);
                    game.FindComponent<TransformComponent>(pointLight).Position = new Vector3(4f * j, 12f * i, -8);
                    Vector3 ranColor = new Vector3((float) random.NextDouble(), (float) random.NextDouble(),
                        (float) random.NextDouble());
                    PointLightComponent c = game.FindComponent<PointLightComponent>(pointLight);
                    LightData data = c.LightData;
                    data.AmbientColor = ranColor;
                    data.DiffuseColor = ranColor;
                    data.SpecularColor = ranColor;
                    c.LightData = data;
                }
            }
            
            // Hook to the escape press event using the OpenGL.UI class library
            Input.Subscribe((char) Keys.Escape, Window.OnClose);

            Input.Subscribe('f', SwapPolygonModeFill);
            Input.Subscribe('l', SwapPolygonModeLine);

            // Make sure to set up mouse event handlers for the window
            Window.OnMouseCallbacks.Add(global::OpenGL.UI.UserInterface.OnMouseClick);
            Window.OnMouseMoveCallbacks.Add(global::OpenGL.UI.UserInterface.OnMouseMove);

            int frames = 0;
            float time = 0;
            float lastTime = 0;
            
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
                
                frames++;
                time += Time.DeltaTime;
                if (time - lastTime > 1)
                {
                    Console.WriteLine(1000.0d/frames);
                    frames = 0;
                    lastTime++;
                }
            }
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

        private static void SwapPolygonModeLine()
        {
            Gl.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
        }

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