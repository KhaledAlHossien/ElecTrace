using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Department.Command.Delete
{
    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, bool>
    {
        private readonly IDepartmentService _deptService;

        public DeleteDepartmentCommandHandler(IDepartmentService deptService)
        {
            _deptService = deptService;
        }

        public async Task<bool> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            var success = await _deptService.DeleteAsync(request.Id);

            if (!success)
            {
                throw new KeyNotFoundException($"Department with ID {request.Id} was not found.");
            }

            return true;
        }
    }
}