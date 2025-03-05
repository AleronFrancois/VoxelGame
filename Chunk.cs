using SimplexNoise;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Concurrent;


/// ------------------Chunk------------------
/// 
/// Generates a chunk mesh by checking each block
/// for neighboring, and only rendering faces
/// that are adjcacent to air/null.
///
/// ------------------------------------------


public class Chunk {
    public List<Block> chunkBlocks; // List of blocks in single chunk
    public static ConcurrentDictionary<Vector3, Block> blockLookup = new ConcurrentDictionary<Vector3, Block>(); // Dictionary lookup system
    private List<float> vertices = new List<float>(); // For storing vertices
    private List<uint> indices = new List<uint>(); // For storing the indices
    private int vao, vbo, ebo;
    private Vector3 positionOffset; // Offset of chunk
    public static int chunkWidth = 16; // Chunk width
    public static int chunkHeight = 32; // Chunk height
    public Vector3 Position { get; private set; } // Add this line



    public Chunk(Vector3 positionOffset) {
        this.positionOffset = positionOffset;
        this.Position = positionOffset; // Initialise the Position property
        chunkBlocks = new List<Block>(); // Initialises a new list of blocks for each chunk
    }



    public Block? GetAdjacentBlock(Block block, Vector3 direction) {
        // Calculates position of the adjacent block and uses dictionary lookup system to get block
        Vector3 adjacentPosition = block.Position + direction;
        if (blockLookup.TryGetValue(adjacentPosition, out Block? adjacentBlock)) return adjacentBlock;

        return null;
    }



    public void GenerateChunk() {
        float scale = 0.01f; // Adjusts smoothness of noise
        float surfaceLevel = 25f; // Surface level for Perlin noise

        // Generate chunk with perlin noise
        for (int x = 0; x < chunkWidth; x++) {
            for (int z = 0; z < chunkWidth; z++) {
                float height = PerlinNoise((x + positionOffset.X) * scale, (z + positionOffset.Z) * scale) * (chunkHeight - surfaceLevel) + surfaceLevel;
                
                for (int y = 0; y < (int)height; y++) {
                    BlockType.AddGrassBlock(x + (int)positionOffset.X, y + (int)positionOffset.Y, z + (int)positionOffset.Z, this); // Add block with offset
                }    
            }
        }
    }



    private float PerlinNoise(float x, float z) {
        // Calculate perlin noise
        return Noise.CalcPixel2D((int)(x * 50), (int)(z * 50), 0.05f) / 255f;
    }



    public void GenerateChunkMesh() {
        vertices.Clear();
        indices.Clear();

        // All six directions of block
        Vector3[] directions = new Vector3[] {
            new Vector3(0, 1, 0),  // Top
            new Vector3(0, -1, 0), // Bottom
            new Vector3(-1, 0, 0), // Left
            new Vector3(1, 0, 0),  // Right
            new Vector3(0, 0, 1),  // Front
            new Vector3(0, 0, -1)  // Back
        };

        // Block face array
        BlockFace[] faces = new BlockFace[] {
            BlockFace.Top,
            BlockFace.Bottom,
            BlockFace.Left,
            BlockFace.Right,
            BlockFace.Front,
            BlockFace.Back
        };

        // Checks each block in chunk block list
        foreach (var block in chunkBlocks) {
            block.VisibleFaces = BlockFace.None; // Sets all faces invisible by default

            // Checks all six directions of block
            for (int i = 0; i < directions.Length; i++) {
                // Finds adjacent air block and sets face visibility accordingly
                if (GetAdjacentBlock(block, directions[i]) == null) {
                    block.SetFaceVisible(faces[i]); // Set visibility accordingly
                    AddBlockFaceToMesh(block, faces[i]); // Add visible block face to the mesh
                }
            }
        }

        // Generate buffers after mesh is constructed
        GenerateBuffers();
    }



