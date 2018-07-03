using System;
using System.Media;
using System.Windows.Forms;
using System.Drawing;
using System.Text;
using Ex05.CheckersLogic;

namespace Ex05.CheckersWinFormUI
{
    public delegate void TwoPictureBoxesClickedEventHandler(TwoPictureBoxesClickedEventArgs e);

    public delegate void NewGameEventHandler();

    public class FormCheckersGame : Form
    {
        private const int  k_PictureBoxSize = 50;
        private const int  k_SpaceFromLeft = 10;
        private const int  k_SpaceFromTop = 50;
        private const char k_Space = ' ';
        private const int  k_AmountOfSoundEffect = 3;
        private const int  k_CheckerMoveSoundEffect = 0;
        private const int  k_ApplauseSoundEffect = 1;
        private const int  k_ErrorSoundEffect = 2;

        private readonly FormSettings r_SettingsForm = new FormSettings();
        private PictureBox            pictureBoxChoosen = null;
        private SoundPlayer[]         m_SoundPlayerEffects;

        public event TwoPictureBoxesClickedEventHandler TwoPictureBoxesClicked;

        public event NewGameEventHandler StartNewGame;

        public FormCheckersGame()
        {
            initializeComponent();
        }

        public FormSettings SettingsForm
        {
            get { return r_SettingsForm; }
        }

        private void initializeComponent()
        {
            this.Name = "GameBoardForm";
            this.Load += new System.EventHandler(this.gameBoardForm_Load);
            this.ResumeLayout(false);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Fixed3D;
            this.BackgroundImage = Properties.Resources.FormBackground;
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Text = "Damka";
            this.MaximizeBox = false;
        }

        private void gameBoardForm_Load(object sender, EventArgs e)
        {
            int height, width;

            r_SettingsForm.ShowDialog();
            height = (r_SettingsForm.BoardSize * k_PictureBoxSize) + (k_SpaceFromLeft * 4);
            width = (r_SettingsForm.BoardSize * k_PictureBoxSize) + (k_SpaceFromTop * 2);
            this.Size = new Size(height, width);
        }

        public void InitializeGameForm(Game i_Game)
        {
            for (int i = 0; i < SettingsForm.BoardSize; i++)
            {
                for (int j = 0; j < SettingsForm.BoardSize; j++)
                {
                    initializeImageBox(i, j, i_Game);
                }
            }

            setPlayersNamesLabel();
            setPlayersScoreLabel();
            setSoundEffects();
            updateImageBox(i_Game);
            updatePlayerScores(i_Game);
        }

        private void setPlayersNamesLabel()
        {
            Label labelPlayer1Name = new Label();
            Label labelPlayer2Name = new Label();

            labelPlayer1Name.Name = "Player1Name";
            labelPlayer1Name.Text = SettingsForm.PlayerName1 + ":";
            labelPlayer1Name.Left = this.Width / 6;
            labelPlayer1Name.Top = k_SpaceFromTop / 2;
            labelPlayer1Name.AutoSize = true;
            labelPlayer1Name.Font = new Font("Ariel", 10, FontStyle.Bold);
            labelPlayer1Name.ForeColor = Color.Yellow;
            labelPlayer1Name.BackColor = Color.Transparent;
            labelPlayer2Name.Name = "Player2Name";
            labelPlayer2Name.Text = SettingsForm.PlayerName2 + ":";
            labelPlayer2Name.Left = labelPlayer1Name.Left * 3;
            labelPlayer2Name.Top = k_SpaceFromTop / 2;
            labelPlayer2Name.AutoSize = true;
            labelPlayer2Name.Font = new Font("Ariel", 10, FontStyle.Bold);
            labelPlayer2Name.BackColor = Color.Transparent;
            this.Controls.Add(labelPlayer1Name);
            this.Controls.Add(labelPlayer2Name);
        }

        private void setPlayersScoreLabel()
        {
            Label labelPlayer1Score = new Label();
            Label labelPlayer2Score = new Label();

            labelPlayer1Score.Name = "Player1Score";
            labelPlayer1Score.Left = this.Controls["Player1Name"].Right;
            labelPlayer1Score.Top = k_SpaceFromTop / 2;
            labelPlayer1Score.AutoSize = true;
            labelPlayer1Score.Font = new Font("Ariel", 10, FontStyle.Bold);
            labelPlayer1Score.BackColor = Color.Transparent;
            labelPlayer2Score.Name = "Player2Score";
            labelPlayer2Score.Left = this.Controls["Player2Name"].Right;
            labelPlayer2Score.Top = k_SpaceFromTop / 2;
            labelPlayer2Score.AutoSize = true;
            labelPlayer2Score.Font = new Font("Ariel", 10, FontStyle.Bold);
            labelPlayer2Score.BackColor = Color.Transparent;
            this.Controls.Add(labelPlayer1Score);
            this.Controls.Add(labelPlayer2Score);
        }

