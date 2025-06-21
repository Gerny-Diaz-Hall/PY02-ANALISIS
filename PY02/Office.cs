using PY02.ViewModels;
using ReactiveUI;
using System.Collections.ObjectModel;
using System;

namespace PY02 {
    public class Office : ReactiveObject {
        private bool _open = true;
        private string? _especialidadSeleccionada;

        // --- INICIO: PROPIEDADES ACTUALIZADAS PARA MOSTRAR DETALLES ---
        private Patient? _pacienteEnConsulta;
        public Patient? PacienteEnConsulta {
            get => _pacienteEnConsulta;
            set => this.RaiseAndSetIfChanged(ref _pacienteEnConsulta, value);
        }

        private PacienteEspecialidad? _especialidadEnConsulta;
        public PacienteEspecialidad? EspecialidadEnConsulta {
            get => _especialidadEnConsulta;
            set => this.RaiseAndSetIfChanged(ref _especialidadEnConsulta, value);
        }

        private int _tiempoRestanteConsulta;
        public int TiempoRestanteConsulta {
            get => _tiempoRestanteConsulta;
            set => this.RaiseAndSetIfChanged(ref _tiempoRestanteConsulta, value);
        }

        // La cola de pacientes ahora usará la nueva clase auxiliar.
        public ObservableCollection<PacientQueue> PatientQueue { get; }
        // --- FIN: PROPIEDADES ACTUALIZADAS ---


        public int Id { get; }
        public string DisplayName => $"Consultorio #{Id}";
        public ObservableCollection<string> Specialties { get; }
        public HospitalViewModel? RootViewModel { get; set; }
        public ObservableCollection<string> TodasLasEspecialidades => RootViewModel?.TodasLasEspecialidades ?? new ObservableCollection<string>();

        public string? SelectedSpecialty {
            get => _especialidadSeleccionada;
            set => this.RaiseAndSetIfChanged(ref _especialidadSeleccionada, value);
        }

        public bool Open {
            get => _open;
            set => this.RaiseAndSetIfChanged(ref _open, value);
        }

        public Office(int id) {
            Id = id;
            Specialties = new ObservableCollection<string>();
            PatientQueue = new ObservableCollection<PacientQueue>(); // Tipo de colección actualizado
            AddSpecialty("Medicina General");
        }

        public void AddSpecialty(string specialty) {
            if (!string.IsNullOrWhiteSpace(specialty) && !Specialties.Contains(specialty)) {
                Specialties.Add(specialty);
            }
        }

        public void RemoveSpecialty(string specialty) {
            if (Specialties.Count > 1 && Specialties.Contains(specialty)) {
                Specialties.Remove(specialty);
            }
        }
    }
}
