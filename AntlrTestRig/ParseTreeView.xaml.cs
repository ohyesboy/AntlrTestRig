using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AntlrTestRig
{
    /// <summary>
    /// Interaction logic for ParseTreeView.xaml
    /// </summary>
    public partial class ParseTreeView : UserControl
    {

        private const double LevelHeightBase = 45;
        private const double PaddingBase = 30;
        private const double LineThicknessBase = 1;
        private const double FontSizeBase = 20;
        private double _padding;
        private FontFamily _fontFamily = new FontFamily("Arial");
        private double _needWidth;
        private double _needHeight;
        private double _fontSize;
        private double _lineThickness;

        public ParseTreeView()
        {
            InitializeComponent();
        }

        public void ShowNodeTree(DisplayNode rootNode, double zoomLevel)
        {
            if(zoomLevel<=0)
                throw new ArgumentException("zoomLevel must be > 0");
            canvas.Children.Clear();
            _needWidth = 0;
            _needHeight = 0;
            _lineThickness = zoomLevel * LineThicknessBase;
            _padding = PaddingBase * zoomLevel;
            _fontSize = FontSizeBase * zoomLevel;
            new DisplayNodePositioner().PositionNodeModel(rootNode, "Arial", _fontSize, LevelHeightBase * zoomLevel, _padding);
            canvas.Children.Clear();
            RenderNode(rootNode);
            canvas.Height = _needHeight;
            canvas.Width = _needWidth;
        }

        private void RenderNode(DisplayNode node)
        {
            TextBlock tb = new TextBlock();
            tb.FontSize = _fontSize;
            tb.FontFamily = _fontFamily;
            tb.Text = node.String;
            if (node.IsToken)
            {
                tb.Foreground = Brushes.Green;
            }

            if (node.HasError)
            {
                tb.Background =new SolidColorBrush(Color.FromRgb(244,213,211));
            }


            canvas.Children.Add(tb);
            tb.SetValue(Canvas.LeftProperty, node.Middle - node.Width / 2 + _padding / 2);
            tb.SetValue(Canvas.TopProperty, node.Top);

            _needWidth = Math.Max(_needWidth, node.Middle + node.Width / 2);
            _needHeight = Math.Max(_needHeight, node.Top + LevelHeightBase);

            foreach (var child in node.Children)
            {
                DrawLine(node.Middle, node.Top + _fontSize * 1.1, child.Middle, child.Top);
                RenderNode(child);
            }


        }

        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line();

            line.Visibility = System.Windows.Visibility.Visible;
            line.StrokeThickness = _lineThickness;
            line.Stroke = System.Windows.Media.Brushes.Black;
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            canvas.Children.Add(line);
        }

    }
}
