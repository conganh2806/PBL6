using System.ComponentModel.DataAnnotations;

namespace WebNovel.API.Areas.Models.Roles.Schemas
{
    public class RoleDto
    {
        [StringLength(21)]
        public string Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;
    }
}