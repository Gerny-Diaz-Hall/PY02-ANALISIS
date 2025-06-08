using System;

public class Patient
{
    // Propiedades básicas
    public string Id { get; set; }                // ID único del paciente
    public string Name { get; set; }            // Nombre completo
    public int Age { get; set; }                 // Edad
    public string SpecialtyRequired { get; set; } // Especialidad médica solicitada
    public int Priority { get; set; }            // Nivel de urgencia (0 = baja, 1 = media, 2 = alta, etc.)
    public DateTime ArrivalHour { get; set; }     // Hora de llegada al hospital

    //public TimeSpan TiempoEstimadoAtencion { get; set; } // Tiempo estimado para atenderlo


    // Constructor
    public Patient(string id, string nombre, int edad, string Specialty, int priority, DateTime arrivalHour)//, TimeSpan tiempoAtencion)
    {
        Id = id;
        Name = nombre;
        Age = edad;
        SpecialtyRequired = Specialty;
        Priority = priority;
        ArrivalHour = arrivalHour;
        //TiempoEstimadoAtencion = tiempoAtencion;
    }

    // Constructor vacio
    public Patient()
    {
    }

    public Patient setUrgency(Patient patient)
    {
        // 1. Buscar pacientes urgentes (prioridad 1, 2 o 3)
        var urgent = patientInWaitingList
            .Where(p => p.Priority >= 1 && p.Priority <= 3)
            .OrderBy(p => p.Priority)
            .ThenBy(p => p.ArrivalHour)
            .ToList();

    // Método para mostrar la información del paciente (opcional)
    public override string ToString()
    {
        return $"[{Id}] {Nombre}, Edad: {Edad}, Especialidad: {SpecialtyRequired}, Prioridad: {Priority}, Llegó: {HoraLlegada:HH:mm}";
    }
}

