using System;
using Ex05.CheckersLogic;

namespace Ex05.CheckersWinFormUI
{
    public class GameManager
    {
        private Game          m_Game;
        private FormCheckersGame m_FormGame = new FormCheckersGame();

        public void Run()
        {
            m_FormGame.Shown += m_FormGame_Shown;
            m_FormGame.ShowDialog();
        }

        private void registerToEvents()
        {
            m_FormGame.TwoPictureBoxesClicked += m_FormGame_m_TwoPictureBoxesClicked;
            m_FormGame.StartNewGame += m_FormGame_StartNewGame;
            m_Game.EndUnsuccessfulTurn += m_Game_EndUnsuccessfulTurn;
            m_Game.BoardUpdated += m_Game_BoardUpdated;
            m_Game.GameOver += m_Game_gameOver;
        }

        private void m_FormGame_Shown(object sender, EventArgs e)
        {
            initializeLogicGame();
            registerToEvents();
        }

        private void m_FormGame_m_TwoPictureBoxesClicked(TwoPictureBoxesClickedEventArgs e)
        {
            m_Game.PlayTurn(e.StartLocation, e.DestLocation);
        }

        private void m_FormGame_StartNewGame()
        {
            m_Game.StartNewGame();
        }

        private void m_Game_BoardUpdated(object sender)
        {
            m_FormGame.UpdateBoard(sender);
        }

        private void m_Game_EndUnsuccessfulTurn(object sender, EndTurnEventArgs e)
        {
            m_FormGame.DisplayErrorMessage(sender, e);
        }
   
        private void m_Game_gameOver(object sender, GameOverEventArgs e)
        {
            m_FormGame.EndGame(sender, e);
        }

        private void initializeLogicGame()
        {
            int    boardSize;
            string playerName1, playerName2 = null;

            playerName1 = m_FormGame.SettingsForm.PlayerName1;
            boardSize = m_FormGame.SettingsForm.BoardSize;
            if (m_FormGame.SettingsForm.IsVSPlayer2)
            {
                playerName2 = m_FormGame.SettingsForm.PlayerName2;
            }

            m_Game = new Game(playerName1, playerName2, boardSize);
            m_Game.StartNewGame();
            m_FormGame.InitializeGameForm(m_Game);
        }
    }
}
