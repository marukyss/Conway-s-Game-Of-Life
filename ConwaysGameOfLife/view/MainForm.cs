using System;
using System.Drawing;
using System.Windows.Forms;
using ConwaysGameOfLife.controller;
using ConwaysGameOfLife.model;

namespace ConwaysGameOfLife
{
    public partial class MainForm : Form
    {
        private readonly GameController _controller;

        private PictureBox pictureBoxGrid;
        private Panel sidePanel;
        private NumericUpDown numWidth, numHeight, numBirth, numMinSurv, numMaxSurv;
        private ComboBox cmbTopology, cmbStamp;
        private Button btnStart, btnStop, btnReset, btnInitialize, btnSave, btnLoad, btnAliveColor, btnDeadColor;

        private Color _aliveColor = Color.FromArgb(0, 200, 100);
        private Color _deadColor = Color.FromArgb(24, 24, 27); 
        private bool _isDrawing = false;
        private bool _drawState = true;

        public MainForm()
        {
            this.Text = "Conway's Game of Life";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = _deadColor;

            _controller = new GameController();
            _controller.OnBoardUpdate += () => pictureBoxGrid.Invalidate();

            BuildProgrammaticUI();

            cmbTopology.SelectedIndex = 0;
        }

        private void BuildProgrammaticUI()
        { 
            pictureBoxGrid = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = _deadColor
            };
            pictureBoxGrid.Paint += pictureBoxGrid_Paint;
            pictureBoxGrid.MouseDown += pictureBoxGrid_MouseDown;
            pictureBoxGrid.MouseMove += pictureBoxGrid_MouseMove;
            pictureBoxGrid.MouseUp += (s, e) => _isDrawing = false;

            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(pictureBoxGrid, true, null);

