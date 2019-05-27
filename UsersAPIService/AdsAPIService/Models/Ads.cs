using System;
using System.Collections.Generic;

namespace AdsAPIService.Models
{
    public partial class Ads
    {
        public Guid Adid { get; set; }
        public Guid Userid { get; set; }
        public string Caption { get; set; }
        public string City { get; set; }
        public string Adress { get; set; }
        public string Type { get; set; }
        public string WhatRented { get; set; }
        public int? Bedrooms { get; set; }
        public int? Beds { get; set; }
        public int? Bathrooms { get; set; }
        public string Description { get; set; }
        public int? Price { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
