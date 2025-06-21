using ReactiveUI;

namespace PY02 {
    /// <summary>
    /// Representa a un paciente dentro de una cola de consultorio,
    /// vinculándolo a la especialidad específica que se le atenderá allí.
    /// </summary>
    public class PacientQueue : ReactiveObject {
        /// <summary>
        /// El objeto completo del paciente.
        /// </summary>
        public Patient Patient { get; }

        /// <summary>
        /// La especialidad específica para la cual el paciente está en esta cola.
        /// </summary>
        public PacienteEspecialidad Specialty { get; }

        public PacientQueue(Patient patient, PacienteEspecialidad specialty) {
            Patient = patient;
            Specialty = specialty;
        }
    }
}
