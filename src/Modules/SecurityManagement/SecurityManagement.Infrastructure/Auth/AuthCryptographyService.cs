using System.Security.Cryptography;
using SecurityManagement.Application.Features.AuthFeatures.Interfaces;

namespace SecurityManagement.Infrastructure.Auth;

public class AuthCryptographyService : IAuthCryptographyService
{
    public Task<int> GetSecureTokenAsync()
    {
        return Task.Run(() => RandomNumberGenerator.GetInt32(100000, 1000000));
    }

    public Task<int> GetSecureTokenAsync(int lowerBound, int upperBound)
    {
        upperBound += 1;
        return Task.Run(() => RandomNumberGenerator.GetInt32(lowerBound, upperBound));
    }

    public Task<string> HashPasswordAsync(string plainText)
    {
        return Task.Run(() => BCrypt.Net.BCrypt.HashPassword(plainText));
    }

    public Task<bool> VerifyPasswordAsync(string plainText, string hash)
    {
        return Task.Run(() => BCrypt.Net.BCrypt.Verify(plainText, hash));
    }
}
