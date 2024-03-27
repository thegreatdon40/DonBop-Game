using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections;

namespace Prototype_Windows_
{
    public partial class Form1 : Form
    {
        class Button
        {
            bool on;
            bool bomb;
            string name;
            int x;
            int y;
            int x2;
            int y2;


            PictureBox pic;
            Color Layer1;

            public Button(string Name, bool Active, bool BOOM, Color baseColor, int X, int Y, int X2, int Y2)
            {
                on = Active;
                bomb = BOOM;
                Layer1 = baseColor;
                x = X;
                y = Y;
                x2 = X2;
                y2 = Y2;
            }
            public string Name { get { return name; } set { name = value; } }
            public bool Active { get { return on; } set { on = value; } }
            public bool BOOM { get { return bomb; } set { bomb = value; } }
            public int X { get { return x; } set { x = value; } }
            public int Y { get { return y; } set { y = value; } }
            public int X2 { get { return x2; } set { x2 = value; } }
            public int Y2 { get { return y2; } set { y2 = value; } }
            public Color baseColor { get { return Layer1; } set { Layer1 = value; } }
        }


        System.Collections.ArrayList ButtonArray = new System.Collections.ArrayList();
        System.Collections.ArrayList NoneActiveArray = new System.Collections.ArrayList();
        System.Collections.ArrayList BombButtonList = new System.Collections.ArrayList();
        System.Collections.ArrayList MemoryArray = new System.Collections.ArrayList();

        List<int> HSSArray;
        string[] HSNArray;

        PictureBox Board;
        Bitmap View;
        Random R;
        Graphics g;
        SoundPlayer SP;
        SoundPlayer SP2;
        StreamReader Reader;
        DateTime TimerClock;
        Color mainColor;
        Color chosenColor;
        Color ButtonColor;
        bool LevelClear;
        bool pause;
        bool playing;
        double clock;
        int Level;
        int score;//need to calculate score somehow
        int ButtonCounter;//Global button counter to deterimine if buttons are in noneactive array
        int GAMEMODE;//Random:1,Bombs:2
        int MemCheck;
        string playerName;





        //------------------------------------------------MAIN--------------------------------------------------------
        public Form1()//MAIN
        {
            this.Text = "DonBop";
            Board = new PictureBox();
            Board.Size = new Size(800, 500);
            View = new Bitmap(Board.Width, Board.Height);
            Board.Image = View;
            g = Graphics.FromImage(View);

            Level = 1;
            chosenColor = Color.Orange;
            mainColor = Color.Red;
            ButtonColor = chosenColor;

            ButtonArray = new System.Collections.ArrayList();
            NoneActiveArray = new System.Collections.ArrayList();
            MemoryArray = new System.Collections.ArrayList();
            BombButtonList = new System.Collections.ArrayList();

            SP = new SoundPlayer("pop.wav");
            SP2 = new SoundPlayer("YAY.wav");
            R = new Random();

            Initializer();
            InitializeComponent();
        }
        //------------------------------------------------MAIN--------------------------------------------------------

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void Initializer()
        {
            ButtonArray.Add(new Button("one", true, false, ButtonColor, 225, 75, 200, 50));
            ButtonArray.Add(new Button("two", true, false, ButtonColor, 350, 75, 325, 50));
            ButtonArray.Add(new Button("three", true, false, ButtonColor, 475, 75, 450, 50));
            ButtonArray.Add(new Button("four", true, false, ButtonColor, 175, 175, 150, 150));
            ButtonArray.Add(new Button("five", true, false, ButtonColor, 290, 175, 265, 150));
            ButtonArray.Add(new Button("six", true, false, ButtonColor, 410, 175, 385, 150));
            ButtonArray.Add(new Button("seven", true, false, ButtonColor, 525, 175, 500, 150));
            ButtonArray.Add(new Button("eight", true, false, ButtonColor, 225, 275, 200, 250));
            ButtonArray.Add(new Button("nine", true, false, ButtonColor, 350, 275, 325, 250));
            ButtonArray.Add(new Button("ten", true, false, ButtonColor, 475, 275, 450, 250));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            drawTS();
            if (GAMEMODE == 1 || GAMEMODE == 2)//Random/Bomb
            {
                clock++;
                if (clock == 10)//was 6 for about 5 seconds
                    //it is necessary that we improve on this clock majorly. Both in ticking and in timing accuracy
                {
                    EraseButtons(View, g);
                    MessageBox.Show("Ran out of time:(");
                    timer1.Enabled = false;
                    playing = false;
                    mainMenue.Show();
                    button1.Show();
                    label1.Show();
                }
                else
                    if (LevelClear)
                {
                    LevelClear = false;
                }
            }
            if (GAMEMODE == 3 || GAMEMODE == 5)//Score/memorize
            {
                clock--;
                if (clock == 0)
                {
                    EraseButtons(View, g);
                    timer1.Enabled = false;
                    MessageBox.Show("Times up! Your Score was: " + score);
                    mainMenue.Show();
                    button1.Show();
                    label1.Show();
                }
            }               
            GameLoop(GAMEMODE);//this will get every time the timer ticks(I think 1000 times per second)
            //big calculations could result in a major slow down
        }

