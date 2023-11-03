using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebNovel.API.Core.Extentions.ServiceExtention;

namespace WebNovel.API.Core.Schemas
{
    public class ParamsSearch
    {
        public ParamsSearch()
        {
            Sort = "ASC";
            SortBy = "Id";
            PageSize = 100;
            CurrentPage = 1;
        }

        public string? Key {get; set;}

        public int CurrentPage { set; get; }

        public int PageSize { set; get; }

        public string Sort { set; get; }

        public string SortBy { set; get; }

        [SwaggerExclude]
        public string Order
        {
            get
            {
                return String.IsNullOrEmpty(Sort) ? "ASC" : Sort;
            }
        }

        [SwaggerExclude]
        public virtual string OrderBy
        {
            get
            {
                return String.IsNullOrEmpty(SortBy) ? "Id" : SortBy;
            }
        }
    }
}