using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;
using PY02.Algoritmos;

namespace PY02.ViewModels {
    public class HospitalViewModel : ViewModelBase {
        private readonly Dictionary<Office, IDisposable> _officeSubscriptions = new();
        private const int MaximoConsultorios = 15;
        private int _siguienteIdConsultorio = 1;
        private int _siguienteIdPaciente = 1;

        // --- LÓGICA DE SIMULACIÓN Y TIEMPO ---
        private readonly System.Timers.Timer _simulationTimer;
        private readonly Time _hospitalClock = new();
        private string _currentTime = "Día: 0, 00:00";
        private bool _isSimulationRunning;

        public string CurrentTime { get => _currentTime; set => this.RaiseAndSetIfChanged(ref _currentTime, value); }
        public bool IsSimulationRunning { get => _isSimulationRunning; set => this.RaiseAndSetIfChanged(ref _isSimulationRunning, value); }
        public ICommand StartSimulationCommand { get; }
        public ICommand StopSimulationCommand { get; }

        // --- Formulario del paciente ---
        private string _nombreNuevoPaciente = "";
        public string NombreNuevoPaciente { get => _nombreNuevoPaciente; set => this.RaiseAndSetIfChanged(ref _nombreNuevoPaciente, value); }
        public ObservableCollection<PacienteEspecialidad> EspecialidadesSeleccionadas { get; } = new();
        private string _especialidadTemporal = "";
        public string EspecialidadTemporal { get => _especialidadTemporal; set => this.RaiseAndSetIfChanged(ref _especialidadTemporal, value); }
        private string _prioridadTemporal = "Leve";
        public string PrioridadTemporal { get => _prioridadTemporal; set => this.RaiseAndSetIfChanged(ref _prioridadTemporal, value); }
        public Dictionary<string, int> NivelesPrioridad { get; } = new() { { "Leve", 1 }, { "Media", 2 }, { "Extrema", 3 } };

        // --- Colecciones principales ---
        public ObservableCollection<Office> Consultorios { get; } = new();
        public ObservableCollection<Patient> ListaEsperaGeneral { get; } = new();
        public ObservableCollection<string> TodasLasEspecialidades { get; } = new(ManagerSpecialty.LoadSpecialties());

        // --- Comandos ---
        public ICommand ComandoAgregarConsultorio { get; }
        public ICommand ComandoAgregarPaciente { get; }
        public ICommand ComandoEliminarConsultorio { get; }
        public ICommand ComandoAgregarEspecialidadConsultorio { get; }
        public ICommand ComandoEliminarEspecialidadConsultorio { get; }
        public ICommand ComandoAgregarEspecialidadPaciente { get; }
        public ICommand AsignarPacientesAConsultoriosCommand { get; }
        public ICommand CargarDatosCommand { get; } // <-- NUEVO COMANDO

        private readonly ObservableAsPropertyHelper<bool> _puedeAgregarConsultorio;
        public bool PuedeAgregarConsultorio => _puedeAgregarConsultorio.Value;

        // NUEVA PROPIEDAD PARA CONTROLAR LA HABILITACIÓN DEL BOTÓN DE ELIMINAR
        private readonly ObservableAsPropertyHelper<bool> _puedeEliminarConsultorio;
        public bool PuedeEliminarConsultorio => _puedeEliminarConsultorio.Value;

