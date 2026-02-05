using GajaTrack.Application.DTOs.Protocol;

namespace GajaTrack.Application.Interfaces;

public interface IProtocolService
{
    Task<List<ProtocolDay>> GetProtocolAsync(DateOnly startDate, DateOnly endDate, bool mostRecentFirst = false, CancellationToken cancellationToken = default);
}