using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

/// ----------------Shader Program----------------
/// 
/// Handles and manages rendering graphics
/// 
/// --------------------------------------------

class Shader
{
    public int CreateShaderProgram() {
        string vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec3 aPos;
            layout(location = 1) in vec3 aNormal; // Add normal attribute

            uniform mat4 model;
            uniform mat4 view;
            uniform mat4 projection;

            out vec3 fragPos;
            out vec3 normal; // Pass normal to fragment shader

            void main()
            {
                fragPos = vec3(model * vec4(aPos, 1.0));
                normal = mat3(transpose(inverse(model))) * aNormal; // Transform normal to world space
                gl_Position = projection * view * model * vec4(aPos, 1.0);
            }";

        string fragmentShaderSource = @"
            #version 330 core
            in vec3 fragPos;
            out vec4 FragColor;

            uniform vec3 lightDirection;  // Direction from the sun (light)
            uniform vec3 lightColor;      // Color of the sunlight (yellowish for the sun)
            uniform vec3 objectColor;     // Color of the object being lit

            void main()
            {
                // Normalise light direction (sun is infinitely far away)
                vec3 lightDir = normalize(lightDirection);

                // Calculate the diffuse lighting (Lambertian reflection model)
                // Assuming fragPos is the normal
                vec3 normal = normalize(fragPos);
                float diff = max(dot(normal, -lightDir), 0.0);  // Directional light, so invert lightDir
                vec3 diffuse = diff * lightColor;

                // Combine the diffuse lighting with the object's color
                vec3 result = diffuse * objectColor;

                FragColor = vec4(result, 1.0);
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



    public int CreateCrosshairShaderProgram() {
        string vertexShaderSource = @"
            #version 330 core
            layout(location = 0) in vec2 aPos;

            uniform mat4 projection;
            uniform mat4 scaling;

            void main()
            {
                gl_Position = projection * scaling * vec4(aPos, 0.0, 1.0);
            }";

        string fragmentShaderSource = @"
            #version 330 core
            out vec4 FragColor;

            void main()
            {
                FragColor = vec4(1.0, 1.0, 1.0, 1.0); // White color for the crosshair
            }";

        int vertexShader = CompileShader(ShaderType.VertexShader, vertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentShaderSource);

        int shaderProgram = GL.CreateProgram();
        GL.AttachShader(shaderProgram, vertexShader);
        GL.AttachShader(shaderProgram, fragmentShader);
        GL.LinkProgram(shaderProgram);

        // Check for linking errors
        GL.GetProgram(shaderProgram, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)  {
            string infoLog = GL.GetProgramInfoLog(shaderProgram);
            Console.WriteLine($"Error linking shader program: {infoLog}");
        }

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return shaderProgram;
    }



    private int CompileShader(ShaderType type, string source) {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        // Check for compilation errors
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)  {
            string infoLog = GL.GetShaderInfoLog(shader);
            Console.WriteLine($"Error compiling {type}: {infoLog}");
        }

        return shader;
    }



    public void SetLightAndObjectColors(int shaderProgram, Vector3 lightDirection, Vector3 lightColor, Vector3 objectColor) {
        // Use the shader program
        GL.UseProgram(shaderProgram);

        // Set uniform values for light direction, light color, and object color
        int lightDirLocation = GL.GetUniformLocation(shaderProgram, "lightDirection");
        int lightColorLocation = GL.GetUniformLocation(shaderProgram, "lightColor");
        int objectColorLocation = GL.GetUniformLocation(shaderProgram, "objectColor");

        GL.Uniform3(lightDirLocation, lightDirection);
        GL.Uniform3(lightColorLocation, lightColor);
        GL.Uniform3(objectColorLocation, objectColor);
    }
}