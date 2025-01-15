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
    public static ConcurrentDictionary<Vector3, Block> blockLookup = new ConcurrentDictionary<Vector3, Block>(); // Thread-safe dictionary lookup system



    public Chunk() {
        chunkBlocks = new List<Block>(); // Initialize the list of blocks for this chunk
    }



    public Block? GetAdjacentBlock(Block block, Vector3 direction) {
        // Calculates position of the adjacent block and uses dictionary lookup system to get block
        Vector3 adjacentPosition = block.Position + direction;
        if (blockLookup.TryGetValue(adjacentPosition, out Block? adjacentBlock)) return adjacentBlock;

        return null;
    }



    public void GenerateChunk() {
        int chunkSize = 16; // Chunk size
    
        // Generate chunk with perlin noise
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int z = 0; z < chunkSize; z++) {
                    BlockType.AddGrassBlock(x, y, z, this);
                }
            }
        }
    }



    public void RenderChunkMesh() {
        // All six directions of block
        Vector3[] directions = new Vector3[]
        {
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
            // Set up matrix for each block in chunk
            Matrix4 model = Matrix4.CreateTranslation(block.Position);
            GL.UniformMatrix4(Game.modelLoc, false, ref model);
            block.Render();

            block.VisibleFaces = BlockFace.None;
            
            // Checks each direction for adjacent block
            for (int i = 0; i < directions.Length; i++) {
                if (GetAdjacentBlock(block, directions[i]) == null) {
                    block.SetFaceVisible(faces[i]); // Set visibility
                }
            }
        }
    }
}