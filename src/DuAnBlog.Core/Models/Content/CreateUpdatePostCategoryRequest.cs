using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DuAnBlog.Core.Domain.Content;

namespace DuAnBlog.Core.Models.Content;
public class CreateUpdatePostCategoryRequest
{
    [MaxLength(250)]
    public required string Name { get; set; }

    [Column(TypeName = "varchar(250)")]
    public required string Slug { get; set; }

    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }

    [MaxLength(160)]
    public string? SeoDescription { get; set; }
    public int SortOrder { get; set; }
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CreateUpdatePostCategoryRequest, PostCategory>();
        }
    }
}
