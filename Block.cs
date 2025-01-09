using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


public class Block 
{
    public int VAO { get; private set; } // Vertex array object
    public int VBO { get; private set; } // Vertex buffer object
    public int EBO { get; private set; } // Element buffer object
    public Vector3 Position { get; set; } // Position of block



    public Block() {
        // Vertice array
        float[] vertices = {
           // Back face
            1f, 1f, 0f,  
            1f, 0f, 0f,  
            0f, 0f, 0f,  
            0f, 1f, 0f,  
            
            // Front face
            0f, 1f, 1f,  
            0f, 0f, 1f,  
            1f, 0f, 1f, 
            1f, 1f, 1f,  

            // Right face
            1f, 1f, 1f,  
            1f, 0f, 1f,  
            1f, 0f, 0f,  
            1f, 1f, 0f,  

            // Left face
            0f, 1f, 0f,  
            0f, 0f, 0f,  
            0f, 0f, 1f,  
            0f, 1f, 1f,  

            // Top face
            0f, 1f, 1f,  
            1f, 1f, 1f,  
            1f, 1f, 0f,  
            0f, 1f, 0f,  

            // Bottom face
            0f, 0f, 1f,  
            0f, 0f, 0f,  
            1f, 0f, 0f,  
            1f, 0f, 1f,  
        };

        // Indice array
        uint[] indices = {
            // Back face
            0, 1, 2, 2, 3, 0,
            // Front face
            4, 5, 6, 6, 7, 4,
            // Right face
            8, 9, 10, 10, 11, 8,
            // Left face
            12, 13, 14, 14, 15, 12,
            // Top face
            16, 17, 18, 18, 19, 16,
            // Bottom face
            20, 21, 22, 22, 23, 20
        };

        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EBO = GL.GenBuffer();

        // Bind vao, vbo and ebo
        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);
    }



    public void Render() {
        // Render object
        GL.BindVertexArray(VAO);
        GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
    }



    public void Dispose() {
        // Delete object
        GL.DeleteBuffer(VBO);
        GL.DeleteBuffer(EBO);
        GL.DeleteVertexArray(VAO);
    }
}
