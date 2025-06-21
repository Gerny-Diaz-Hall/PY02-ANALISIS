using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PY02 {
    /// <summary>
    /// Convierte el valor numérico de la prioridad (1, 2, 3) a su
    /// representación en texto ("Leve", "Media", "Extrema").
    /// </summary>
    public class Priority_Str : IValueConverter {
        private static readonly Dictionary<int, string> PriorityMap = new Dictionary<int, string>
        {
            { 1, "Leve" },
            { 2, "Media" },
            { 3, "Extrema" }
        };

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
            // Verifica si el valor de entrada es un entero (int).
            if (value is int priorityInt) {
                // Busca el texto correspondiente en el diccionario.
                if (PriorityMap.TryGetValue(priorityInt, out var priorityString)) {
                    // Devuelve el texto formateado, por ejemplo: "(Leve)".
                    return $"({priorityString})";
                }
            }
            // Si el valor no es válido, devuelve un texto por defecto.
            return "(Desconocida)";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
            // No necesitamos convertir de texto a número, así que este método no se implementa.
            throw new NotSupportedException();
        }
    }
}
