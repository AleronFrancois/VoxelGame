using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;


public class Crosshair
{
    private int vao;
    private int vbo;
    float crosshairScale = 0.25f; // Scale of crosshair
    

    private float[] vertices = {
        // Horizontal line
        -0.05f, 0.0f,  // Left point
        0.05f, 0.0f,   // Right point

        // Vertical line
        0.0f, -0.05f,  // Bottom point
        0.0f,  0.05f   // Top point
    };



    public void InitialiseCrosshair() {
        // Create and bind the Vertex Array Object
        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        // Create and bind the Vertex Buffer Object
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // Enable vertex attribute array
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // Unbind the VAO
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
        GL.DrawArrays(PrimitiveType.Lines, 0, vertices.Length / 2);
        GL.BindVertexArray(0);
    }



    public void Cleanup()
    {
        // Delete the VAO and VBO
        GL.DeleteVertexArray(vao);
        GL.DeleteBuffer(vbo);
    }
}