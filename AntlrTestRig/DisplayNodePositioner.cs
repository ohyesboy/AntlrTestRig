using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace AntlrTestRig
{
    class DisplayNodePositioner
    {
        private Typeface _fontType;
        private double _fontSize;
        private double _levelHeight;
        private double _padding;
        private List<double> _startPointOfLevels;


        public void PositionNodeModel(DisplayNode node, string fontName, double fontSize, double levelHeight, double padding)
        {
            this._fontType = new Typeface(fontName);
            this._fontSize = fontSize;
            this._levelHeight = levelHeight;
            this._padding = padding;
            ClearPreviousCalculation (node);
            CalculateWidth(node);
            _startPointOfLevels = new List<double>();
            ArangePosition(node, 0);
            ArangePositionPass2(node);
        }

        private void ClearPreviousCalculation(DisplayNode node)
        {
            node.Middle = 0;
            node.Top = 0;
            foreach (var child in node.Children)
            {
                ClearPreviousCalculation(child);
            }

        }

        //put ternimal nodes in the middle of surrounding nodes(if any)
        private void ArangePositionPass2(DisplayNode node)
        {
            for(int i=0;i<node.Children.Count;i++)
            {
                var child = node.Children[i];
                if (i != 0 && i != node.Children.Count - 1 && child.Children.Count == 0)
                {
                    child.Middle = (node.Children[i - 1].Middle + node.Children[i - 1].Width/2 + node.Children[i + 1].Middle - node.Children[i + 1].Width/2) / 2;
                }
                ArangePositionPass2(child);
            }

        }
        
        private void ArangePosition(DisplayNode node, int level)
        {
            if (_startPointOfLevels.Count <= level)
                _startPointOfLevels.Add(0);
            node.Top = _levelHeight * level;
            if (node.IsToken)
            {

                node.Middle = _startPointOfLevels[level] + node.Width / 2;
                _startPointOfLevels[level] += node.Width;
            }

            else
            {
                foreach (var child in node.Children)
                {
                    ArangePosition(child, level + 1);
                }

                if (node.Children.Count == 0) 
                {
                    //If the input has good syntax, a non-terminal node must have children
                    //however, when there is missing token, the children will be empty
                    //in this case, node will get its possition just like Tokens(terminal nodes)
                    node.Middle = _startPointOfLevels[level] + node.Width/2;

                }
                else
                {
                    var childrenMidPoint = node.Children.Average(x => x.Middle);
                    if (_startPointOfLevels[level] + node.Width / 2 > childrenMidPoint)
                    {
                        node.Middle = _startPointOfLevels[level] + node.Width / 2;
                        var childrenNeedsRightShift = node.Middle - childrenMidPoint;
                        ShiftRight(node, childrenNeedsRightShift, level);

                    }
                    else
                    {
                        node.Middle = childrenMidPoint;
                    }
                }
               
                _startPointOfLevels[level] = node.Middle + node.Width / 2;

            }
        }

        private void ShiftRight(DisplayNode displayNode, double amount, int level)
        {
            foreach (var child in displayNode.Children)
            {

                child.Middle += amount;
                if (_startPointOfLevels[level + 1] < child.Middle + child.Width / 2)
                    _startPointOfLevels[level + 1] = child.Middle + child.Width / 2;

                ShiftRight(child, amount, level + 1);

            }
        }

        private double StringWidth(string str)
        {
            var formattedText = new FormattedText(str,
                System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                _fontType, _fontSize, Brushes.Black);
            return formattedText.Width;
        }


        private void CalculateWidth(DisplayNode displayNode)
        {
            if (displayNode.IsToken)
            {
                displayNode.Width = StringWidth(displayNode.String) + _padding ;
            }

            else
            {
                double childrenWidth = 0.0;
                foreach (var child in displayNode.Children)
                {
                    CalculateWidth(child);
                    childrenWidth += child.Width;
                }
                displayNode.Width = StringWidth(displayNode.String) + _padding;
            }

        }
    }
}