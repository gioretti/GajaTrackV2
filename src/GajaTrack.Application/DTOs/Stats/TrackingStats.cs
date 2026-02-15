namespace GajaTrack.Application.DTOs.Stats;

public record TrackingStats(
    int NursingCount,
    int BottleCount,
    int SleepCount,
    int DiaperCount,
    int CryingCount);
