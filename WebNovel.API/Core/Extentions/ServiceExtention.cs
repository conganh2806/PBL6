using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebNovel.API.Core.Extentions
{
    public class ServiceExtention
    {
        [AttributeUsage(AttributeTargets.Property)]
        public class SwaggerExcludeAttribute : Attribute
        {
        }
    }
}