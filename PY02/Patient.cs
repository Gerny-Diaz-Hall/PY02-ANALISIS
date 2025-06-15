using System;
using System.Collections.Generic;
using System.Linq;

namespace PY02 {
    /// <summary>
    /// Representa una especialidad con su prioridad para un paciente.
    /// </summary>
    public class PacienteEspecialidad {
        public string NombreEspecialidad { get; set; } = "";
        public int Prioridad { get; set; } = 1; // 1 = Leve, 2 = Media, 3 = Extrema
    }

    /// <summary>
    /// Representa un paciente que puede tener múltiples especialidades.
    /// </summary>
    public class Patient {
        public string Id { get; set; }              // Identificador único (ej: P-1)
        public string Name { get; set; }            // Nombre del paciente
        public DateTime ArrivalHour { get; set; }   // Hora de llegada

        public List<PacienteEspecialidad> Especialidades { get; set; } = new();

        // NUEVO: permite selección manual del paciente en la UI
        public bool IsSelected { get; set; } = false;

        public Patient(string id, string name, List<PacienteEspecialidad> especialidades, DateTime arrivalHour) {
            Id = id;
            Name = name;
            Especialidades = especialidades ?? new();
            ArrivalHour = arrivalHour;
        }

        public Patient() { }

        public override string ToString() {
            var lista = string.Join(", ", Especialidades.Select(e => $"{e.NombreEspecialidad} (P{e.Prioridad})"));
            return $"[{Id}] {Name}, Especialidades: {lista}, Llegó: {ArrivalHour:HH:mm}";
        }
    }
}
