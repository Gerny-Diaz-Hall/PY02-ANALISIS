using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PY02.ViewModels;

namespace PY02 {
    public static class ArchivoCargador {
        public static void CargarDesdeArchivo(string rutaArchivo, HospitalViewModel viewModel) {
            var lineas = File.ReadAllLines(rutaArchivo);
            bool cargandoEspecialidades = false;
            bool cargandoPacientes = false;

            foreach (var linea in lineas) {
                var l = linea.Trim();

                if (string.IsNullOrWhiteSpace(l)) continue;

                if (l.StartsWith("# Especialidades")) {
                    cargandoEspecialidades = true;
                    cargandoPacientes = false;
                    continue;
                }

                if (l.StartsWith("# Pacientes")) {
                    cargandoEspecialidades = false;
                    cargandoPacientes = true;
                    continue;
                }

                if (cargandoEspecialidades) {
                    var partes = l.Split(',');
                    if (partes.Length == 2) {
                        string nombre = partes[0].Trim();
                        if (int.TryParse(partes[1].Trim(), out int duracion)) {
                            Time.SetTime(nombre, duracion);
                            if (!viewModel.TodasLasEspecialidades.Contains(nombre)) {
                                viewModel.TodasLasEspecialidades.Add(nombre);
                            }
                        }
                    }
                }

                if (cargandoPacientes) {
                    var partes = l.Split(',');
                    if (partes.Length >= 3) {
                        string nombre = partes[0].Trim();
                        var especialidadesRaw = partes.Skip(1).ToArray();

                        List<PacienteEspecialidad> especialidades = new();
                        foreach (var e in especialidadesRaw) {
                            var sub = e.Split(';');
                            foreach (var item in sub) {
                                var especYPrio = item.Split(':');
                                if (especYPrio.Length == 2) {
                                    string esp = especYPrio[0].Trim();
                                    string prio = especYPrio[1].Trim();

                                    if (viewModel.NivelesPrioridad.TryGetValue(prio, out int nivel)) {
                                        especialidades.Add(new PacienteEspecialidad {
                                            NombreEspecialidad = esp,
                                            Prioridad = nivel
                                        });
                                    }
                                } else if (especYPrio.Length == 1 && especialidadesRaw.Length == 2) {
                                    string esp = especialidadesRaw[0].Trim();
                                    string prio = especialidadesRaw[1].Trim();
                                    if (viewModel.NivelesPrioridad.TryGetValue(prio, out int nivel)) {
                                        especialidades.Add(new PacienteEspecialidad {
                                            NombreEspecialidad = esp,
                                            Prioridad = nivel
                                        });
                                        break;
                                    }
                                }
                            }
                        }

                        viewModel.ListaEsperaGeneral.Add(new Patient {
                            Id = $"P-{Guid.NewGuid().ToString().Substring(0, 4)}",
                            Name = nombre,
                            Especialidades = especialidades,
                            ArrivalHour = DateTime.Now
                        });
                    }
                }
            }
        }
    }
}