        private void drawTS() 
        {
            pictureBox2.Size = new Size(100, 100);
            Bitmap TS = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            Graphics g1 = Graphics.FromImage(TS);
            FontFamily ff = new FontFamily("Arial");
            System.Drawing.Font font = new System.Drawing.Font(ff, 12);

            g1.DrawString("Level: " + Level.ToString(), font, new SolidBrush(Color.Black), 0, 0);
            g1.DrawString("Score: " + score.ToString(), font, new SolidBrush(Color.Black), 0, 35);
            g1.DrawString("Clock: " + clock.ToString(), font, new SolidBrush(Color.Black), 0, 70);

            pictureBox2.Image = TS;
        }

        private void drawButton() //First Layer
        {
            g.Clear(Color.White);
            //This is creating all the cirlce of the game
            if (GAMEMODE == 1 || GAMEMODE == 3)
            {
                foreach (Button B in ButtonArray)
                {
                    if (B.Active)
                    {
                        g.FillEllipse(new SolidBrush(mainColor), B.X2, B.Y2, 100, 100);
                        g.FillEllipse(new SolidBrush(chosenColor), B.X, B.Y, 50, 50);
                    }
                    else {
                        g.FillEllipse(new SolidBrush(mainColor), B.X2, B.Y2, 100, 100);
                    }
                }
            }
            if (GAMEMODE == 2)
            {
                foreach (Button B in ButtonArray)
                {
                    if (B.Active && B.BOOM)
                    {
                        g.FillEllipse(new SolidBrush(Color.Black), B.X2, B.Y2, 100, 100);//BACKGROUND BUTTON
                        g.FillEllipse(new SolidBrush(chosenColor), B.X, B.Y, 50, 50);//ACTIVE BUTTON
                    }
                    else if (B.Active)
                    {
                        g.FillEllipse(new SolidBrush(mainColor), B.X2, B.Y2, 100, 100);
                        g.FillEllipse(new SolidBrush(chosenColor), B.X, B.Y, 50, 50);
                    }
                    else if (!B.Active)
                    {
                        g.FillEllipse(new SolidBrush(mainColor), B.X2, B.Y2, 100, 100);
                    }
                }
            }
            if(GAMEMODE == 5) 
            {
                //....
                foreach (Button B in ButtonArray)
                {
                    if (B.Active)
                    {
                        g.FillEllipse(new SolidBrush(mainColor), B.X2, B.Y2, 100, 100);
                        g.FillEllipse(new SolidBrush(chosenColor), B.X, B.Y, 50, 50);
                    }
                    if (!B.Active)
                    {
                        g.FillEllipse(new SolidBrush(mainColor), B.X2, B.Y2, 100, 100);
                    }
                }
            }
            pictureBox1.Image = View;
        }

        private void EraseButtons(Bitmap Map, Graphics G)
        {
            G.Clear(Color.White);
            pictureBox1.Image = Map;
        }

