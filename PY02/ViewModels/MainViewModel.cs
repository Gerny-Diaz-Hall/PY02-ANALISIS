using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using PY02.Views;
using ReactiveUI;
using System.Windows.Input;

namespace PY02.ViewModels {
    public class MainViewModel : ViewModelBase {
        private Control _currentView;
        public Control CurrentView {
            get => _currentView;
            private set => this.RaiseAndSetIfChanged(ref _currentView, value);
        }

        private string _navButtonText;
        public string NavButtonText {
            get => _navButtonText;
            private set => this.RaiseAndSetIfChanged(ref _navButtonText, value);
        }

        public ICommand NavButtonCommand { get; }

        private readonly MainView _mainView;
        // La vista del hospital ya no será una instancia única
        // private readonly HospitalView _hospitalView; 

        public MainViewModel() {
            _mainView = new MainView();

            NavButtonCommand = ReactiveCommand.Create(() => {
                // Si la vista actual no es la principal, regresa a ella
                if (CurrentView is not MainView) {
                    NavigateToMain();
                }
                // Si es la principal, cierra la aplicación
                else {
                    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                        desktop.Shutdown();
                    }
                }
            });

            NavigateToMain();
        }

        public void NavigateToHospital() {
            // Crea una nueva vista y ViewModel cada vez para reiniciar el estado
            CurrentView = new HospitalWindow {
                DataContext = new HospitalViewModel()
            };
            NavButtonText = "Regresar";
        }

        private void NavigateToMain() {
            CurrentView = _mainView;
            NavButtonText = "Salir";
        }
    }
}