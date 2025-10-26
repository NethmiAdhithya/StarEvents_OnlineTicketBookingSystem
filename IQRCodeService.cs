// StarEvents/Services/IQRCodeService.cs
using StarEvents.Models;

namespace StarEvents.Services
{
    public interface IQRCodeService
    {
        /// <summary>
        /// Generates the unique string data (e.g., ticket number, booking reference) 
        /// to be encoded into the QR code image.
        /// </summary>
        /// <param name="ticket">The Ticket object containing necessary IDs.</param>
        /// <returns>A string of data to be encoded.</returns>
        string GenerateTicketData(Ticket ticket);

        /// <summary>
        /// Takes the raw encoded data string, generates a QR code image, 
        /// and converts it to a Base64 string for storage or display.
        /// </summary>
        /// <param name="data">The raw data string.</param>
        /// <returns>A Base64-encoded image string (prefixed with "data:image/png;base64,")</returns>
        string GenerateQRCodeBase64(string data);
    }
}