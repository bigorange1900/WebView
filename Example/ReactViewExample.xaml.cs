using System.Windows;
using System.Windows.Input;

namespace Example {
    /// <summary>
    /// Interaction logic for ReactViewExample.xaml
    /// </summary>
    public partial class ReactViewExample : Window {
        public ReactViewExample() {
            InitializeComponent();
            exampleView.Plugins = new [] {
                new Plugin()
            };
        }

        private void OnExampleViewClick(SomeType arg) {
            MessageBox.Show("Clicked on a button inside the React view", ".Net Says");
        }

        private void OnWPFButtonClick(object sender, RoutedEventArgs e) {
            exampleView.CallMe("some text", false, 1.5);
        }

        private void OnDragPlaceholderMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop((DependencyObject) sender, "Drag Me", DragDropEffects.All);
            }
        }

        private string OnExampleViewCanDrop(string data) {
            return data;
        }
    }
}
