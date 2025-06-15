using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace PY02.ViewModels {
    public class HospitalViewModel : ViewModelBase {
        private const int MaximoConsultorios = 15;
        private int _siguienteIdConsultorio = 1;
        private int _siguienteIdPaciente = 1;

        // --- Formulario del paciente ---
        private string _nombreNuevoPaciente = "";
        public string NombreNuevoPaciente {
            get => _nombreNuevoPaciente;
            set => this.RaiseAndSetIfChanged(ref _nombreNuevoPaciente, value);
        }

        public ObservableCollection<PacienteEspecialidad> EspecialidadesSeleccionadas { get; } = new();

        private string _especialidadTemporal = "";
        public string EspecialidadTemporal {
            get => _especialidadTemporal;
            set => this.RaiseAndSetIfChanged(ref _especialidadTemporal, value);
        }

        private string _prioridadTemporal = "Leve";
        public string PrioridadTemporal {
            get => _prioridadTemporal;
            set => this.RaiseAndSetIfChanged(ref _prioridadTemporal, value);
        }

        public Dictionary<string, int> NivelesPrioridad { get; } = new()
        {
            { "Leve", 1 },
            { "Media", 2 },
            { "Extrema", 3 }
        };

        // --- Colecciones principales ---
        public ObservableCollection<Office> Consultorios { get; } = new();
        public ObservableCollection<Patient> ListaEsperaGeneral { get; } = new();
        public ObservableCollection<string> TodasLasEspecialidades { get; } = new(ManagerEspecialidad.LoadSpecialties());

        // Pacientes seleccionados mediante checkbox
        public ObservableCollection<Patient> PacientesSeleccionados { get; } = new();

        // --- Comandos ---
        public ICommand ComandoAgregarConsultorio { get; }
        public ICommand ComandoAgregarPaciente { get; }
        public ICommand ComandoEliminarConsultorio { get; }
        public ICommand ComandoAgregarEspecialidadConsultorio { get; }
        public ICommand ComandoEliminarEspecialidadConsultorio { get; }
        public ICommand ComandoAgregarEspecialidadPaciente { get; }
        public ICommand AsignarPacientesAConsultoriosCommand { get; }

        private readonly ObservableAsPropertyHelper<bool> _puedeAgregarConsultorio;
        public bool PuedeAgregarConsultorio => _puedeAgregarConsultorio.Value;

        public HospitalViewModel() {
            var puedeAgregar = this.WhenAnyValue(x => x.Consultorios.Count, count => count < MaximoConsultorios);
            _puedeAgregarConsultorio = puedeAgregar.ToProperty(this, x => x.PuedeAgregarConsultorio);
            ComandoAgregarConsultorio = ReactiveCommand.Create(AgregarConsultorio, puedeAgregar);

            var puedeAgregarPaciente = this.WhenAnyValue(
                x => x.NombreNuevoPaciente,
                x => x.EspecialidadesSeleccionadas.Count,
                (nombre, cantidad) => !string.IsNullOrWhiteSpace(nombre) && cantidad > 0
            );
            ComandoAgregarPaciente = ReactiveCommand.Create(AgregarNuevoPaciente, puedeAgregarPaciente);

            ComandoEliminarConsultorio = ReactiveCommand.Create<Office>(EliminarConsultorio);
            ComandoAgregarEspecialidadConsultorio = ReactiveCommand.Create<Tuple<Office, string>>(param => AgregarEspecialidadAConsultorio(param.Item1, param.Item2));
            ComandoEliminarEspecialidadConsultorio = ReactiveCommand.Create<Tuple<Office, string>>(param => EliminarEspecialidadDeConsultorio(param.Item1, param.Item2));
            ComandoAgregarEspecialidadPaciente = ReactiveCommand.Create(AgregarEspecialidadTemporal);
            AsignarPacientesAConsultoriosCommand = ReactiveCommand.Create(AsignarPacientesAConsultorios);

            AgregarConsultorio();
        }

        // --- Consultorios ---
        private void AgregarConsultorio() {
            if (Consultorios.Count < MaximoConsultorios) {
                Consultorios.Add(new Office(_siguienteIdConsultorio++));
            }
        }

        private void EliminarConsultorio(Office consultorio) {
            if (consultorio != null && Consultorios.Contains(consultorio) && Consultorios.Count > 1) {
                Consultorios.Remove(consultorio);
            }
        }

        private void AgregarEspecialidadAConsultorio(Office consultorio, string especialidad) {
            if (consultorio != null && !string.IsNullOrWhiteSpace(especialidad)) {
                consultorio.AddSpecialty(especialidad);
            }
        }

        private void EliminarEspecialidadDeConsultorio(Office consultorio, string especialidad) {
            if (consultorio != null && !string.IsNullOrWhiteSpace(especialidad)) {
                consultorio.RemoveSpecialty(especialidad);
            }
        }

        // --- Especialidades del paciente ---
        private void AgregarEspecialidadTemporal() {
            if (!string.IsNullOrWhiteSpace(EspecialidadTemporal) && !EspecialidadesSeleccionadas.Any(e => e.NombreEspecialidad == EspecialidadTemporal)) {
                EspecialidadesSeleccionadas.Add(new PacienteEspecialidad {
                    NombreEspecialidad = EspecialidadTemporal,
                    Prioridad = NivelesPrioridad[PrioridadTemporal]
                });
            }
        }

        // --- Agregar paciente ---
        private void AgregarNuevoPaciente() {
            var nuevo = new Patient {
                Id = $"P-{_siguienteIdPaciente++}",
                Name = this.NombreNuevoPaciente,
                Especialidades = EspecialidadesSeleccionadas.ToList(),
                ArrivalHour = DateTime.Now
            };

            ListaEsperaGeneral.Add(nuevo);

            // Limpiar formulario
            NombreNuevoPaciente = "";
            EspecialidadesSeleccionadas.Clear();
            EspecialidadTemporal = "";
            PrioridadTemporal = "Leve";
        }

        // --- Asignar pacientes seleccionados a consultorios ---
        private void AsignarPacientesAConsultorios() {
            var asignados = new List<Patient>();

            foreach (var paciente in PacientesSeleccionados) {
                var especialidadesPaciente = paciente.Especialidades.Select(e => e.NombreEspecialidad).ToList();

                var consultorio = Consultorios
                    .Where(c => c.Open)
                    .FirstOrDefault(c => c.Specialties.Any(especialidadesPaciente.Contains));

                if (consultorio != null) {
                    consultorio.PatientQueue.Add(paciente);
                    asignados.Add(paciente);
                }
            }

            foreach (var p in asignados) {
                ListaEsperaGeneral.Remove(p);
            }

            PacientesSeleccionados.Clear();
        }
    }
}