            sidePanel = new FlowLayoutPanel
            {
                Width = 260,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(39, 39, 42),
                Padding = new Padding(15),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            AddLabel("Grid Dimensions (W x H):");
            numWidth = CreateNumberInput(50);
            numHeight = CreateNumberInput(50);

            AddLabel("Topology Space Type:");
            cmbTopology = new ComboBox { Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTopology.Items.AddRange(new string[] { "Euclidean Plane", "Torus (Wrapped)", "Klein Bottle", "Mobius Strip" });
            
            sidePanel.Controls.Add(cmbTopology);

            AddLabel("Stamp Pattern:");
            cmbStamp = new ComboBox
            {
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(63, 63, 70),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cmbStamp.Items.AddRange(new string[] { "None (Normal Draw)", "Glider", "Blinker", "Toad", "Block" });
            cmbStamp.SelectedIndex = 0;
            sidePanel.Controls.Add(cmbStamp);

            AddLabel("Rules (Birth / Min Surv / Max Surv):");
            numBirth = CreateNumberInput(3);
            numMinSurv = CreateNumberInput(2);
            numMaxSurv = CreateNumberInput(3);

            numBirth.ValueChanged += RuleControls_ValueChanged;
            numMinSurv.ValueChanged += RuleControls_ValueChanged;
            numMaxSurv.ValueChanged += RuleControls_ValueChanged;

            btnInitialize = CreateFlatButton("Build / Clear Map", Color.FromArgb(59, 130, 246), btnInitialize_Click);
            btnStart = CreateFlatButton("Run Simulation", Color.FromArgb(16, 185, 129), btnStart_Click);
            btnStop = CreateFlatButton("Pause Simulation", Color.FromArgb(239, 68, 68), btnStop_Click);
            btnReset = CreateFlatButton("Reset Map", Color.FromArgb(107, 114, 128), btnReset_Click);

            AddLabel("File Actions:");
            btnSave = CreateFlatButton("Save Blueprint", Color.FromArgb(79, 70, 229), btnSave_Click);
            btnLoad = CreateFlatButton("Load Blueprint", Color.FromArgb(79, 70, 229), btnLoad_Click);

            AddLabel("Themes:");
            btnAliveColor = CreateFlatButton("Pick Active Color", Color.FromArgb(63, 63, 70), btnAliveColor_Click);
            btnDeadColor = CreateFlatButton("Pick Empty Color", Color.FromArgb(63, 63, 70), btnDeadColor_Click);

            this.Controls.Add(pictureBoxGrid);
            this.Controls.Add(sidePanel);
        }

        private void AddLabel(string text) => sidePanel.Controls.Add(new Label { Text = text, ForeColor = Color.White, AutoSize = true, Margin = new Padding(0, 12, 0, 4) });

        private NumericUpDown CreateNumberInput(int defaultValue)
        {
            NumericUpDown num = new NumericUpDown
            {
                Width = 220,
                Minimum = 1,
                Maximum = 500,
                Value = defaultValue,
                BackColor = Color.FromArgb(63, 63, 70),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            sidePanel.Controls.Add(num);
            return num;
        }

        private Button CreateFlatButton(string text, Color baseColor, EventHandler clickEvent)
        {
            Button btn = new Button
            {
                Text = text,
                Width = 220,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = baseColor,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 6, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += clickEvent;
            sidePanel.Controls.Add(btn);
            return btn;
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            _controller.InitializeGame((int)numWidth.Value, (int)numHeight.Value, GetSelectedTopology(), (int)numBirth.Value, (int)numMinSurv.Value, (int)numMaxSurv.Value);
        }

        private ITopology GetSelectedTopology() => cmbTopology.SelectedIndex switch { 1 => new Torus(), 2 => new KleinBottle(), 3 => new MobiusStrip(), _ => new EucledeanPlane() };
        private void RuleControls_ValueChanged(object sender, EventArgs e) => _controller.UpdateRules((int)numBirth.Value, (int)numMinSurv.Value, (int)numMaxSurv.Value);
        private void btnStart_Click(object sender, EventArgs e) => _controller.ToggleSimulation(true);
        private void btnStop_Click(object sender, EventArgs e) => _controller.ToggleSimulation(false);
        private void btnReset_Click(object sender, EventArgs e) => _controller.Reset();

        private void btnSave_Click(object sender, EventArgs e)
        {
            using SaveFileDialog sfd = new SaveFileDialog { Filter = "JSON File|*.json" };
            if (sfd.ShowDialog() == DialogResult.OK) _controller.SaveGame(sfd.FileName);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog { Filter = "JSON File|*.json" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _controller.LoadGame(ofd.FileName, GetSelectedTopology(), (int)numBirth.Value, (int)numMinSurv.Value, (int)numMaxSurv.Value);
                var grid = _controller.GetCurrentGrid();
                numWidth.Value = grid.GetLength(0);
                numHeight.Value = grid.GetLength(1);
            }
        }

        private void btnAliveColor_Click(object sender, EventArgs e) { using ColorDialog cd = new ColorDialog { Color = _aliveColor }; if (cd.ShowDialog() == DialogResult.OK) { _aliveColor = cd.Color; pictureBoxGrid.Invalidate(); } }
        private void btnDeadColor_Click(object sender, EventArgs e) { using ColorDialog cd = new ColorDialog { Color = _deadColor }; if (cd.ShowDialog() == DialogResult.OK) { _deadColor = cd.Color; pictureBoxGrid.Invalidate(); } }

        private void pictureBoxGrid_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(_deadColor);

            bool[,] grid = null;
            try
            {
                grid = _controller.GetCurrentGrid();
            }
            catch (NullReferenceException)
            { 
                return;
            }

            if (grid == null) return;

            int width = grid.GetLength(0);
            int height = grid.GetLength(1);
            float cellWidth = (float)pictureBoxGrid.Width / width;
            float cellHeight = (float)pictureBoxGrid.Height / height;

            using Brush aliveBrush = new SolidBrush(_aliveColor);
            using Pen gridPen = new Pen(Color.FromArgb(20, 255, 255, 255));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (grid[x, y]) e.Graphics.FillRectangle(aliveBrush, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                    e.Graphics.DrawRectangle(gridPen, x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                }
            }
        }

        private void pictureBoxGrid_MouseDown(object sender, MouseEventArgs e) { _isDrawing = true; ApplyMouseAction(e.X, e.Y, e.Button, false); }
        private void pictureBoxGrid_MouseMove(object sender, MouseEventArgs e) { if (_isDrawing) ApplyMouseAction(e.X, e.Y, e.Button, true); }

        private void ApplyMouseAction(int mouseX, int mouseY, MouseButtons button, bool isDrag)
        {
            bool[,] grid = null;
            try { grid = _controller.GetCurrentGrid(); } catch (NullReferenceException) { return; }
            if (grid == null) return;

            if (cmbStamp.SelectedIndex > 0 && isDrag) return;

            float cellWidth = (float)pictureBoxGrid.Width / grid.GetLength(0);
            float cellHeight = (float)pictureBoxGrid.Height / grid.GetLength(1);

            int x = (int)(mouseX / cellWidth);
            int y = (int)(mouseY / cellHeight);

            if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
            {
                if (button == MouseButtons.Left) _drawState = true;
                else if (button == MouseButtons.Right) _drawState = false;

                if (cmbStamp.SelectedIndex == 0)
                {
                    _controller.ToggleCell(x, y, _drawState);
                }
                else
                {
                    ApplyStamp(x, y, _drawState, grid.GetLength(0), grid.GetLength(1));
                }
            }
        }

        private void ApplyStamp(int centerX, int centerY, bool state, int maxX, int maxY)
        {
            (int dx, int dy)[] offsets = cmbStamp.SelectedItem.ToString() switch
            {
                "Glider" => new[] { (0, -1), (1, 0), (-1, 1), (0, 1), (1, 1) },
                "Blinker" => new[] { (-1, 0), (0, 0), (1, 0) },
                "Toad" => new[] { (0, 0), (1, 0), (2, 0), (-1, 1), (0, 1), (1, 1) },
                "Block" => new[] { (0, 0), (1, 0), (0, 1), (1, 1) },
                _ => Array.Empty<(int, int)>()
            };

            foreach (var (dx, dy) in offsets)
            {
                int nx = centerX + dx;
                int ny = centerY + dy;

                if (nx >= 0 && nx < maxX && ny >= 0 && ny < maxY)
                {
                    _controller.ToggleCell(nx, ny, state);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _controller?.Dispose();
            base.OnFormClosing(e);
        }
    }
}