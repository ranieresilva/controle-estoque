using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControleEstoque.Web.Models
{
    public class FornecedorModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        [MaxLength(60, ErrorMessage = "O nome pode ter no máximo 60 caracteres.")]
        public string Nome { get; set; }

        [MaxLength(100, ErrorMessage = "A razão social pode ter no máximo 100 caracteres.")]
        public string RazaoSocial { get; set; }

        [MaxLength(20, ErrorMessage = "O número do documento pode ter no máximo 20 caracteres.")]
        public string NumDocumento { get; set; }

        [Required]
        public TipoPessoa Tipo { get; set; }

        [Required(ErrorMessage = "Preencha o telefone.")]
        [MaxLength(20, ErrorMessage = "O telefone deve ter 20 caracteres.")]
        public string Telefone { get; set; }

        [Required(ErrorMessage = "Preencha o contato.")]
        [MaxLength(60, ErrorMessage = "O contato deve ter 60 caracteres.")]
        public string Contato { get; set; }

        [MaxLength(100, ErrorMessage = "O logradouro do endereço pode ter no máximo 100 caracteres.")]
        public string Logradouro { get; set; }

        [MaxLength(20, ErrorMessage = "O número do endereço pode ter no máximo 20 caracteres.")]
        public string Numero { get; set; }

        [MaxLength(100, ErrorMessage = "O complemento do endereço pode ter no máximo 100 caracteres.")]
        public string Complemento { get; set; }

        [MaxLength(10, ErrorMessage = "O CEP do endereço pode ter no máximo 10 caracteres.")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "Selecione o país.")]
        public int IdPais { get; set; }

        [Required(ErrorMessage = "Selecione o estado.")]
        public int IdEstado { get; set; }

        [Required(ErrorMessage = "Selecione a cidade.")]
        public int IdCidade { get; set; }

        public bool Ativo { get; set; }

        public static int RecuperarQuantidade()
        {
            var ret = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select count(*) from fornecedor";
                    ret = (int)comando.ExecuteScalar();
                }
            }

            return ret;
        }

        public static List<FornecedorModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "")
        {
            var ret = new List<FornecedorModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    var filtroWhere = "";
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        filtroWhere = string.Format(" where lower(nome) like '%{0}%'", filtro.ToLower());
                    }

                    var paginacao = "";
                    if (pagina > 0 && tamPagina > 0)
                    {
                        paginacao = string.Format(" offset {0} rows fetch next {1} rows only",
                            pos > 0 ? pos - 1 : 0, tamPagina);
                    }

                    comando.Connection = conexao;
                    comando.CommandText =
                        "select *" +
                        " from fornecedor" +
                        filtroWhere +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                        paginacao;

                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new FornecedorModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            RazaoSocial = (string)reader["razao_social"],
                            NumDocumento = (string)reader["num_documento"],
                            Tipo = (TipoPessoa)((int)reader["tipo"]),
                            Telefone = (string)reader["telefone"],
                            Contato = (string)reader["contato"],
                            Logradouro = (string)reader["logradouro"],
                            Numero = (string)reader["numero"],
                            Complemento = (string)reader["complemento"],
                            Cep = (string)reader["cep"],
                            IdPais = (int)reader["id_pais"],
                            IdEstado = (int)reader["id_estado"],
                            IdCidade = (int)reader["id_cidade"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }

            return ret;
        }

        public static FornecedorModel RecuperarPeloId(int id)
        {
            FornecedorModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from fornecedor where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new FornecedorModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            RazaoSocial = (string)reader["razao_social"],
                            NumDocumento = (string)reader["num_documento"],
                            Tipo = (TipoPessoa)((int)reader["tipo"]),
                            Telefone = (string)reader["telefone"],
                            Contato = (string)reader["contato"],
                            Logradouro = (string)reader["logradouro"],
                            Numero = (string)reader["numero"],
                            Complemento = (string)reader["complemento"],
                            Cep = (string)reader["cep"],
                            IdPais = (int)reader["id_pais"],
                            IdEstado = (int)reader["id_estado"],
                            IdCidade = (int)reader["id_cidade"],
                            Ativo = (bool)reader["ativo"]
                        };
                    }
                }
            }

            return ret;
        }

        public static bool ExcluirPeloId(int id)
        {
            var ret = false;

            if (RecuperarPeloId(id) != null)
            {
                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();
                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "delete from fornecedor where (id = @id)";

                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                        ret = (comando.ExecuteNonQuery() > 0);
                    }
                }
            }

            return ret;
        }

        public int Salvar()
        {
            var ret = 0;

            var model = RecuperarPeloId(this.Id);

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;

                    if (model == null)
                    {
                        comando.CommandText = "insert into fornecedor (nome, razao_social, num_documento, tipo, telefone, contato, logradouro," +
                            " numero, complemento, cep, id_pais, id_estado, id_cidade, ativo) values (@nome, @razao_social, @num_documento," +
                            " @tipo, @telefone, @contato, @logradouro, @numero, @complemento, @cep, @id_pais, @id_estado, @id_cidade, @ativo);" +
                            " select convert(int, scope_identity())";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@razao_social", SqlDbType.VarChar).Value = this.RazaoSocial ?? "";
                        comando.Parameters.Add("@num_documento", SqlDbType.VarChar).Value = this.NumDocumento ?? "";
                        comando.Parameters.Add("@tipo", SqlDbType.Int).Value = this.Tipo;
                        comando.Parameters.Add("@telefone", SqlDbType.VarChar).Value = this.Telefone ?? "";
                        comando.Parameters.Add("@contato", SqlDbType.VarChar).Value = this.Contato ?? "";
                        comando.Parameters.Add("@logradouro", SqlDbType.VarChar).Value = this.Logradouro ?? "";
                        comando.Parameters.Add("@numero", SqlDbType.VarChar).Value = this.Numero ?? "";
                        comando.Parameters.Add("@complemento", SqlDbType.VarChar).Value = this.Complemento ?? "";
                        comando.Parameters.Add("@cep", SqlDbType.VarChar).Value = this.Cep ?? "";
                        comando.Parameters.Add("@id_pais", SqlDbType.Int).Value = this.IdPais;
                        comando.Parameters.Add("@id_estado", SqlDbType.Int).Value = this.IdEstado;
                        comando.Parameters.Add("@id_cidade", SqlDbType.Int).Value = this.IdCidade;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText = "update fornecedor set nome=@nome, razao_social=@razao_social, num_documento=@num_documento," +
                            " tipo=@tipo, telefone=@telefone, contato=@contato, logradouro=@logradouro, numero=@numero, complemento=@complemento," +
                            " cep=@cep, id_pais=@id_pais, id_estado=@id_estado, id_cidade=@id_cidade, ativo=@ativo where id = @id";

                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@razao_social", SqlDbType.VarChar).Value = this.RazaoSocial ?? "";
                        comando.Parameters.Add("@num_documento", SqlDbType.VarChar).Value = this.NumDocumento ?? "";
                        comando.Parameters.Add("@tipo", SqlDbType.Int).Value = this.Tipo;
                        comando.Parameters.Add("@telefone", SqlDbType.VarChar).Value = this.Telefone ?? "";
                        comando.Parameters.Add("@contato", SqlDbType.VarChar).Value = this.Contato ?? "";
                        comando.Parameters.Add("@logradouro", SqlDbType.VarChar).Value = this.Logradouro ?? "";
                        comando.Parameters.Add("@numero", SqlDbType.VarChar).Value = this.Numero ?? "";
                        comando.Parameters.Add("@complemento", SqlDbType.VarChar).Value = this.Complemento ?? "";
                        comando.Parameters.Add("@cep", SqlDbType.VarChar).Value = this.Cep ?? "";
                        comando.Parameters.Add("@id_pais", SqlDbType.Int).Value = this.IdPais;
                        comando.Parameters.Add("@id_estado", SqlDbType.Int).Value = this.IdEstado;
                        comando.Parameters.Add("@id_cidade", SqlDbType.Int).Value = this.IdCidade;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;

                        if (comando.ExecuteNonQuery() > 0)
                        {
                            ret = this.Id;
                        }
                    }
                }
            }

            return ret;
        }
    }
}