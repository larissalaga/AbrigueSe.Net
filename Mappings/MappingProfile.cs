using AutoMapper;
using AbrigueSe.Dtos;
using AbrigueSe.Models;
using System.Linq; // Required for Linq operations like OrderByDescending, FirstOrDefault

namespace AbrigueSe.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Pais
            CreateMap<PaisCreateDto, Pais>();
            CreateMap<PaisUpdateDto, Pais>();
            CreateMap<Pais, PaisGetDto>();

            // Estado
            CreateMap<EstadoDto, Estado>(); // For Create/Update
            CreateMap<Estado, EstadoGetDto>()
                .ForMember(dest => dest.Pais, opt => opt.MapFrom(src => src.Pais)); // Map the Pais object

            // Cidade
            CreateMap<CidadeDto, Cidade>();
            CreateMap<Cidade, CidadeGetDto>()
                .ForMember(dest => dest.Estado, opt => opt.MapFrom(src => src.Estado)); // Map the Estado object

            // Endereco
            CreateMap<EnderecoDto, Endereco>();
            CreateMap<Endereco, EnderecoGetDto>()
                .ForMember(dest => dest.Cidade, opt => opt.MapFrom(src => src.Cidade)); // Assuming EnderecoGetDto has a CidadeGetDto property

            // Pessoa
            CreateMap<PessoaDto, Pessoa>();
            CreateMap<Pessoa, PessoaGetDto>()
                .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
                // As propriedades AbrigoAtual, Usuario e UltimoCheckIn serão preenchidas manualmente no repositório.
                // Se desejar que o AutoMapper tente mapeá-las (caso as entidades estejam carregadas na Pessoa de origem),
                // você pode adicionar os mapeamentos aqui, mas o método GetDetailsByIdAsync já cuida disso.
                .ForMember(dest => dest.AbrigoAtual, opt => opt.Ignore()) // Ignora para preenchimento manual
                .ForMember(dest => dest.Usuario, opt => opt.Ignore())     // Ignora para preenchimento manual
                .ForMember(dest => dest.UltimoCheckIn, opt => opt.Ignore()) // Ignora para preenchimento manual
                .ForMember(dest => dest.DsCondicaoMedica, opt => opt.MapFrom(src => src.DsCondicaoMedica)); // Adicionado mapeamento
                

            // TipoUsuario
            CreateMap<TipoUsuarioDto, TipoUsuario>();
            CreateMap<TipoUsuario, TipoUsuarioGetDto>();

            // Usuario
            CreateMap<UsuarioCreateDto, Usuario>();
            CreateMap<UsuarioUpdateDto, Usuario>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Usuario, UsuarioGetDto>()
                .ForMember(dest => dest.TipoUsuario, opt => opt.MapFrom(src => src.TipoUsuario)) // Map the TipoUsuario object
                .ForMember(dest => dest.Pessoa, opt => opt.MapFrom(src => src.Pessoa)); // Map the Pessoa object

            CreateMap<Usuario, LoginDto>().ReverseMap();

            // Abrigo
            CreateMap<AbrigoDto, Abrigo>();
            CreateMap<AbrigoCreateDto, Abrigo>();
            CreateMap<Abrigo, AbrigoGetDto>()
                 .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco));
            
                 

            // CheckIn
            CreateMap<CheckInDto, CheckIn>();
            CreateMap<CheckIn, CheckInGetDto>()
                .ForMember(dest => dest.Abrigo, opt => opt.MapFrom(src => src.Abrigo)) // Map the Abrigo object
                .ForMember(dest => dest.Pessoa, opt => opt.MapFrom(src => src.Pessoa)); // Map the Pessoa object

            // Recurso
            CreateMap<RecursoDto, Recurso>();
            CreateMap<Recurso, RecursoGetDto>();

            // EstoqueRecurso
            CreateMap<EstoqueRecursoDto, EstoqueRecurso>();
            CreateMap<EstoqueRecurso, EstoqueRecursoGetDto>()
                .ForMember(dest => dest.Abrigo, opt => opt.MapFrom(src => src.Abrigo)) // Map the Abrigo object
                .ForMember(dest => dest.Recurso, opt => opt.MapFrom(src => src.Recurso)); // Map the Recurso object
        }
    }
}
