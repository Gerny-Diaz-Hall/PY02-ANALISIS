// PY02/Office.cs CORREGIDO

using System;
using System.Collections.Generic; // Asegúrate de tener este 'using' para List<T>

// Probablemente querías una clase 'Specialty'. Si no, debes usar 'string'
// Por ahora, lo cambiaré a una lista de strings para que compile.
public class Specialty { /* ... tus propiedades para la especialidad ... */ }

public class Office {
    public int Id { get; set; }

    // CORRECCIÓN 1: La lista debe ser de un solo tipo, por ejemplo 'Specialty' o 'string'.
    // Si no tienes una clase 'Specialty', puedes usar 'string' temporalmente.
    public List<string> Specialties { get; set; }
    public bool Open { get; set; }

    public Office(int id, List<string> schedule, List<string> specialties, bool open) {
        Id = id + 1;
        Specialties = specialties;
        // No tiene sentido pasar 'schedule' y 'open' si no hay dónde guardarlos.
        // Quizás necesites más propiedades en la clase Office.
        Open = open;
    }

    public bool changeStatus() {
        // CORRECCIÓN 2 y 3: Se usan paréntesis y el operador de asignación (=)
        if (Open) // if (Open) es una forma corta de if (Open == true)
        {
            Open = false; // Asignación para cambiar el valor
        } else {
            Open = true; // Asignación para cambiar el valor
        }

        // CORRECCIÓN 4: Devuelve el nuevo estado
        return Open;
    }

    public bool getStatus() {
        // CORRECCIÓN 5: Se añade el punto y coma
        return Open;
    }
}