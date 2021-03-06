namespace Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Template
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string CssClass { get; set; }

        [Column(TypeName = "text")]
        [Required]
        public string Css { get; set; }

        [Required]
        public int BoxCount { get; set; }

        [Required]
        public int BoxesPerRow { get; set; }

        [Required]
        public int BorderRadiusPercent { get; set; }


        [Required]
        public string BoxWidth { get; set; }


        [Required]
        public string BoxHeight { get; set; }

        public string MarginTop { get; set; }

        public string MarginBottom { get; set; }
        public string MarginLeft { get; set; }

        public string MarginRIght { get; set; }

        public int Order { get; set; }
    }
}
