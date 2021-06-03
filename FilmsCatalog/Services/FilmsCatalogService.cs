using FilmsCatalog.Data;
using LiteX.Storage.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Services
{
    public class FilmsCatalogService : IFilmsCatalogService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILiteXBlobService _blobsStorage;

        public IQueryable<Film> Films => _context.Films;

        public FilmsCatalogService(ApplicationDbContext context, ILiteXBlobService blobsStorage)
        {
            _context = context;
            _blobsStorage = blobsStorage;

        }
        public async Task Add(Film film, IFormFile poster)
        {
            if (poster != null && film != null)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();

                var blobname = $"{film.Id}{film.PosterImageExt}";
                var ok = await _blobsStorage.UploadBlobAsync(blobname, poster.OpenReadStream());
                if( !ok )
                {
                    _context.Remove(film);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task Delete(int Id)
        {
            var film = _context.Films.FirstOrDefault(x => x.Id == Id);
            if( film != null )
            {
                var blobname = $"{film.Id}{film.PosterImageExt}";
                _context.Remove(film);
                await _context.SaveChangesAsync();
                try
                {
                    await _blobsStorage.DeleteBlobAsync(blobname);
                }
                catch { }
            }
        }

        public IQueryable<Film> GetAll()
        {
            return _context.Films;
        }

        public Film GetById(int Id)
        {
            return _context.Films.FirstOrDefault(x => x.Id == Id);
        }

        public async Task<byte[]> GetPoster(Film film)
        {
            try
            {
                var blobname = $"{film.Id}{film.PosterImageExt}";
                using (var stream = await _blobsStorage.GetBlobAsync(blobname))
                {
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    {
                        stream.CopyTo(ms);
                        ms.Position = 0;
                        return ms.ToArray();
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        public async Task Update(Film film, IFormFile poster)
        {
            if( poster == null )
            {
                var _film = await Films.AsNoTracking().FirstOrDefaultAsync(x => x.Id == film.Id);
                film.PosterImageExt = _film?.PosterImageExt; 
            }
            _context.Update(film);
            await _context.SaveChangesAsync();
            if (poster != null)
            {
                var blobname = $"{film.Id}{film.PosterImageExt}";
                try
                {
                    await _blobsStorage.DeleteBlobAsync(blobname);
                }
                catch { }
                try
                {
                    await _blobsStorage.UploadBlobAsync(blobname, poster.OpenReadStream());
                }
                catch { }
            }
        }
    }
}
