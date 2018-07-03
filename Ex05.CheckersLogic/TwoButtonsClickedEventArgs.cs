using System;
using System.Drawing;

namespace Ex05.CheckersLogic
{
    public class TwoPictureBoxesClickedEventArgs : EventArgs
    {
        private Point m_DestLocation;
        private Point m_StartLocation;

        public Point StartLocation
        {
            get { return m_StartLocation; }
            set { m_StartLocation = value; }
        }

        public Point DestLocation
        {
            get { return m_DestLocation; }
            set { m_DestLocation = value; }
        }
    }
}