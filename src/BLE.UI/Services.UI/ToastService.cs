using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;

namespace BLE.UI.Services.UI;

public class ToastService
{
    public Task ShowAsync(string message, string title = "Info", string accept = "OK")
    {
        return MainThread.InvokeOnMainThreadAsync(() =>
        {
            if (Application.Current?.MainPage is null)
            {
                return Task.CompletedTask;
            }

            return Application.Current.MainPage.DisplayAlert(title, message, accept);
        });
    }
}
