using System;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.Dtos
{
    public record ItemDto(Guid Id, string Name, decimal Price, DateTimeOffset CreatedDate);
    public record CreateItemDto([Required]string Name, [Range(1,1000)]decimal Price);
    public record UpdateItemDto(Guid Id, string Name, decimal Price, DateTimeOffset CreatedDate);


}