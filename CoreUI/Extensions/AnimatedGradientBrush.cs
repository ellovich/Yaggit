// using Avalonia;
// using Avalonia.Media;
// using Avalonia.Threading;
//
//
// namespace CoreUI.Extensions;
//
// // private const string c1 = "#ee4c62c9";
// // private const string c2 = "#ee2a83d1";
// // private const string c3 = "#ee4c62c9";
// // private const string c4 = "#ee2a83d1";
//
// public class AnimatedGradientHelper
// {
//     private readonly DispatcherTimer _timer;
//     private double _time = 0.0;
//     private readonly LinearGradientBrush _brush;
//     public double AnimationSpeed { get; set; } = 0.01;
//
//     public Color Color1 { get; set; } = Color.Parse("#ee4c62c9");
//     public Color Color2 { get; set; } = Color.Parse("#ee2a83d1");
//     public Color Color3 { get; set; } = Color.Parse("#ee4c62c9");
//     public Color Color4 { get; set; } = Color.Parse("#ee2a83d1");
//
//     public AnimatedGradientHelper(LinearGradientBrush brush)
//     {
//         _brush = brush;
//
//         if (_brush.GradientStops.Count < 2)
//         {
//             _brush.GradientStops = new GradientStops
//             {
//                 new GradientStop { Color = Color1, Offset = 0 }, new GradientStop { Color = Color2, Offset = 1 },
//             };
//         }
//
//         _timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(30) };
//         _timer.Tick += OnTimerTick;
//         _timer.Start();
//     }
//
//     private void OnTimerTick(object? sender, EventArgs e)
//     {
//         _time += AnimationSpeed;
//         double t = (1 + Math.Sin(2 * Math.PI * _time)) / 2;
//
//         Color newColor1 = Lerp(Color1, Color2, t);
//         Color newColor2 = Lerp(Color2, Color3, t);
//         Color newColor3 = Lerp(Color3, Color4, t);
//         Color newColor4 = Lerp(Color4, Color1, t);
//
//         _brush.GradientStops[0] = new GradientStop { Color = newColor1, Offset = 0.0 };
//         _brush.GradientStops[1] = new GradientStop { Color = newColor2, Offset = 0.3 };
//         _brush.GradientStops[2] = new GradientStop { Color = newColor3, Offset = 0.6 };
//         _brush.GradientStops[3] = new GradientStop { Color = newColor4, Offset = 1.0 };
//     }
//
//     public void Stop() => _timer.Stop();
//
//     private Color Lerp(Color start, Color end, double t)
//     {
//         return Color.FromArgb(
//             (byte)(start.A + (end.A - start.A) * t),
//             (byte)(start.R + (end.R - start.R) * t),
//             (byte)(start.G + (end.G - start.G) * t),
//             (byte)(start.B + (end.B - start.B) * t)
//         );
//     }
// }
//
// public static class AnimatedGradientBehavior
// {
//     public static readonly AttachedProperty<bool> EnableAnimatedGradientProperty =
//         AvaloniaProperty.RegisterAttached<Control, bool>("EnableAnimatedGradient", typeof(AnimatedGradientBehavior));
//
//     public static bool GetEnableAnimatedGradient(Control control) => control.GetValue(EnableAnimatedGradientProperty);
//
//     public static void SetEnableAnimatedGradient(Control control, bool value) =>
//         control.SetValue(EnableAnimatedGradientProperty, value);
//
//     private static readonly Dictionary<Control, AnimatedGradientHelper> _helpers = new();
//
//     static AnimatedGradientBehavior()
//     {
//         EnableAnimatedGradientProperty.Changed.Subscribe(args =>
//             {
//                 if (args.Sender is Control control)
//                 {
//                     bool enabled = args.NewValue is Avalonia.Data.BindingValue<bool> vb ? vb.Value : (bool)args.NewValue!;
//                     if (enabled)
//                     {
//                         if (control.Background is LinearGradientBrush brush)
//                         {
//                             var helper = new AnimatedGradientHelper(brush);
//                             _helpers[control] = helper;
//                         }
//                     }
//                     else
//                     {
//                         if (_helpers.TryGetValue(control, out var helper))
//                         {
//                             helper.Stop();
//                             _helpers.Remove(control);
//                         }
//                     }
//                 }
//             }
//         );
//     }
// }