﻿using System.Collections.Generic;

namespace Clockify.Net.Models.Reports
{
    public class TotalsDto
    {
        public string Id { get; set; }
        public int TotalTime { get; set; }
        public int TotalBillableTime { get; set; }
        public int EntriesCount { get; set; }
        public decimal TotalAmount { get; set; } 
    }
}