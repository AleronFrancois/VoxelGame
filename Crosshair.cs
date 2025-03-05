using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


public class Crosshair
{
    private int vao;
    private int vbo;
    private int ebo; // Add EBO for indices
    float crosshairScale = 0.04f; // Scale of crosshair
    

    float[] vertices = {
        // Horizontal rectangle
        -0.4f, -0.05f,
        0.4f, -0.05f, 
        0.4f,  0.05f,
        -0.4f,  0.05f,  

        // Vertical rectangle
        -0.05f, -0.4f, 
        0.05f, -0.4f, 
        0.05f,  0.4f, 
        -0.05f,  0.4f  
    };

    uint[] indices = {
        // Horizontal rectangle
        0, 1, 2, 
        2, 3, 0, 

        // Vertical rectangle
        4, 5, 6, 
        6, 7, 4  
    };



    public void InitialiseCrosshair() {
        // Create and bind the Vertex Array Object
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        // Create and bind the Vertex Buffer Object
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Create and bind the Element Buffer Object
        ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // Enable vertex attribute array
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Unbind the VAO (the EBO stays bound to the VAO)
        GL.BindVertexArray(0);
    }



    public void Render(int crosshairShaderProgram, int windowWidth, int windowHeight) {
        // Use the crosshair shader program
        GL.UseProgram(crosshairShaderProgram);

        // Set up orthographic projection
        Matrix4 projection = Matrix4.CreateOrthographicOffCenter(-1.0f, 1.0f, -1.0f, 1.0f, -1.0f, 1.0f);
        int projectionLoc = GL.GetUniformLocation(crosshairShaderProgram, "projection");
        GL.UniformMatrix4(projectionLoc, false, ref projection);

        // Set up scaling matrix with aspect ratio correction
        float aspectRatio = windowWidth / (float)windowHeight;
        Matrix4 scaling = Matrix4.CreateScale(crosshairScale / aspectRatio, crosshairScale, 1.0f);
        int scalingLoc = GL.GetUniformLocation(crosshairShaderProgram, "scaling");
        GL.UniformMatrix4(scalingLoc, false, ref scaling);

        // Bind the VAO and draw the crosshair
        GL.BindVertexArray(vao);
        GL.DrawElements(PrimitiveType.Triangles, 12, DrawElementsType.UnsignedInt, 0);
        GL.BindVertexArray(0);
    }



    public void Cleanup()
    {
        // Delete the VAO, VBO, and EBO
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
        GL.DeleteBuffer(ebo); // Delete EBO
    }
}