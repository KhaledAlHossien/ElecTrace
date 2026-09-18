using Application_Contract.DTOs.SystemInfo;

namespace Application_Contract.Interfaces
{
    public interface IAmeenConnectionService
    {
        Task<AmeenConnectionDto> GetAsync();
        Task<AmeenConnectionDto> UpdateAsync(UpdateAmeenConnectionRequestDto request);
        Task<AmeenConnectionTestResultDto> TestAsync(UpdateAmeenConnectionRequestDto request);
        string GetConnectionString();
    }
}
