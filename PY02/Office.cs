using PY02.ViewModels;
using ReactiveUI;
using System.Collections.ObjectModel;

public class Office : ReactiveObject {
    private bool _open = true;

    public int Id { get; }

    public string DisplayName => $"Consultorio #{Id}";

    public ObservableCollection<string> Specialties { get; }
    public ObservableCollection<Patient> PatientQueue { get; }

    public HospitalViewModel? RootViewModel { get; set; }

    public bool Open {
        get => _open;
        set => this.RaiseAndSetIfChanged(ref _open, value);
    }

    public Office(int id) {
        Id = id;
        Specialties = new ObservableCollection<string>();
        PatientQueue = new ObservableCollection<Patient>();

        // Requisito: Todo consultorio inicia con al menos una especialidad
        AddSpecialty("Medicina General");
    }

    // --- Métodos de gestión de especialidades ---

    public void AddSpecialty(string specialty) {
        if (!string.IsNullOrWhiteSpace(specialty) && !Specialties.Contains(specialty)) {
            Specialties.Add(specialty);
        }
    }

    public void RemoveSpecialty(string specialty) {
        // Requisito: siempre debe haber al menos una especialidad
        if (Specialties.Count > 1 && Specialties.Contains(specialty)) {
            Specialties.Remove(specialty);
        }
    }
}
