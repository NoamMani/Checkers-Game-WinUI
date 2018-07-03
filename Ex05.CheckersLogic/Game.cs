using System;
using System.Drawing;

namespace Ex05.CheckersLogic
{
    // $G$ SFN-012 (+15) Bonus: Events in the Logic layer are handled by the UI.

    public delegate void GameOverEventHandler(object sender, GameOverEventArgs e);

    public delegate void UnsuccessfulTurnEventHandler(object sender, EndTurnEventArgs e);

    public delegate void UpdateBoardEventHandler(object sender);

    public class Game
    {
        internal const int k_Empty = 0;
        internal const int k_CheckerValue = 1;
        internal const int k_KingValue = 4;

        public enum eMoveType
        {
            SimpleMove,
            JumpOver,
        }

        public enum ePlayerId
        {
            Player1,
            Player2,
            Computer,
            None,
        }

        public enum eMessage
        {
            ActionNotValid,
            MustJumpOver,
            MustJumpOverAgain,
            EndTurn,
            EndGame,
        }

        private Board     m_Board;
        private Player[]  m_Players;
        private ePlayerId m_ActivePlayerId;

        public event GameOverEventHandler GameOver;

        public event UnsuccessfulTurnEventHandler EndUnsuccessfulTurn;

        public event UpdateBoardEventHandler BoardUpdated;

        public Game(string i_FirstPlayerName, string i_SecondPlayerName, int i_BoardSize)
        {
            m_Players = new Player[3];
            m_Players[(int)ePlayerId.Player1] = new Player(i_FirstPlayerName, ePlayerId.Player1);
            if (i_SecondPlayerName == null)
            {
                m_Players[(int)ePlayerId.Computer] = new Player("Computer", ePlayerId.Computer);
            }
            else
            {
                m_Players[(int)ePlayerId.Player2] = new Player(i_SecondPlayerName, ePlayerId.Player2);
            }

            m_Board = new Board(i_BoardSize);
        }

        public int BoardSize
        {
            get { return m_Board.BoardSize; }
        }

        public BoardCell.eSigns BoardCellSign(Point i_CellLocation)
        {
            return m_Board[i_CellLocation].Sign;
        }

        public ePlayerId FirstPlayerId
        {
            get { return m_Players[(int)ePlayerId.Player1].Id; }
        }

        public ePlayerId SecondPlayerId
        {
            get { return m_Players[(int)ePlayerId.Player2] == null ? ePlayerId.Computer : ePlayerId.Player2; }
        }

        public string FirstPlayerName
        {
            get { return m_Players[(int)FirstPlayerId].Name; }
        }

        public string SecondPlayerName
        {
            get { return m_Players[(int)SecondPlayerId].Name; }
        }

        public string GetPlayerNameById(ePlayerId i_PlayerId)
        {
            return m_Players[(int)i_PlayerId].Name;
        }

        public int FirstPlayerScore
        {
            get { return m_Players[(int)FirstPlayerId].Score; }
        }

        public int SecondPlayerScore
        {
            get { return m_Players[(int)SecondPlayerId].Score; }
        }

        private ePlayerId opponentId
        {
            get { return m_ActivePlayerId != FirstPlayerId ? FirstPlayerId : SecondPlayerId; }
        }

        public ePlayerId ActivePlayer
        {
            get { return m_ActivePlayerId; }
        }

        public void StartNewGame()
        {
            updateScores();
            initializePlayersOnBoard();
            m_ActivePlayerId = FirstPlayerId;
        }

        private void initializePlayersOnBoard()
        {
            int secondPlayerLastRow = m_Board.BoardSize / 2;
            int firstPlayerFirstRow = m_Board.BoardSize / 2;
            int lastRow = m_Board.BoardSize;

            initializePlayer(firstPlayerFirstRow, lastRow, FirstPlayerId);
            initializePlayer(Board.k_FirstRow, secondPlayerLastRow, SecondPlayerId);
            initializeOptionsLists();
        }

        private void initializePlayer(int i_From, int i_To, ePlayerId i_PlayerId)
        {
            BoardCell.eSigns sign = BoardCell.GetSignFromPlayerId(i_PlayerId);
            bool             needToInitialize = i_From != Board.k_FirstRow;
            Point            currentLocation = new Point();

            m_Players[(int)i_PlayerId].ChekersList.Clear();
            for (int i = i_From; i < i_To; i++)
            {
                for (int j = 0; j < m_Board.BoardSize; j++)
                {
                    currentLocation.X = j;
                    currentLocation.Y = i;
                    m_Board[currentLocation] = new BoardCell();
                    if (!isEmptyRow(i) && needToSetChecker(i, j))
                    {
                        m_Board[currentLocation].InitializeCell(currentLocation, i_PlayerId);
                        m_Players[(int)i_PlayerId].AddChecker(currentLocation);
                    }
                    else
                    {
                        m_Board[currentLocation].InitializeCell(currentLocation, ePlayerId.None);
                    }
                }
            }
        }