        public void ButtonCLick(int ID, int GAMEMODE)
        {

            //Gamemode: Random/Score
            if (GAMEMODE == 1 || GAMEMODE == 3)
            {
                if (((Button)ButtonArray[ID]).Active == true)//Controls the pop sound
                    SP.Play();

                //if you tap a button when it is off it will result in a loss and turn the timer off
                if (((Button)ButtonArray[ID]).Active == false)
                {
                    buttonLoss();
                }
                else
                //this will turn the button off and add it to the NoneActiveArray
                if (((Button)ButtonArray[ID]).Active == true)
                {
                    ((Button)ButtonArray[ID]).Active = false;
                    NoneActiveArray.Add(ButtonArray[ID]);
                    drawButton();
                }
            }

            //Gamemode: Bombs
            if (GAMEMODE == 2)
            {
                //if bomb touched
                if (((Button)ButtonArray[ID]).BOOM == true)
                {
                    EraseButtons(View, g);
                    timer1.Enabled = false;
                    MessageBox.Show("BOOOOOM YOU LOSE");
                    button1.Show();
                    button5.Show();
                    mainMenue.Show();
                    label1.Show();
                    playing = false;
                    GAMEMODE = 0;
                    foreach (Button B in ButtonArray)
                        B.BOOM = false;
                    BombButtonList.Clear();
                }
                else
                if (((Button)ButtonArray[ID]).Active == false)
                {
                    buttonLoss();
                }
                else
                if (((Button)ButtonArray[ID]).Active == true)
                {
                    ((Button)ButtonArray[ID]).Active = false;
                    NoneActiveArray.Add(ButtonArray[ID]);
                    drawButton();
                }
            }
            if (GAMEMODE == 5) 
            {
                if (MemoryArray[MemCheck] == ButtonArray[ID])
                //if the current memory array slot that we are checking is equal to the button just pressed
                {
                    ((Button)ButtonArray[ID]).Active = true;
                    drawButton();
                }
                if (MemoryArray[MemCheck] != ButtonArray[ID])
                //if the current memory array slot that we are checking is NOT equal to the button jus pressed
                {
                    buttonLoss();
                }
            }
        }

        //This method will provide a randomized board each button having 50/50 chance of being turned on
        //Future Note, each gamemode will have to be its own method like this game is random mode
        private void RandomGenerator() 
        {
            NoneActiveArray.Clear();
            ButtonCounter = 0;
            int checker;
            foreach (Button B in ButtonArray)
            {
                checker = R.Next(100);
                if (checker >= 50)
                    B.Active = true;
                else
                {
                    NoneActiveArray.Add(B);
                    B.Active = false;
                }
            }
        drawButton();
        }

        private void BombGenerator()
        {
            BombButtonList.Clear();
            ButtonCounter = 0;
            int checker;
            foreach (Button B in ButtonArray)
            {
                B.Active = true;
                checker = R.Next(100);
                if (checker >= 75)
                {
                    B.BOOM = true;
                    BombButtonList.Add(B);
                }
                else
                {
                    B.BOOM = false;
                }
            }
            //ButtonCounter = ButtonArray.Count - BombButtonList.Count;
            drawButton();
        }
        private void MemGen()
        {
            int checker;
            //use the past template and append to the end a new state
            foreach (Button B in ButtonArray)
                B.Active = false;
            checker = R.Next(9);
            ((Button)ButtonArray[checker]).Active = true;
            MemoryArray.Add(((Button)ButtonArray[checker]));
        }

        //game logic
        private void GameLoop(int GAMEMODE)//maybe add another contructor that takes an array in and maybe use recursion
        {
            //bool playing = true;
            //GameMode Random/Score mode
            if (GAMEMODE == 1 || GAMEMODE == 3)
            {
                ButtonCounter = NoneActiveArray.Count;
                if (ButtonCounter == 10)
                {
                    LevelClear = true;
                    SP2.Play();
                    Level++;
                    foreach (Button B in ButtonArray)
                    {
                        B.Active = true;
                    }
                    RandomGenerator();
                    if(GAMEMODE == 1)
                        clock = 0;
                    if (GAMEMODE == 3)
                        score++;
                }
            }

            //GameMode Bombs
            if (GAMEMODE == 2)
            {
                if (BombButtonList.Count + NoneActiveArray.Count == ButtonArray.Count)
                {
                    LevelClear = true;
                    SP2.Play();
                    Level++;

                    foreach (Button B in ButtonArray)
                    {
                        B.BOOM = false;
                        B.Active = false;
                    }
                    ButtonCounter = 0;
                    clock = 0;
                    BombGenerator();
                }
            }
            //Memory Mode
            if (GAMEMODE == 5)
            {
                MemGen();
            }
        }

