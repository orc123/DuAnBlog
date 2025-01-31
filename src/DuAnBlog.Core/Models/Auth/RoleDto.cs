using AutoMapper;
using DuAnBlog.Core.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuAnBlog.Core.Models.Auth;
public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppRole, RoleDto>();
        }
    }
}
