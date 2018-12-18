﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PB.WebAPI.Models
{
    public class Article : IDbModel
    {
        public long Id { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }
    }
}
