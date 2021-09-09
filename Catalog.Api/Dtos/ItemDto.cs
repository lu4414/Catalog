using System;

namespace Catalog.Api.Dtos
{   
    public record ItemDto ()
    {
        public Guid Id {get; init;}//init property inicializers (value can only be set during inicialization)
        public string Name {get; init;}
        public decimal Price {get; init;}
        public DateTimeOffset CreatedDate {get; init;}
    }
}