using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;



public class Input 
{
    public static void DeveloperTools(KeyboardState input, GameWindow window) {
        // Exit window
        if (input.IsKeyDown(Keys.Escape)) window.Close(); 

        // Toggle wireframe mode
        if (input.IsKeyPressed(Keys.D1)) {
            if (Game.wireframe) {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                Game.wireframe = false;
            } 
            else {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                Game.wireframe = true;
            }
        }

        // Toggle cursor visibility
        if (input.IsKeyPressed(Keys.D2)) {
            if (!Game.cursorGrabbed) {
                window.CursorState = CursorState.Grabbed;
                Game.cursorGrabbed = true;
            }
            else {
                window.CursorState = CursorState.Normal;
                Game.cursorGrabbed = false;
            }
        }
    }
}