using Application.Features.MeterReading.Command.Delete;
using Application_Contract.Interfaces;
using MediatR;

public class DeleteMeterReadingCommandHandler : IRequestHandler<DeleteMeterReadingCommand, bool>
{
    private readonly IMeterReadingService _meterReadingService;

    public DeleteMeterReadingCommandHandler(IMeterReadingService meterReadingService)
    {
        _meterReadingService = meterReadingService;
    }

    public async Task<bool> Handle(DeleteMeterReadingCommand request, CancellationToken cancellationToken)
    {
        var meterReading = await _meterReadingService.GetByIdAsync(request.Id);

        if (meterReading == null)
        {
            throw new KeyNotFoundException($"Meter reading with ID {request.Id} was not found.");
        }

        await _meterReadingService.DeleteAsync(meterReading);

        return true;
    }
}
