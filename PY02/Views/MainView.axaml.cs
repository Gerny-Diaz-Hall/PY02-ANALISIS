using Avalonia.Controls;
using Avalonia.Interactivity;
using PY02.Views;

namespace PY02.Views {
    public partial class MainView : UserControl {
        public MainView() {
            InitializeComponent();
            IrAlHospitalButton.Click += OnIrAlHospitalClick;
        }
        private void OnIrAlHospitalClick(object? sender, RoutedEventArgs e) {
            var ventana = new HospitalView();
            ventana.Show();
        }
    }
}