using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace PensionSystem.Entities.Models
{
    public class UserPDU
    {
        public int Id { get; set; }

        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }

        public IdentityUser User { get; set; }

        [ForeignKey("PDU")]
        public int PDUId { get; set; }

        public PDU PDU { get; set; }
    }
}