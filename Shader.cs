using OpenTK.Graphics.OpenGL4;


class Shader
{
    public int CreateShaderProgram()  {
        string vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPos;

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            out vec3 fragPos;

            void main()
            {
                fragPos = aPos;
                gl_Position = projection * view * model * vec4(aPos, 1.0);
            }";

        string fragmentShaderSource = @"
            #version 330 core
            in vec3 fragPos;
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(0.0, 1.0, 1.0, 1.0);
            }";

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        // Check for linking errors
        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0) {
            string infoLog = GL.GetProgramInfoLog(shaderProgram);
            Console.WriteLine($"Error linking shader program: {infoLog}");
        }

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return shaderProgram;
    }

    private int CompileShader(ShaderType type, string source)  {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        // Check for compilation errors
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0) {
            string infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine($"Error compiling {type}: {infoLog}");
        }

        return shader;
    }
}