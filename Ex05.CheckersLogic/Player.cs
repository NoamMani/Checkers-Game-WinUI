using System.Collections.Generic;
using System.Drawing;

namespace Ex05.CheckersLogic
{
    public class Player
    {
        private readonly List<Point> r_PlayerCheckers = null;
        private string               m_Name;
        private int                  m_Score;
        private Game.ePlayerId       m_Id;

        public Player(string i_Name, Game.ePlayerId i_PlayerSerial)
        {
            m_Name = i_Name;
            m_Id = i_PlayerSerial;
            r_PlayerCheckers = new List<Point>();
        }

        public int Score
        {
            get { return m_Score; }
            set { m_Score = value; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public List<Point> ChekersList
        {
            get { return r_PlayerCheckers; }
        }

        public Game.ePlayerId Id
        {
            get { return m_Id; }
        }

        public void AddChecker(Point i_CheckerLocation)
        {
            r_PlayerCheckers.Add(i_CheckerLocation);
        }

        public void RemoveChecker(Point i_CheckerLocation)
        {
            r_PlayerCheckers.Remove(i_CheckerLocation);
        }

        public void Move(Point i_CurrentLocation, Point i_NextLocation)
        {
            RemoveChecker(i_CurrentLocation);
            AddChecker(i_NextLocation);
        }

        public bool IsMyChecker(Point i_CheckerLocation)
        {
            return r_PlayerCheckers.Contains(i_CheckerLocation);
        }

        public void ClearPlayerChekers()
        {
            r_PlayerCheckers.Clear();
        }
    }
}