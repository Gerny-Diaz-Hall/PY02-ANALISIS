using System;
using System.Collections.Generic;

public class Specialty { /* ... */ }

public class Office {
    public int Id { get; set; }
    public List<string> Specialties { get; set; }
    public bool Open { get; set; }

    // NUEVA PROPIEDAD: para que el {Binding DisplayName} funcione
    public string DisplayName => $"Consultorio #{Id}";

    public Office(int id) {
        Id = id;
        Specialties = new List<string>();
        Open = true;
    }

    public Office(int id, List<string> schedule, List<string> specialties, bool open) {
        Id = id + 1;
        Specialties = specialties;
        Open = open;
    }

    public bool changeStatus() {
        Open = !Open; // Forma simplificada de cambiar el booleano
        return Open;
    }

    public bool getStatus() {
        return Open;
    }
}