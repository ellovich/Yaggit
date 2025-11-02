using System.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace CoreUI.Controls
{
    /// <summary>
    /// Контрол ActiveLoadingsControl предназначен для отображения списка активных индикаторов загрузки.
    /// </summary>
    public class ActiveLoadingsControl : TemplatedControl
    {
        /// <summary>
        /// ItemsSource – коллекция экземпляров Loading, которые отображаются в контроле.
        /// </summary>
        public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
            AvaloniaProperty.Register<ActiveLoadingsControl, IEnumerable?>(nameof(ItemsSource));
        
        public IEnumerable? ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        
        /// <summary>
        /// Свойство для задания шаблона отображения одного элемента загрузки.
        /// Это свойство позволяет переопределять визуальное представление элемента загрузки.
        /// </summary>
        public static readonly StyledProperty<IDataTemplate?> LoadingTemplateProperty =
            AvaloniaProperty.Register<ActiveLoadingsControl, IDataTemplate?>(nameof(LoadingTemplate));

        /// <summary>
        /// Шаблон, который используется для отображения элемента загрузки.
        /// Может быть задан извне, чтобы переопределить стандартное отображение.
        /// </summary>
        public IDataTemplate? LoadingTemplate
        {
            get => GetValue(LoadingTemplateProperty);
            set => SetValue(LoadingTemplateProperty, value);
        }
    }
}