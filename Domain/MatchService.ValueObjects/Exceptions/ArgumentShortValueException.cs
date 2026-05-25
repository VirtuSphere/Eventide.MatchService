using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatchService.ValueObjects.Exceptions;

public class ArgumentShortValueException(string paramName, string value, int minLength)
: ArgumentException($"The \"{paramName}\" of note mustn't be shorter than {minLength} characters. Actual length: {value.Length}.", paramName);