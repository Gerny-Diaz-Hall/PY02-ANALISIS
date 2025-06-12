using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PY02 {
    public class TupleConverter : IMultiValueConverter {
        public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
            if (values.Count >= 2) {
                return (values[0], values[1]);
            }

            throw new ArgumentException("TupleConverter necesita al menos dos valores.");
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture) {
            if (value is ValueTuple<object?, object?> tuple) {
                return new object[] { tuple.Item1!, tuple.Item2! };
            }

            throw new InvalidOperationException("Conversión inversa no válida en TupleConverter.");
        }
    }
}
