using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Core.Services.Schemas
{
    public class JwtSettings : IValidatableObject
    {
        public string Audience { get; set; } = string.Empty;

        public string Issuer { get; set; } = string.Empty;

        public string Key { get; set; } = string.Empty;

        public int TokenExpirationInMinutes { get; set; }

        public int RefreshTokenExpirationInDays { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Key))
            {
                yield return new ValidationResult("No Key defined in JwtSettings config", new[] { nameof(Key) });
            }
        }
    }
}