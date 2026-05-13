using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchService.ValueObjects.Base;

public interface IValidator<T>
{
    /// <summary>
    /// Validates data.
    /// </summary>
    /// <param name="value">The verified value</param>
    void Validate(T value);
}