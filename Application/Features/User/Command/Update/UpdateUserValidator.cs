using Application.Features.User.Command.Update;
using FluentValidation;

namespace Application.Features.Users.Validators
{
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            // 1. التحقق من اسم المستخدم (UserName) - اختياري
            RuleFor(x => x.UserDto.UserName)
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
                .When(x => !string.IsNullOrEmpty(x.UserDto.UserName));

            // 2. التحقق من كلمة المرور (Password) - اختياري
            RuleFor(x => x.UserDto.Password)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .When(x => !string.IsNullOrEmpty(x.UserDto.Password));

            // 3. التحقق من الاسم الشخصي (Name) - اختياري
            RuleFor(x => x.UserDto.Name)
                .NotEmpty().WithMessage("Name cannot be empty if provided.") // لو بعت الحقل ما يكون فاضي
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .When(x => x.UserDto.Name != null); // يشتغل الفحص فقط لو الاسم مش null

        }
    }
}