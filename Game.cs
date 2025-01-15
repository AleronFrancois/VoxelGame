using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Mathematics;
using System;


/// ---------------3D Voxel Game---------------
/// 
/// [Author]          > Aleron Francois
/// [Date Started]    > 23/12/2024
/// [Current state]   > Currently in development
/// 
/// Handles the game window and all rendering.
/// 
/// -------------------------------------------


public class Game : GameWindow
{   
    private int shaderProgram; // Shader used for rendering
    private Camera camera; // Camera used for view the scene
    public static int modelLoc, viewLoc, projLoc; // Model, view and projection matrices
    private Vector2 lastMousePosition; // Last mouse position
    public static List<Block> blocks = new List<Block>(); // List of all blocks in the game
    Chunk chunk = new Chunk(); // Chunk of blocks

    public static bool wireframe = true; // Flag for wireframe mode 
    public static bool cursorGrabbed = true; // frag for cursor state



    public Game(int width, int height, string title)
    : base(GameWindowSettings.Default, new NativeWindowSettings() 
        { 
            ClientSize = new Vector2i(width, height), Title = title 
        }) 
        {
            camera = new Camera(new Vector3(0.0f, 34.0f, 0.0f), Vector3.UnitY, 0.0f, 0.0f); // initialise camera position and orientation
            UpdateFrequency = 144.0; // Framerate cap
        }



    protected override void OnLoad() {
        base.OnLoad();

        InitialiseShaders(); // Load shaders
        GL.Enable(EnableCap.DepthTest); // Enable depth testing
        GL.ClearColor(0.3f, 0.5f, 1.0f, 1.0f); // Set background color
        //GL.Enable(EnableCap.CullFace); // Enable back face culling
        CursorState = CursorState.Grabbed; // Grab and hide cursor

        // Center mouse cursor
        lastMousePosition = new Vector2(Size.X / 2f, Size.Y / 2f);
        MousePosition = lastMousePosition;

        // Set up projection and view matrices
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);
        GL.UniformMatrix4(projLoc, false, ref projection);
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLoc, false, ref view);

        // Generate chunk mesh
        chunk.GenerateChunk(); 
        BlockType.AddGrassBlock(0, 32, 0, chunk);
        BlockType.AddGrassBlock(0, 32, 1, chunk);
        BlockType.AddGrassBlock(0, 32, 2, chunk);
        BlockType.AddGrassBlock(1, 32, 0, chunk);
        BlockType.AddGrassBlock(1, 32, 1, chunk);
        BlockType.AddGrassBlock(1, 32, 2, chunk);
        chunk.GenerateChunkMesh(); 
    }



    protected override void OnUpdateFrame(FrameEventArgs e) {
        base.OnUpdateFrame(e);

        if (!IsFocused) return; // Pause updates if the window is not focused

        // Handle keyboard input
        var input = KeyboardState;
        float deltaTime = (float)e.Time;
        camera.HandleKeyboardInput(input, deltaTime);
        Input.DeveloperTools(input, this); // Tools for debugging

        // Handle mouse input
        var mouse = MouseState;
        var mouseDelta = mouse.Position - lastMousePosition;
        lastMousePosition = mouse.Position;
        camera.HandleMouseInput(mouseDelta);

        Console.Write($"\rCamera position: {camera.Position.X:F1} {camera.Position.Y:F1} {camera.Position.Z:F1}          "); // Camera position for debugging 

        // Update new camera view matrix
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLoc, false, ref view);
    }



    protected override void OnRenderFrame(FrameEventArgs e) {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(shaderProgram);

        // Update view matrix
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLoc, false, ref view);

        // Set model matrix for chunk
        Matrix4 model = Matrix4.Identity;
        GL.UniformMatrix4(modelLoc, false, ref model);

        chunk.Render(); // Render chunk mesh

        SwapBuffers();
    }



    private void InitialiseShaders() {   
        // Initialise shader program
        Shader shader = new Shader();
        shaderProgram = shader.CreateShaderProgram();
        GL.UseProgram(shaderProgram);
        modelLoc = GL.GetUniformLocation(shaderProgram, "model");
        viewLoc = GL.GetUniformLocation(shaderProgram, "view");
        projLoc = GL.GetUniformLocation(shaderProgram, "projection");

        // Set up projection matrix
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);
        GL.UniformMatrix4(projLoc, false, ref projection);
    }



    protected override void OnUnload() {
        base.OnUnload();
        // Clean resources on unload
        foreach (var block in blocks) block.Dispose();
        GL.DeleteProgram(shaderProgram);
    }
}