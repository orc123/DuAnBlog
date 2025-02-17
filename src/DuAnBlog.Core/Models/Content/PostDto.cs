﻿using AutoMapper;
using DuAnBlog.Core.Domain.Content;

namespace DuAnBlog.Core.Models.Content;
public class PostDto : PostInListDto
{
    public Guid CategoryId { get; set; }

    public string? Content { get; set; }

    public Guid AuthorUserId { get; set; }
    public string? Source { get; set; }
    public string? Tags { get; set; }

    public string? SeoDescription { get; set; }
    public DateTime? DateModified { get; set; }
    public bool IsPaid { get; set; }
    public double RoyaltyAmount { get; set; }
    public PostStatus Status { get; set; }
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Post, PostDto>();
        }
    }
}
