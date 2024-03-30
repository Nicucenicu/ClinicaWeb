using AutoMapper;
using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Users;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Users.Queries
{
    public class UserQuery : IRequest<UserDto>
    {
        public UserQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    public class UserQueryHandler : IRequestHandler<UserQuery, UserDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public UserQueryHandler(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }
        public async Task<UserDto> Handle(UserQuery request, CancellationToken cancellationToken)
        {
            var user = _appDbContext.Users.Where(c => c.Id == request.Id).FirstOrDefault();

            return _mapper.Map<UserDto>(user);
        }
    }

    public class UserQueryValidator : AbstractValidator<UserQuery>
    {
        public UserQueryValidator(IServiceProvider services)
        {
            //Custom validations with database
            RuleFor(d => d).Custom((obj, context) =>
            {
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetService<AppDbContext>();

                if (db == null)
                {
                    context.AddFailure("Internal problem - needs admin attention.");
                    return;
                }

                var userExists = db.Users.Any(d => d.Id == obj.Id);
                if (!userExists)
                {
                    context.AddFailure("Invalid user ");
                }

            });
        }
    }

}
