using AutoMapper;
using ControleEstoque.Web.Models;

namespace ControleEstoque.Web
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CidadeViewModel, CidadeModel>();
            CreateMap<EstadoViewModel, EstadoModel>();
            CreateMap<FornecedorViewModel, FornecedorModel>();
            CreateMap<GrupoProdutoViewModel, GrupoProdutoModel>();
            CreateMap<LocalArmazenamentoViewModel, LocalArmazenamentoModel>();
            CreateMap<MarcaProdutoViewModel, MarcaProdutoModel>();
            CreateMap<PaisViewModel, PaisModel>();
            CreateMap<PerfilViewModel, PerfilModel>();
            CreateMap<MarcaProdutoViewModel, MarcaProdutoModel>();
            CreateMap<ProdutoViewModel, ProdutoModel>();
            CreateMap<UnidadeMedidaViewModel, UnidadeMedidaModel>();
            CreateMap<UsuarioViewModel, UsuarioModel>();
        }
    }
}