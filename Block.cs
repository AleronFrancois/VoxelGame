using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


/// ---------------Block Geometry---------------
/// 
/// Defines the block geometry and uses a bitmask
/// to flag visible faces.
/// 
/// --------------------------------------------


public enum BlockFace : byte {
    None = 0,       // 00000000
    Back = 1 << 0,  // 00000001
    Front = 1 << 1, // 00000010
    Right = 1 << 2, // 00000100
    Left = 1 << 3,  // 00001000
    Top = 1 << 4,   // 00010000
    Bottom = 1 << 5 // 00100000
}


public class Block {
    public static int VAO { get; private set; } // Shared Vertex Array Object for all blocks
    public static int VBO { get; private set; } // Shared Vertex Buffer Object
    public static int EBO { get; private set; } // Shared Element Buffer Object
    public Vector3 Position { get; set; } // Position of block
    public BlockFace VisibleFaces { get; set; } // Gets visibility of block faces
    public Vector2 AtlasOffset { get; private set; }
    public Vector2 AtlasScale { get; private set; }
    private Vector2 textureCoordStart;
    private Vector2 textureCoordEnd;



    // Vertices array
    private static float[] vertices = new float[] {
        // Back face
        1f, 1f, 0f,   0f, 1f,
        1f, 0f, 0f,   0f, 0f,
        0f, 0f, 0f,   1f, 0f,
        0f, 1f, 0f,   1f, 1f,
        // Front face
        0f, 1f, 1f,   0f, 1f,
        0f, 0f, 1f,   0f, 0f,
        1f, 0f, 1f,   1f, 0f,
        1f, 1f, 1f,   1f, 1f,
        // Right face
        1f, 1f, 1f,   0f, 1f,
        1f, 0f, 1f,   0f, 0f,
        1f, 0f, 0f,   1f, 0f,
        1f, 1f, 0f,   1f, 1f,
        // Left face
        0f, 1f, 0f,   0f, 1f,
        0f, 0f, 0f,   0f, 0f,
        0f, 0f, 1f,   1f, 0f,
        0f, 1f, 1f,   1f, 1f,
        // Top face
        0f, 1f, 1f,   0f, 1f,
        1f, 1f, 1f,   1f, 1f,
        1f, 1f, 0f,   1f, 0f,
        0f, 1f, 0f,   0f, 0f,
        // Bottom face
        0f, 0f, 1f,  0f, 1f,
        0f, 0f, 0f,  0f, 0f,
        1f, 0f, 0f,  1f, 0f,
        1f, 0f, 1f,  1f, 1f          
    };

    // Indices array
    private static uint[] indices = new uint[] {
        // Back face
        0, 1, 2, 2, 3, 0,
        // Front face
        4, 5, 6, 6, 7, 4,
        // Right face
        8, 9, 10, 10, 11, 8,
        // Left face
        12, 13, 14, 14, 15, 12,
        // Top face
        16, 17, 18, 18, 19, 16,
        // Bottom face
        20, 21, 22, 22, 23, 20
    };



    static Block() {
        InitialiseBlock(); // Initialise buffers once
    }



    private static void InitialiseBlock() {
        // Bind shared VAO, VBO, EBO for all blocks
        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EBO = GL.GenBuffer();

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // Position attribute
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Texture coordinate attribute
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }



    public bool IsFaceVisible(BlockFace face) {
        return (VisibleFaces & face) != 0;
    }



    public void SetFaceVisible(BlockFace face) {
        VisibleFaces |= face;
    }



    public void SetTextureCoordinates(Vector2 start, Vector2 end) {
        textureCoordStart = start;
        textureCoordEnd = end;
    }


    // Method to get texture coordinates for rendering
    public Vector2[] GetTextureCoordinates()
    {
        return new Vector2[]
        {
            textureCoordStart,
            new Vector2(textureCoordEnd.X, textureCoordStart.Y),
            textureCoordEnd,
            new Vector2(textureCoordStart.X, textureCoordEnd.Y)
        };
    }



    public void UpdateVisibility(Block adjacentTop, Block adjacentBottom, Block adjacentLeft, Block adjacentRight, Block adjacentFront, Block adjacentBack) {
        VisibleFaces = BlockFace.None; // Reset visibility only if there's a change

        // Check for adjacent blocks and set visibility accordingly
        if (adjacentTop == null) SetFaceVisible(BlockFace.Top);
        if (adjacentBottom == null) SetFaceVisible(BlockFace.Bottom);
        if (adjacentLeft == null) SetFaceVisible(BlockFace.Left);
        if (adjacentRight == null) SetFaceVisible(BlockFace.Right);
        if (adjacentFront == null) SetFaceVisible(BlockFace.Front);
        if (adjacentBack == null) SetFaceVisible(BlockFace.Back);
    }



    public void Render(int shaderProgram) {
        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO); // Bind EBO once

        // Set texture atlas uniforms
        int atlasOffsetLocation = GL.GetUniformLocation(shaderProgram, "atlasOffset");
        int atlasScaleLocation = GL.GetUniformLocation(shaderProgram, "atlasScale");
        GL.Uniform2(atlasOffsetLocation, textureCoordStart);
        GL.Uniform2(atlasScaleLocation, textureCoordEnd - textureCoordStart);

        // Bind the texture
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, BlockType.GrassTextureID);
        int textureLocation = GL.GetUniformLocation(shaderProgram, "texture1");
        GL.Uniform1(textureLocation, 0);

        uint indexOffset = 0;

        // Loop through the 6 faces (back, front, right, left, top, bottom)
        for (int i = 0; i < 6; i++) {
            BlockFace face = (BlockFace)(1 << i);
            if (IsFaceVisible(face)) {
                GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (IntPtr)(indexOffset * sizeof(uint)));
            }
            indexOffset += 6;
        }

        GL.BindVertexArray(0); // Unbind VAO once
    }




    public void Dispose() {
        // Only dispose when needed (remove objects from memory when no longer needed)
    }
}
