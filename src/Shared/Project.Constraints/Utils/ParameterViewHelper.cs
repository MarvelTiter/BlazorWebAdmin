using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.Utils;

public static class ParameterViewHelper
{
    public static bool ValueHadChanged<T>(this ParameterView parameters, string parameterName, T currentValue, out T? newValue)
    {
        if (parameters.TryGetValue(parameterName, out newValue))
        {
            if (newValue is null && currentValue is null)
            {
                return false;
            }

            if (newValue is null && currentValue is not null)
            {
                return true;
            }

            if (newValue is not null && currentValue is null)
            {
                return true;
            }

            if (newValue is string[] stringNewValue && currentValue is string[] stringValue)
            {
                return !stringNewValue.SequenceEqual(stringValue);
            }

            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                return true;
            }
        }

        return false;
    }
}
