using Avalonia.Media;

namespace CoreUI.Controls;

public class GridLinesDecorator : Decorator
{
    public static readonly StyledProperty<bool> IsHorizontalLinesVisibleProperty =
        AvaloniaProperty.Register<GridLinesDecorator, bool>(nameof(IsHorizontalLinesVisible), true);

    public bool IsHorizontalLinesVisible
    {
        get => this.GetValue(IsHorizontalLinesVisibleProperty);
        set => SetValue(IsHorizontalLinesVisibleProperty, value);
    }

    public static readonly StyledProperty<bool> IsVerticalLinesVisibleProperty =
    AvaloniaProperty.Register<GridLinesDecorator, bool>(nameof(IsVerticalLinesVisible), true);

    public bool IsVerticalLinesVisible
    {
        get => this.GetValue(IsVerticalLinesVisibleProperty);
        set => SetValue(IsVerticalLinesVisibleProperty, value);
    }


    public static readonly StyledProperty<IBrush> BorderBrushProperty =
        AvaloniaProperty.Register<GridLinesDecorator, IBrush>(nameof(BorderBrush));

    public IBrush BorderBrush
    {
        get => this.GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }


    public static readonly StyledProperty<double> LinesThicknessProperty =
        AvaloniaProperty.Register<GridLinesDecorator, double>(nameof(LinesThickness), 1);

    public double LinesThickness
    {
        get => this.GetValue(LinesThicknessProperty);
        set => SetValue(LinesThicknessProperty, value);
    }




    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Child is Grid grid && grid.ColumnDefinitions.Count > 0)
        {
            var pen = new Pen(BorderBrush, LinesThickness);

            if (IsVerticalLinesVisible)
            {
                double x = -1;
                foreach (var colDef in grid.ColumnDefinitions.Take(grid.ColumnDefinitions.Count - 1))
                {
                    x += colDef.ActualWidth;
                    context.DrawLine(pen, new Point(x, 0), new Point(x, grid.Bounds.Height));
                }
            }

            if (IsVerticalLinesVisible)
            {
                double y = -1;
                foreach (var rowDef in grid.RowDefinitions.Take(grid.RowDefinitions.Count - 1))
                {
                    y += rowDef.ActualHeight;
                    context.DrawLine(pen, new Point(0, y), new Point(grid.Bounds.Width, y));
                }
            }
        }
    }
}