    private void AddBlockFaceToMesh(Block block, BlockFace face) {
        Vector3[] faceVertices = GetBlockFaceVertices(block.Position, face);
        Vector2[] faceTexCoords = block.GetTextureCoordinates();
        int startIndex = vertices.Count / 5; // Update to account for position and texture coordinates

        // Add vertices for specific face
        for (int i = 0; i < faceVertices.Length; i++) {
            vertices.Add(faceVertices[i].X);
            vertices.Add(faceVertices[i].Y);
            vertices.Add(faceVertices[i].Z);
            vertices.Add(faceTexCoords[i].X);
            vertices.Add(faceTexCoords[i].Y);
        }

        // Add indices for specific face
        indices.Add((uint)startIndex);
        indices.Add((uint)(startIndex + 1));
        indices.Add((uint)(startIndex + 2));
        indices.Add((uint)(startIndex + 2));
        indices.Add((uint)(startIndex + 3));
        indices.Add((uint)(startIndex));
    }



    private Vector2[] GetBlockFaceTexCoords(Vector2 offset, Vector2 scale) {
        // Generates texture coordinates for each block face based on the offset and scale
        return new Vector2[] {
            offset,
            new Vector2(offset.X + scale.X, offset.Y),
            new Vector2(offset.X + scale.X, offset.Y + scale.Y),
            new Vector2(offset.X, offset.Y + scale.Y)
        };
    }



    private Vector3[] GetBlockFaceVertices(Vector3 position, BlockFace face) {
        // Returns the four vertices of the block face based on the position and face
        Vector3[] vertices = new Vector3[4];

        switch (face) {
            case BlockFace.Top: // Top face
                vertices[0] = position + new Vector3(0f, 1f, 1f);
                vertices[1] = position + new Vector3(1f, 1f, 1f);
                vertices[2] = position + new Vector3(1f, 1f, 0f);
                vertices[3] = position + new Vector3(0f, 1f, 0f);
                break;
            case BlockFace.Bottom: // Bottom face
                vertices[0] = position + new Vector3(0f, 0f, 1f);
                vertices[1] = position + new Vector3(0f, 0f, 0f);
                vertices[2] = position + new Vector3(1f, 0f, 0f);
                vertices[3] = position + new Vector3(1f, 0f, 1f);
                break;
            case BlockFace.Left: // Left face
                vertices[0] = position + new Vector3(0f, 1f, 0f);
                vertices[1] = position + new Vector3(0f, 0f, 0f);
                vertices[2] = position + new Vector3(0f, 0f, 1f);
                vertices[3] = position + new Vector3(0f, 1f, 1f);
                break;
            case BlockFace.Right: // Right face
                vertices[0] = position + new Vector3(1f, 1f, 1f);
                vertices[1] = position + new Vector3(1f, 0f, 1f);
                vertices[2] = position + new Vector3(1f, 0f, 0f);
                vertices[3] = position + new Vector3(1f, 1f, 0f);
                break;
            case BlockFace.Front: // Front face
                vertices[0] = position + new Vector3(0f, 1f, 1f);
                vertices[1] = position + new Vector3(0f, 0f, 1f);
                vertices[2] = position + new Vector3(1f, 0f, 1f);
                vertices[3] = position + new Vector3(1f, 1f, 1f);
                break;
            case BlockFace.Back: // Back face
                vertices[0] = position + new Vector3(1f, 1f, 0f);
                vertices[1] = position + new Vector3(1f, 0f, 0f);
                vertices[2] = position + new Vector3(0f, 0f, 0f);
                vertices[3] = position + new Vector3(0f, 1f, 0f);
                break;
            default:
                throw new ArgumentException("Invalid face");
        }

        // Return all vertices
        return vertices;
    }



    private void GenerateBuffers() {
        // Generate VAO
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        // Generate VBO
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * sizeof(float), vertices.ToArray(), BufferUsageHint.StaticDraw);

        // Generate EBO
        ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(uint), indices.ToArray(), BufferUsageHint.StaticDraw);

        // Setup vertex attribute pointers
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindVertexArray(0);
    }



    public void Render() {
        // Render chunk mesh
        GL.BindVertexArray(vao);
        GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }
}