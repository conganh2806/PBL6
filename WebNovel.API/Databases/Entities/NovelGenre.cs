using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebNovel.API.Databases.Entities
{
    public class NovelGenre
    {
        public long NovelId {get; set;}
        public long GenreId {get; set;}
    }
}