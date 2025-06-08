using System;

public class WaitingList
{

    //private List<Patient> patientInWaitingList;
    public < Patient > registeredPatients { get; set; } = new List<Patient>(); //Lista de pacientes registrados

    public WaitingList()
    {
        patientInWaitingList = new List<Patient>();
    }

    // Agregar paciente a la lista
    public void AgregarPaciente(Patient patient)
    {
        patientInWaitingList.Add(patient);
    }

    // Obtener el siguiente paciente a ser atendido según la lógica de prioridad
    public Patient setUrgency(Patient patient)
    {
        // 1. Buscar pacientes urgentes (prioridad 1, 2 o 3)
        var urgent = patientInWaitingList
            .Where(p => p.Priority >= 1 && p.Priority <= 3)
            .OrderBy(p => p.Priority)
            .ThenBy(p => p.ArrivalHour)
            .ToList();

    // Eliminar paciente atendido
    public void delatePatient(Patient patient)
    {
        patientInWaitingList.Remove(patient);
    }

    // Ver lista completa
    public List<Patient> VerLista()
    {
        return patientInWaitingList;
    }
}

