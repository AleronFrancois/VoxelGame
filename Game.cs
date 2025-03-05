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


public class Game : GameWindow { 
    public static int modelLoc, viewLoc, projLoc; // Model, view and projection matrices
    private int shaderProgram; // Shader used for rendering
    private Vector2 lastMousePosition; // Last mouse position
    private Crosshair crosshair; // Crosshair 
    private int crosshairShaderProgram; // Shader program for crosshair
    public static List<Block> blocks = new List<Block>(); // List of all blocks in the game
    private ChunkSystem chunkSystem; // Chunk system to manage chunks
    private double lastTime; // Last time for fps counter
    private int frameCount; // Frame count for fps counter

    // Player and camera
    public Camera camera; // Camera used for view the scene
    public Player player; // Player object
    private PlayerMovement playerMovement; // Handles player movement
    
    // Control flags
    public static bool wireframe = true; // Flag for wireframe mode 
    public static bool cursorGrabbed = true; // frag for cursor state
    public static bool attachCamera = true; // Flag for attaching and detaching camera



    public Game(int width, int height, string title)
    : base(GameWindowSettings.Default, new NativeWindowSettings() 
    { 
        ClientSize = new Vector2i(width, height), Title = title
    }) 
    {
        this.CenterWindow(); // Position window in center of screen
        camera = new Camera(new Vector3(0.0f, 34.0f, 0.0f), Vector3.UnitY, 0.0f, 0.0f); // initialise camera position and orientation
        crosshair = new Crosshair(); // Initialise crosshair
        chunkSystem = new ChunkSystem(); // Initialise chunk system
        player = new Player(new Vector3(0.0f, 32.0f, 0.0f)); // Initialise player
        playerMovement = new PlayerMovement(player, camera);
    }



    protected override void OnLoad() {
        base.OnLoad();
        lastMousePosition = new Vector2(Size.X / 2f, Size.Y / 2f); // Center mouse cursor
        MousePosition = lastMousePosition; // Track last mouse position
        InitialiseShaders(); // Load shaders
        BlockType.LoadTextures(); // Load textures
        GL.Enable(EnableCap.DepthTest); // Enable depth testing
        GL.ClearColor(0.3f, 0.5f, 1.0f, 1.0f); // Set background color
        //GL.Enable(EnableCap.CullFace); // Enable face culling
        //GL.CullFace(TriangleFace.Back); // Set backface culling
        CursorState = CursorState.Grabbed; // Grab and hide cursor

        // Set up projection and view matrices
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);
        GL.UniformMatrix4(projLoc, false, ref projection);
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLoc, false, ref view);

        lastTime = GLFW.GetTime(); // Initialise last time for fps calculation
    }



    protected override void OnUpdateFrame(FrameEventArgs e) {
        base.OnUpdateFrame(e);
        if (!IsFocused) return; // Pause updates if the window is not focused

        // Handle keyboard input
        var input = KeyboardState;
        float deltaTime = (float)e.Time;

        // Handle camera based on attachment 
        if (attachCamera) playerMovement.HandleKeyboardInput(input, deltaTime);
        else camera.HandleKeyboardInput(input, deltaTime);

        // Handle developer tools input
        Input.DeveloperTools(input, this, this);

        // Handle mouse input
        var mouse = MouseState;
        var mouseDelta = mouse.Position - lastMousePosition;
        lastMousePosition = mouse.Position;
        camera.HandleMouseInput(mouseDelta);

        // Update camera view matrix
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLoc, false, ref view);

        // Display debug information
        DebugInformation();
    }



    protected override void OnRenderFrame(FrameEventArgs e) {
        base.OnRenderFrame(e);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Update background color
        GL.UseProgram(shaderProgram); // Use shader program

        // Update view and model matrix
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLoc, false, ref view);
        Matrix4 model = Matrix4.Identity;
        GL.UniformMatrix4(modelLoc, false, ref model);

        // Bind the texture
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, BlockType.GrassTextureID);
        int textureLocation = GL.GetUniformLocation(shaderProgram, "texture1");
        GL.Uniform1(textureLocation, 0);

        // Render chunks
        Vector3 cameraPosition = camera.Position;
        chunkSystem.GenerateChunkAtCameraPosition(cameraPosition);
        chunkSystem.RenderChunks();

        crosshair.Render(crosshairShaderProgram, Size.X, Size.Y); // Render crosshair

        player.Render(shaderProgram); // Render player

        SwapBuffers();
    }



    private void InitialiseShaders() {
        // Initialise shader program
        Shader shader = new Shader();
        shaderProgram = shader.CreateShaderProgram();
        GL.UseProgram(shaderProgram);

        // Get uniform locations
        modelLoc = GL.GetUniformLocation(shaderProgram, "model");
        viewLoc = GL.GetUniformLocation(shaderProgram, "view");
        projLoc = GL.GetUniformLocation(shaderProgram, "projection");
        
        // Set up projection matrix
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);
        GL.UniformMatrix4(projLoc, false, ref projection);

        // Initialise crosshair
        crosshair = new Crosshair();
        crosshair.InitialiseCrosshair();
        crosshairShaderProgram = new Shader().CreateCrosshairShaderProgram();
    }



    protected override void OnResize(ResizeEventArgs e) {
        base.OnResize(e);
        GL.Viewport(0, 0, e.Width, e.Height); // Update the OpenGL viewport

        // Update projection matrix to maintain the aspect ratio
        float aspectRatio = e.Width / (float)e.Height;
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), aspectRatio, 0.1f, 100.0f);

        // Assuming you have a uniform location for the projection matrix
        GL.UseProgram(shaderProgram);
        GL.UniformMatrix4(projLoc, false, ref projection);
    }



    protected override void OnUnload() {
        base.OnUnload();
        // Clean up resources on unload
        foreach (var block in blocks) block.Dispose();
        GL.DeleteProgram(shaderProgram);
        crosshair.Cleanup();
        GL.DeleteProgram(crosshairShaderProgram);
    }



    private void DebugInformation() {
        // Debug information
        double currentTime = GLFW.GetTime();
        frameCount++;
        if (currentTime - lastTime >= 1.0) {
            Title = $"3D Engine         FPS: {frameCount}   Position: {camera.Position.X:F1}, {camera.Position.Y:F1}, {camera.Position.Z:F1}";
            frameCount = 0;
            lastTime = currentTime;
        }
    }
}