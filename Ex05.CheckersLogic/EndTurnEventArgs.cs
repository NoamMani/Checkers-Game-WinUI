using System;

namespace Ex05.CheckersLogic
{
    public class EndTurnEventArgs : EventArgs
    {
        private Game.eMessage m_Message;

        public Game.eMessage Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
    }
}