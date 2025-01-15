using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class BlockType
{
    public static void AddGrassBlock(float x, float y, float z, Chunk chunk) {
        Block block = new Block();
        block.Position = new Vector3(x, y, z);

        if (Chunk.blockLookup.TryAdd(block.Position, block)) {
            chunk.chunkBlocks.Add(block); // Add block to chunk's list
        } 
    }
}