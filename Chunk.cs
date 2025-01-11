using SimplexNoise;


/// ----------------Chunk System----------------
/// 
/// Generates a chunk mesh and applies perlin noise.
/// 
/// --------------------------------------------


class Chunk 
{
    public void GenerateChunk(int chunkX, int chunkZ) {
        int chunkSize = 16; // Chunk size
        float scale = 0.05f; // Scale for perlin noise
        int heightMultiplier = 5; // Height multiplier for hills
        int surfaceLevel = 5; // Surface level where Perlin noise starts
        bool[,,] blocks = new bool[chunkSize, chunkSize, chunkSize];

        // Generate chunk with perlin noise
        for (int x = 0; x < chunkSize; x++) {
            for (int z = 0; z < chunkSize; z++) {
                // Generate height using perlin noise
                float height = Noise.CalcPixel2D(x + chunkX * chunkSize, z + chunkZ * chunkSize, scale) / 255.0f * heightMultiplier;
                int intHeight = surfaceLevel + (int)height;

                for (int y = 0; y < chunkSize; y++) {
                    // Set blocks based on the height
                    blocks[x, y, z] = y <= intHeight;
                    
                    // Add the block only if its within the valid terrain height range
                    if (blocks[x, y, z]) {
                        Game.AddBlock(x + chunkX * chunkSize, y, z + chunkZ * chunkSize);
                    }
                }
            }
        }
    }
}
