using FluentValidation;

namespace Application.Features.SystemInfo.Command.UpdateAmeenConnection
{
    public class UpdateAmeenConnectionCommandValidator : AbstractValidator<UpdateAmeenConnectionCommand>
    {
        public UpdateAmeenConnectionCommandValidator()
        {
            RuleFor(x => x.Dto.Server)
                .NotEmpty().WithMessage("اسم السيرفر مطلوب.");

            RuleFor(x => x.Dto.Database)
                .NotEmpty().WithMessage("اسم قاعدة البيانات مطلوب.");

            RuleFor(x => x.Dto.UserId)
                .NotEmpty().WithMessage("اسم المستخدم مطلوب.");
        }
    }
}
