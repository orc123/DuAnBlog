using AutoMapper;
using DuAnBlog.Core.Domain.Content;
using System.ComponentModel.DataAnnotations;

namespace DuAnBlog.Core.Models.Content;
public class SeriesDto : SeriesInListDto
{
    [MaxLength(250)]
    public string? SeoDescription { get; set; }

    [MaxLength(250)]
    public string? Thumbnail { set; get; }

    public string? Content { get; set; }

    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Series, SeriesDto>();
        }
    }
}
