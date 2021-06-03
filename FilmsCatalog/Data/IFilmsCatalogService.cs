using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Data
{
    public interface IFilmsCatalogService
    {
        IQueryable<Film> Films { get; }
        Film GetById(int id);
        Task Add(Film film, IFormFile poster);
        Task Update(Film film, IFormFile poster);
        Task Delete(int id);
        Task<byte[]> GetPoster(Film film);
    }
}
