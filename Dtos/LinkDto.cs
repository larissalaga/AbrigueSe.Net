namespace AbrigueSe.Dtos
{
    /// <summary>
    /// Representa um link HATEOAS (Hypermedia as the Engine of Application State).
    /// </summary>
    public class LinkDto
    {
        /// <summary>
        /// URL (Uniform Resource Locator) do link.
        /// </summary>
        /// <example>https://api.example.com/usuarios/15</example>
        public string Href { get; private set; }
        /// <summary>
        /// Relação (rel) do link com o recurso atual. Descreve o significado do link.
        /// </summary>
        /// <example>self</example>
        public string Rel { get; private set; }
        /// <summary>
        /// Método HTTP recomendado para interagir com o Href.
        /// </summary>
        /// <example>GET</example>
        public string Method { get; private set; }

        /// <summary>
        /// Cria uma nova instância de LinkDto.
        /// </summary>
        /// <param name="href">A URL do link.</param>
        /// <param name="rel">A relação do link.</param>
        /// <param name="method">O método HTTP.</param>
        public LinkDto(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}