using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class GameOfLife : Form
    {
        private const int cellSize = 20;

        private Vector2 size = new Vector2(1200, 600);
        private Vector2[] neighborOffsets = new Vector2[]
        {
            new Vector2(-cellSize, -cellSize), new Vector2(0, -cellSize), new Vector2(cellSize, -cellSize),
            new Vector2(-cellSize, 0), /* Cell Itself */       new Vector2(cellSize, 0),
            new Vector2(-cellSize, cellSize), new Vector2(0, cellSize), new Vector2(cellSize, cellSize)
        };

        private Dictionary<Button, bool> cells = new Dictionary<Button, bool>();
        private Dictionary<Vector2, Button> positions = new Dictionary<Vector2, Button>();

        private bool isRunning = false;

        private int Population => cells.Values.Count(v => v);
        private Label populationLabel;

        private Button startStop;
        private TrackBar alivePercentBar;
        private TextBox birthTextBox, survivalTextBox;

        private CheckBox connectTopAndBottom, connectLeftAndRight;

        public GameOfLife()
        {
            InitializeComponent();
            this.ClientSize = new Size(size.X + 200, size.Y);
            this.KeyPreview = true;

            #region timer
            Timer tmr = new Timer
            {
                Interval = 250
            };
            tmr.Tick += OnTick;
            tmr.Start();
            #endregion

            #region grid generation
            for (int x = 0; x <= this.size.X; x += cellSize)
            {
                for (int y = 0; y <= this.size.Y; y += cellSize)
                {
                    Vector2 position = new Vector2(x, y);

                    Button gridButton = new Button
                    {
                        Location = new Point(x + 1, y + 1),
                        Size = new Size(cellSize - 1, cellSize - 1),
                        BackColor = Color.Black,
                        FlatStyle = FlatStyle.Flat
                    };
                    gridButton.FlatAppearance.BorderColor = Color.Green;
                    gridButton.Click += OnButtonClick;

                    cells.Add(gridButton, false);
                    positions.Add(position, gridButton);

                    Controls.Add(gridButton);
                }
            }
            #endregion

            populationLabel = new Label
            {
                Location = new Point(size.X + 25, size.Y - 550),
                Size = new Size(160, 40),
                Text = $"Population: {Population}",
                Font = new Font("Arial", 12),
                ForeColor = Color.White
            };
            Controls.Add(populationLabel);

            #region buttons
            startStop = new Button
            {
                Location = new Point(size.X + 25, size.Y - 500),
                Size = new Size(120, 40),
                Font = new Font("Arial", 18),
                BackColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            startStop.FlatAppearance.BorderColor = Color.Green;
            startStop.FlatAppearance.BorderSize = 3;
            startStop.Click += StartOrStop;
            Controls.Add(startStop);
            Stop();


            Button button = new Button
            {
                Location = new Point(size.X + 25, size.Y - 450),
                Size = new Size(120, 40),
                Text = "Clear",
                Font = new Font("Arial", 18),
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderColor = Color.Green;
            button.FlatAppearance.BorderSize = 3;
            button.Click += (s, e) => Clear();
            Controls.Add(button);

            button = new Button
            {
                Location = new Point(size.X + 25, size.Y - 400),
                Size = new Size(120, 40),
                Text = "Random",
                Font = new Font("Arial", 18),
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderColor = Color.Green;
            button.FlatAppearance.BorderSize = 3;
            button.Click += (s, e) => Randomize();
            Controls.Add(button);
            #endregion

            alivePercentBar = new TrackBar
            {
                Location = new Point(size.X + 25, size.Y - 350),
                Size = new Size(120, 40),
                Minimum = 0,
                Maximum = 100,
                Value = 4,
                TickStyle = TickStyle.None,
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Orientation = Orientation.Horizontal
            };
            Controls.Add(alivePercentBar);

            #region birth textbox
            birthTextBox = new TextBox
            {
                Location = new Point(size.X + 25, size.Y - 270),
                Size = new Size(120, 30),
                Text = "3",
                MaxLength = 10,
                Font = new Font("Arial", 14),
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };
            birthTextBox.TextChanged += (s, e) =>
            {
                StringBuilder b = new StringBuilder();
                foreach (char c in birthTextBox.Text)
                    if (char.IsDigit(c) && !b.ToString().Contains(c))
                        b.Append(c);
                birthTextBox.Text = b.ToString();
            };
            Controls.Add(birthTextBox);
            Label label = new Label
            {
                Location = new Point(size.X + 25, size.Y - 300),
                Size = new Size(120, 20),
                Text = "Birth if:",
                Font = new Font("Arial", 10),
                ForeColor = Color.White
            };
            Controls.Add(label);
            #endregion

            #region survival textbox
            survivalTextBox = new TextBox
            {
                Location = new Point(size.X + 25, size.Y - 200),
                Size = new Size(120, 30),
                Text = "23",
                MaxLength = 10,
                Font = new Font("Arial", 14),
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };
            survivalTextBox.TextChanged += (s, e) =>
            {
                StringBuilder b = new StringBuilder();
                foreach (char c in survivalTextBox.Text)
                    if (char.IsDigit(c) && !b.ToString().Contains(c))
                        b.Append(c);
                survivalTextBox.Text = b.ToString();
            };
            Controls.Add(survivalTextBox);
            label = new Label
            {
                Location = new Point(size.X + 25, size.Y - 230),
                Size = new Size(120, 20),
                Text = "Surive if: ",
                Font = new Font("Arial", 10),
                ForeColor = Color.White
            };
            Controls.Add(label);
            #endregion

            #region checkboxes
            connectTopAndBottom = new CheckBox
            {
                Location = new Point(size.X + 25, size.Y - 150),
                Size = new Size(200, 20),
                Text = "Connect Top and Bottom",
                Font = new Font("Arial", 10),
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Checked = false
            };
            Controls.Add(connectTopAndBottom);

            connectLeftAndRight = new CheckBox
            {
                Location = new Point(size.X + 25, size.Y - 120),
                Size = new Size(200, 20),
                Text = "Connect Left and Right",
                Font = new Font("Arial", 10),
                BackColor = Color.Black,
                ForeColor = Color.Lime,
                Checked = false
            };
            Controls.Add(connectLeftAndRight);
            #endregion
        }

        #region get neighbors
        private int ReturnAliveNeighborsCount(Button button)
        {
            int count = 0;
            foreach (Button neighbor in GetNeighbors(button))
            {
                if (cells[neighbor])
                    count++;
            }
            return count;
        }
        private List<Button> GetNeighbors(Button button)
        {
            Vector2 buttonPosition = positions.KeyByValue(button);
            if (buttonPosition == null)
                return new List<Button>();

            List<Button> neighbors = new List<Button>();
            foreach (Vector2 offset in neighborOffsets)
            {
                int newX = buttonPosition.X + offset.X;
                int newY = buttonPosition.Y + offset.Y;
                if (connectLeftAndRight.Checked)
                    newX = (buttonPosition.X + offset.X + size.X + cellSize) % (size.X + cellSize);
                if (connectTopAndBottom.Checked)
                    newY = (buttonPosition.Y + offset.Y + size.Y + cellSize) % (size.Y + cellSize);

                if (positions.TryGetValue(new Vector2(newX, newY), out Button b))
                    neighbors.Add(b);
            }
            return neighbors;
        }
        #endregion

        #region on grid button click
        private void RecolorButton(Button button) => RecolorButton(button, !cells[button]);
        private void RecolorButton(Button button, bool alive)
        {
            if (alive == cells[button] || button == null)
                return;

            if (alive)
            {
                button.BackColor = Color.White;
                cells[button] = true;
            }
            else
            {
                button.BackColor = Color.Black;
                cells[button] = false;
            }
        }
        private void OnButtonClick(object sender, EventArgs e) => RecolorButton((Button)sender);
        #endregion

        #region start or stop simulation   
        private void StartOrStop(object sender, EventArgs e)
        {
            if (isRunning)
                Stop();
            else
                Start();
        }
        private void Start()
        {
            isRunning = true;
            startStop.Text = "Stop";
            startStop.ForeColor = Color.Red;
        }
        private void Stop()
        {
            isRunning = false;
            startStop.Text = "Start";
            startStop.ForeColor = Color.Lime;
        }
        #endregion

        #region states
        private void Clear()
        {
            foreach (Button button in cells.Keys.ToArray())
                RecolorButton(button, false);
        }

        private void Randomize()
        {
            Random rand = new Random();
            foreach (Button button in cells.Keys.ToArray())
                RecolorButton(button, rand.NextDouble() < alivePercentBar.Value / 100f);
        }

        private void PreviousState()
        {
            // Not implemented
        }

        private void NextState()
        {
            List<Button> toRecolor = new List<Button>();

            foreach (var pair in cells.ToArray())
            {
                Button button = pair.Key;
                bool isAlive = pair.Value;
                string aliveNeighbors = ReturnAliveNeighborsCount(button).ToString();

                if (isAlive && !survivalTextBox.Text.Contains(aliveNeighbors))
                    toRecolor.Add(button);
                else if (!isAlive && birthTextBox.Text.Contains(aliveNeighbors))
                    toRecolor.Add(button);
            }
            foreach (Button button in toRecolor)
                RecolorButton(button);
        }
        #endregion

        private void OnTick(object sender, EventArgs e)
        {
            populationLabel.Text = $"Population: {Population}";
            if (!isRunning)
                return;

            NextState();
        }
    }
}
