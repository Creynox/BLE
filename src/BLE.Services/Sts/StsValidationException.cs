using System;
using System.Collections.Generic;
using System.Linq;

namespace BLE.Services.Sts;

public class StsValidationException : Exception
{
    public StsValidationException(string message)
        : base(message)
    {
        Errors = Array.Empty<string>();
    }

    public StsValidationException(IEnumerable<string> errors)
        : base(string.Join(Environment.NewLine, errors ?? Array.Empty<string>()))
    {
        Errors = errors?.ToArray() ?? Array.Empty<string>();
    }

    public IReadOnlyList<string> Errors { get; }
}
