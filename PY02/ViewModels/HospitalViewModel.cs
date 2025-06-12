using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace PY02.ViewModels {
    public class HospitalViewModel : ViewModelBase {
        private const int MaxOffices = 15;
        private int _nextOfficeId = 1;
        private int _nextPatientId = 1;

        // --- Propiedades del formulario de paciente ---
        private string _newPatientName = "";
        public string NewPatientName {
            get => _newPatientName;
            set => this.RaiseAndSetIfChanged(ref _newPatientName, value);
        }

        private string _newPatientSpecialty = "";
        public string NewPatientSpecialty {
            get => _newPatientSpecialty;
            set => this.RaiseAndSetIfChanged(ref _newPatientSpecialty, value);
        }

        private int _newPatientPriority = 1;
        public int NewPatientPriority {
            get => _newPatientPriority;
            set => this.RaiseAndSetIfChanged(ref _newPatientPriority, value);
        }

        // --- Colecciones principales ---
        public ObservableCollection<Office> Offices { get; }
        public ObservableCollection<Patient> GeneralWaitingList { get; }
        public ObservableCollection<string> AllAvailableSpecialties { get; }

        // --- Comandos ---
        public ICommand AddOfficeCommand { get; }
        public ICommand AddPatientCommand { get; }
        public ICommand DeleteOfficeCommand { get; }
        public ICommand AddSpecialtyCommand { get; }
        public ICommand RemoveSpecialtyCommand { get; }

        private readonly ObservableAsPropertyHelper<bool> _canAddOffice;
        public bool CanAddOffice => _canAddOffice.Value;

        public HospitalViewModel() {
            Offices = new ObservableCollection<Office>();
            GeneralWaitingList = new ObservableCollection<Patient>();
            AllAvailableSpecialties = new ObservableCollection<string>(ManagerEspecialidad.LoadSpecialties());

            var canAddExecute = this.WhenAnyValue(x => x.Offices.Count, count => count < MaxOffices);
            _canAddOffice = canAddExecute.ToProperty(this, x => x.CanAddOffice);
            AddOfficeCommand = ReactiveCommand.Create(AddOffice, canAddExecute);

            var canAddPatient = this.WhenAnyValue(
                x => x.NewPatientName,
                x => x.NewPatientSpecialty,
                (name, specialty) => !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(specialty)
            );
            AddPatientCommand = ReactiveCommand.Create(AddNewPatient, canAddPatient);

            DeleteOfficeCommand = ReactiveCommand.Create<Office>(DeleteOffice);
            AddSpecialtyCommand = ReactiveCommand.Create<Tuple<Office, string>>(param => AddSpecialtyToOffice(param.Item1, param.Item2));
            RemoveSpecialtyCommand = ReactiveCommand.Create<Tuple<Office, string>>(param => RemoveSpecialtyFromOffice(param.Item1, param.Item2));

            // Se inicia con un consultorio por defecto
            AddOffice();
        }

        // --- Consultorios ---
        private void AddOffice() {
            if (Offices.Count < MaxOffices) {
                Offices.Add(new Office(_nextOfficeId++));
            }
        }

        private void DeleteOffice(Office office) {
            if (office != null && Offices.Contains(office) && Offices.Count > 1) {
                Offices.Remove(office);
            }
        }

        private void AddSpecialtyToOffice(Office office, string specialty) {
            if (office != null && !string.IsNullOrWhiteSpace(specialty)) {
                office.AddSpecialty(specialty);
            }
        }

        private void RemoveSpecialtyFromOffice(Office office, string specialty) {
            if (office != null && !string.IsNullOrWhiteSpace(specialty)) {
                office.RemoveSpecialty(specialty);
            }
        }

        // --- Pacientes ---
        private void AddNewPatient() {
            var newPatient = new Patient {
                Id = $"P-{_nextPatientId++}",
                Name = this.NewPatientName,
                SpecialtyRequired = this.NewPatientSpecialty,
                Priority = this.NewPatientPriority,
                ArrivalHour = DateTime.Now
            };

            var bestOffice = Offices
                .Where(o => o.Open && o.Specialties.Contains(newPatient.SpecialtyRequired))
                .OrderBy(o => o.PatientQueue.Count)
                .FirstOrDefault();

            if (bestOffice != null) {
                bestOffice.PatientQueue.Add(newPatient);
            } else {
                GeneralWaitingList.Add(newPatient);
            }

            // Reset formulario
            NewPatientName = "";
            NewPatientSpecialty = "";
            NewPatientPriority = 1;
        }
    }
}
