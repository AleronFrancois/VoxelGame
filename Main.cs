

/// ------------------Program------------------
/// 
/// Runs the game and sets window resolution and title.
/// 
/// -------------------------------------------


class Program
{
    static void Main(string[] args) {
        // Initialize and run the game
        using (Game game = new Game(1600, 900, "3D Engine         FPS:                Position: ")) {
            game.Run();
        }
    }
}