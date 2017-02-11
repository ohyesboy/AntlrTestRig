using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Microsoft.CSharp;
using Microsoft.Win32;

namespace AntlrTestRig
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DisplayNode _rootNode;
        private const int AutoExpandTreeLevel = 5;

        public MainWindow(DisplayNode model)
        {
            InitializeComponent();

            _rootNode = model;
            PopulateTree(_rootNode, 0);
            view.ShowNodeTree(_rootNode, slider.Value);
        }

        private void PopulateTree(DisplayNode node, int level, TreeViewItem parentItem = null)
        {
            var item = new TreeViewItem();
            item.Header = node.String;
            item.DataContext = node;
            item.Selected += Item_Selected;
            if (node.IsToken)
            {
                item.Foreground = Brushes.Green;
            }
              

            if (parentItem == null)
                nodeTree.Items.Add(item);
            else
                parentItem.Items.Add(item);

            if (level < AutoExpandTreeLevel)
                item.IsExpanded = true;
            foreach (var child in node.Children)
            {
                PopulateTree(child, level+1, item);
            }
        }

        private void Item_Selected(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            var node = (sender as TreeViewItem).DataContext as DisplayNode;
            if (node.IsToken)
                return;
            _rootNode = node;
            view.ShowNodeTree(node, slider.Value);
        }

        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(_rootNode!=null)
            view.ShowNodeTree(_rootNode, slider.Value);
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Png files(*.png)|*.png"
            };

            if (dialog.ShowDialog() == false)
                return;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)view.ActualWidth, (int)view.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(view);

            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));

            using (FileStream fs = new FileStream(dialog.FileName, FileMode.Create))
            {
                png.Save(fs);
            }

        }
    }
}
