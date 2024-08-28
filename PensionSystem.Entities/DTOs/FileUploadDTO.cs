using Microsoft.AspNetCore.Http;

namespace PensionSystem.DTOs
{
    public class FileUploadDTO
    {
        public IFormFile File { get; set; }

        public int PensionerId { get; set; }
        public FileType Type { get; set; }
    }

    public enum FileType
    {
        photo,
        new_case,
        restoration,
        wwf,
        conversion,
        cnic
    }
}