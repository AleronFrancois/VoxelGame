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
    private int shaderProgram; // Shader program for rendering
    private int modelLocation, viewLocation, projLocation; // Location for model, view and projection matrices
    private static List<Block> blocks = new List<Block>(); // List of all blocks
    private Camera camera; // Camera for viewing the scene
    private Vector2 lastMousePosition; // Last mouse position
    private bool wireframe = true; // Wireframe mode
    private Vector3 currentChunkPosition; // Current chunk position of the camera

    // Dictionary based lookup system for adjacent block checking
    public static Dictionary<Vector3, Block> blockLookup = new Dictionary<Vector3, Block>(); 


    #region Game
    public Game(int width, int height, string title)
    : base(GameWindowSettings.Default, new NativeWindowSettings() 
        { ClientSize = new Vector2i(width, height), Title = title }) 
        {
            camera = new Camera(new Vector3(0.0f, 16.0f, 0.0f), Vector3.UnitY, 0.0f, 0.0f);
            currentChunkPosition = GetChunkPosition(camera.Position);
        }
    #endregion


    #region OnLoad
    protected override void OnLoad() {
        base.OnLoad();

        InitialiseShaders(); // Load shaders
        GL.Enable(EnableCap.DepthTest); // Enable depth testing
        GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f); // Set background color
        CursorState = CursorState.Grabbed; // Grab and hide cursor
        GL.Enable(EnableCap.CullFace); // Enable back face culling

        // Center mouse cursor
        lastMousePosition = new Vector2(Size.X / 2f, Size.Y / 2f);
        MousePosition = lastMousePosition;

        // Set up projection and view matrices
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), Size.X / (float)Size.Y, 0.1f, 100.0f);
        GL.UniformMatrix4(projLocation, false, ref projection);
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLocation, false, ref view);

        // Generate chunk mesh
        Chunk chunk = new Chunk();
        chunk.GenerateChunk(0, 0);

        AddBlock(-1, 0, 0);
        AddBlock(-1, 1, 0);
        AddBlock(-1, 2, 0);
        AddBlock(-1, 3, 0);

        RenderBlocks(); // Render blocks and only visible block faces
    }
    #endregion


    #region InitialiseShaders
    private void InitialiseShaders() {   
        // Initialise and use shader program
        Shader shader = new Shader();
        shaderProgram = shader.CreateShaderProgram();
        GL.UseProgram(shaderProgram);
        modelLocation = GL.GetUniformLocation(shaderProgram, "model");
        viewLocation = GL.GetUniformLocation(shaderProgram, "view");
        projLocation = GL.GetUniformLocation(shaderProgram, "projection");
    }
    #endregion


    #region OnUpdateFrame
    protected override void OnUpdateFrame(FrameEventArgs e) {
        base.OnUpdateFrame(e);

        if (!IsFocused) return;

        // Handle keyboard input
        var input = KeyboardState;
        float deltaTime = (float)e.Time;
        camera.HandleKeyboardInput(input, deltaTime);
        DeveloperTools(input);

        // Handle mouse input
        var mouse = MouseState;
        var mouseDelta = mouse.Position - lastMousePosition;
        lastMousePosition = mouse.Position;
        camera.HandleMouseInput(mouseDelta);

        // Update new camera view matrix
        Matrix4 view = camera.GetViewMatrix();
        GL.UniformMatrix4(viewLocation, false, ref view);

        // Check if the camera has moved to a new chunk
        Vector3 newChunkPosition = GetChunkPosition(camera.Position);
        if (newChunkPosition != currentChunkPosition) {
            currentChunkPosition = newChunkPosition;
            Chunk chunk = new Chunk();
            chunk.GenerateChunk((int)currentChunkPosition.X, (int)currentChunkPosition.Z);
            RenderBlocks(); // Ensure blocks are rendered after generating new chunk
        }
    }
    #endregion


    #region OnRenderFrame
    protected override void OnRenderFrame(FrameEventArgs e) {
        base.OnRenderFrame(e);

        // Clear the color and depth buffers
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(shaderProgram);

        // Use shader program for rendering
        foreach (var block in blocks) {
            Matrix4 model = Matrix4.CreateTranslation(block.Position);
            GL.UniformMatrix4(modelLocation, false, ref model);
            block.Render();
        }

        SwapBuffers();
    }
    #endregion


    #region OnUnload
    protected override void OnUnload() {
        base.OnUnload();

        // Clean resources on unload
        foreach (var block in blocks) block.Dispose();
        GL.DeleteProgram(shaderProgram);
    }
    #endregion 

    
    #region AddBlock
    public static void AddBlock(float x, float y, float z) {
        Block block = new Block();
        block.Position = new Vector3(x, y, z);

        // Append block to list and dictionary only if it does not exist
        if (!blockLookup.ContainsKey(block.Position)) {
            blocks.Add(block);
            blockLookup[block.Position] = block;
        }
    }
    #endregion 


    #region GetAdjacentBlock
    public Block? GetAdjacentBlock(Block block, Vector3 direction) {
        // Calculates position of the adjacent block then gets that block and returns it
        Vector3 adjacentPosition = block.Position + direction;
        blockLookup.TryGetValue(adjacentPosition, out Block? adjacentBlock);
        if (adjacentBlock == null) return null;

        return adjacentBlock;
    }
    #endregion 


    #region RenderBlock
    public void RenderBlocks() {
        // Direction of each six faces of the block
        Vector3[] directions = new Vector3[] {
            new Vector3(0, 1, 0),  // Top
            new Vector3(0, -1, 0), // Bottom
            new Vector3(-1, 0, 0), // Left
            new Vector3(1, 0, 0),  // Right
            new Vector3(0, 0, 1),  // Front
            new Vector3(0, 0, -1)  // Back
        };

        // Corresponding block face flags
        BlockFace[] faces = new BlockFace[] {
            BlockFace.Top,
            BlockFace.Bottom,
            BlockFace.Left,
            BlockFace.Right,
            BlockFace.Front,
            BlockFace.Back
        };

        // Renders each block
        foreach (var block in blockLookup.Values) {
            block.VisibleFaces = BlockFace.None;

            // Loop through the faces and check for adjacent blocks
            for (int i = 0; i < directions.Length; i++) {
                if (GetAdjacentBlock(block, directions[i]) == null) {
                    block.SetFaceVisible(faces[i]);
                }
            }
        }
    }
    #endregion 



    #region DeveloperTools
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
    #endregion

    #region GetChunkPosition
    private Vector3 GetChunkPosition(Vector3 position) {
        int chunkSize = 16;
        return new Vector3(
            MathF.Floor(position.X / chunkSize),
            MathF.Floor(position.Y / chunkSize),
            MathF.Floor(position.Z / chunkSize)
        );
    }
    #endregion
}