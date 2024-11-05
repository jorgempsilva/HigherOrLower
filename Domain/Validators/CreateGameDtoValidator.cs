using Domain.Dto;
using FluentValidation;

namespace Domain.Validators
{
    public class CreateGameDtoValidator : AbstractValidator<CreateGameDto>
    {
        public CreateGameDtoValidator()
        {
            RuleFor(x => x.Players).Must(x => x.Count > 1).WithMessage("At leat 2 players");
        }
    }
}
