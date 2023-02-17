using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Models
{
    public class Slider
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage ="El nombre es requerido")]
        [Display(Name ="El nombre es requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage ="El estado es requerido")]
        [Display(Name ="Estado por defecto: ")]
        public bool Estado { get; set; }

        [DataType(DataType.ImageUrl)]
        [Display(Name ="Imagen del slider: ")]
        public string UrlImagen { get; set; }

    }
}
