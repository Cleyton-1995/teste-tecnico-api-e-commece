using MediatR;
using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Common.Interfaces;
using RO.DevTest.Application.Exceptions;
using RO.DevTest.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace RO.DevTest.Application.Features.Auth.Commands.LoginCommand;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(IApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
            throw new UnauthorizedException("Usuário ou senha inválidos.");

        if (!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedException("Usuário ou senha inválidos.");

        var token = _tokenService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            UserName = user.Name,
            Email = user.Email
        };
    }

    private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        return computedHash.SequenceEqual(storedHash);
    }
}
