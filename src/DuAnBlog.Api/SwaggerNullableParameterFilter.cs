using AutoMapper.Internal;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace DuAnBlog.Api;

public class SwaggerNullableParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        parameter.Schema.Nullable = true;
        if (!parameter.Schema.Nullable &&
            (context.ApiParameterDescription.Type.IsNullableType() || !context.ApiParameterDescription.Type.IsValueType))
        {
        }
    }
}