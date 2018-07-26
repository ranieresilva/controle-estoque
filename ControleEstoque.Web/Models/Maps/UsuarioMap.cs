using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace ControleEstoque.Web.Models
{
    public class UsuarioMap : EntityTypeConfiguration<UsuarioModel>
    {
        public UsuarioMap()
        {
            ToTable("usuario");

            HasKey(x => x.Id);
            Property(x => x.Id).HasColumnName("id").HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.Login).HasColumnName("login").HasMaxLength(50).IsRequired();
            Property(x => x.Senha).HasColumnName("senha").HasMaxLength(32).IsRequired();
            Property(x => x.Nome).HasColumnName("nome").HasMaxLength(100).IsRequired();
            Property(x => x.Email).HasColumnName("email").HasMaxLength(150).IsRequired();
        }
    }
}