        public HospitalViewModel() {
            var puedeAgregar = this.WhenAnyValue(x => x.Consultorios.Count, count => count < MaximoConsultorios);
            _puedeAgregarConsultorio = puedeAgregar.ToProperty(this, x => x.PuedeAgregarConsultorio);

            // NUEVA LÓGICA PARA ACTUALIZAR LA PROPIEDAD
            var puedeEliminar = this.WhenAnyValue(x => x.Consultorios.Count, count => count > 1);
            _puedeEliminarConsultorio = puedeEliminar.ToProperty(this, x => x.PuedeEliminarConsultorio);

            ComandoAgregarConsultorio = ReactiveCommand.Create(AgregarConsultorio, puedeAgregar);
            var puedeAgregarPaciente = this.WhenAnyValue(x => x.NombreNuevoPaciente, x => x.EspecialidadesSeleccionadas.Count, (nombre, cantidad) => !string.IsNullOrWhiteSpace(nombre) && cantidad > 0);
            ComandoAgregarPaciente = ReactiveCommand.Create(AgregarNuevoPaciente, puedeAgregarPaciente);
            ComandoEliminarConsultorio = ReactiveCommand.Create<Office>(EliminarConsultorio);
            ComandoAgregarEspecialidadConsultorio = ReactiveCommand.Create<Tuple<Office, string>>(param => { if (param?.Item1 == null || param.Item2 == null) return; AgregarEspecialidadAConsultorio(param.Item1, param.Item2); ReordenarPacientes(); });
            ComandoEliminarEspecialidadConsultorio = ReactiveCommand.Create<Tuple<Office, string>>(param => { if (param?.Item1 == null || param.Item2 == null) return; EliminarEspecialidadDeConsultorio(param.Item1, param.Item2); ReordenarPacientes(); });
            ComandoAgregarEspecialidadPaciente = ReactiveCommand.Create(AgregarEspecialidadTemporal);
            AsignarPacientesAConsultoriosCommand = ReactiveCommand.Create(AsignarPacientesConAlgoritmo);
            _simulationTimer = new System.Timers.Timer(1000);
            _simulationTimer.Elapsed += OnSimulationTick;
            _hospitalClock.OnChangeTime += (newTime) => CurrentTime = newTime;
            var canStart = this.WhenAnyValue(x => x.IsSimulationRunning, isRunning => !isRunning);
            var canStop = this.WhenAnyValue(x => x.IsSimulationRunning);
            StartSimulationCommand = ReactiveCommand.Create(StartSimulation, canStart);
            StopSimulationCommand = ReactiveCommand.Create(StopSimulation, canStop);

            CargarDatosCommand = ReactiveCommand.Create(CargarDatosDesdeArchivo); // <-- INICIALIZACIÓN DEL COMANDO

            // Se limpia para no mostrar el consultorio por defecto al iniciar
            Consultorios.Clear();
        }

        private void CargarDatosDesdeArchivo() {
            // Detener la simulación si se está ejecutando para evitar conflictos
            if (IsSimulationRunning) {
                StopSimulation();
            }

            // Limpiar completamente el estado actual
            Consultorios.Clear();
            ListaEsperaGeneral.Clear();
            TodasLasEspecialidades.Clear();
            EspecialidadesSeleccionadas.Clear(); // También limpiar las especialidades del formulario
            _siguienteIdConsultorio = 1;
            _siguienteIdPaciente = 1;

            // Cargar los datos desde el archivo. Esto poblará la lista de espera y TodasLasEspecialidades.
            ArchivoCargador.CargarDesdeArchivo("datos_carga.txt", this);

            // Obtener una lista única de las especialidades que se cargaron desde el archivo
            var especialidadesUnicas = TodasLasEspecialidades.Distinct().ToList();

            // Si el archivo no contenía especialidades (o no se encontró), la lista estará vacía.
            if (!especialidadesUnicas.Any()) {
                // Si no hay consultorios, agregar uno por defecto para que la UI no quede vacía.
                if (!Consultorios.Any()) {
                    AgregarConsultorio();
                }
                // No hay más que hacer si no se cargaron especialidades.
                return;
            }

            // Crear un consultorio por cada especialidad única encontrada en el archivo.
            foreach (var especialidad in especialidadesUnicas) {
                // Salir del bucle si se alcanza el máximo de consultorios permitidos.
                if (Consultorios.Count >= MaximoConsultorios) break;

                // 1. Crear un nuevo consultorio. Por defecto, su constructor le añade "Medicina General".
                var consultorio = new Office(_siguienteIdConsultorio++) { RootViewModel = this };

                // 2. ¡CAMBIO CLAVE! Limpiar la especialidad por defecto que añade el constructor.
                consultorio.Specialties.Clear();

                // 3. Añadir la especialidad correcta que leímos del archivo.
                consultorio.AddSpecialty(especialidad);

                // 4. Registrar el consultorio para que reaccione a cambios y añadirlo a la lista visible.
                SubscribeToOfficeChanges(consultorio);
                Consultorios.Add(consultorio);
            }
        }

