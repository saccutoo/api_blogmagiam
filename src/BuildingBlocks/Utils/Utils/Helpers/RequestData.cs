using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Utils
{
   
    public class PaginationRequest
    {
        public string Sort { get; set; } = "+Id";
        public string Fields { get; set; }  
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; }
        [Range(1, int.MaxValue)] public int? PageSize { get; set; }
        public string Filter { get; set; }
        public string FullTextSearch { get; set; }
        public decimal? Id { get; set; }
        public List<decimal> ListId   { get; set; }
    }

}