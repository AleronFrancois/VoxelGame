using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;


public class Game : GameWindow
{   
    private int shaderProgram;
    private int modelLoc, viewLoc, projLoc;
    private static List<Block> blocks = new List<Block>();
    private Camera camera;
    private Vector2 lastMousePosition;
    private bool wireframe = true;



    public Game(int width, int height, string title)
    : base(GameWindowSettings.Default, new NativeWindowSettings() 
        { ClientSize = new Vector2i(width, height), Title = title }) 
        {
            camera = new Camera(new Vector3(0.0f, 0.0f, 3.0f), Vector3.UnitY, -90.0f, 0.0f);
        }



    protected override void OnLoad() {
        base.OnLoad();

        InitialiseShaders(); // Load shaders
        GL.Enable(EnableCap.DepthTest); // Enable depth testing
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); // Set background color
        CursorState = CursorState.Grabbed; // Grab and hide cursor

        // Center mouse cursor
        lastMousePosition = new Vector2(Size.X / 2f, Size.Y / 2f);
        MousePosition = lastMousePosition;

        // Set up projection and view matrices
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);
        GL.UniformMatrix4(projLoc, false, ref projection);
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLoc, false, ref view);

        // Generate chunk
        Chunk chunk = new Chunk();
        chunk.GenerateChunk();
    }



    private void InitialiseShaders() {   
        // Initialise shader program
        Shader shader = new Shader();
        shaderProgram = shader.CreateShaderProgram();
        GL.UseProgram(shaderProgram);
        modelLoc = GL.GetUniformLocation(shaderProgram, "model");
        viewLoc = GL.GetUniformLocation(shaderProgram, "view");
        projLoc = GL.GetUniformLocation(shaderProgram, "projection");
    }



    protected override void OnUpdateFrame(FrameEventArgs e) {
        base.OnUpdateFrame(e);

        if (!IsFocused) return;

        // Handle keyboard input
        var input = KeyboardState;
        float deltaTime = (float)e.Time;
        camera.HandleKeyboardInput(input, deltaTime);
        DeveloperTools(input); // Tools for debugging

        // Handle mouse input
        var mouse = MouseState;
        var mouseDelta = mouse.Position - lastMousePosition;
        lastMousePosition = mouse.Position;
        camera.HandleMouseInput(mouseDelta);

        // Update new camera view matrix
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLoc, false, ref view);
    }



    private void DeveloperTools(KeyboardState input) {
        if (input.IsKeyDown(Keys.Escape)) Close(); // Exit window

        // Toggle wireframe mode
        if (input.IsKeyPressed(Keys.D1)) {
            if (wireframe) {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                wireframe = false;
            } 
            else {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                wireframe = true;
            }
        }
    }



    protected override void OnRenderFrame(FrameEventArgs e) {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(shaderProgram);

        foreach (var block in blocks) {
            Matrix4 model = Matrix4.CreateTranslation(block.Position);
            GL.UniformMatrix4(modelLoc, false, ref model);
            block.Render();
        }

        SwapBuffers();
    }



    protected override void OnUnload() {
        base.OnUnload();

        // Clean resources on unload
        foreach (var block in blocks) block.Dispose();
        GL.DeleteProgram(shaderProgram);
    }



    public static void AddBlock(float x, float y, float z) {
        Block block = new Block();
        block.Position = new Vector3(x, y, z);
        blocks.Add(block);
    }
}