        private bool isEmptyRow(int i_Row)
        {
            int halfBoard = m_Board.BoardSize / 2;

            return i_Row == halfBoard || i_Row == halfBoard - 1;
        }

        private bool needToSetChecker(int i_Row, int i_Col)
        {
            return (i_Row + i_Col) % 2 != 0;
        }

        private void initializeOptionsLists()
        {
            Point currentLocation = new Point();

            for (int i = 0; i < m_Board.BoardSize; i++)
            {
                for (int j = 0; j < m_Board.BoardSize; j++)
                {
                    currentLocation.X = j;
                    currentLocation.Y = i;
                    if (m_Board[currentLocation].OwnerId != ePlayerId.None)
                    {
                        m_Board.FindOptions(m_Board[currentLocation]);
                    }
                }
            }
        }

        private void playComputerTurn()
        {
            Point startLocation, destLocation;

            getComputerInstruction(out startLocation, out destLocation);
            PlayTurn(startLocation, destLocation);
        }

        public void PlayTurn(Point i_StartLocation, Point i_DestLocation)
        {
            bool     isSimpleMove;
            eMessage outputMessage;

            if (isValidMoveForPlayer(i_StartLocation, i_DestLocation))
            {
                isSimpleMove = m_Board[i_StartLocation].OptionsList[i_DestLocation] == eMoveType.SimpleMove;
                if (isSimpleMove && mustJumpOver())
                {
                    outputMessage = eMessage.MustJumpOver;
                }
                else
                {
                    move(i_StartLocation, i_DestLocation);
                    outputMessage = needToJumpOverAgain(i_DestLocation, isSimpleMove);
                }
            }
            else
            {
                outputMessage = eMessage.ActionNotValid;
            }

            invokeOnEndMove(ref outputMessage);
            if (m_ActivePlayerId == ePlayerId.Computer && outputMessage != eMessage.EndGame)
            {
                playComputerTurn();
            }
        }

        private void invokeOnEndMove(ref eMessage i_OutputMessage)
        {
            EndTurnEventArgs  endUnsuccessfulTurnArgs = new EndTurnEventArgs();
            GameOverEventArgs gameOverArgs = new GameOverEventArgs();

            if (playerCanPlay(m_ActivePlayerId))
            {
                if (i_OutputMessage == eMessage.EndTurn || i_OutputMessage == eMessage.MustJumpOverAgain)
                {
                    OnUpdateBoard();
                }
                else
                {
                    endUnsuccessfulTurnArgs.Message = i_OutputMessage;
                    OnEndUnsuccessfulTurn(endUnsuccessfulTurnArgs);
                }
            }
            else
            {
                gameOverArgs.Tie = !playerCanPlay(opponentId);
                gameOverArgs.Winner = opponentId;
                i_OutputMessage = eMessage.EndGame;
                OnGameOver(gameOverArgs);
            }
        }

        private void getComputerInstruction(out Point o_StartCell, out Point o_DestinationCell)
        {
            bool needToJumpOver = mustJumpOver();
            bool isSimpleMove = !needToJumpOver;

            getRandInstruction(out o_StartCell, out o_DestinationCell, needToJumpOver);
        }

        private void getRandInstruction(out Point o_StartCell, out Point o_DestinationCell, bool i_NeedToJumpOver)
        {
            Random    rndNum = new Random();
            int       countCheckers = m_Players[(int)ePlayerId.Computer].ChekersList.Count;
            int       rndChecker = rndNum.Next(0, countCheckers);
            eMoveType moveType = i_NeedToJumpOver ? eMoveType.JumpOver : eMoveType.SimpleMove;

            o_DestinationCell = new Point();
            o_StartCell = m_Players[(int)ePlayerId.Computer].ChekersList[rndChecker];
            while ((i_NeedToJumpOver && !m_Board[o_StartCell].NeedToJumpOver()) || m_Board[o_StartCell].OptionsList.Count == k_Empty)
            {
                rndChecker = (rndChecker + 1) % countCheckers;
                o_StartCell = m_Players[(int)ePlayerId.Computer].ChekersList[rndChecker];
            }

            foreach (Point option in m_Board[o_StartCell].OptionsList.Keys)
            {
                if (m_Board[o_StartCell].OptionsList[option] == moveType)
                {
                    o_DestinationCell = option;
                    break;
                }
            }
        }

        private bool isValidMoveForPlayer(Point i_Start, Point i_Destination)
        {
            bool isValidMove = m_Board[i_Start].IsPossiableMove(i_Destination);

            if (isValidMove)
            {
                isValidMove = m_Players[(int)m_ActivePlayerId].IsMyChecker(i_Start);
            }

            return isValidMove;
        }

