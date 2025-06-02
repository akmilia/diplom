using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace diplom.Components
{
    public class SnackbarAdorner : Adorner
    {
        private readonly UIElement _child;

        public SnackbarAdorner(UIElement adornedElement) : base(adornedElement) { }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _child;

        protected override Size MeasureOverride(Size constraint)
        {
            _child.Measure(constraint);
            return _child.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _child.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}