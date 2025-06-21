using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PY02 {
    public class TupleConverter : IMultiValueConverter {
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
            if (values.Count >= 2 &&
                values[0] is Office office &&
                values[1] is string especialidad) {
                return Tuple.Create(office, especialidad);
            }

            return null!;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture) {
            if (value is Tuple<Office, string> tuple) {
                return new object[] { tuple.Item1, tuple.Item2 };
            }

            throw new InvalidOperationException("Conversión inversa no válida en TupleConverter.");
        }
    }
}
