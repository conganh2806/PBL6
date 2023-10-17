using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebNovel.API.Databases.Entities;

namespace WebNovel.API.Databases.Entitites
{
    public class Account : TableHaveIdInt
    {
        public Account()
        { 
            Novels = new HashSet<Novel>();
            Comments = new HashSet<Comment>();
            Bookmarkeds = new HashSet<Bookmarked>();
            Preferences = new HashSet<Preferences>();
            Roles = new HashSet<Role>();
        } 

        [StringLength(100)]
        public string Username {get; set;} = null!;
        [StringLength(255)]
        public string Password {get; set;} = null!;
        [StringLength(255)]
        public string Email {get; set;} = null!;
        [StringLength(100)]
        public string NickName {get; set;} = null!;
        [DataType(DataType.Date)]
        public DateOnly DateJoined {get; set;}
        public bool Status {get; set;}
        public float WalletAmmount {get; set;} = 0.0f;
        public virtual ICollection<Novel> Novels {get; set;} = null!;
        public virtual ICollection<Comment>? Comments {get; set;}
        public virtual ICollection<Bookmarked>? Bookmarkeds {get; set;}
        public virtual ICollection<Preferences>? Preferences {get; set;}       
        public virtual ICollection<Roles> Roles {get; set;} = null!; 

    }
}