        private void StartSimulation() { _hospitalClock.StartTimer(); _simulationTimer.Start(); IsSimulationRunning = true; }
        private void StopSimulation() { _hospitalClock.StopTimer(); _simulationTimer.Stop(); IsSimulationRunning = false; }
        private void OnSimulationTick(object? sender, System.Timers.ElapsedEventArgs e) { Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(ProcesarColasDePacientes); }

        private void ProcesarColasDePacientes() {
            var pacientesQueTerminaron = new List<(Patient, Office)>();
            foreach (var office in Consultorios) {
                if (office.Open && office.PacienteEnConsulta == null && office.PatientQueue.Any()) {
                    var pacienteEnCola = office.PatientQueue.First();
                    office.PatientQueue.RemoveAt(0);

                    office.PacienteEnConsulta = pacienteEnCola.Patient;
                    office.EspecialidadEnConsulta = pacienteEnCola.Specialty;
                    office.TiempoRestanteConsulta = Time.GetTime(pacienteEnCola.Specialty.NombreEspecialidad);
                }
                if (office.PacienteEnConsulta != null) {
                    office.TiempoRestanteConsulta--;
                    if (office.TiempoRestanteConsulta <= 0) {
                        pacientesQueTerminaron.Add((office.PacienteEnConsulta, office));
                        office.PacienteEnConsulta = null;
                        office.EspecialidadEnConsulta = null;
                    }
                }
            }
            if (pacientesQueTerminaron.Any()) {
                ProcesarCitasTerminadas(pacientesQueTerminaron);
            }
        }

        private void ProcesarCitasTerminadas(List<(Patient, Office)> pacientesTerminados) {
            List<Patient> pacientesParaReasignar = new();
            foreach (var (paciente, consultorio) in pacientesTerminados) {
                var especialidadAtendida = GetPrioritizedSpecialtyForOffice(paciente, consultorio);
                paciente.Especialidades.Remove(especialidadAtendida);
                if (paciente.Especialidades.Any()) {
                    pacientesParaReasignar.Add(paciente);
                }
            }
            if (pacientesParaReasignar.Any()) {
                ReordenarPacientes(pacientesParaReasignar);
            }
        }

        public void ReordenarPacientes(List<Patient>? pacientesAdicionales = null) {
            var pacientesParaReasignar = new List<Patient>();
            if (pacientesAdicionales != null) { pacientesParaReasignar.AddRange(pacientesAdicionales); }

            foreach (var office in Consultorios.Where(o => o.Open)) {
                pacientesParaReasignar.AddRange(office.PatientQueue.Select(pq => pq.Patient));
                if (office.PacienteEnConsulta != null) { pacientesParaReasignar.Add(office.PacienteEnConsulta); }
                office.PatientQueue.Clear();
                office.PacienteEnConsulta = null;
                office.EspecialidadEnConsulta = null;
            }

            foreach (var office in Consultorios.Where(o => !o.Open)) {
                pacientesParaReasignar.AddRange(office.PatientQueue.Select(pq => pq.Patient));
                office.PatientQueue.Clear();
            }

            pacientesParaReasignar = pacientesParaReasignar.Distinct().ToList();
            var consultoriosActivos = Consultorios.Where(c => c.Open).ToList();
            if (!pacientesParaReasignar.Any()) return;

            if (!consultoriosActivos.Any()) {
                foreach (var paciente in pacientesParaReasignar) {
                    if (!ListaEsperaGeneral.Contains(paciente)) ListaEsperaGeneral.Add(paciente);
                }
                return;
            }

            var asignaciones = AG.AsignarPacientesOptimo(pacientesParaReasignar, consultoriosActivos);
            var pacientesAsignados = new HashSet<Patient>();

            foreach (var kvp in asignaciones) {
                var consultorio = kvp.Key;
                var listaPacientes = kvp.Value;
                foreach (var paciente in listaPacientes) {
                    var especialidadRelevante = GetPrioritizedSpecialtyForOffice(paciente, consultorio);
                    consultorio.PatientQueue.Add(new PacientQueue(paciente, especialidadRelevante));
                    pacientesAsignados.Add(paciente);
                }
            }

            var pacientesNoAsignados = pacientesParaReasignar.Where(p => !pacientesAsignados.Contains(p)).ToList();
            foreach (var paciente in pacientesNoAsignados) {
                if (!ListaEsperaGeneral.Contains(paciente)) ListaEsperaGeneral.Add(paciente);
            }
        }

