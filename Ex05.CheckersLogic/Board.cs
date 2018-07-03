using System.Collections.Generic;
using System.Drawing;

namespace Ex05.CheckersLogic
{
    public class Board
    {
        internal const int k_Row = 1;
        internal const int k_Col = 0;
        internal const int k_ToRow = 4;
        internal const int k_ToCol = 3;
        internal const int k_FirstRow = 0;
        internal const int k_Down = 1;
        internal const int k_Up = -1;
        internal const int k_Left = -1;
        internal const int k_Right = 1;

        private BoardCell[,] m_Board;

        public Board(int i_BoardSize)
        {
            m_Board = new BoardCell[i_BoardSize, i_BoardSize];
        }

        public BoardCell this[Point i_CellLocation]
        {
            get { return m_Board[i_CellLocation.Y, i_CellLocation.X]; }
            set { m_Board[i_CellLocation.Y, i_CellLocation.X] = value; }
        }

        public int BoardSize
        {
            get { return m_Board.GetLength(k_FirstRow); }
        }

        public bool IsInBoardBorders(Point i_CellLocation)
        {
            return i_CellLocation.X < BoardSize && i_CellLocation.Y < BoardSize && i_CellLocation.X >= 0 && i_CellLocation.Y >= 0;
        }

        private bool needToBeKing(Point i_CellLocation)
        {
            return i_CellLocation.Y == BoardSize - 1 || i_CellLocation.Y == 0;
        }

        public void MakeCellEmpty(Point i_CellToDeleteLocation)
        {
            this[i_CellToDeleteLocation].Sign = BoardCell.eSigns.Empty;
            this[i_CellToDeleteLocation].Direction = BoardCell.eDirection.None;
            this[i_CellToDeleteLocation].OwnerId = Game.ePlayerId.None;
            this[i_CellToDeleteLocation].OptionsList.Clear();
        }

        public void MoveCheckerOnBoard(Point i_CurrentLocation, Point i_NextLocation)
        {
            if (needToBeKing(i_NextLocation))
            {
                this[i_NextLocation].Sign = BoardCell.GetKingSignFromPlayerId(this[i_CurrentLocation].OwnerId);
                this[i_NextLocation].Direction = BoardCell.eDirection.UpAndDown;
            }
            else
            {
                this[i_NextLocation].Sign = this[i_CurrentLocation].Sign;
                this[i_NextLocation].Direction = this[i_CurrentLocation].Direction;
            }

            this[i_NextLocation].OwnerId = this[i_CurrentLocation].OwnerId;
            if (this[i_NextLocation].OptionsList == null)
            {
                this[i_NextLocation].OptionsList = new Dictionary<Point, Game.eMoveType>();
            }

            MakeCellEmpty(i_CurrentLocation);
        }

        public void FindOptions(BoardCell i_Cell)
        {
            BoardCell.eDirection cellDirection = i_Cell.Direction;

            i_Cell.OptionsList.Clear();
            if (cellDirection == BoardCell.eDirection.Down || cellDirection == BoardCell.eDirection.UpAndDown)
            {
                checkNextDiagonalCell(i_Cell, k_Down, k_Left);
                checkNextDiagonalCell(i_Cell, k_Down, k_Right);
            }

            if (cellDirection == BoardCell.eDirection.Up || cellDirection == BoardCell.eDirection.UpAndDown)
            {
                checkNextDiagonalCell(i_Cell, k_Up, k_Left);
                checkNextDiagonalCell(i_Cell, k_Up, k_Right);
            }
        }

        private void checkNextDiagonalCell(BoardCell i_Cell, int i_IndentationRow, int i_IndentationCol)
        {
            Point nextCellLocation = new Point(i_Cell.Location.X + i_IndentationCol, i_Cell.Location.Y + i_IndentationRow);
            bool  isOpponent;

            if (IsInBoardBorders(nextCellLocation))
            {
                isOpponent = isOpponentCell(i_Cell, this[nextCellLocation]);
                if (!tryUpdateCellOption(i_Cell, nextCellLocation, Game.eMoveType.SimpleMove) && isOpponent)
                {
                    nextCellLocation.X = i_Cell.Location.X + (i_IndentationCol * 2);
                    nextCellLocation.Y = i_Cell.Location.Y + (i_IndentationRow * 2);
                    if (IsInBoardBorders(nextCellLocation))
                    {
                        tryUpdateCellOption(i_Cell, nextCellLocation, Game.eMoveType.JumpOver);
                    }
                }
            }
        }

        private bool tryUpdateCellOption(BoardCell i_Cell, Point i_OptionalLocation, Game.eMoveType i_MoveType)
        {
            bool isUpdateSuccessed = false;

            if (this[i_OptionalLocation].Sign == BoardCell.eSigns.Empty)
            {
                if (i_Cell.OptionsList == null)
                {
                    i_Cell.OptionsList = new Dictionary<Point, Game.eMoveType>();
                }

                i_Cell.OptionsList.Add(i_OptionalLocation, i_MoveType);
                isUpdateSuccessed = true;
            }

            return isUpdateSuccessed;
        }

        private bool isOpponentCell(BoardCell i_StartCell, BoardCell i_DestinationCell)
        {
            bool isOpponentCell = i_StartCell.OwnerId != i_DestinationCell.OwnerId;

            return isOpponentCell && i_DestinationCell.OwnerId != Game.ePlayerId.None;
        }
    }
}