using System;
using System.Collections.Generic;
using System.Linq;

namespace PY02.Algoritmos {
    public class AG {
        private readonly List<Office> _consultorios;
        private readonly List<Patient> _pacientes;
        private readonly int _populationSize;
        private readonly int _generations;
        private readonly double _mutationRate;
        private readonly Random _random = new();

        public AG(List<Office> consultorios, List<Patient> pacientes, int populationSize = 100, int generations = 200, double mutationRate = 0.01) {
            _consultorios = consultorios.Where(c => c.Open).ToList();
            _pacientes = pacientes;
            _populationSize = populationSize;
            _generations = generations;
            _mutationRate = mutationRate;
        }

        public Dictionary<Office, List<Patient>> Run() {
            if (!_consultorios.Any() || !_pacientes.Any()) {
                return new Dictionary<Office, List<Patient>>();
            }

            var population = GenerateInitialPopulation();
            for (int i = 0; i < _generations; i++) {
                population = Evolve(population);
            }
            return GetBestIndividual(population);
        }

        private List<Dictionary<Office, List<Patient>>> GenerateInitialPopulation() {
            var population = new List<Dictionary<Office, List<Patient>>>();
            for (int i = 0; i < _populationSize; i++) {
                var assignment = new Dictionary<Office, List<Patient>>();
                foreach (var office in _consultorios) {
                    assignment[office] = new List<Patient>();
                }

                foreach (var patient in _pacientes) {
                    var possibleOffices = _consultorios.Where(o => o.Specialties.Any(s => patient.Especialidades.Any(pe => pe.NombreEspecialidad == s))).ToList();
                    if (possibleOffices.Any()) {
                        var selectedOffice = possibleOffices[_random.Next(possibleOffices.Count)];
                        assignment[selectedOffice].Add(patient);
                    }
                }
                population.Add(assignment);
            }
            return population;
        }

        private List<Dictionary<Office, List<Patient>>> Evolve(List<Dictionary<Office, List<Patient>>> population) {
            var newPopulation = new List<Dictionary<Office, List<Patient>>>();
            var bestIndividual = GetBestIndividual(population);
            newPopulation.Add(bestIndividual);

            while (newPopulation.Count < _populationSize) {
                var parent1 = TournamentSelection(population, 5);
                var parent2 = TournamentSelection(population, 5);
                var child = Crossover(parent1, parent2);
                Mutate(child);
                newPopulation.Add(child);
            }
            return newPopulation;
        }

        private Dictionary<Office, List<Patient>> TournamentSelection(List<Dictionary<Office, List<Patient>>> population, int tournamentSize) {
            var tournamentContestants = new List<Dictionary<Office, List<Patient>>>();
            for (int i = 0; i < tournamentSize; i++) {
                int randomIndex = _random.Next(population.Count);
                tournamentContestants.Add(population[randomIndex]);
            }
            return GetBestIndividual(tournamentContestants);
        }

        private Dictionary<Office, List<Patient>> Crossover(Dictionary<Office, List<Patient>> parent1, Dictionary<Office, List<Patient>> parent2) {
            var child = new Dictionary<Office, List<Patient>>();
            foreach (var office in _consultorios) {
                child[office] = new List<Patient>();
            }

            foreach (var patient in _pacientes) {
                var officeInParent1 = parent1.FirstOrDefault(kvp => kvp.Value.Contains(patient)).Key;
                var officeInParent2 = parent2.FirstOrDefault(kvp => kvp.Value.Contains(patient)).Key;

                if (officeInParent1 == null && officeInParent2 != null) {
                    child[officeInParent2].Add(patient);
                    continue;
                }
                if (officeInParent2 == null && officeInParent1 != null) {
                    child[officeInParent1].Add(patient);
                    continue;
                }
                if (officeInParent1 == null && officeInParent2 == null) continue;

                if (_random.NextDouble() < 0.5) {
                    child[officeInParent1].Add(patient);
                } else {
                    child[officeInParent2].Add(patient);
                }
            }
            return child;
        }

        private void Mutate(Dictionary<Office, List<Patient>> individual) {
            foreach (var patient in _pacientes) {
                if (_random.NextDouble() < _mutationRate) {
                    var possibleOffices = _consultorios.Where(o => o.Specialties.Any(s => patient.Especialidades.Any(pe => pe.NombreEspecialidad == s))).ToList();
                    if (possibleOffices.Any()) {
                        var newOffice = possibleOffices[_random.Next(possibleOffices.Count)];
                        var oldOffice = individual.FirstOrDefault(kvp => kvp.Value.Contains(patient)).Key;
                        if (oldOffice != null) {
                            individual[oldOffice].Remove(patient);
                            individual[newOffice].Add(patient);
                        }
                    }
                }
            }
        }

        private int Fitness(Dictionary<Office, List<Patient>> assignment) {
            var officeTimes = new Dictionary<Office, int>();
            var pacientesAsignados = new HashSet<Patient>();

            foreach (var office in _consultorios) {
                officeTimes[office] = 0;
            }

            foreach (var kvp in assignment) {
                var office = kvp.Key;
                var patients = kvp.Value;
                int officeTotalTime = 0;

                var sortedPatients = patients.OrderByDescending(p => p.Especialidades.Max(e => e.Prioridad))
                                             .ThenBy(p => p.ArrivalHour);

                foreach (var patient in sortedPatients) {
                    var selectedSpecialty = GetPrioritizedSpecialty(patient, office);
                    officeTotalTime += Time.GetTime(selectedSpecialty.NombreEspecialidad);
                    pacientesAsignados.Add(patient);
                }
                officeTimes[office] = officeTotalTime;
            }

            int penalizacion = 0;
            foreach (var paciente in _pacientes) {
                if (!pacientesAsignados.Contains(paciente)) {
                    var tieneOficinaCompatible = _consultorios.Any(c =>
                        c.Specialties.Any(s => paciente.Especialidades.Any(e => e.NombreEspecialidad == s)));

                    if (tieneOficinaCompatible) {
                        int prioridadMax = paciente.Especialidades.Max(e => e.Prioridad);
                        penalizacion += prioridadMax switch {
                            3 => 3000,
                            2 => 2000,
                            _ => 1000
                        };
                    }
                }
            }

            int tiempoMaximo = officeTimes.Values.Max();
            int tiempoTotal = officeTimes.Values.Sum();

            return (int) (0.7 * tiempoMaximo + 0.3 * tiempoTotal + penalizacion);
        }

        private PacienteEspecialidad GetPrioritizedSpecialty(Patient patient, Office office) {
            return patient.Especialidades
                         .Where(e => office.Specialties.Contains(e.NombreEspecialidad))
                         .OrderByDescending(e => e.Prioridad)
                         .ThenBy(e => Time.GetTime(e.NombreEspecialidad))
                         .First();
        }

        private Dictionary<Office, List<Patient>> GetBestIndividual(List<Dictionary<Office, List<Patient>>> population) {
            return population.OrderBy(p => Fitness(p)).First();
        }

        public static Dictionary<Office, List<Patient>> AsignarPacientesOptimo(List<Patient> pacientes, List<Office> consultorios) {
            var algoritmo = new AG(consultorios, pacientes);
            return algoritmo.Run();
        }
    }
}
