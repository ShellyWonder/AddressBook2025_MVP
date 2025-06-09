using System.ComponentModel.DataAnnotations;

namespace AddressBook2025.Client.Models
{
    /// <summary>
    /// This model is only used by the client to hold email data, not persisting to the database.
    /// Therefore no server side model is needed.
    /// The model can live client side without the need for a DTO.
    /// </summary>
    public class EmailData
    {
        //coma delimited list of category recipients
        public required string Recipients { get; set; }

        [Length(5, 100, ErrorMessage ="The email subject must be between 5 and 100 characters.")]
        public required string Subject { get; set; }

        [Length(10, 1000, ErrorMessage ="The email body must be between 10 and 1000 characters.")]
        public required string Body { get; set; }

    }
}
