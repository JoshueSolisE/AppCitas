using AppCitas.Entities;

namespace AppCitas.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}
