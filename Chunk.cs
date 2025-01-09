
using SimplexNoise;


/// <summary>
/// Responsible for generating a chunk of blocks using perlin noise.
/// Determines the visibility of each block's faces based on its neighbors.
/// </summary>


class Chunk 
{
    public void GenerateChunk() {
        int chunkSize = 10; // Chunk size
        float scale = 0.1f; // Scale for Perlin noise
        int heightMultiplier = 5; // Height multiplier for hills
        int surfaceLevel = 5; // Surface level where Perlin noise starts
        bool[,,] blocks = new bool[chunkSize, chunkSize, chunkSize];

        // Generate chunk with perlin noise
        for (int x = 0; x < chunkSize; x++) {
            for (int z = 0; z < chunkSize; z++) {

                // Generate height using perlin noise
                float height = Noise.CalcPixel2D(x, z, scale) / 255.0f * heightMultiplier;
                int intHeight = surfaceLevel + (int)height;

                for (int y = 0; y < chunkSize; y++) {
                    blocks[x, y, z] = y <= intHeight;
                }
            }
        }

        // Calculate neighbors
        for (int x = 0; x < chunkSize; x++) {
            for (int y = 0; y < chunkSize; y++) {
                for (int z = 0; z < chunkSize; z++) {
                    if (blocks[x, y, z]) {
                        bool[] visibleFaces = new bool[6];
                        visibleFaces[0] = (z == 0 || !blocks[x, y, z - 1]);             // Back face
                        visibleFaces[1] = (z == chunkSize - 1 || !blocks[x, y, z + 1]); // Front face
                        visibleFaces[2] = (x == chunkSize - 1 || !blocks[x + 1, y, z]); // Right face
                        visibleFaces[3] = (x == 0 || !blocks[x - 1, y, z]);             // Left face
                        visibleFaces[4] = (y == chunkSize - 1 || !blocks[x, y + 1, z]); // Top face
                        visibleFaces[5] = (y == 0 || !blocks[x, y - 1, z]);             // Bottom face

                        // Process only if at least one face is visible
                        if (visibleFaces.Any(face => face)) {
                            Game.AddBlock(x, y, z, visibleFaces);
                        }
                    }
                }
            }
        }
    }
}
