using Application_Contract.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.User.Command.Delete
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من وجود المستخدم في قاعدة البيانات
            var user = await _userService.GetByIdAsync(request.Id);

            if (user == null)
            {
                // نلقي خطأ ليتم معالجته في الـ Middleware أو نرجع false
                throw new KeyNotFoundException($"User with ID {request.Id} not found.");
            }

            // 2. استدعاء ميثود الحذف من السيرفس
            await _userService.DeleteAsync(user);

            return true;
        }
    }
}   
