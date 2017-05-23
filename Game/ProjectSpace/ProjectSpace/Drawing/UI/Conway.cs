using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gwen;
using Gwen.Control;
using Gwen.Control.Layout;
using OpenTK;
using System.Drawing;

namespace OutpostOmega.Drawing.UI
{
    class Conway : Menu
    {
        bool IsGameScene = false;
        ImagePanel playfield;
        Bitmap playfieldBitmap;
        Texture2D playfieldTex;
        HorizontalSlider speed;
        public Conway(Scene Scene, Base parent)
            : base(Scene, parent, "Conways Game of Life")
        {
            this.SetSize(600, 400);
            this.Position(Pos.Center);

            IsGameScene = typeof(Scenes.Game) == Scene.GetType() && ((Scenes.Game)Scene).World != null;
                

            //Center meh
            this.SetPosition(Scene.Game.Width / 2 - this.Width / 2, Scene.Game.Height / 2 - this.Height / 2);


            GroupBox contgroup = new GroupBox(this);
            contgroup.Dock = Pos.Bottom;
            contgroup.Height = 25;
            contgroup.Text = "Control";

            Button nextStep = new Button(contgroup);
            nextStep.SetPosition(5, 5);
            nextStep.Text = "Step";
            nextStep.Clicked += nextStep_Clicked;

            Button playpause = new Button(contgroup);
            playpause.SetPosition(10 + nextStep.Width, 5);
            playpause.Text = "Play";
            playpause.Clicked += playpause_Clicked;

            Button clear = new Button(contgroup);
            clear.SetPosition(15 + playpause.Width + nextStep.Width, 5);
            clear.Text = "Clear";
            clear.Clicked += clear_Clicked;


            speed = new HorizontalSlider(contgroup);
            speed.SetPosition(20 + playpause.Width + nextStep.Width + clear.Width, 5);
            speed.Width = 50;
            speed.Height = 25;
            speed.Max = 1000;
            speed.Min = 50;
            speed.Value = 100;

            contgroup.Height = 50;

            field = new bool[50,50];

            playfield = new ImagePanel(this);
            playfield.BoundsChanged += playfield_BoundsChanged;
            playfield.Dock = Pos.Fill;

            playfieldBitmap = new Bitmap(playfield.Width, playfield.Height);
            playfieldTex = new Texture2D(playfieldBitmap);
            playfield.ImageHandle = playfieldTex.Handle;
            playfield.Clicked += playfield_Clicked;
            //playfield.SetPosition(5, 10 + nextStep.Height);
            //playfield.Height = 200;
            //playfield.Width = 200;
            graphics = Graphics.FromImage(playfieldBitmap);
        }

        void clear_Clicked(Base sender, ClickedEventArgs arguments)
        {
            field = new bool[field.GetLength(0), field.GetLength(1)];
            RenderField();
        }

        void playfield_BoundsChanged(Base sender, EventArgs arguments)
        {
            field = ResizeArray(field, playfield.Width / 5, playfield.Height / 5);
            RenderField();
        }

        bool Play = false;
        void playpause_Clicked(Base sender, ClickedEventArgs arguments)
        {
            if (Play)
                ((Button)sender).Text = "Play";
            else
                ((Button)sender).Text = "Pause";

            Play = !Play;
        }

        System.Diagnostics.Stopwatch PlayTimer = new System.Diagnostics.Stopwatch();
        public override void Think()
        {
            if (!PlayTimer.IsRunning) PlayTimer.Start();

            if (Play && PlayTimer.ElapsedMilliseconds > speed.Value)
            {
                UpdateField();
                PlayTimer.Reset();
            }

            base.Think();
        }

        Graphics graphics;
        bool[,] field;

        public void UpdateField()
        {
            bool[,] newfield = new bool[field.GetLength(0), field.GetLength(1)];
            Array.Copy(field, 0, newfield, 0, field.Length);

            for (int x = 0; x < field.GetLength(0); x++)
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    int aliveNeighbours = 0;
                    for (int sx = -1; sx <= 1; sx++)
                        for (int sy = -1; sy <= 1; sy++)
                            if (!(sx == 0 && sy == 0) &&
                                sx + x >= 0 && sx + x < field.GetLength(0) &&
                                sy + y >= 0 && sy + y < field.GetLength(1) &&
                                field[sx + x, sy + y])
                                aliveNeighbours++;
                    if (field[x, y])
                    {
                        if (aliveNeighbours < 2 || aliveNeighbours > 3)
                            newfield[x, y] = false;
                    }
                    else if (aliveNeighbours == 3)
                        newfield[x, y] = true;
                }

            field = newfield;
            RenderField();
        }

        public void RenderField()
        {
            if (playfield.Width <= 0 || playfield.Height <= 0) return;

            playfieldBitmap = new Bitmap(playfield.Width, playfield.Height);

            graphics = Graphics.FromImage(playfieldBitmap);

            var gridpen = new Pen(Color.White);

            int StepX = 5,
                StepY = 5;

            // Draw grid
            /*for(int x = 0; x < field.GetLength(0); x++)
            {
                graphics.DrawLine(gridpen, new Point(x * StepX, 0), new Point(x * StepX, playfield.Height));
            }      
            for(int y = 0; y < field.GetLength(1); y++)
            {
                graphics.DrawLine(gridpen, new Point(0, y * StepY), new Point(playfield.Width, y * StepY));
            }*/
            
            var lifepen = new Pen(Color.Yellow);
            // Draw life
            for (int x = 0; x < field.GetLength(0); x++)
                for (int y = 0; y < field.GetLength(1); y++)
                {
                    if(field[x,y])
                    {
                        graphics.FillRectangle(lifepen.Brush, new Rectangle(new Point(x*StepX, y*StepY), new Size(5,5)));
                    }
                }



            if (playfieldTex != null) playfieldTex.Dispose();
            playfieldTex = new Texture2D(playfieldBitmap);
        }

        void playfield_Clicked(Base sender, ClickedEventArgs arguments)
        {
            var panel = (ImagePanel)sender;
            var lPos = panel.CanvasPosToLocal(new Point(arguments.X, arguments.Y));
            int iX = (int)((double)lPos.X / (double)panel.Width * (double)field.GetLength(0));
            int iY = (int)((double)lPos.Y / (double)panel.Height * (double)field.GetLength(1));
            field[iX, iY] = !field[iX, iY];
            RenderField();
        }

        void nextStep_Clicked(Base sender, ClickedEventArgs arguments)
        {
            UpdateField();
        }

        T[,] ResizeArray<T>(T[,] original, int rows, int cols)
        {
            var newArray = new T[rows, cols];
            int minRows = Math.Min(rows, original.GetLength(0));
            int minCols = Math.Min(cols, original.GetLength(1));
            for (int i = 0; i < minRows; i++)
                for (int j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }
    }
}
