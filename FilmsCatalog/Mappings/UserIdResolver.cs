using AutoMapper;
using FilmsCatalog.Data;
using FilmsCatalog.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Mappings
{
    public class UserIdResolver : IValueResolver<FilmViewModel, Film, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserIdResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string Resolve(FilmViewModel source, Film destination, string destMember, ResolutionContext context)
        {
            return _httpContextAccessor?.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
        }
    }
}
