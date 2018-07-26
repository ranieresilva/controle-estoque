using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ControleEstoque.Web.Models
{
    public class ProdutoMap : EntityTypeConfiguration<ProdutoModel>
    {
        public ProdutoMap()
        {
            ToTable("produto");

            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Codigo).HasColumnName("codigo").HasMaxLength(10).IsRequired();
            Property(x => x.Nome).HasColumnName("nome").HasMaxLength(50).IsRequired();
            Property(x => x.PrecoCusto).HasColumnName("preco_custo").HasPrecision(7, 2).IsRequired();
            Property(x => x.PrecoVenda).HasColumnName("preco_venda").HasPrecision(7, 2).IsRequired();
            Property(x => x.QuantEstoque).HasColumnName("quant_estoque").IsRequired();
            Property(x => x.Imagem).HasColumnName("imagem").HasMaxLength(100).IsRequired();
            Property(x => x.Ativo).HasColumnName("ativo").IsRequired();

            Property(x => x.IdUnidadeMedida).HasColumnName("id_unidade_medida").IsRequired();
            HasRequired(x => x.UnidadeMedida).WithMany().HasForeignKey(x => x.IdUnidadeMedida).WillCascadeOnDelete(false);

            Property(x => x.IdGrupo).HasColumnName("id_grupo").IsRequired();
            HasRequired(x => x.Grupo).WithMany().HasForeignKey(x => x.IdGrupo).WillCascadeOnDelete(false);

            Property(x => x.IdMarca).HasColumnName("id_marca").IsRequired();
            HasRequired(x => x.Marca).WithMany().HasForeignKey(x => x.IdMarca).WillCascadeOnDelete(false);

            Property(x => x.IdFornecedor).HasColumnName("id_fornecedor").IsRequired();
            HasRequired(x => x.Fornecedor).WithMany().HasForeignKey(x => x.IdFornecedor).WillCascadeOnDelete(false);

            Property(x => x.IdLocalArmazenamento).HasColumnName("id_local_armazenamento").IsRequired();
            HasRequired(x => x.LocalArmazenamento).WithMany().HasForeignKey(x => x.IdLocalArmazenamento).WillCascadeOnDelete(false);
        }
    }
}