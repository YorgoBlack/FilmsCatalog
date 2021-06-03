using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Models
{
    public class FilmViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Название")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Год выпуска")]
        [Range(1910,2030)]
        public int ReleaseYear { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [Display(Name = "Режиссер")]
        public string Producer { get; set; }

        [Display(Name = "Постер")]
        public IFormFile PosterFile { get; set; }
        
        public bool IsCurrentUserPosted { get; set; }
        public bool isDisabled { get; set; }
        public int? PageNumber { get; set; }

    }
}
