namespace AddressBook2025.Client.Services
{
    public class ToastService
    {
        public event Action<string, string>? OnShow;
        private (string Message, string Color)? _queuedToast;

        // Keyword mappings
        private static readonly string[] _errorKeywords = ["error", "wrong", "failed", "exception", "invalid"];
        private static readonly string[] _warningKeywords = ["warning", "try again", "not found", "no match"];
        private static readonly string[] _infoKeywords = ["info", "reminder", "note", "tip"];
        private static readonly string[] _successKeywords = ["success"]; // Only this triggers green

        public void Show(string message)
        {
            var color = GetColorForMessage(message);
            Show(message, color);
        }

        public void Show(string message, string color)
        {
            if (OnShow is not null)
            {
                OnShow.Invoke(message, color);
            }
            else
            {
                _queuedToast = (message, color); // store temporarily
            }
        }

        public (string Message, string Color)? GetQueuedToast()
        {
            var toast = _queuedToast;
            _queuedToast = null;
            return toast;
        }

        private static string GetColorForMessage(string message)
        {
            if (_errorKeywords.Any(k => message.Contains(k, StringComparison.OrdinalIgnoreCase)))
                return "danger";

            if (_warningKeywords.Any(k => message.Contains(k, StringComparison.OrdinalIgnoreCase)))
                return "warning";

            if (_infoKeywords.Any(k => message.Contains(k, StringComparison.OrdinalIgnoreCase)))
                return "info";

            if (_successKeywords.Any(k => message.Contains(k, StringComparison.OrdinalIgnoreCase)))
                return "success";

            return "info"; // Default fallback
        }
    }
}
