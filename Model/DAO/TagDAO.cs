﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Conduit.Model.DAO
{
    public class TagDAO
    {
        [Key]
        public int Id { get; set; }

        public string Tag { get; set; }
    }
}
