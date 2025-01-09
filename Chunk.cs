using SimplexNoise;


class Chunk 
{
    public void GenerateChunk() {
        int chunkSize = 10; // Chunk size
        float scale = 0.0f; // Perlin noise scale

        // Generate chunk with perlin noise
        for (int x = 0; x < chunkSize; x++) {
            for (int z = 0; z < chunkSize; z++) {
                float height = PerlinNoise(x * scale, z * scale) * chunkSize; // Apply perlin noise to chunk

                for (int y = 0; y < (int)height; y++) {
                    Game.AddBlock(0 + x, 0 + y, 0 + z);
                }
            }
        }
    }



    private float PerlinNoise(float x, float z) {
        // Calculate perlin noise
        return Noise.CalcPixel2D((int)(x * 100), (int)(z * 100), 0.1f) / 255f;
    }
}