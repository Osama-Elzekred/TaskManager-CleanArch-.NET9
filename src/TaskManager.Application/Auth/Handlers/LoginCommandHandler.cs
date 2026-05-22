namespace TaskManager.Application.Auth.Handlers;

using Commands;
using Dtos;
using Common.Interfaces;
using Common.Exceptions;
using MediatR;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IPasswordHasher _passwordHasher;
  private readonly IJwtTokenGenerator _jwtTokenGenerator;

  public LoginCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
  {
    _unitOfWork = unitOfWork;
    _passwordHasher = passwordHasher;
    _jwtTokenGenerator = jwtTokenGenerator;
  }

  public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
    var user = users.FirstOrDefault(u => u.Email == request.Email);

    if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
    {
      throw new NotFoundException("Invalid email or password");
    }

    var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email, user.Role.ToString());

    return new AuthResponse
    {
      UserId = user.Id,
      Email = user.Email,
      FullName = user.FullName,
      Role = user.Role.ToString(),
      Token = token
    };
  }
}
