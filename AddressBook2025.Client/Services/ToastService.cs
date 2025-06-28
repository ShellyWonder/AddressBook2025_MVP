namespace AddressBook2025.Client.Services
{
    public class ToastService
    {
        public event Action<string, string>? OnShow;

        public void Show(string message, string color = "info")
        {
            OnShow?.Invoke(message, color);
        }
    }
}