        private void move(Point i_StartCell, Point i_DestinationCell)
        {
            if (m_Board[i_StartCell].OptionsList[i_DestinationCell] == eMoveType.JumpOver)
            {
                removeCheckerFromOpponent(i_StartCell, i_DestinationCell);
            }

            m_Players[(int)m_ActivePlayerId].Move(i_StartCell, i_DestinationCell);
            m_Board.MoveCheckerOnBoard(i_StartCell, i_DestinationCell);
            updateOptionsOnBoard();
        }

        private eMessage needToJumpOverAgain(Point i_DestinationCell, bool i_IsSimpleMove)
        {
            eMessage outputMessage;

            if (!i_IsSimpleMove && m_Board[i_DestinationCell].OptionsList.ContainsValue(eMoveType.JumpOver))
            {
                outputMessage = eMessage.MustJumpOverAgain;
            }
            else
            {
                outputMessage = eMessage.EndTurn;
                m_ActivePlayerId = opponentId;
            }

            return outputMessage;
        }

        private bool mustJumpOver()
        {
            bool mustJumpOver = false;

            foreach (Point checkerLocation in m_Players[(int)m_ActivePlayerId].ChekersList)
            {
                mustJumpOver |= m_Board[checkerLocation].NeedToJumpOver();
            }

            return mustJumpOver;
        }

        private void removeCheckerFromOpponent(Point i_StartCell, Point i_DestinationCell)
        {
            Point skippedCellId = getSkippedCell(i_StartCell, i_DestinationCell);

            m_Players[(int)opponentId].ChekersList.Remove(skippedCellId);
            m_Board.MakeCellEmpty(skippedCellId);
        }

        private Point getSkippedCell(Point i_StartCell, Point i_DestinationCell)
        {
            int skippedRow, skippedCol;
            int diffRow = i_DestinationCell.Y - i_StartCell.Y;
            int diffCol = i_DestinationCell.X - i_StartCell.X;

            skippedRow = (diffRow / 2) + i_StartCell.Y;
            skippedCol = (diffCol / 2) + i_StartCell.X;

            return new Point(skippedCol, skippedRow);
        }

        private void updateOptionsOnBoard()
        {
            updatePlayerOptions(FirstPlayerId);
            updatePlayerOptions(SecondPlayerId);
        }

        private void updatePlayerOptions(ePlayerId i_PlayerId)
        {
            foreach (Point option in m_Players[(int)i_PlayerId].ChekersList)
            {
                m_Board.FindOptions(m_Board[option]);
            }
        }

        private bool playerCanPlay(ePlayerId i_PlayerId)
        {
            bool hasMoves = false;

            foreach (Point checker in m_Players[(int)i_PlayerId].ChekersList)
            {
                hasMoves |= m_Board[checker].OptionsList.Count != k_Empty;
            }

            return hasMoves;
        }

        private void updateScores()
        {
            string winnerName = string.Empty;
            int    firstPlayerValue = calculatePlayerValue(FirstPlayerId);
            int    secondPlayerValue = calculatePlayerValue(SecondPlayerId);
            bool   firstPlayerCanPlay = playerCanPlay(FirstPlayerId);
            bool   secondPlayerCanPlay = playerCanPlay(SecondPlayerId);

            if (!firstPlayerCanPlay)
            {
                if (secondPlayerCanPlay)
                {
                    m_Players[(int)SecondPlayerId].Score += secondPlayerValue - firstPlayerValue;
                    winnerName = SecondPlayerName;
                }
            }
            else if (!secondPlayerCanPlay || firstPlayerValue > secondPlayerValue)
            {
                m_Players[(int)FirstPlayerId].Score += firstPlayerValue - secondPlayerValue;
                winnerName = FirstPlayerName;
            }
            else
            {
                m_Players[(int)SecondPlayerId].Score += secondPlayerValue - firstPlayerValue;
                winnerName = SecondPlayerName;
            }
        }

        private int calculatePlayerValue(ePlayerId i_PlayerId)
        {
            int value = 0;

            foreach (Point checker in m_Players[(int)i_PlayerId].ChekersList)
            {
                if (m_Board[checker].Direction == BoardCell.eDirection.UpAndDown)
                {
                    value += k_KingValue;
                }
                else
                {
                    value += k_CheckerValue;
                }
            }

            return value;
        }

        protected virtual void OnEndUnsuccessfulTurn(EndTurnEventArgs e)
        {
            if (EndUnsuccessfulTurn != null)
            {
                EndUnsuccessfulTurn(this, e);
            }
        }

        protected virtual void OnGameOver(GameOverEventArgs e)
        {
            if (GameOver != null)
            {
                GameOver(this, e);
            }
        }

        protected virtual void OnUpdateBoard()
        {
            if (BoardUpdated != null)
            {
                BoardUpdated.Invoke(this);
            }
        }
    }
}