using AutoMapper;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Models;

namespace WebApi.Hal.Web
{
    public class AutomapperMappings
    {
        public static void RegisterMaps(ConfigurationStore configurationProvider)
        {
            configurationProvider
                .CreateMap<Beer, BeerResource>()
                .ForMember(b => b.BreweryId, m => m.MapFrom(b => b.Brewery.Id))
                .ForMember(b => b.BreweryName, m => m.MapFrom(b => b.Brewery.Name))
                .ForMember(b => b.StyleId, m => m.MapFrom(b => b.Style.Id))
                .ForMember(b => b.StyleName, m => m.MapFrom(b => b.Style.Name))
                .ForMember(b=>b.Id, m=>m.MapFrom(b=>b.Id))
                .ForMember(b=>b.Name, m=>m.MapFrom(b=>b.Name));
        }
    }
}