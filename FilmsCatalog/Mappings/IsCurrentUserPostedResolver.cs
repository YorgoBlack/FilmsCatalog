using AutoMapper;
using FilmsCatalog.Data;
using FilmsCatalog.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Mappings
{
    public class IsCurrentUserPostedResolver : IValueResolver<Film, FilmViewModel, bool>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public IsCurrentUserPostedResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool Resolve(Film source, FilmViewModel destination, bool destMember, ResolutionContext context)
        {
            var userId_claim = _httpContextAccessor?.HttpContext.User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return userId_claim?.Value == source.UserId;
        }
    }
}
