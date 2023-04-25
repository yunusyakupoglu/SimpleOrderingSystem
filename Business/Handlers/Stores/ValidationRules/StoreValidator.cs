
using Business.Handlers.Stores.Commands;
using FluentValidation;

namespace Business.Handlers.Stores.ValidationRules
{

    public class CreateStoreValidator : AbstractValidator<CreateStoreCommand>
    {
        public CreateStoreValidator()
        {
            //RuleFor(x => x.Stock).NotEmpty();
            //RuleFor(x => x.isReady).NotEmpty();

        }
    }
    public class UpdateStoreValidator : AbstractValidator<UpdateStoreCommand>
    {
        public UpdateStoreValidator()
        {
            //RuleFor(x => x.Stock).NotEmpty();
            //RuleFor(x => x.isReady).NotEmpty();

        }
    }
}