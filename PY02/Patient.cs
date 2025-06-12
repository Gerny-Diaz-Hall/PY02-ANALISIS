using System;

public class Patient {
    // --- Propiedades del paciente ---

    public string Id { get; set; }                // Identificador único (ej: P-1)
    public string Name { get; set; }              // Nombre completo del paciente
    public int Age { get; set; }                  // Edad del paciente
    public string SpecialtyRequired { get; set; } // Especialidad médica requerida
    public int Priority { get; set; }             // Prioridad (0 = baja, 1 = media, 2 = alta, etc.)
    public DateTime ArrivalHour { get; set; }     // Hora de llegada al hospital

    // --- Constructor completo ---
    public Patient(string id, string name, int age, string specialty, int priority, DateTime arrivalHour) {
        Id = id;
        Name = name;
        Age = age;
        SpecialtyRequired = specialty;
        Priority = priority;
        ArrivalHour = arrivalHour;
    }

    // --- Constructor vacío (necesario para bindings o deserialización) ---
    public Patient() { }

    // --- Representación de texto útil para debugging o UI ---
    public override string ToString() {
        return $"[{Id}] {Name}, Edad: {Age}, Especialidad: {SpecialtyRequired}, Prioridad: {Priority}, Llegó: {ArrivalHour:HH:mm}";
    }
}
