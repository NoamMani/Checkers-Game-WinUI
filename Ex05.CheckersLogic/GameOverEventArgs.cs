using System;

namespace Ex05.CheckersLogic
{
    public class GameOverEventArgs : EventArgs
    {
        private Game.ePlayerId m_Winner;
        private bool           m_Tie;

        public bool Tie
        {
            get { return m_Tie; }
            set { m_Tie = value; }
        }

        public Game.ePlayerId Winner
        {
            get { return m_Winner; }
            set { m_Winner = value; }
        }
    }
}