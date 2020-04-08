using System;
using System.Linq;
using FluentValidation;
using OKN.Core.Models;
using OKN.WebApp.Models.Objects;

namespace OKN.WebApp.Validation
{
    public class UpdateObjectValidator : AbstractValidator<UpdateObjectViewModel>
    {
        public UpdateObjectValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Please specify a name");
            RuleForEach(x => x.Links).Must(ValidLinkType).WithMessage("Link type not valid")
                .When(x => x.Links != null && x.Links.Any());
        }

        private static bool ValidLinkType(LinkInfo link)
        {
            return Enum.TryParse<ELinkTypes>(link.Type, true, out _);
        }
    }
}
