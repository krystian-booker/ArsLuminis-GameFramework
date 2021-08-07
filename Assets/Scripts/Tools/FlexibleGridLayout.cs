using System;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns
        }

        public FitType fitType;

        public int rows;
        
        public int columns;
        
        public Vector2 cellSize;
        
        public Vector2 spacing;

        public bool fitX;

        public bool fitY;
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (fitType == FitType.Width || fitType == FitType.Height || fitType == FitType.Uniform)
            {
                fitX = true;
                fitY = true;
                var sqrRt = Mathf.Sqrt(rectChildren.Count);
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(sqrRt);
            }

            switch (fitType)
            {
                case FitType.Width:
                case FitType.FixedColumns:
                    rows = Mathf.CeilToInt(rectChildren.Count / (float)columns);
                    break;
                case FitType.Height:
                case FitType.FixedRows:
                    rows = Mathf.CeilToInt(rectChildren.Count / (float)rows);
                    break;
                case FitType.Uniform:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var rect = rectTransform.rect;
            var parentWidth = fitX ? rect.width : cellSize.x;
            var parentHeight = fitY ?  rect.height : cellSize.y;

            var cellWidth = (parentWidth / (float) columns) - ((spacing.x / ((float) columns)) * (columns - 1)) -
                            (padding.left / (float) columns) - (padding.right / (float) columns);
            var cellHeight = (parentHeight / (float) rows) - ((spacing.y / ((float) rows)) * (rows - 1)) -
                             (padding.top / (float) rows) - (padding.bottom / (float) rows);

            cellSize.x = cellWidth;
            cellSize.y = cellHeight;

            for (var i = 0; i < rectChildren.Count; i++)
            {
                var rowCount = i / columns;
                var columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);
            }
        }

        public override void CalculateLayoutInputVertical()
        {
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}