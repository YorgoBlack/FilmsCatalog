using AutoMapper;
using FilmsCatalog.Data;
using FilmsCatalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmsCatalog.Mappings
{
    public class FilmMap : Profile
    {
        public FilmMap()
        {
            CreateMap<Film, FilmViewModel>()
                .ForMember(dest => dest.IsCurrentUserPosted, opt=>opt.MapFrom<IsCurrentUserPostedResolver>()); 


            CreateMap<FilmViewModel, Film>()
                .ForMember(dest=> dest.PosterImageExt, opt=>opt.MapFrom(src=> System.IO.Path.GetExtension(src.PosterFile.FileName)))
                .ForMember(dest=> dest.UserId, opt=>opt.MapFrom<UserIdResolver>());
        }
    }
}
