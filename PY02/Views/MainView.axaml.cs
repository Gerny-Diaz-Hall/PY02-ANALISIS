using Avalonia.Controls;
using Avalonia.Interactivity;
using PY02.ViewModels;
using PY02.Views;

namespace PY02.Views {
    public partial class MainView : UserControl {
        public MainView() {
            InitializeComponent();
            IrAlHospitalButton.Click += OnIrAlHospitalClick;
        }
        private void OnIrAlHospitalClick(object? sender, RoutedEventArgs e) {
            // Buscamos el DataContext de la ventana principal (que es MainViewModel)
            if (this.VisualRoot is Window window && window.DataContext is MainViewModel mainViewModel) {
                // Le pedimos a ViewModel que vaya a la vista del hospital
                mainViewModel.NavigateToHospital();
            }
        }
    }
}