using System;
using BLE.Data;

namespace BLE.UI.Services.UI;

public class UserSession
{
    private ApplicationUser? _currentUser;

    public event EventHandler? UserChanged;

    public ApplicationUser? CurrentUser
    {
        get => _currentUser;
        private set
        {
            _currentUser = value;
            UserChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsAuthenticated => CurrentUser is not null;
    public bool IsAdmin => CurrentUser?.IsAdmin ?? false;

    public void SetCurrentUser(ApplicationUser? user)
    {
        CurrentUser = user;
    }
}
