using System;
using System.Windows.Forms;

namespace Ex05.CheckersWinFormUI
{
    public partial class FormSettings : Form
    {
        private const int k_Size6X6 = 6;
        private const int k_Size8X8 = 8;
        private const int k_Size10X10 = 10;

        public FormSettings()
        {
            InitializeComponent();
        }

        public Button ButtonDone
        {
            get { return this.buttonDone; }
        }

        public string PlayerName1
        {
            get { return textBoxPlayer1.Text; }
        }

        public string PlayerName2
        {
            get { return textBoxPlayer2.Text; }
        }

        public int BoardSize
        {
            get
            {
                int size;

                if (radioButton6X6.Checked)
                {
                    size = k_Size6X6;
                }
                else if (radioButton8X8.Checked)
                {
                    size = k_Size8X8;
                }
                else
                {
                    size = k_Size10X10;
                }

                return size;
            }
        }

        public bool IsVSPlayer2
        {
            get { return checkBoxPlayer2.Checked; }
        }

        private void checkBoxPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                textBoxPlayer2.Enabled = true;
            }
            else
            {
                textBoxPlayer2.Enabled = false;
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (textBoxPlayer1.Text == string.Empty)
            {
                textBoxPlayer1.Text = "Player 1";
            }

            if (checkBoxPlayer2.Checked == false)
            {
                textBoxPlayer2.Text = "Computer";
            }
            else if(textBoxPlayer2.Text == string.Empty)
            {
                textBoxPlayer2.Text = "Player 2";
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            radioButton6X6.Checked = true;
        }
    }
}