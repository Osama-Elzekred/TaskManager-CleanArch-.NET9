namespace TaskManager.Application.Auth.Handlers;

using Commands;
using Dtos;
using Common.Interfaces;
using Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly IPasswordHasher _passwordHasher;
  private readonly IJwtTokenGenerator _jwtTokenGenerator;

  public RegisterCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
  {
    _unitOfWork = unitOfWork;
    _passwordHasher = passwordHasher;
    _jwtTokenGenerator = jwtTokenGenerator;
  }

  public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
  {
    var existingUser = await _unitOfWork.Users.GetAllAsync(cancellationToken);
    if (existingUser.Any(u => u.Email == request.Email))
    {
      throw new InvalidOperationException("User with this email already exists");
    }

    var user = new User
    {
      Id = Guid.NewGuid(),
      FullName = request.FullName,
      Email = request.Email,
      PasswordHash = _passwordHasher.HashPassword(request.Password),
      Role = UserRole.User,
      CreatedAt = DateTime.UtcNow
    };

    await _unitOfWork.Users.AddAsync(user, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

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