        private void setSoundEffects()
        {
            m_SoundPlayerEffects = new SoundPlayer[k_AmountOfSoundEffect];
            m_SoundPlayerEffects[k_CheckerMoveSoundEffect] = new SoundPlayer(Properties.Resources.CheckerMove);
            m_SoundPlayerEffects[k_ApplauseSoundEffect] = new SoundPlayer(Properties.Resources.applause2);
            m_SoundPlayerEffects[k_ErrorSoundEffect] = new SoundPlayer(Properties.Resources.Error);
        }

        private void playApplauseSoundEffect()
        {
            m_SoundPlayerEffects[k_ApplauseSoundEffect].Play();
        }

        private void playCheckerMoveSoundEffect()
        {
            m_SoundPlayerEffects[k_CheckerMoveSoundEffect].Play();
        }

        private void playErrorSoundEffect()
        {
            m_SoundPlayerEffects[k_ErrorSoundEffect].Play();
        }

        private void updatePlayerScores(Game i_Game)
        {
            this.Controls["Player1Score"].Text = i_Game.FirstPlayerScore.ToString();
            this.Controls["Player2Score"].Text = i_Game.SecondPlayerScore.ToString();
        }

        private void initializeImageBox(int i_Row, int i_Col, Game i_Game)
        {
            Point      currentLocation = new Point();
            PictureBox pictureBoxCurrentCell = new PictureBox();

            currentLocation.Y = i_Row;
            currentLocation.X = i_Col;
            pictureBoxCurrentCell.Text = ((char)i_Game.BoardCellSign(currentLocation)).ToString();
            currentLocation.Y = (i_Row * k_PictureBoxSize) + k_SpaceFromTop;
            currentLocation.X = (i_Col * k_PictureBoxSize) + k_SpaceFromLeft;
            pictureBoxCurrentCell.Location = currentLocation;
            pictureBoxCurrentCell.Width = k_PictureBoxSize;
            pictureBoxCurrentCell.Height = k_PictureBoxSize;
            setPictureBoxImage(pictureBoxCurrentCell, i_Row, i_Col, i_Game);
            this.Controls.Add(pictureBoxCurrentCell);
            pictureBoxCurrentCell.Click += pictureBoxCurrentCell_Click;
        }

