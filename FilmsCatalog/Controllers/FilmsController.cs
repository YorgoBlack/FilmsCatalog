using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FilmsCatalog.Data;
using FilmsCatalog.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using FilmsCatalog.Helpers;

namespace FilmsCatalog.Controllers
{
    public class FilmsController : Controller
    {
        private readonly IFilmsCatalogService _filmsCatalogService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilmsController(IFilmsCatalogService filmsCatalogService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _filmsCatalogService = filmsCatalogService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Films
        public async Task<IActionResult> Index(int? pageNumber)
        {
            var films_models = _filmsCatalogService.Films.Select(film => _mapper.Map<FilmViewModel>(film));
            int pageSize = 3;
            return View(await PaginatedList<FilmViewModel>.CreateAsync(films_models.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Films/Poster/5
        public async Task<IActionResult> Poster(int id)
        {
            var film = _filmsCatalogService.GetById(id);
            if (film != null)
            {
                var bytes = await _filmsCatalogService.GetPoster(film);
                if( bytes != null 
                    && Helpers.MimeContentTypesProvider.TryGetMime(film.PosterImageExt, out string mimetype) )
                {
                    return File(bytes, mimetype);
                }
            }
            var path = System.IO.Path.Combine(_webHostEnvironment.WebRootPath, "images\\noimage.png");
            return File(System.IO.File.ReadAllBytes(path), "image/png");
        }

        // GET: Films/Details/5
        public IActionResult Details(int? id, int? PageNumber)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = _filmsCatalogService.GetById(id.Value);
            if ( film == null )
            {
                return NotFound();
            }
            var model = _mapper.Map<FilmViewModel>(film);
            model.isDisabled = true;
            model.PageNumber = PageNumber;
            return View(model);
        }

        // GET: Films/Create
        [Authorize]
        public IActionResult Create(int? PageNumber)
        {
            return View( new FilmViewModel() { PageNumber = PageNumber });
        }

        // POST: Films/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FilmViewModel filmViewModel)
        {
            if (ModelState.IsValid)
            {
                var film = _mapper.Map<Film>(filmViewModel);
                await _filmsCatalogService.Add(film, filmViewModel.PosterFile);
                return RedirectToAction(nameof(Index), new { PageNumber = filmViewModel.PageNumber });
            }
            return View(filmViewModel);
        }

        // GET: Films/Edit/5
        [Authorize]
        public IActionResult Edit(int? id, int? PageNumber)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = _filmsCatalogService.GetById(id.Value);
            if (film == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<FilmViewModel>(film);
            model.PageNumber = PageNumber;
            return View(model);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FilmViewModel filmViewModel)
        {
            if (id != filmViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var film = _mapper.Map<Film>(filmViewModel);
                await _filmsCatalogService.Update(film, filmViewModel.PosterFile);
                return RedirectToAction(nameof(Index), new { PageNumber = filmViewModel.PageNumber });
            }
            return View(filmViewModel);
        }

        // GET: Films/Delete/5
        [Authorize]
        public IActionResult Delete(int? id, int? PageNumber)
        {
            if (id == null)
            {
                return NotFound();
            }
            var film = _filmsCatalogService.GetById(id.Value);
            if (film == null)
            {
                return NotFound();
            }
            var model = _mapper.Map<FilmViewModel>(film);
            model.isDisabled = true;
            model.PageNumber = PageNumber;
            return View(model);
        }

        // POST: Films/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int? PageNumber)
        {
            await _filmsCatalogService.Delete(id);
            return RedirectToAction(nameof(Index), new { PageNumber = PageNumber });
        }

    }
}
