﻿using DesignAndPrintStickers.Infrastructure.Mapping;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignAndPrintStickers.Models
{
    public class IndexViewModels : IMapFrom<Template>
    {
        public string Name { get; set; }
        
        public string CssClass { get; set; }
        
        public string Css { get; set; }
    }
}