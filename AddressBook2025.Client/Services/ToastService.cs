namespace AddressBook2025.Client.Services
{
    public class ToastService
    {
        public event Action<string>? OnShow;

        public void Show(string message)
        {
            OnShow?.Invoke(message);
        }
    }
}
