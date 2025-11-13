using FluentValidation;
using NebuloHub.Application.DTOs.Request;

namespace NebuloHub.Application.Validators
{
    public class CreateUsuarioRequestValidator : AbstractValidator<CreateUsuarioRequest>
    {
        public CreateUsuarioRequestValidator()
        {

        }
    }
}
