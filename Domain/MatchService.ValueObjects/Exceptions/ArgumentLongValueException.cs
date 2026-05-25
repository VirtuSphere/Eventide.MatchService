using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchService.ValueObjects.Exceptions;

public class ArgumentLongValueException(string paramName, string value, int maxLength)
: ArgumentException($"The \"{paramName}\" of note mustn't exceed {maxLength} characters. Actual length: {value.Length}.", paramName);