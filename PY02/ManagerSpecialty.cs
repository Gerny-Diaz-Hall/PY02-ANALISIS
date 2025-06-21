using System;
using System.Collections.Generic;
using System.IO; // Necesario para leer archivos (File.Exists, File.ReadAllLines)

namespace PY02 {
    // Se convierte la clase a 'static' porque solo contendrá métodos de utilidad
    // y no necesita ser instanciada para usarse.
    internal static class ManagerSpecialty {
        /// <summary>
        /// Carga la lista de especialidades desde un archivo de texto.
        /// </summary>
        /// <param name="filePath">La ruta del archivo. Por defecto es 'specialties.txt'.</param>
        /// <returns>Una lista de strings con las especialidades.</returns>
        public static List<string> LoadSpecialties(string filePath = "Especialidades.txt") {
            var specialties = new List<string>();
            try {
                // Asegurarse de que el archivo exista antes de intentar leerlo.
                if (File.Exists(filePath)) {
                    // Lee todas las líneas del archivo.
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines) {
                        // Se asegura de no agregar líneas vacías a la lista.
                        if (!string.IsNullOrWhiteSpace(line)) {
                            specialties.Add(line.Trim());
                        }
                    }
                }
            } catch (Exception ex) {
                // En caso de un error (ej: permisos de archivo), se mostrará en la consola.
                Console.WriteLine($"Error al leer el archivo de especialidades: {ex.Message}");
            }
            return specialties;
        }
    }
}