using OpenTK.Mathematics;


/// ---------------Chunk System---------------
/// 
/// Generates a chunk mesh around the camera.
/// 
/// ------------------------------------------


public class ChunkSystem {
    public static List<Chunk> chunks = new List<Chunk>(); // List of chunks
    private Vector3 lastChunkPosition; // Last chunk position



    public void GenerateChunkAtCameraPosition(Vector3 cameraPosition) {
        int chunkX = (int)Math.Floor(cameraPosition.X / Chunk.chunkWidth);
        int chunkZ = (int)Math.Floor(cameraPosition.Z / Chunk.chunkWidth);

        Vector3 chunkPosition = new Vector3(chunkX * Chunk.chunkWidth, 0, chunkZ * Chunk.chunkWidth);

        // Generate chunk if chunk does not exist at position
        if (chunkPosition != lastChunkPosition) {
            Chunk newChunk = new Chunk(chunkPosition);
            newChunk.GenerateChunk();
            newChunk.GenerateChunkMesh();

            chunks.Add(newChunk); // Add new chunk

            // Add blocks to global block lookup
            foreach (var block in newChunk.chunkBlocks) {
                Chunk.blockLookup[block.Position] = block;
            }

            UpdateAdjacentChunks(newChunk); // Update adjacent chunks to cull shared faces

            lastChunkPosition = chunkPosition;
        }
    }



    private void UpdateAdjacentChunks(Chunk newChunk) {
        // Updates adjacent chunks to cull shared faces
        Vector3[] directions = new Vector3[] {
            new Vector3(Chunk.chunkWidth, 0, 0),
            new Vector3(-Chunk.chunkWidth, 0, 0),
            new Vector3(0, 0, Chunk.chunkWidth),
            new Vector3(0, 0, -Chunk.chunkWidth)
        };

        foreach (var direction in directions) {
            Vector3 adjacentChunkPosition = newChunk.Position + direction;
            foreach (var chunk in chunks) {
                if (chunk.Position == adjacentChunkPosition) {
                    chunk.GenerateChunkMesh();
                    break;
                }
            }
        }
    }



    public void RenderChunks() {
        // Render each chunk in list of chunks
        foreach (var chunk in chunks) {
            chunk.Render();
        }
    }
}