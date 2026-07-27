using Application.Features.Department.Queries.GetById;
using Application_Contract.DTOs.Department;
using Application_Contract.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Queries.GetUnReadYet
{
  internal class GetUnReadYetQueryHandler
: IRequestHandler<GetUnReadYetQuery, IEnumerable<DepartmentResponseDto>>
  {
    private readonly IDepartmentService _deptService;
    private readonly IMeterReadingService _meterReadingService;
    private readonly IMapper _mapper;

    public GetUnReadYetQueryHandler(IDepartmentService deptService, IMapper mapper, IMeterReadingService meterReadingService)
    {
      _deptService = deptService;
      _mapper = mapper;
      _meterReadingService = meterReadingService;
    }

    public async Task<IEnumerable<DepartmentResponseDto>> Handle(GetUnReadYetQuery request, CancellationToken cancellationToken)
    {
      var departments = await _deptService.GetAllAsync();

      var result = new List<DepartmentResponseDto>();

      foreach (var d in departments)
      {
        if (!await _meterReadingService.IsRead(d.Id, request.month, request.year) && d.IsActive && !d.IsFixed)
          result.Add(_mapper.Map<DepartmentResponseDto>(d));
      }

      return result;
    }

  }
}
