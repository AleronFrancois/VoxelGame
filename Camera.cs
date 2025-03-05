using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;


public class Camera {
    // Camera properties
    public Vector3 Position { get; set; }
    public Vector3 Front { get; private set; }
    public Vector3 Up { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 WorldUp { get; private set; }
    private float yaw;
    private float pitch;
    private float speed;
    private float sensitivity;



    public Camera(Vector3 position, Vector3 up, float yaw, float pitch) {
        // Camera properties
        Position = position;
        WorldUp = up;
        Front = -Vector3.UnitZ;
        this.yaw = yaw;
        this.pitch = pitch;
        speed = 2.5f;
        sensitivity = 0.05f;
        UpdateCameraVectors();
    }



    public Matrix4 GetViewMatrix() {
        // Calculate the view matrix
        return Matrix4.LookAt(Position, Position + Front, Up);
    }



    public void ProcessMouseMovement(float xOffset, float yOffset, bool constrainPitch = true) {
        // Apply sensitivity to offsets
        xOffset *= sensitivity;
        yOffset *= sensitivity;

        yaw += xOffset;
        pitch -= yOffset;

        if (constrainPitch) {
            if (pitch > 89.0f)
                pitch = 89.0f;
            if (pitch < -89.0f)
                pitch = -89.0f;
        }

        UpdateCameraVectors();
    }



    public void HandleKeyboardInput(KeyboardState input, float deltaTime) {   
        // Handle WASDQE movment
        float velocity = speed * deltaTime;
        if (input.IsKeyDown(Keys.W)) Position += Front * velocity;
        if (input.IsKeyDown(Keys.S)) Position -= Front * velocity;
        if (input.IsKeyDown(Keys.A)) Position -= Right * velocity;
        if (input.IsKeyDown(Keys.D)) Position += Right * velocity;
        if (input.IsKeyDown(Keys.Q)) Position += WorldUp * velocity;
        if (input.IsKeyDown(Keys.E)) Position -= WorldUp * velocity;

        // Shift to increase speed
        if (input.IsKeyDown(Keys.LeftShift)) speed = 30.0f;
        else speed = 2.5f;
    }   



    public void HandleMouseInput(Vector2 mouseDelta) {
        // Process mouse movement
        ProcessMouseMovement(mouseDelta.X, mouseDelta.Y);
    }



    private void UpdateCameraVectors() {
        // Calculate the new front vector
        Vector3 front;
        front.X = MathF.Cos(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
        front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
        front.Z = MathF.Sin(MathHelper.DegreesToRadians(yaw)) * MathF.Cos(MathHelper.DegreesToRadians(pitch));
        Front = Vector3.Normalize(front);
        Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
}
