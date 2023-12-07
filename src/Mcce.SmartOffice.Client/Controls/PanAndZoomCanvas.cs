using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Mcce.SmartOffice.Client.Controls
{
    public partial class PanAndZoomCanvas : Canvas
    {
        private readonly MatrixTransform _transform = new();
        private Point _initialMousePosition;

        private bool _dragging;
        private UIElement _selectedElement;
        private Vector _draggingDelta;

        private Color _lineColor = Color.FromArgb(0xFF, 0x66, 0x66, 0x66);
        private Color _backgroundColor = Color.FromArgb(0xFF, 0x33, 0x33, 0x33);
        private readonly List<Line> _gridLines = new();

        public PanAndZoomCanvas()
        {
            MouseDown += PanAndZoomCanvas_MouseDown;
            MouseUp += PanAndZoomCanvas_MouseUp;
            MouseMove += PanAndZoomCanvas_MouseMove;
            MouseWheel += PanAndZoomCanvas_MouseWheel;

            BackgroundColor = _backgroundColor;
        }

        public float Zoomfactor { get; set; } = 1.1f;

        public bool EnableZoom { get; set; }

        public bool EnablePan { get; set; }

        public bool EnableEdit { get; set; }

        public Color LineColor
        {
            get { return _lineColor; }

            set
            {
                _lineColor = value;

                foreach (var line in _gridLines)
                {
                    line.Stroke = new SolidColorBrush(_lineColor);
                }
            }
        }

        public Color BackgroundColor
        {
            get { return _backgroundColor; }

            set
            {
                _backgroundColor = value;
                Background = new SolidColorBrush(_backgroundColor);
            }
        }

        public void SetGridVisibility(Visibility value)
        {
            foreach (var line in _gridLines)
            {
                line.Visibility = value;
            }
        }

        private void PanAndZoomCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right && EnablePan)
            {
                _initialMousePosition = _transform.Inverse.Transform(e.GetPosition(this));
            }

            if (e.ChangedButton == MouseButton.Left && EnableEdit)
            {
                if (Children.Contains((UIElement)e.Source))
                {
                    _selectedElement = (UIElement)e.Source;
                    var mousePosition = Mouse.GetPosition(this);
                    double x = Canvas.GetLeft(_selectedElement);
                    double y = Canvas.GetTop(_selectedElement);
                    var elementPosition = new Point(x, y);
                    _draggingDelta = elementPosition - mousePosition;
                }
                _dragging = true;
            }
        }

        private void PanAndZoomCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _dragging = false;
            _selectedElement = null;
        }

        private void PanAndZoomCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && EnablePan)
            {
                var mousePosition = _transform.Inverse.Transform(e.GetPosition(this));
                var delta = Point.Subtract(mousePosition, _initialMousePosition);
                var translate = new TranslateTransform(delta.X, delta.Y);
                _transform.Matrix = translate.Value * _transform.Matrix;

                foreach (UIElement child in Children)
                {
                    child.RenderTransform = _transform;
                }
            }

            if (_dragging && e.LeftButton == MouseButtonState.Pressed && EnableEdit)
            {
                double x = Mouse.GetPosition(this).X;
                double y = Mouse.GetPosition(this).Y;

                if (_selectedElement != null)
                {
                    SetLeft(_selectedElement, x + _draggingDelta.X);
                    SetTop(_selectedElement, y + _draggingDelta.Y);
                }
            }
        }

        private void PanAndZoomCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (EnableZoom)
            {
                float scaleFactor = Zoomfactor;
                if (e.Delta < 0)
                {
                    scaleFactor = 1f / scaleFactor;
                }

                var mousePostion = e.GetPosition(this);

                var scaleMatrix = _transform.Matrix;
                scaleMatrix.ScaleAt(scaleFactor, scaleFactor, mousePostion.X, mousePostion.Y);
                _transform.Matrix = scaleMatrix;

                foreach (UIElement child in Children)
                {
                    double x = GetLeft(child);
                    double y = GetTop(child);

                    double sx = x * scaleFactor;
                    double sy = y * scaleFactor;

                    SetLeft(child, sx);
                    SetTop(child, sy);

                    child.RenderTransform = _transform;
                }
            }
        }

        public void ResetZoom()
        {
            foreach (UIElement child in Children)
            {
                Canvas.SetLeft(child, 0);
                Canvas.SetTop(child, 0);

                child.RenderTransform = null;
                _transform.Matrix = new Matrix();
            }
        }
    }
}
