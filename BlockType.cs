using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class BlockType
{
    public static int GrassTextureID { get; private set; }

    public static void LoadTextures() {
        GrassTextureID = TextureLoader.LoadTexture("C:/Users/Aleron/Documents/VoxelGame/TextureAtlas.png");

        // Set texture parameters
        GL.BindTexture(TextureTarget.Texture2D, GrassTextureID);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
    }

    public static void AddGrassBlock(float x, float y, float z, Chunk chunk) {
        Block block = new Block();
        block.Position = new Vector3(x, y, z);

        if (Chunk.blockLookup.TryAdd(block.Position, block)) {
            chunk.chunkBlocks.Add(block); // Add block to chunk's list
        }

        // Set texture coordinates for the grass block to use the bottom right corner of the 2x2 texture atlas
        Vector2 textureCoordStart = new Vector2(0.5f, 0.0f); // Start point 
        Vector2 textureCoordEnd = new Vector2(1f, 0.5f); // End point

        block.SetTextureCoordinates(textureCoordStart, textureCoordEnd);
    }



    public static void AddDirtBlock(float x, float y, float z, Chunk chunk) {
        Block block = new Block();
        block.Position = new Vector3(x, y, z);

        if (Chunk.blockLookup.TryAdd(block.Position, block)) {
            chunk.chunkBlocks.Add(block); // Add block to chunk's list
        }

        // Set texture coordinates for the grass block to use the bottom right corner of the 2x2 texture atlas
        Vector2 textureCoordStart = new Vector2(0.5f, 0.5f); // Start point 
        Vector2 textureCoordEnd = new Vector2(1f, 1.0f); // End point

        block.SetTextureCoordinates(textureCoordStart, textureCoordEnd);
    }



    public static void AddStoneBlock(float x, float y, float z, Chunk chunk) {
        Block block = new Block();
        block.Position = new Vector3(x, y, z);

        if (Chunk.blockLookup.TryAdd(block.Position, block)) {
            chunk.chunkBlocks.Add(block); // Add block to chunk's list
        }

        // Set texture coordinates for the grass block to use the bottom right corner of the 2x2 texture atlas
        Vector2 textureCoordStart = new Vector2(0.0f, 0.5f); // Start point 
        Vector2 textureCoordEnd = new Vector2(0.5f, 1.0f); // End point

        block.SetTextureCoordinates(textureCoordStart, textureCoordEnd);
    }



    public static void AddSandBlock(float x, float y, float z, Chunk chunk) {
        Block block = new Block();
        block.Position = new Vector3(x, y, z);

        if (Chunk.blockLookup.TryAdd(block.Position, block)) {
            chunk.chunkBlocks.Add(block); // Add block to chunk's list
        }

        // Set texture coordinates for the grass block to use the bottom right corner of the 2x2 texture atlas
        Vector2 textureCoordStart = new Vector2(0.0f, 0.0f); // Start point 
        Vector2 textureCoordEnd = new Vector2(0.5f, 0.5f); // End point

        block.SetTextureCoordinates(textureCoordStart, textureCoordEnd);
    }
}