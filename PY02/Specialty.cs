using System;
using System.Collections.Generic;

public class Specialty
{
    public string Nombre { get; set; }
    public List<string> Horarios { get; set; }

    //public List<string> Horarios { get; set; } = new List<string>();

    public Specialty()
    {
        Nombre = "Sin especificar";
        Horarios = new List<string>();
    }

    //Constructor que permite establecer el nombre y los horarios de la especialidad
    public Specialty(string nombre, List<string> horarios)
    {
        Nombre = nombre;
        Horarios = horarios ?? new List<string>();
    }

    public void AgregarHorario(string nuevoHorario)
    {
        if (!string.IsNullOrWhiteSpace(nuevoHorario))
        {
            Horarios.Add(nuevoHorario);
        }
    }

    public string getHorarios()
    {
        if (Horarios.Count == 0)
        {
            return "No hay horarios disponibles para esta especialidad";

        }
        return string.Join(",", Horarios);

    }
}
