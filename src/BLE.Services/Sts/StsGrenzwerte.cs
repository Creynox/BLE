namespace BLE.Services.Sts;

public class StsGrenzwerte
{
    public decimal? S1Min { get; set; } = 50m;

    public decimal? S1Max { get; set; } = 80m;

    public decimal? S2Min { get; set; } = 20m;

    public decimal? S2Max { get; set; } = 40m;

    public decimal? KornformIndexMax { get; set; } = 15m;
}
