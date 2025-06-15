using System;
using System.Collections.Generic;
using System.Linq;

namespace PY02
{
    /// <summary>
    /// Representa la lista de espera general de pacientes.
    /// </summary>
    public class WaitingList {
        /// <summary>
        /// Lista de pacientes registrados en espera.
        /// </summary>
        public List<Patient> RegisteredPatients { get; set; }

        public WaitingList() {
            RegisteredPatients = new List<Patient>();
        }

        /// <summary>
        /// Agrega un nuevo paciente a la lista.
        /// </summary>
        public void AgregarPaciente(Patient paciente) {
            RegisteredPatients.Add(paciente);
        }

        /// <summary>
        /// Obtiene el siguiente paciente a ser atendido, según la mayor prioridad de sus especialidades.
        /// </summary>
        public Patient GetNextPatient() {
            var pacienteUrgente = RegisteredPatients
                .Where(p => p.Especialidades.Any())
                .OrderByDescending(p => p.Especialidades.Max(e => e.Prioridad)) // Prioridad más alta primero
                .ThenBy(p => p.ArrivalHour)
                .FirstOrDefault();

            return pacienteUrgente;
        }

        /// <summary>
        /// Elimina un paciente que ya fue atendido.
        /// </summary>
        public void DeletePatient(Patient paciente) {
            RegisteredPatients.Remove(paciente);
        }

        /// <summary>
        /// Devuelve la lista completa de pacientes en espera.
        /// </summary>
        public List<Patient> VerLista() {
            return RegisteredPatients;
        }
    }

}
