using ClinicaWeb.Persistence;
using ClinicaWeb.Shared.Dtos.Users;
using MediatR;

namespace ClinicaWeb.Application.Users.Commands
{
    public class AddUserAvatarCommand : IRequest<Unit>
    {
        public AddUserAvatarCommand(UserAvatarDto model)
        {
            UserId = model.UserId;
            PhotoId = model.PhotoId;
        }
        public int UserId { get; set; }
        public int PhotoId { get; set; }
    }

    public class AddUserAvatarCommandHandler : IRequestHandler<AddUserAvatarCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public AddUserAvatarCommandHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Unit> Handle(AddUserAvatarCommand request, CancellationToken cancellationToken)
        {
            var user = _appDbContext.Users.Where(usr => usr.Id == request.UserId).FirstOrDefault();

            user.PhotoId = request.PhotoId;

            _appDbContext.SaveChanges();

            return Unit.Value;
        }
    }
}
