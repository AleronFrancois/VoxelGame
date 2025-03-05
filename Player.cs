using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


public class Player {
    public static int VAO { get; private set; }
    public static int VBO { get; private set; }
    public static int EBO { get; private set; }
    public Vector3 Position { get; set; }


    private static float[] vertices = {
        1f, 2f, 0f,   
        1f, 0f, 0f,   
        0f, 0f, 0f,   
        0f, 2f, 0f,   
        // Front face
        0f, 2f, 1f,  
        0f, 0f, 1f,  
        1f, 0f, 1f,  
        1f, 2f, 1f,  
        // Right face
        1f, 2f, 1f,   
        1f, 0f, 1f,   
        1f, 0f, 0f,   
        1f, 2f, 0f,   
        // Left face
        0f, 2f, 0f,   
        0f, 0f, 0f, 
        0f, 0f, 1f, 
        0f, 2f, 1f,   
        // Top face
        0f, 2f, 1f, 
        1f, 2f, 1f,   
        1f, 2f, 0f,   
        0f, 2f, 0f,   
        // Bottom face
        0f, 0f, 1f,  
        0f, 0f, 0f,  
        1f, 0f, 0f, 
        1f, 0f, 1f, 
    };

    private static uint[] indices = {
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

    public Player(Vector3 startPosition) {
        Position = startPosition;
        InitialisePlayer();
    }

    private static void InitialisePlayer() {
        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EBO = GL.GenBuffer();

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

    public void Render(int shaderProgram) {
        GL.UseProgram(shaderProgram);

        // Update model matrix with player's position
        Matrix4 model = Matrix4.CreateTranslation(Position);
        int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
        GL.UniformMatrix4(modelLoc, false, ref model);

        GL.BindVertexArray(VAO);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);

        GL.BindVertexArray(0); // Unbind VAO once
    }
}