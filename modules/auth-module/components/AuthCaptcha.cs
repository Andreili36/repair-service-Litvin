using RepairService.Services.Ui;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RepairService.Modules.AuthModule.Components
{
    public sealed class AuthCaptcha : UserControl
    {
        private const int GridSize = 2;
        private const int PieceSize = 50;

        private const int BoardLeft = 0;
        private const int BoardTop = 0;

        private readonly Canvas _canvas;
        private readonly List<PuzzlePiece> _pieces = new();

        private PuzzlePiece? _draggingPiece;

        private Point _mouseOffset;

        private bool _completed;

        public event EventHandler? Completed;

        public bool IsCompleted => _completed;

        public AuthCaptcha()
        {
            _canvas = new Canvas
            {
                Width = PieceSize * GridSize,
                Height = PieceSize * GridSize,
                Background = new SolidColorBrush(
                    Color.FromRgb(245, 245, 245))
            };

            Content = _canvas;

            CreateBoard();
        }

        private BitmapSource ResizeImage(BitmapSource source)
        {
            int size = GridSize * PieceSize;

            double scaleX =
                (double)size / source.PixelWidth;

            double scaleY =
                (double)size / source.PixelHeight;

            var transformed = new TransformedBitmap(
                source,
                new ScaleTransform(scaleX, scaleY));

            transformed.Freeze();

            return transformed;
        }

        public void LoadImage(BitmapSource image)
        {
            Reset();

            var resizedImage = ResizeImage(image);

            CreatePieces(resizedImage);
            ShufflePieces();
        }

        public void Reset()
        {
            _completed = false;
            _draggingPiece = null;

            _pieces.Clear();

            _canvas.Children.Clear();

            CreateBoard();
        }

        private void CreateBoard()
        {
            var board = new Border
            {
                Width = GridSize * PieceSize,
                Height = GridSize * PieceSize,

                Background = Brushes.White
            };

            Canvas.SetLeft(board, BoardLeft);
            Canvas.SetTop(board, BoardTop);

            Panel.SetZIndex(board, -100);

            _canvas.Children.Add(board);

            var verticalLine = new Border
            {
                Width = 1,
                Height = GridSize * PieceSize,
                Background = Brushes.LightGray
            };

            Canvas.SetLeft(
                verticalLine,
                BoardLeft + PieceSize);

            Canvas.SetTop(
                verticalLine,
                BoardTop);

            Panel.SetZIndex(verticalLine, -99);

            _canvas.Children.Add(verticalLine);

            var horizontalLine = new Border
            {
                Width = GridSize * PieceSize,
                Height = 1,
                Background = Brushes.LightGray
            };

            Canvas.SetLeft(
                horizontalLine,
                BoardLeft);

            Canvas.SetTop(
                horizontalLine,
                BoardTop + PieceSize);

            Panel.SetZIndex(horizontalLine, -99);

            _canvas.Children.Add(horizontalLine);
        }

        private void CreatePieces(BitmapSource source)
        {
            if (source.PixelWidth < PieceSize * GridSize ||
                source.PixelHeight < PieceSize * GridSize)
            {
                UITools.Toast($"Изображение должно быть минимум " +
                    $"{PieceSize * GridSize}x{PieceSize * GridSize}.", Severity.Warning);
                return;
            }

            for (int row = 0; row < GridSize; row++)
            {
                for (int column = 0;
                     column < GridSize;
                     column++)
                {
                    int index =
                        row * GridSize + column;

                    var cropped = new CroppedBitmap(
                        source,
                        new Int32Rect(
                            column * PieceSize,
                            row * PieceSize,
                            PieceSize,
                            PieceSize));

                    var image = new System.Windows.Controls.Image
                    {
                        Width = PieceSize,
                        Height = PieceSize,
                        Source = cropped,
                        Stretch = Stretch.Fill
                    };

                    var border = new Border
                    {
                        Width = PieceSize,
                        Height = PieceSize,

                        Background = Brushes.White,

                        Child = image,

                        Cursor = Cursors.Hand
                    };

                    var piece = new PuzzlePiece
                    {
                        Index = index,

                        CorrectRow = row,
                        CorrectColumn = column,

                        Visual = border
                    };

                    border.MouseLeftButtonDown +=
                        Piece_MouseLeftButtonDown;

                    border.MouseMove +=
                        Piece_MouseMove;

                    border.MouseLeftButtonUp +=
                        Piece_MouseLeftButtonUp;

                    _pieces.Add(piece);
                    _canvas.Children.Add(piece.Visual);
                }
            }
        }

        private void ShufflePieces()
        {
            var cells = new List<Cell>();

            for (int row = 0; row < GridSize; row++)
            {
                for (int column = 0; column < GridSize; column++)
                {
                    cells.Add(new Cell(row, column));
                }
            }

            if (_pieces.Count != cells.Count)
            {
                UITools.Toast($"Количество пазлов ({_pieces.Count}) " +
                            $"не совпадает с количеством клеток ({cells.Count}).", Severity.Error);
                return;
            }

            for (int i = cells.Count - 1; i > 0; i--)
            {
                int j = Random.Shared.Next(i + 1);

                (cells[i], cells[j]) =
                    (cells[j], cells[i]);
            }

            for (int i = 0; i < _pieces.Count; i++)
            {
                var piece = _pieces[i];

                var cell = cells[i];

                piece.Row = cell.Row;
                piece.Column = cell.Column;

                MovePieceToCell(piece);
            }
        }

        private void Piece_MouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs e)
        {
            if (sender is not Border border)
                return;

            var piece = FindPiece(border);

            if (piece == null)
                return;

            _draggingPiece = piece;

            var mouse =
                e.GetPosition(_canvas);

            _mouseOffset = new Point(
                mouse.X - GetCellX(piece.Column),
                mouse.Y - GetCellY(piece.Row));

            Panel.SetZIndex(
                piece.Visual,
                100);

            border.CaptureMouse();

            e.Handled = true;
        }

        private void Piece_MouseMove(
            object sender,
            MouseEventArgs e)
        {
            if (_draggingPiece == null)
                return;

            if (e.LeftButton !=
                MouseButtonState.Pressed)
                return;

            var mouse =
                e.GetPosition(_canvas);

            int column =
                GetColumnFromMouse(mouse.X);

            int row =
                GetRowFromMouse(mouse.Y);

            if (column < 0 ||
                row < 0 ||
                column >= GridSize ||
                row >= GridSize)
            {
                return;
            }

            if (row != _draggingPiece.Row ||
                column != _draggingPiece.Column)
            {
                MovePieceToCell(
                    _draggingPiece,
                    row,
                    column);
            }
        }

        private void Piece_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            if (_draggingPiece == null)
                return;

            var piece = _draggingPiece;

            piece.Visual.ReleaseMouseCapture();

            Panel.SetZIndex(
                piece.Visual,
                1);

            _draggingPiece = null;

            CheckCompleted();

            e.Handled = true;
        }

        private void MovePieceToCell(
            PuzzlePiece piece,
            int row,
            int column)
        {
            if (row < 0 ||
                column < 0 ||
                row >= GridSize ||
                column >= GridSize)
                return;

            var other = _pieces.FirstOrDefault(
                x =>
                    x != piece &&
                    x.Row == row &&
                    x.Column == column);

            if (other != null)
            {
                int oldRow = piece.Row;
                int oldColumn = piece.Column;

                other.Row = oldRow;
                other.Column = oldColumn;

                piece.Row = row;
                piece.Column = column;

                MovePieceToCell(other);
                MovePieceToCell(piece);

                return;
            }

            piece.Row = row;
            piece.Column = column;

            MovePieceToCell(piece);
        }

        private void MovePieceToCell(
            PuzzlePiece piece)
        {
            Canvas.SetLeft(
                piece.Visual,
                GetCellX(piece.Column));

            Canvas.SetTop(
                piece.Visual,
                GetCellY(piece.Row));
        }

        private double GetCellX(int column)
        {
            return BoardLeft +
                   column * PieceSize;
        }

        private double GetCellY(int row)
        {
            return BoardTop +
                   row * PieceSize;
        }

        private int GetColumnFromMouse(double x)
        {
            if (x < BoardLeft)
                return -1;

            int column =
                (int)((x - BoardLeft) / PieceSize);

            return column;
        }

        private int GetRowFromMouse(double y)
        {
            if (y < BoardTop)
                return -1;

            int row =
                (int)((y - BoardTop) / PieceSize);

            return row;
        }

        private void CheckCompleted()
        {
            foreach (var piece in _pieces)
            {
                if (piece.Row != piece.CorrectRow ||
                    piece.Column != piece.CorrectColumn)
                {
                    _completed = false;
                    return;
                }
            }

            _completed = true;

            Completed?.Invoke(this, EventArgs.Empty);
        }

        private PuzzlePiece? FindPiece(Border border)
        {
            return _pieces.FirstOrDefault(
                x => x.Visual == border);
        }

        private sealed class PuzzlePiece
        {
            public int Index { get; init; }

            public int CorrectRow { get; init; }

            public int CorrectColumn { get; init; }

            public int Row { get; set; }

            public int Column { get; set; }

            public Border Visual { get; init; } = null!;
        }

        private readonly record struct Cell(int Row, int Column);
    }
}
