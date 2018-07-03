using System.Collections.Generic;
using System.Drawing;

namespace Ex05.CheckersLogic
{
    public class BoardCell
    {
        public enum eSigns
        {
            Empty,
            FirstPlayerSign,
            FirstPlayerKingSign,
            SecondPlayerSign,
            SecondPlayerKingSign,
        }

        public enum eDirection
        {
            Up,
            Down,
            UpAndDown,
            None,
        }

        private Point                             m_Location = new Point();
        private Dictionary<Point, Game.eMoveType> m_MovingOptions = null;
        private eSigns                            m_Sign;
        private eDirection                        m_Direction = eDirection.None;
        private Game.ePlayerId                    m_OwnerId = Game.ePlayerId.None;

        public static eSigns GetSignFromPlayerId(Game.ePlayerId i_PlayerId)
        {
            eSigns sign;

            if (i_PlayerId == Game.ePlayerId.None)
            {
                sign = eSigns.Empty;
            }
            else
            {
                sign = i_PlayerId == Game.ePlayerId.Player1 ? eSigns.FirstPlayerSign : eSigns.SecondPlayerSign;
            }

            return sign;
        }

        public static eSigns GetKingSignFromPlayerId(Game.ePlayerId i_PlayerId)
        {
            return i_PlayerId == Game.ePlayerId.Player1 ? eSigns.FirstPlayerKingSign : eSigns.SecondPlayerKingSign;
        }

        public eSigns Sign
        {
            get { return m_Sign; }
            set { m_Sign = value; }
        }

        public eDirection Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public Dictionary<Point, Game.eMoveType> OptionsList
        {
            get { return m_MovingOptions; }
            set { m_MovingOptions = value; }
        }

        public Point Location
        {
            get { return m_Location; }
        }

        public Game.ePlayerId OwnerId
        {
            get { return m_OwnerId; }
            set { m_OwnerId = value; }
        }

        public bool NeedToJumpOver()
        {
            return m_MovingOptions.ContainsValue(Game.eMoveType.JumpOver);
        }

        public bool IsPossiableMove(Point i_NextLocation)
        {
            return m_MovingOptions != null ? m_MovingOptions.ContainsKey(i_NextLocation) : false;
        }

        public void InitializeCell(Point i_Location, Game.ePlayerId i_OwnerId)
        {
            m_OwnerId = i_OwnerId;
            m_Sign = GetSignFromPlayerId(i_OwnerId);
            m_Location = i_Location;
            if (m_Sign != eSigns.Empty)
            {
                if (m_Sign == eSigns.FirstPlayerSign)
                {
                    m_Direction = eDirection.Up;
                }
                else if (m_Sign == eSigns.SecondPlayerSign)
                {
                    m_Direction = eDirection.Down;
                }

                m_MovingOptions = new Dictionary<Point, Game.eMoveType>();
            }
        }

        public void ClearList()
        {
            if (m_MovingOptions != null)
            {
                m_MovingOptions.Clear();
            }
        }
    }
}