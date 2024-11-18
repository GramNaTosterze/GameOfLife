using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GameOfLife;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Path = System.IO.Path;
using Rectangle = System.Windows.Shapes.Rectangle;
using ValidationResult = System.Printing.ValidationResult;

namespace GameOfLife.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game? _game;
        private System.Timers.Timer _timer = new System.Timers.Timer(200);
        private bool _running = false;
        private SolidColorBrush _cellFill = Brushes.White;
        private bool _drawing = false;
        private ScaleTransform _st = new ScaleTransform();
        private Cell _lastCell = Cell.Dead();

        public MainWindow()
        {
            InitializeComponent();

            GameGrid.RenderTransform = _st;

            MouseLeftButtonUp += (_, _) => _drawing = false;

            _timer.Elapsed += (_, _) =>
            {
                if (_running)
                    NextGeneration();
            };
            _timer.Start();
        }

        #region Events

        private void NextButton_OnClick(object sender, RoutedEventArgs e) => NextGeneration();

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            _running = !_running;
            StartButton.Content = _running ? "Stop" : "Start";
        }

        private void SaveMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (_game == null)
                return;
            _game.Export(Path.Join(Directory.GetCurrentDirectory(), "SaveState"));
            MessageBox.Show("Game Saved");
        }

        private void CleanMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            _game?.Clean();
        }

        private void RandomMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            if (_game == null)
                return;

            _game = Game.Rand(_game.X, _game.Y);
            RebindGameGrid();
        }

        private void LoadMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _game = Game.Import(Path.Join(Directory.GetCurrentDirectory(), "SaveState"));
                DrawGameGrid();
                MessageBox.Show("Game Loaded");
                UpdateStats();
            }
            catch
            {
                MessageBox.Show("Cannot Load game state");
            }
        }

        private void GameGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_drawing)
                return;
            if (e.OriginalSource is not Shape tmp)
                return;

            if (tmp.DataContext is not Cell cell || cell == _lastCell)
                return;
            cell.State = !cell.State;
            _lastCell = cell;
        }

        private void GameGrid_OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is not Shape tmp)
                return;
            if (tmp.DataContext is Cell cell)
            {
                cell.State = !cell.State;
                _lastCell = cell;
            }
            _drawing = true;
        }

        private void ResizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var x = uint.Parse(GridX.Text);
                var y = uint.Parse(GridY.Text);
                if (_game == null)
                    InitGame(x, y);
                else
                    ResizeGame(x, y);
            }
            catch
            {
                MessageBox.Show($"Grid size must be integers");
            }
        }

        private void AnimationSpeedSlider_OnValueChanged(
            object sender,
            RoutedPropertyChangedEventArgs<double> e
        )
        {
            _timer.Stop();
            _timer.Interval = AnimationSpeedSlider.Maximum - AnimationSpeedSlider.Value;
            _timer.Start();
        }

        private void IsRoundMenuItem_OnChecked(object sender, RoutedEventArgs e) =>
            RebindGameGrid();

        private void GameGrid_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var posX = e.GetPosition(GameGrid).X;
            var posY = e.GetPosition(GameGrid).Y;

            DoubleAnimation an;
            if (e.Delta > 0)
            {
                an = new DoubleAnimation
                {
                    From = _st.ScaleX,
                    To = _st.ScaleX * 2,
                    Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                };
            }
            else
            {
                if (!(_st.ScaleX / 2 < 1))
                {
                    an = new DoubleAnimation
                    {
                        From = _st.ScaleX,
                        To = _st.ScaleX / 2,
                        Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    };
                }
                else
                {
                    an = new DoubleAnimation
                    {
                        From = _st.ScaleX,
                        To = 1,
                        Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                    };
                    posX = 0;
                    posY = 0;
                }
            }

            var canX = new DoubleAnimation
            {
                From = _st.CenterX,
                To = posX,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            };
            var canY = new DoubleAnimation
            {
                From = _st.CenterY,
                To = posY,
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
            };

            _st.BeginAnimation(ScaleTransform.ScaleXProperty, an);
            _st.BeginAnimation(ScaleTransform.ScaleYProperty, an);

            _st.BeginAnimation(ScaleTransform.CenterXProperty, canX);
            _st.BeginAnimation(ScaleTransform.CenterYProperty, canY);
        }

        #endregion

        #region ManipulateGameGrid
        private void NextGeneration()
        {
            if (_game == null)
                return;
            _game.NextGeneration();
            UpdateStats();
        }

        private void ResizeGame(uint x, uint y)
        {
            _game?.Resize(x, y);
            InitGame(x, y);
        }

        private void InitGame(uint x, uint y)
        {
            _game = Game.Empty(x, y);
            DrawGameGrid();
        }

        private void DrawGameGrid()
        {
            if (_game == null)
                return;

            GameGrid.ColumnDefinitions.Clear();
            GameGrid.RowDefinitions.Clear();

            for (var i = 0; i < _game.X; i++)
                GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (var i = 0; i < _game.Y; i++)
                GameGrid.RowDefinitions.Add(new RowDefinition());

            RebindGameGrid();
        }

        private void UpdateStats()
        {
            if (_game == null)
                return;
            Dispatcher.Invoke(() =>
            {
                StatsTextBlock.Text =
                    $"Statistics: \nBorn: {_game.Statistics.born}\nDied: {_game.Statistics.died}\nGeneration: {_game.Statistics.generation}";
            });
        }

        private void RebindGameGrid()
        {
            if (_game == null)
                return;

            GameGrid.Children.Clear();

            for (var x = 0; x < _game.X; x++)
            {
                for (var y = 0; y < _game.Y; y++)
                {
                    Shape cell = IsRoundMenuItem.IsChecked ? new Ellipse() : new Rectangle();
                    cell.Fill = _cellFill;
                    cell.DataContext = _game.Grid[x, y];
                    cell.SetBinding(OpacityProperty, "State");

                    Grid.SetColumn(cell, x);
                    Grid.SetRow(cell, y);
                    GameGrid.Children.Add(cell);

                    var changeColorMenuItem = new MenuItem { Header = "Change Color" };
                    changeColorMenuItem.Click += (_, _) =>
                    {
                        var colorDialog = new ColorDialog();
                        if (colorDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                            return;
                        var color = Color.FromArgb(
                            colorDialog.Color.A,
                            colorDialog.Color.R,
                            colorDialog.Color.G,
                            colorDialog.Color.B
                        );
                        _cellFill = new SolidColorBrush(color);
                        RecolorCells();
                    };
                    cell.ContextMenu = new ContextMenu();
                    cell.ContextMenu.Items.Add(changeColorMenuItem);
                }
            }
        }

        private void RecolorCells()
        {
            foreach (var child in GameGrid.Children)
            {
                if (child is not Shape cell)
                    return;
                cell.Fill = _cellFill;
            }
        }
        #endregion
    }
}
