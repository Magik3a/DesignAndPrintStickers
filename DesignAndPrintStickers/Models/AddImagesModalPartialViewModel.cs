﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignAndPrintStickers.Models
{
    public class AddImagesModalPartialViewModel
    {
        public string PaperSizeName { get; set; }

        public string PaperSizeWith { get; set; }

        public string PaperSizeHeight { get; set; }

        public string TemplateName { get; set; }

        public string TemplateClass { get; set; }

        public int BoxesCount { get; set; }
        
        public int BorderRadiusPercent { get; set; }

        public int BoxesPerRow { get; set; }

        public string BoxWidth { get; set; }

        public string BoxHeight { get; set; }
    }
}