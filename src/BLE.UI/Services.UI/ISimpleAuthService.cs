using System.Threading;
using System.Threading.Tasks;
using BLE.Data;

namespace BLE.UI.Services.UI;

public interface ISimpleAuthService
{
    Task<ApplicationUser?> SignInAsync(string username, string password, CancellationToken cancellationToken = default);
    Task SignOutAsync(CancellationToken cancellationToken = default);
}

