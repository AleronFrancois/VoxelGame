using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;


public class Input {
    public static void DeveloperTools(KeyboardState input, GameWindow window, Game game) {
        if (input.IsKeyDown(Keys.Escape)) window.Close(); // Exit window

        // Toggle wireframe mode
        if (input.IsKeyPressed(Keys.D1)) {
            if (Game.wireframe) {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                Game.wireframe = false;
            } else {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                Game.wireframe = true;
            }
        }

        // Toggle cursor visibility
        if (input.IsKeyPressed(Keys.D2)) {
            if (!Game.cursorGrabbed) {
                window.CursorState = CursorState.Grabbed;
                Game.cursorGrabbed = true;
            } else {
                window.CursorState = CursorState.Normal;
                Game.cursorGrabbed = false;
            }
        }

        // Toggle camera attachment
        if (input.IsKeyPressed(Keys.D3)) {
            Game.attachCamera = !Game.attachCamera;
        }

        // Control camera boom
        if (Game.attachCamera) {
            Vector3 cameraOffset = -game.camera.Front * 7.0f + new Vector3(0.5f, 3.0f, 0.5f); 
            game.camera.Position = game.player.Position + cameraOffset;
        }
    }
}