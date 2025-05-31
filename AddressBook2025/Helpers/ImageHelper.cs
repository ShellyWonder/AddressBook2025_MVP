using AddressBook2025.Models;
using System.Text.RegularExpressions;

namespace AddressBook2025.Helpers
{
    public static class ImageHelper
    {
        public static readonly string DefaultProfilePictureUrl = "/img/ProfilePlaceHolder.png";

        public static async Task<ImageUpload> GetImageUploadAsync(IFormFile file)
        {
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            byte[] data = ms.ToArray();
            if (ms.Length > 1 * 1024 * 1024) throw new Exception("The image is too large.");

            ImageUpload imageUpload = new()
            {
                Id = Guid.NewGuid(),
                Data = data,
                Type = file.ContentType
            };
            return imageUpload;
        }

        public static ImageUpload GetImageUploadFromURL(string dataUrl)
        {
            //pulls out the type and the byte data from the dataUrl
            GroupCollection matchGroups = Regex.Match(dataUrl, @"data:(?<type>[^;]+);base64,(?<data>.+)").Groups;

            if (matchGroups.ContainsKey("type") && matchGroups.ContainsKey("data"))
            {
                //returns "image/png" or "image/jpeg" or similar
                string contentType = matchGroups["type"].Value;
                //converts data from base64 to byte array
                byte[] data = Convert.FromBase64String(matchGroups["data"].Value);
                if (data.Length <= 5 * 1024 * 1024)//5 MB max size
                {
                    ImageUpload upload = new ImageUpload
                    {
                        Id = Guid.NewGuid(),
                        Data = data,
                        Type = contentType
                    };
                    return upload;
                }

            }
            throw new IOException("The image is either too large or invalid.");

        }
    }

}

