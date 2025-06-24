using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PY02.ViewModels;

namespace PY02 {
    public static class ArchivoCargador {
        public static void CargarDesdeArchivo(string rutaArchivo, HospitalViewModel viewModel) {
            // Mensaje de diagnóstico para ver qué ruta se está usando
            Console.WriteLine($"Intentando cargar datos desde la ruta: {rutaArchivo}");

            if (!File.Exists(rutaArchivo)) {
                // Mensaje de error claro si no se encuentra el archivo
                Console.WriteLine($"¡ERROR! Archivo no encontrado en la ruta especificada. Asegúrese de que 'datos_carga.txt' esté en el directorio de salida (bin/Debug) y que su propiedad 'Copiar en el directorio de salida' esté establecida como 'Copiar si es posterior'.");
                return;
            }

            var lineas = File.ReadAllLines(rutaArchivo);
            bool cargandoEspecialidades = false;
            bool cargandoPacientes = false;

            int especialidadesCargadas = 0;
            int pacientesCargados = 0;

            foreach (var linea in lineas) {
                var l = linea.Trim();

                if (string.IsNullOrWhiteSpace(l)) continue;
                if (l.StartsWith("#")) {
                    if (l.Contains("Especialidades")) {
                        cargandoEspecialidades = true;
                        cargandoPacientes = false;
                    } else if (l.Contains("Pacientes")) {
                        cargandoEspecialidades = false;
                        cargandoPacientes = true;
                    }
                    continue;
                }

                if (cargandoEspecialidades) {
                    var partes = l.Split(';');
                    if (partes.Length == 2) {
                        string nombre = partes[0].Trim();
                        if (int.TryParse(partes[1].Trim(), out int duracion)) {
                            Time.SetTime(nombre, duracion);
                            if (!viewModel.TodasLasEspecialidades.Contains(nombre)) {
                                viewModel.TodasLasEspecialidades.Add(nombre);
                                especialidadesCargadas++;
                            }
                        }
                    }
                }

                if (cargandoPacientes) {
                    var partes = l.Split(';');
                    if (partes.Length == 3) {
                        string nombre = partes[0].Trim();
                        string esp = partes[1].Trim();
                        string prio = partes[2].Trim();

                        if (viewModel.NivelesPrioridad.TryGetValue(prio, out int nivel)) {
                            var nuevaEspecialidad = new PacienteEspecialidad {
                                NombreEspecialidad = esp,
                                Prioridad = nivel
                            };
                            viewModel.ListaEsperaGeneral.Add(new Patient {
                                Id = $"P-{Guid.NewGuid().ToString().Substring(0, 4)}",
                                Name = nombre,
                                Especialidades = new List<PacienteEspecialidad> { nuevaEspecialidad },
                                ArrivalHour = DateTime.Now
                            });
                            pacientesCargados++;
                        }
                    }
                }
            }

            // Mensaje final de diagnóstico
            Console.WriteLine($"Carga finalizada. Especialidades cargadas: {especialidadesCargadas}. Pacientes cargados: {pacientesCargados}.");
        }
    }
}
