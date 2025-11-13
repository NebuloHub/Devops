using FluentValidation;
using NebuloHub.Application.DTOs.Request;

namespace NebuloHub.Application.Validators
{
    public class CreateStartupRequestValidator : AbstractValidator<CreateStartupRequest>
    {
        public CreateStartupRequestValidator()
        {

        }
    }
}
