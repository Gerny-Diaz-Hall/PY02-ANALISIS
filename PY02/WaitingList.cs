using System;
using System.Collections.Generic; // Necesario para List<>
using System.Linq;              // Necesario para los métodos LINQ (.Where, .OrderBy, etc.)

public class WaitingList
{
    // CORRECCIÓN: Se renombró la propiedad para seguir las convenciones de C# (PascalCase).
    // La declaración original "public < Patient > registeredPatients" era sintácticamente incorrecta.
    public List<Patient> RegisteredPatients { get; set; }

    public WaitingList()
    {
        // CORRECCIÓN: Se inicializó la propiedad correcta.
        RegisteredPatients = new List<Patient>();
    }

    // Agregar paciente a la lista
    public void AgregarPaciente(Patient patient)
    {
        RegisteredPatients.Add(patient);
    }

    // Obtener el siguiente paciente a ser atendido según la lógica de prioridad
    // CORRECCIÓN: El método ahora retorna un paciente y usa la lista correcta.
    public Patient GetNextPatient()
    {
        // 1. Buscar pacientes urgentes (prioridad 1, 2 o 3)
        var urgentPatient = RegisteredPatients
            .Where(p => p.Priority >= 1 && p.Priority <= 3)
            .OrderBy(p => p.Priority)
            .ThenBy(p => p.ArrivalHour)
            .FirstOrDefault(); // Devuelve el primer paciente o null si no hay ninguno.
        
        return urgentPatient;
    } // CORRECCIÓN: Se añadió la llave de cierre faltante del método.

    // Eliminar paciente atendido
    // CORRECCIÓN: Se cambió el nombre de "delatePatient" a "DeletePatient".
    public void DeletePatient(Patient patient)
    {
        RegisteredPatients.Remove(patient);
    }

    // Ver lista completa
    public List<Patient> VerLista()
    {
        return RegisteredPatients;
    }
}