using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using PY02.ViewModels;
using PY02.Views;
using ReactiveUI;
using System.Windows.Input;

namespace PY02.ViewModels {
    public class MainViewModel : ViewModelBase {
        // Propiedad para la vista actual que se muestra en la ventana
        private Control _currentView;
        public Control CurrentView {
            get => _currentView;
            private set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        // Propiedad para el texto del botón de navegación (Salir/Regresar)
        private string _navButtonText;
        public string NavButtonText {
            get => _navButtonText;
            private set => this.RaiseAndSetIfChanged(ref _navButtonText, value);
        }

        // Comando para la acción del botón
        public ICommand NavButtonCommand { get; }

        // Instancias de las vistas
        private readonly MainView _mainView;
        private readonly HospitalView _hospitalView;

        public MainViewModel() {
            // Creamos una sola instancia de cada vista
            _mainView = new MainView();
            _hospitalView = new HospitalView();

            // --- Lógica de Navegación y del Botón ---
            NavButtonCommand = ReactiveCommand.Create(() => {
                // Si estamos en la vista principal, la acción es Salir
                if (CurrentView is MainView) {
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                        desktop.Shutdown();
                    }
                }
                // Si no, la acción es Regresar
                else {
                    NavigateToMain();
                }
            });

            // Establecemos la vista inicial
            NavigateToMain();
        }

        // Método para navegar a la vista del hospital
        public void NavigateToHospital() {
            CurrentView = _hospitalView;
            NavButtonText = "Regresar";
        }

        // Método para navegar a la vista principal
        private void NavigateToMain() {
            CurrentView = _mainView;
            NavButtonText = "Salir";
        }
    }
}