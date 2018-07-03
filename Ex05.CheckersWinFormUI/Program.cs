namespace Ex05.CheckersWinFormUI
{
    public class Program
    {
        // $G$ SFN-013 (+8) Bonus: UI with richer graphics / motion / sound.

        public static void Main()
        {
            GameManager gameManager = new GameManager();

            gameManager.Run();
        }
    }
}