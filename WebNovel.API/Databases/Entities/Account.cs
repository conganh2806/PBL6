using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Databases.Entitites
{
        public class Account : TableHaveIdString
        {
                public Account()
                {
                        Comments = new HashSet<Comment>();
                        Bookmarkeds = new HashSet<Bookmarked>();
                        Preferences = new HashSet<Preferences>();
                        Roles = new HashSet<RolesOfUser>();
                        Ratings = new HashSet<Rating>();
                        Orders = new HashSet<Order>();
                        ChapterOfAccounts = new HashSet<ChapterOfAccount>();
                        Payouts = new HashSet<Payout>();
                        Reports = new HashSet<Report>();
                }

                [StringLength(100)]
                public string Username { get; set; } = null!;
                [StringLength(255)]
                public string? Password { get; set; }
                [StringLength(255)]
                public string Email { get; set; } = null!;
                [StringLength(100)]
                public string? NickName { get; set; }
                [DataType(DataType.Date)]
                public DateOnly? DateJoined { get; set; }
                [StringLength(500)]
                public string? ImageURL { get; set; }
                public int? Status { get; set; }
                public float WalletAmmount { get; set; }
                public float CreatorWallet { get; set; }
                public bool IsVerifyEmail { get; set; }
                public bool? IsActive { get; set; }
                public bool? IsAdmin { get; set; }
                public string? Phone { get; set; }
                public string? RefreshToken { get; set; }
                public DateTime? RefreshTokenExpiryTime { get; set; }
                public virtual ICollection<Comment>? Comments { get; set; }
                public virtual ICollection<Bookmarked>? Bookmarkeds { get; set; }
                public virtual ICollection<Preferences>? Preferences { get; set; }
                public virtual ICollection<RolesOfUser> Roles { get; set; } = null!;
                public virtual ICollection<Rating> Ratings { get; set; }
                public virtual ICollection<Order> Orders { get; set; }
                public virtual ICollection<ChapterOfAccount> ChapterOfAccounts { get; set; }
                public virtual ICollection<Payout> Payouts { get; set; }
                public virtual ICollection<Report> Reports { get; set; }
        }
}