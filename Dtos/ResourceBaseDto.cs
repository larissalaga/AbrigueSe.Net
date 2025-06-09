using System.Collections.Generic;

namespace AbrigueSe.Dtos
{
    /// <summary>
    /// Classe base para DTOs que suportam links HATEOAS.
    /// </summary>
    public abstract class ResourceBaseDto
    {
        /// <summary>
        /// Lista de links HATEOAS relacionados ao recurso.
        /// </summary>
        public List<LinkDto> Links { get; set; } = new List<LinkDto>();
    }
}