using System.ComponentModel.DataAnnotations;

namespace WebNovel.API.Areas.Models.Roles.Schemas
{
    public class RoleCreateUpdateEntity
    {
        [StringLength(21, ErrorMessage = "E005")]
        public string Id { get; set; }

        [StringLength(100, ErrorMessage = "E005")]
        public string Name { get; set; } = null!;
    }
}