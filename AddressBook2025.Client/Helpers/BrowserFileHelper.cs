using Microsoft.AspNetCore.Components.Forms;

namespace AddressBook2025.Client.Helpers
{
    public class BrowserFileHelper
    {
        public static readonly string DefaultContactImage ="/img/ProfilePlaceHolder.png";
        public static int MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public static async Task<string> GetDataUrl(IBrowserFile file)
        { 
            using Stream filestream = file.OpenReadStream(MaxFileSize);
            using MemoryStream ms = new();
            await filestream.CopyToAsync(ms);

            byte[] imageBytes = ms.ToArray();
            // Convert the byte array to a Base64 string(displayed as the image)
            string imageBase64 = Convert.ToBase64String(imageBytes);
            string dataUrl = $"data:{file.ContentType};base64,{imageBase64}";
            return dataUrl;

        }
        
    }
}
