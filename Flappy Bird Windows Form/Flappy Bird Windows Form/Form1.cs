using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Flappy_Bird_Windows_Form
{
    public partial class Form1 : Form
    {

        // coded for MOO ICT Flappy Bird Tutorial

        // Variables start here

        int pipeSpeed = 8; // default pipe speed defined with an integer
        int gravity = 15; // default gravity speed defined with an integer
        int score = 0; // default score integer set to 0
        private int highScore = 0; // Add this field to track the high score
        private Button btnRestart; // Add this field to manage the Restart button
        private DateTime roundStartTime;
        private TimeSpan roundDuration;
        private Point scoreTextDefaultLocation = new Point(10, 10); // Default location for scoreText
        private readonly string highScoreFile = "highscore.txt";
        // variable ends

        public Form1()
        {
            InitializeComponent();

            // Create and configure the Restart button
            btnRestart = new Button();
            btnRestart.Name = "btnRestart";
            btnRestart.Text = "Restart";
            btnRestart.Size = new Size(100, 40);
            btnRestart.Location = new Point(350, 300); // Adjust as needed
            btnRestart.Font = new Font("Arial", 12, FontStyle.Bold);
            btnRestart.BackColor = Color.LightGreen;
            btnRestart.ForeColor = Color.DarkBlue;
            btnRestart.Click += btnRestart_Click;
            btnRestart.Visible = false; // Hide by default

            // Add the button to the form
            this.Controls.Add(btnRestart);

            scoreText.Location = scoreTextDefaultLocation; // Use default location
            scoreText.Font = new Font("Arial", 16, FontStyle.Bold);
            scoreText.ForeColor = Color.Black;
            scoreText.BackColor = Color.Moccasin;
            scoreText.BringToFront();
            scoreText.AutoSize = true;

            if (System.IO.File.Exists(highScoreFile))
            {
                string text = System.IO.File.ReadAllText(highScoreFile);
                int.TryParse(text, out highScore);
            }
            else
            {
                highScore = 0;
            }

            this.KeyDown += gamekeyisdown;
            this.KeyUp += gamekeyisup;
        }

        private void gamekeyisdown(object sender, KeyEventArgs e)
        {
            // this is the game key is down event thats linked to the main form
            if (e.KeyCode == Keys.Space)
            {
                // if the space key is pressed then the gravity will be set to -15
                gravity = -15;
            }    


        }

        private void gamekeyisup(object sender, KeyEventArgs e)
        {
            // this is the game key is up event thats linked to the main form

            if (e.KeyCode == Keys.Space)
            {
                // if the space key is released then gravity is set back to 15
                gravity = 15;
            }

        }

        private void endGame()
        {
            gameTimer.Stop();

            // Oppdater highscore hvis nødvendig
            if (score > highScore)
            {
                highScore = score;
                System.IO.File.WriteAllText(highScoreFile, highScore.ToString());
            }

            // Beregn tid
            roundDuration = DateTime.Now - roundStartTime;
            int totalSeconds = (int)roundDuration.TotalSeconds;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            long nanoseconds = (roundDuration.Ticks % TimeSpan.TicksPerSecond) * 100;
            string timeString = $"{minutes:D2}:{seconds:D2}:{nanoseconds:D9}";

            // Vis score, highscore og tid midt på skjermen
            scoreText.Font = new Font("Arial", 36, FontStyle.Bold);
            scoreText.ForeColor = Color.DarkGreen;
            scoreText.BackColor = Color.Transparent;
            scoreText.AutoSize = true;
            scoreText.Text = $"Score: {score}\nHigh Score: {highScore}\nTime: {timeString}\nGame over!!!";
            scoreText.Location = new Point(
                (this.ClientSize.Width - scoreText.PreferredWidth) / 2,
                (this.ClientSize.Height - scoreText.PreferredHeight) / 2
            );
            scoreText.BringToFront();

            // Plasser restart-knappen under score
            btnRestart.Location = new Point(
                (this.ClientSize.Width - btnRestart.Width) / 2,
                scoreText.Location.Y + scoreText.PreferredHeight + 20
            );
            btnRestart.Visible = true;
            btnRestart.BringToFront();
        }

        private void RestartGame()
        {
            score = 0;
            pipeSpeed = 8;
            gravity = 15;
            scoreText.Text = "Score: 0";
            scoreText.Location = scoreTextDefaultLocation; // Tilbake til hjørnet
            scoreText.Font = new Font("Arial", 16, FontStyle.Bold);
            scoreText.ForeColor = Color.Black;
            scoreText.BackColor = Color.Moccasin;
            scoreText.AutoSize = true;
            flappyBird.Top = 150;
            pipeBottom.Left = 800;
            pipeTop.Left = 950;
            btnRestart.Visible = false;

            // Start rundetimer
            roundStartTime = DateTime.Now;

            // Fjern "Game over!!!" hvis det finnes
            if (scoreText.Text.Contains("Game over"))
                scoreText.Text = "Score: 0";

            this.Focus();
            gameTimer.Start();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void gameTimerEvent(object sender, EventArgs e)
        {
            flappyBird.Top += gravity; // link the flappy bird picture box to the gravity, += means it will add the speed of gravity to the picture boxes top location so it will move down
            pipeBottom.Left -= pipeSpeed; // link the bottom pipes left position to the pipe speed integer, it will reduce the pipe speed value from the left position of the pipe picture box so it will move left with each tick
            pipeTop.Left -= pipeSpeed; // the same is happening with the top pipe, reduce the value of pipe speed integer from the left position of the pipe using the -= sign
            scoreText.Text = "Score: " + score; // show the current score on the score text label

            // below we are checking if any of the pipes have left the screen

            if(pipeBottom.Left < -150)
            {
                // if the bottom pipes location is -150 then we will reset it back to 800 and add 1 to the score
                pipeBottom.Left = 800;
                score++;
            }
            if(pipeTop.Left < -180)
            {
                // if the top pipe location is -180 then we will reset the pipe back to the 950 and add 1 to the score
                pipeTop.Left = 950;
                score++;
            }

            // the if statement below is checking if the pipe hit the ground, pipes or if the player has left the screen from the top
            // the two pipe symbols stand for OR inside of an if statement so we can have multiple conditions inside of this if statement because its all going to do the same thing
            
            if (flappyBird.Bounds.IntersectsWith(pipeBottom.Bounds) ||
                flappyBird.Bounds.IntersectsWith(pipeTop.Bounds) ||
                flappyBird.Bounds.IntersectsWith(ground.Bounds) || flappyBird.Top < -25
                )
            {
                // if any of the conditions are met from above then we will run the end game function
                endGame();
            }


            // if score is greater then 5 then we will increase the pipe speed to 15
            if(score > 5)
            {
                pipeSpeed = 15;
            }

        }


    }
}
  