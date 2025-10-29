using System.Collections.Generic;
using BLE.Domain.Entities;

namespace BLE.Services.Sts;

public interface IStsEvaluationService
{
    StsErgebnis BerechneErgebnis(StsTest test);

    void BerechneSiebanalyse(StsTest test);

    IReadOnlyList<string> Validieren(StsTest test);
}
