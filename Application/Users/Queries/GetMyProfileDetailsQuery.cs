using AutoMapper;
using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Users.Queries
{
    public class GetMyProfileDetailsQuery : IRequest<UserDto>
    {
        public GetMyProfileDetailsQuery(int id)
        {
            Id = id;
        }

        public int Id { get; set; }
    }

    public class GetMyProfileDetailsQueryHandler : IRequestHandler<GetMyProfileDetailsQuery, UserDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMyProfileDetailsQueryHandler(AppDbContext appDbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserDto> Handle(GetMyProfileDetailsQuery request, CancellationToken cancellationToken)
        {
            var user = new User();

            var claimUserid = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            int.TryParse(claimUserid, out int claimUseridAsInt);

            if (claimUseridAsInt == request.Id)
            {
                user = _appDbContext.Users.Where(c => c.Id == request.Id).FirstOrDefault();
            }

            return _mapper.Map<UserDto>(user);
        }
    }

    public class GetMyProfileDetailsQueryValidator : AbstractValidator<GetMyProfileDetailsQuery>
    {
        public GetMyProfileDetailsQueryValidator(IServiceProvider services)
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