        private void setPictureBoxImage(PictureBox i_PictureBox, int i_Row, int i_Col, Game i_Game)
        {
            if ((i_Row + i_Col) % 2 == 0)
            {
                i_PictureBox.Image = Properties.Resources.InvalidCell;
                i_PictureBox.Enabled = false;
            }
            else
            {
                i_PictureBox.Image = null;
                i_PictureBox.BackgroundImage = Properties.Resources.ValidCell;

                if (i_Game.BoardCellSign(new Point(i_Col, i_Row)) == BoardCell.eSigns.FirstPlayerSign)
                {
                    i_PictureBox.Image = Properties.Resources.WhiteChecker;
                }
                else if (i_Game.BoardCellSign(new Point(i_Col, i_Row)) == BoardCell.eSigns.FirstPlayerKingSign)
                {
                    i_PictureBox.Image = Properties.Resources.WhiteKing;
                }
                else if (i_Game.BoardCellSign(new Point(i_Col, i_Row)) == BoardCell.eSigns.SecondPlayerSign)
                {
                    i_PictureBox.Image = Properties.Resources.BlackChecker;
                }
                else if (i_Game.BoardCellSign(new Point(i_Col, i_Row)) == BoardCell.eSigns.SecondPlayerKingSign)
                {
                    i_PictureBox.Image = Properties.Resources.BlackKing;
                }

                i_PictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void pictureBoxCurrentCell_Click(object sender, EventArgs e)
        {
            PictureBox                      pictureBoxClicked = sender as PictureBox;
            TwoPictureBoxesClickedEventArgs twoPictureBoxesClicked;

            if (pictureBoxClicked != null)
            {
                if (pictureBoxChoosen == null)
                {
                    pictureBoxChoosen = pictureBoxClicked;
                    pictureBoxClicked.BackgroundImage = Properties.Resources.ClickedCell;
                }
                else if (pictureBoxChoosen == pictureBoxClicked)
                {
                    pictureBoxChoosen = null;
                    pictureBoxClicked.BackgroundImage = Properties.Resources.ValidCell;
                }
                else
                {
                    twoPictureBoxesClicked = new TwoPictureBoxesClickedEventArgs();
                    twoPictureBoxesClicked.StartLocation = getBoardPositionByLocation(pictureBoxChoosen);
                    twoPictureBoxesClicked.DestLocation = getBoardPositionByLocation(pictureBoxClicked);
                    OnTwoPictureBoxesClicked(twoPictureBoxesClicked);
                    pictureBoxChoosen.BackgroundImage = Properties.Resources.ValidCell;
                    pictureBoxChoosen = null;
                }
            }
        }

        private Point getBoardPositionByLocation(PictureBox i_PictureBox)
        {
            Point position = new Point();

            position.X = (i_PictureBox.Location.X - k_SpaceFromLeft) / k_PictureBoxSize;
            position.Y = (i_PictureBox.Location.Y - k_SpaceFromTop) / k_PictureBoxSize;

            return position;
        }

        public void DisplayErrorMessage(object sender, EndTurnEventArgs e)
        {
            playErrorSoundEffect();
            MessageBox.Show(fixEnumString(e.Message.ToString()));
        }

        public void UpdateBoard(object sender)
        {
            Game game = sender as Game;

            if (game != null)
            {
                playCheckerMoveSoundEffect();
                updateImageBox(game);
                colorActivePlayerLabel(game.ActivePlayer.ToString());
            }
        }

        private void colorActivePlayerLabel(string i_ActivePlayer)
        {
            if (i_ActivePlayer == "Player1")
            {
                this.Controls["Player1Name"].ForeColor = Color.Yellow;
                this.Controls["Player2Name"].ForeColor = Color.Black;
            }
            else
            {
                this.Controls["Player2Name"].ForeColor = Color.Yellow;
                this.Controls["Player1Name"].ForeColor = Color.Black;
            }
        }

        private void updateImageBox(Game i_Game)
        {
            PictureBox pictureBox;
            Point      currentLocation;

            foreach (object obj in this.Controls)
            {
                pictureBox = obj as PictureBox;
                if (pictureBox != null)
                {
                    currentLocation = getBoardPositionByLocation(pictureBox);
                    setPictureBoxImage(pictureBox, currentLocation.Y, currentLocation.X, i_Game);
                }
            }
        }

        private string fixEnumString(string i_EnumString)
        {
            StringBuilder fixedString = new StringBuilder(i_EnumString.Length);

            for (int i = 0; i < i_EnumString.Length; i++)
            {
                if (char.IsUpper(i_EnumString[i]))
                {
                    fixedString.Append(k_Space);
                }

                fixedString.Append(i_EnumString[i]);
            }

            return fixedString.ToString();
        }

        public void EndGame(object sender, GameOverEventArgs e)
        {
            string       endGameMessage;
            Game         game = sender as Game;
            DialogResult answer;

            if (game != null)
            {
                updateImageBox(game);
                endGameMessage = getStatusEndGameMessage(e);
                playApplauseSoundEffect();
                answer = MessageBox.Show(endGameMessage, "Damka", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer == DialogResult.Yes)
                {
                    OnStartNewGame();
                    InitializeGameForm(game);
                    colorActivePlayerLabel(game.ActivePlayer.ToString());
                }
                else
                {
                    this.Close();
                }
            }
        }

        private string getStatusEndGameMessage(GameOverEventArgs e)
        {
            string endGameMessage;

            if (e.Tie == true)
            {
                endGameMessage = string.Format(
@"Tie!
Another Round?");
            }
            else
            {
                endGameMessage = string.Format(
@"{0} Won!
Another Round?",
e.Winner.ToString());
            }

            return endGameMessage;
        }

        protected virtual void OnTwoPictureBoxesClicked(TwoPictureBoxesClickedEventArgs e)
        {
            if (TwoPictureBoxesClicked != null)
            {
                TwoPictureBoxesClicked.Invoke(e);
            }
        }

        protected virtual void OnStartNewGame()
        {
            if (StartNewGame != null)
            {
                StartNewGame.Invoke();
            }
        }
    }
}