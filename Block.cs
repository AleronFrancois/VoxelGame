using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


public class Block : IDisposable
{
    public int VAO { get; private set; } // Vertex Array Object
    public int VBO { get; private set; } // Vertex Buffer Object
    public int EBO { get; private set; } // Element Buffer Object
    public Vector3 Position { get; private set; } // Position of block


    private static float[][] Vertices = new float[][] {
        // Back face
        new float[] { 
            1f, 1f, 0f,  
            1f, 0f, 0f,  
            0f, 0f, 0f,  
            0f, 1f, 0f,  
        },
        // Front face
        new float[] { 
            0f, 1f, 1f,  
            0f, 0f, 1f,  
            1f, 0f, 1f, 
            1f, 1f, 1f,  
        },
        // Right face
        new float[] { 
            1f, 1f, 1f,  
            1f, 0f, 1f,  
            1f, 0f, 0f,  
            1f, 1f, 0f, 
        },
        // Left face
        new float[] { 0f, 1f, 0f,  
            0f, 0f, 0f,  
            0f, 0f, 1f,  
            0f, 1f, 1f, 
        },
        // Top face
        new float[] { 
            0f, 1f, 1f,  
            1f, 1f, 1f,  
            1f, 1f, 0f,  
            0f, 1f, 0f, 
        },
        // Bottom face
        new float[] { 
            0f, 0f, 1f,  
            0f, 0f, 0f,  
            1f, 0f, 0f,  
            1f, 0f, 1f, 
        }
    };


    public Block(Vector3 position, bool[] visibleFaces) {
        Position = position;
        InitializeBuffers(visibleFaces);
    }


    private void InitializeBuffers(bool[] visibleFaces) {
        List<float> verticesList = new List<float>();
        List<uint> indicesList = new List<uint>();
        uint indexOffset = 0;

        for (int i = 0; i < visibleFaces.Length; i++) {
            if (visibleFaces[i]) {
                AddFace(verticesList, indicesList, Vertices[i], indexOffset);
                indexOffset += 4;
            }
        }

        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EBO = GL.GenBuffer();

        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, verticesList.Count * sizeof(float), verticesList.ToArray(), BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indicesList.Count * sizeof(uint), indicesList.ToArray(), BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }


    private static void AddFace(List<float> vertices, List<uint> indices, float[] faceVertices, uint indexOffset) {
        vertices.AddRange(faceVertices);
        indices.AddRange(new uint[]
        {
            indexOffset, indexOffset + 1, indexOffset + 2,
            indexOffset + 2, indexOffset + 3, indexOffset
        });
    }


    public void Render() {
        GL.BindVertexArray(VAO);
        GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
    }


    public void Dispose() {
        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(EBO);
        GL.DeleteVertexArray(VAO);
    }
}



public static class BlockManager
{
    private static readonly List<Block> blocks = new List<Block>();


    public static void AddBlock(float x, float y, float z, bool[] visibleFaces) {
        Block block = new Block(new Vector3(x, y, z), visibleFaces);
        blocks.Add(block);
    }


    public static void RenderBlocks() {
        foreach (Block block in blocks) block.Render();
    }


    public static void DisposeBlocks() {
        foreach (Block block in blocks) block.Dispose();
        blocks.Clear();
    }
}