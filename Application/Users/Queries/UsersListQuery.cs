using ClinicaWeb.Persistence;
using ClinicaWeb.Persistence.Entities;
using ClinicaWeb.Shared.Dtos.Pagination;
using ClinicaWeb.Shared.Dtos.Pagination.Service;
using ClinicaWeb.Shared.Dtos.Users;
using ClinicaWeb.Shared.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicaWeb.Application.Users.Queries
{
    public class UsersListQuery : IRequest<PaginationResult<UserDto>>
    {
        public PaginationParameter PaginationParameter { get; set; }
        public bool Moderators = false;

        public UsersListQuery(PaginationParameter paginationParameter, bool moderators)
        {
            PaginationParameter = paginationParameter;
            Moderators = moderators;
        }
    }

    public class UsersListQueryHandler : IRequestHandler<UsersListQuery, PaginationResult<UserDto>>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IPaginationService _paginationService;

        public UsersListQueryHandler(AppDbContext appDbContext, IPaginationService paginationService)
        {
            _appDbContext = appDbContext;
            _paginationService = paginationService;
        }
        public async Task<PaginationResult<UserDto>> Handle(UsersListQuery request, CancellationToken cancellationToken)
        {
            if (!request.Moderators)
            {
                var users = _appDbContext.Users.Where(u => u.Role == Role.Administrator);
                return await _paginationService.PaginateAsync<User, UserDto>(users, request.PaginationParameter);
            }
            else
            {
                var users = _appDbContext.Users.Where(u => u.Role != Role.Client);
                return await _paginationService.PaginateAsync<User, UserDto>(users, request.PaginationParameter);
            }
        }

    }
    public class UsersListQueryValidator : AbstractValidator<UsersListQuery>
    {
        public UsersListQueryValidator(IServiceProvider services, IHttpContextAccessor httpContextAccessor)
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

            });
        }
    }
}
