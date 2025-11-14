using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GestordeTaras.EN;
using GestordeTareas.UI.DTOs.CategoriaDTOs;
using AutoMapper;

namespace GestordeTareas.UI.profiles
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<CategoriaCreateDto, Categoria>();
            CreateMap<CategoriaUpdateDto, Categoria>();
            CreateMap<Categoria, CategoriaReadDTO>();
        }
    }
}