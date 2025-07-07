namespace AddressBook2025.Client.Services
{
    public class ToastService
    {
        public event Action<string, string>? OnShow;
        private (string Message, string Color)? _queuedToast;

        public void Show(string message, string color = "info")
        {
            if (OnShow is not null)
            {
                OnShow?.Invoke(message, color);
            }
            else
            {
                // No listeners yet → store for later
                _queuedToast = (message, color);
            }
        }

        // Called by ToastMessage component after subscribing
        public (string Message, string Color)? GetQueuedToast()
        {
            var toast = _queuedToast;
            _queuedToast = null; // only show once
            return toast;
        }
    }
}