        private PacienteEspecialidad GetPrioritizedSpecialtyForOffice(Patient patient, Office office) {
            return patient.Especialidades.Where(e => office.Specialties.Contains(e.NombreEspecialidad)).OrderByDescending(e => e.Prioridad).ThenBy(e => Time.GetTime(e.NombreEspecialidad)).First();
        }

        private void AgregarConsultorio() {
            if (Consultorios.Count < MaximoConsultorios) {
                var consultorio = new Office(_siguienteIdConsultorio++) { RootViewModel = this };
                SubscribeToOfficeChanges(consultorio);
                Consultorios.Add(consultorio);
            }
        }

        private void EliminarConsultorio(Office consultorio) {
            if (consultorio != null && Consultorios.Contains(consultorio) && Consultorios.Count > 1) {
                UnsubscribeFromOfficeChanges(consultorio);
                var pacientesReubicados = consultorio.PatientQueue.Select(pq => pq.Patient).ToList();
                if (consultorio.PacienteEnConsulta != null) {
                    pacientesReubicados.Add(consultorio.PacienteEnConsulta);
                }
                Consultorios.Remove(consultorio);
                ReordenarPacientes(pacientesReubicados);
            }
        }

        private void AgregarEspecialidadAConsultorio(Office consultorio, string especialidad) {
            if (!string.IsNullOrWhiteSpace(especialidad)) {
                consultorio.AddSpecialty(especialidad);
            }
        }

        private void EliminarEspecialidadDeConsultorio(Office consultorio, string especialidad) {
            if (!string.IsNullOrWhiteSpace(especialidad)) {
                consultorio.RemoveSpecialty(especialidad);
            }
        }

        private void AgregarEspecialidadTemporal() {
            if (!string.IsNullOrWhiteSpace(EspecialidadTemporal) && !EspecialidadesSeleccionadas.Any(e => e.NombreEspecialidad == EspecialidadTemporal)) {
                EspecialidadesSeleccionadas.Add(new PacienteEspecialidad { NombreEspecialidad = EspecialidadTemporal, Prioridad = NivelesPrioridad[PrioridadTemporal] });
            }
        }

        private void AgregarNuevoPaciente() {
            var nuevo = new Patient { Id = $"P-{_siguienteIdPaciente++}", Name = this.NombreNuevoPaciente, Especialidades = EspecialidadesSeleccionadas.ToList(), ArrivalHour = DateTime.Now };
            ListaEsperaGeneral.Add(nuevo);
            NombreNuevoPaciente = "";
            EspecialidadesSeleccionadas.Clear();
            EspecialidadTemporal = "";
            PrioridadTemporal = "Leve";
        }

        private void AsignarPacientesConAlgoritmo() {
            var pacientesParaAsignar = new List<Patient>(ListaEsperaGeneral);
            if (!pacientesParaAsignar.Any()) return;
            ListaEsperaGeneral.Clear();
            ReordenarPacientes(pacientesParaAsignar);
        }

        private void SubscribeToOfficeChanges(Office office) {
            var subscription = office.WhenAnyValue(o => o.Open).Skip(1).Subscribe(_ => ReordenarPacientes());
            _officeSubscriptions[office] = subscription;
        }



        private void UnsubscribeFromOfficeChanges(Office office) {
            if (_officeSubscriptions.TryGetValue(office, out var subscription)) {
                subscription.Dispose();
                _officeSubscriptions.Remove(office);
            }
        }

    }
}