        //display high scores
        private void LoadHighScores()
        {
            try
            {
                // Clear existing items in the list box
                highScoresBox.Items.Clear();

                // Read high scores from the text file
                List<string> highScores = ReadHighScores();

                // Display high scores in the list box
                foreach (string score in highScores)
                {
                    highScoresBox.Items.Add(score);
                }
                highScoresBox.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading high scores: " + ex.Message);
            }
        }

        private List<string> ReadHighScores()
        {
            List<string> highScores = new List<string>();

            try
            {
                // Open the text file for reading
                using (StreamReader reader = new StreamReader("highScores.txt"))
                {
                    string line;
                    // Read each line from the file
                    while ((line = reader.ReadLine()) != null)
                    {
                        highScores.Add(line);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("High scores file not found. Creating a new file.");
                File.Create("highScores.txt").Close(); // Create a new file if it doesn't exist
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return highScores;
        }

        public void ReplaceHighScore(string newName, int newScore)
        {
            try
            {
                // Read existing high scores
                List<string> highScores = ReadHighScores();

                // Check if the new score is higher than any existing score
                bool replaced = false;
                for (int i = 0; i < highScores.Count; i++)
                {
                    string[] parts = highScores[i].Split(':');
                    if (parts.Length == 2)
                    {
                        if (int.TryParse(parts[1].Trim(), out score))
                        {
                            if (newScore > score)
                            {
                                if (GAMEMODE == 1 || GAMEMODE == 2)
                                    MessageBox.Show("Congrats! You just achieved a new high level of: " + Level + "\n Enter your name below");//*****
                                if (GAMEMODE == 3 || GAMEMODE == 4)
                                    MessageBox.Show("Congrats! You just achieved a new high score of: " + score + "\n Enter your name below");
                                textBox1.Show();//I want this to be a whole new form entirely that will have a textBox
                                highScores.Insert(i, newName + ": " + newScore); // Insert the new high score
                                replaced = true;
                                break;
                            }
                        }
                    }
                }

                // If the new score wasn't higher than any existing score, add it to the end
                if (!replaced)
                {
                    //highScores.Add(newName + ": " + newScore);
                }

                // Write the updated high scores back to the file
                using (StreamWriter writer = new StreamWriter("highScores.txt"))
                {
                    foreach (string score in highScores)
                    {
                        writer.WriteLine(score);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        //Start/restart
        private void button1_Click_1(object sender, EventArgs e)
        {
            foreach (Button B in ButtonArray)
            {
                B.BOOM = false;
            }
            string selected;
            Level = 1;
            clock = 0;
            score = 0;
            mainMenue.Hide();
            button5.Hide();
            label1.Hide();


            selected = mainMenue.Items[mainMenue.SelectedIndex].ToString();
            if (selected == "Random Mode")
            {
                GAMEMODE = 1;
                RandomGenerator();
            }
            if (selected == "Bombs Mode")
            {
                GAMEMODE = 2;
                BombGenerator();
            }
            if (selected == "Score Mode")
            {
                GAMEMODE = 3;
                RandomGenerator();
                clock = 65;
            }
            if (selected == "Score Mode (1 Minute)")
            {
                GAMEMODE = 3;
                RandomGenerator();
                clock = 120;
            }
            if (selected == "Memorize Mode")
            {
                GAMEMODE = 5;
            }
            timer1.Enabled = true;
            timer1.Start();
            playing = true;
            button1.Hide();
            button3.Hide();
            drawButton();
        }

        private void Form1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //Buttons
            {
                //Button 1
                if (timer1.Enabled && (e.X >= 200 && e.X <= 300) && (e.Y >= 50 && e.Y <= 150))
                {
                    //         ID, GAMEMODE
                    ButtonCLick(0, GAMEMODE);

                }

                //Button 2
                if (timer1.Enabled && (e.X >= 325 && e.X <= 425) && (e.Y >= 50 && e.Y <= 150))
                {
                    ButtonCLick(1, GAMEMODE);
                }

                //Button 3
                if (timer1.Enabled && (e.X >= 450 && e.X <= 550) && (e.Y >= 50 && e.Y <= 150))
                {
                    ButtonCLick(2, GAMEMODE);
                }

                //Button 4
                if (timer1.Enabled && (e.X >= 150 && e.X <= 250) && (e.Y >= 150 && e.Y <= 250))
                {
                    ButtonCLick(3, GAMEMODE);
                }

                //Button 5
                if (timer1.Enabled && (e.X >= 265 && e.X <= 365) && (e.Y >= 150 && e.Y <= 250))
                {
                    ButtonCLick(4, GAMEMODE);
                }

                //Button 6
                if (timer1.Enabled && (e.X >= 385 && e.X <= 485) && (e.Y >= 150 && e.Y <= 250))
                {
                    ButtonCLick(5, GAMEMODE);
                }

                //Button 7
                if (timer1.Enabled && (e.X >= 500 && e.X <= 600) && (e.Y >= 150 && e.Y <= 250))
                {
                    ButtonCLick(6, GAMEMODE);
                }

                //Button 8
                if (timer1.Enabled && (e.X >= 200 && e.X <= 300) && (e.Y >= 250 && e.Y <= 350))
                {
                    ButtonCLick(7, GAMEMODE);

                }

                //Button 9
                if (timer1.Enabled && (e.X >= 325 && e.X <= 425) && (e.Y >= 250 && e.Y <= 350))
                {
                    ButtonCLick(8, GAMEMODE);
                }

                //Button 10
                if (timer1.Enabled && (e.X >= 450 && e.X <= 550) && (e.Y >= 250 && e.Y <= 350))
                {
                    ButtonCLick(9, GAMEMODE);
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //Main Menue Button
        private void button2_Click(object sender, EventArgs e)
        {
            if(playing)//this way the Resume Button will only come up when you are playing
                pause = true;
            EraseButtons(View,g);
            mainMenue.Show();
            highScoresBox.Items.Clear();
            highScoresBox.Hide();
            button1.Show();
            button5.Show();
            label1.Show();
            if(pause && playing)
                button3.Show();
            timer1.Stop();
        }

        //Resume Button
        private void button3_Click(object sender, EventArgs e)
        {
            if (pause)
            {
                drawButton();
                timer1.Start();
                mainMenue.Hide();
                label1.Hide();
                button3.Hide();
                button1.Hide();
                pause = false;
            }
        }

        //test button
        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Show();
        }

        //high score button
        private void button5_Click(object sender, EventArgs e)
        {
            button1.Hide();
            button5.Hide();
            mainMenue.Hide();
            LoadHighScores();
        }
        private void buttonLoss() 
        {
            timer1.Enabled = false;
            EraseButtons(View, g);
            MessageBox.Show("You Lose");

            if(GAMEMODE == 1 || GAMEMODE == 2)
                ReplaceHighScore(textBox1.Text, Level);//you're going to have to make the textbox string a global
            if (GAMEMODE == 3 || GAMEMODE == 4)
                ReplaceHighScore(playerName, score);

            textBox1.Hide();
            button5.Show();
            button1.Show();
            mainMenue.Show();
            label1.Show();
            playing = false;
            GAMEMODE = 0;
            NoneActiveArray.Clear();
            BombButtonList.Clear();
            MemoryArray.Clear();
        }

        private void highScoresBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Get the new name and score from the text boxes
                string newName = textBox1.Text;
                int newScore = score;

                // Replace existing high scores with the new one
                ReplaceHighScore(newName, newScore);

                // Reload high scores to reflect the changes
                LoadHighScores();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while replacing high scores: " + ex.Message);
            }
        }
    }
}
