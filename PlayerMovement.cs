using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;


class PlayerMovement {
    private Player player;
    private Camera camera;
    private float speed = 3.0f;

    public PlayerMovement(Player player, Camera camera) {
        this.player = player;
        this.camera = camera;
    }

    public void HandleKeyboardInput(KeyboardState input, float deltaTime) {
        Vector3 direction = Vector3.Zero;

        if (input.IsKeyDown(Keys.W)) direction += camera.Front;
        if (input.IsKeyDown(Keys.S)) direction -= camera.Front;
        if (input.IsKeyDown(Keys.A)) direction -= camera.Right;
        if (input.IsKeyDown(Keys.D)) direction += camera.Right;

        if (input.IsKeyDown(Keys.LeftShift)) speed = 10.0f;
        else speed = 3.0f;

        if (direction != Vector3.Zero) {
            direction.Y = 0; // Keep movement on the XZ plane
            direction = Vector3.Normalize(direction);
            player.Position += direction * speed * deltaTime;
        }
    } 
}