using PY02.ViewModels;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PY02.ViewModels {
    // Asegúrate de que la clase sea 'public'
    public class HospitalViewModel : ViewModelBase {
        private const int MaxOffices = 15;
        private int _nextOfficeId = 1;

        public ObservableCollection<Office> Offices { get; }
        public ICommand AddOfficeCommand { get; }

        private readonly ObservableAsPropertyHelper<bool> _canAddOffice;
        public bool CanAddOffice => _canAddOffice.Value;

        public HospitalViewModel() {
            Offices = new ObservableCollection<Office>();

            var canAddExecute = this.WhenAnyValue(
                x => x.Offices.Count,
                (count) => count < MaxOffices);

            AddOfficeCommand = ReactiveCommand.Create(AddOffice, canAddExecute);

            _canAddOffice = canAddExecute.ToProperty(this, x => x.CanAddOffice);

            AddOffice();
        }

        private void AddOffice() {
            if (Offices.Count < MaxOffices) {
                Offices.Add(new Office(_nextOfficeId++));
            }
        }
    }
}