class Program
{
    static void Main(string[] args) {
        // Initialize and run the game
        using (Game game = new Game(1600, 900, "3D Engine")) {
            game.Run();
        }
    }
}