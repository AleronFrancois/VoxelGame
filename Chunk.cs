using SimplexNoise;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Concurrent;

/// ---------------Chunk System---------------
/// 
/// Generates a chunk mesh by checking each block
/// for neighboring, and only rendering faces
/// that are adjcacent to air/null.
///
/// --------------------------------------------

public class Chunk
{
    public List<Block> chunkBlocks; // List of blocks in this chunk
    public static ConcurrentDictionary<Vector3, Block> blockLookup = new ConcurrentDictionary<Vector3, Block>(); // Dictionary lookup system

    private List<float> vertices = new List<float>(); // For storing vertex data (positions, normals, etc.)
    private List<uint> indices = new List<uint>();  // For storing the indices of the mesh
    private int vao, vbo, ebo; // OpenGL objects



    public Chunk() {
        chunkBlocks = new List<Block>(); // Initialises a new list of blocks for each chunk
    }



    public Block? GetAdjacentBlock(Block block, Vector3 direction) {
        // Calculates position of the adjacent block and uses dictionary lookup system to get block
        Vector3 adjacentPosition = block.Position + direction;
        if (blockLookup.TryGetValue(adjacentPosition, out Block? adjacentBlock)) return adjacentBlock;

        return null;
    }



    public void GenerateChunk()
    {
        int chunkWidth = 16; // Chunk width
        int chunkHeight = 32; // Chunk height
    
        // Generate and fill chunk
        for (int x = 0; x < chunkWidth; x++) {
            for (int y = 0; y < chunkHeight; y++) {
                for (int z = 0; z < chunkWidth; z++) {
                    BlockType.AddGrassBlock(x, y, z, this); // Add block
                }
            }
        }
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
                // Finds adjacent air block and sets face visibility appropriately
                if (GetAdjacentBlock(block, directions[i]) == null) {
                    block.SetFaceVisible(faces[i]); // Set visibility accordingly
                    AddBlockFaceToMesh(block, faces[i]); // Add the block face to the mesh
                }
            }
        }
        
        // Generate buffers after mesh is constructed
        GenerateBuffers();
    }



    private void AddBlockFaceToMesh(Block block, BlockFace face) {
        Vector3[] faceVertices = GetBlockFaceVertices(block.Position, face);
        int startIndex = vertices.Count / 3; // Keep track of the number of vertices added

        // Add the 4 vertices for this face
        foreach (var vertex in faceVertices) {
            vertices.Add(vertex.X);
            vertices.Add(vertex.Y);
            vertices.Add(vertex.Z);
        }

        // Add indices for two triangles forming the face
        indices.Add((uint)startIndex);
        indices.Add((uint)(startIndex + 1));
        indices.Add((uint)(startIndex + 2));
        indices.Add((uint)(startIndex + 2));
        indices.Add((uint)(startIndex + 3));
        indices.Add((uint)(startIndex));
    }



    private Vector3[] GetBlockFaceVertices(Vector3 position, BlockFace face) {
        // Returns the four vertices of the block face based on the position and face
        Vector3[] vertices = new Vector3[4];

        switch (face) {
            case BlockFace.Top:
                vertices[0] = position + new Vector3(-0.5f, 0.5f, -0.5f);
                vertices[1] = position + new Vector3(0.5f, 0.5f, -0.5f);
                vertices[2] = position + new Vector3(0.5f, 0.5f, 0.5f);
                vertices[3] = position + new Vector3(-0.5f, 0.5f, 0.5f);
                break;
            case BlockFace.Bottom:
                vertices[0] = position + new Vector3(-0.5f, -0.5f, -0.5f);
                vertices[1] = position + new Vector3(0.5f, -0.5f, -0.5f);
                vertices[2] = position + new Vector3(0.5f, -0.5f, 0.5f);
                vertices[3] = position + new Vector3(-0.5f, -0.5f, 0.5f);
                break;
            case BlockFace.Left:
                vertices[0] = position + new Vector3(-0.5f, 0.5f, -0.5f);
                vertices[1] = position + new Vector3(-0.5f, -0.5f, -0.5f);
                vertices[2] = position + new Vector3(-0.5f, -0.5f, 0.5f);
                vertices[3] = position + new Vector3(-0.5f, 0.5f, 0.5f);
                break;
            case BlockFace.Right:
                vertices[0] = position + new Vector3(0.5f, 0.5f, -0.5f);
                vertices[1] = position + new Vector3(0.5f, -0.5f, -0.5f);
                vertices[2] = position + new Vector3(0.5f, -0.5f, 0.5f);
                vertices[3] = position + new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case BlockFace.Front:
                vertices[0] = position + new Vector3(-0.5f, 0.5f, 0.5f);
                vertices[1] = position + new Vector3(0.5f, 0.5f, 0.5f);
                vertices[2] = position + new Vector3(0.5f, -0.5f, 0.5f);
                vertices[3] = position + new Vector3(-0.5f, -0.5f, 0.5f);
                break;
            case BlockFace.Back:
                vertices[0] = position + new Vector3(-0.5f, 0.5f, -0.5f);
                vertices[1] = position + new Vector3(0.5f, 0.5f, -0.5f);
                vertices[2] = position + new Vector3(0.5f, -0.5f, -0.5f);
                vertices[3] = position + new Vector3(-0.5f, -0.5f, -0.5f);
                break;
            default:
                throw new ArgumentException("Invalid face");
        }

        return vertices;
    }



    private void GenerateBuffers() {
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

        // Setup vertex attribute pointers (assuming position-only for simplicity)
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindVertexArray(0);
    }



    public void Render() {
        GL.BindVertexArray(vao);
        GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }
}