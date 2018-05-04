using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControleEstoque.Web.Models
{
    public class ProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o código.")]
        [MaxLength(10, ErrorMessage = "O código pode ter no máximo 10 caracteres.")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        [MaxLength(50, ErrorMessage = "O nome pode ter no máximo 50 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Preencha o preço de custo.")]
        public decimal PrecoCusto { get; set; }

        [Required(ErrorMessage = "Preencha o preço de venda.")]
        public decimal PrecoVenda { get; set; }

        [Required(ErrorMessage = "Preencha a quantidade em estoque.")]
        public int QuantEstoque { get; set; }

        [Required(ErrorMessage = "Selecione a unidade de medida.")]
        public int IdUnidadeMedida { get; set; }

        [Required(ErrorMessage = "Selecione o grupo.")]
        public int IdGrupo { get; set; }

        [Required(ErrorMessage = "Selecione a marca.")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "Selecione o fornecedor.")]
        public int IdFornecedor { get; set; }

        [Required(ErrorMessage = "Selecione o local de armazenamento.")]
        public int IdLocalArmazenamento { get; set; }

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
                    comando.CommandText = "select count(*) from produto";
                    ret = (int)comando.ExecuteScalar();
                }
            }

            return ret;
        }

        public static List<ProdutoModel> RecuperarLista(int pagina = 0, int tamPagina = 0, string filtro = "", string ordem = "", bool somenteAtivos = false)
        {
            var ret = new List<ProdutoModel>();

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
                        filtroWhere = string.Format(" where (lower(nome) like '%{0}%')", filtro.ToLower());
                    }

                    if (somenteAtivos)
                    {
                        filtroWhere = (string.IsNullOrEmpty(filtroWhere) ? " where" : " and") + "(ativo = 1)";
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
                        " from produto" +
                        filtroWhere +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                        paginacao;

                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new ProdutoModel
                        {
                            Id = (int)reader["id"],
                            Codigo = (string)reader["codigo"],
                            Nome = (string)reader["nome"],
                            PrecoCusto = (decimal)reader["preco_custo"],
                            PrecoVenda = (decimal)reader["preco_venda"],
                            QuantEstoque = (int)reader["quant_estoque"],
                            IdUnidadeMedida = (int)reader["id_unidade_medida"],
                            IdGrupo = (int)reader["id_grupo"],
                            IdMarca = (int)reader["id_marca"],
                            IdFornecedor = (int)reader["id_fornecedor"],
                            IdLocalArmazenamento = (int)reader["id_local_armazenamento"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }

            return ret;
        }

        public static ProdutoModel RecuperarPeloId(int id)
        {
            ProdutoModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from produto where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new ProdutoModel
                        {
                            Id = (int)reader["id"],
                            Codigo = (string)reader["codigo"],
                            Nome = (string)reader["nome"],
                            PrecoCusto = (decimal)reader["preco_custo"],
                            PrecoVenda = (decimal)reader["preco_venda"],
                            QuantEstoque = (int)reader["quant_estoque"],
                            IdUnidadeMedida = (int)reader["id_unidade_medida"],
                            IdGrupo = (int)reader["id_grupo"],
                            IdMarca = (int)reader["id_marca"],
                            IdFornecedor = (int)reader["id_fornecedor"],
                            IdLocalArmazenamento = (int)reader["id_local_armazenamento"],
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
                        comando.CommandText = "delete from produto where (id = @id)";

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
                        comando.CommandText =
                            "insert into produto " +
                            "(codigo, nome, preco_custo, preco_venda, quant_estoque, id_unidade_medida, id_grupo, id_marca, " +
                            "id_fornecedor, id_local_armazenamento, ativo) values " +
                            "(@codigo, @nome, @preco_custo, @preco_venda, @quant_estoque, @id_unidade_medida, @id_grupo, @id_marca, " +
                            "@id_fornecedor, @id_local_armazenamento, @ativo); select convert(int, scope_identity())";

                        comando.Parameters.Add("@codigo", SqlDbType.VarChar).Value = this.Codigo;
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@preco_custo", SqlDbType.Decimal).Value = this.PrecoCusto;
                        comando.Parameters.Add("@preco_venda", SqlDbType.Decimal).Value = this.PrecoVenda;
                        comando.Parameters.Add("@quant_estoque", SqlDbType.Int).Value = this.QuantEstoque;
                        comando.Parameters.Add("@id_unidade_medida", SqlDbType.Int).Value = this.IdUnidadeMedida;
                        comando.Parameters.Add("@id_grupo", SqlDbType.Int).Value = this.IdGrupo;
                        comando.Parameters.Add("@id_marca", SqlDbType.Int).Value = this.IdMarca;
                        comando.Parameters.Add("@id_fornecedor", SqlDbType.Int).Value = this.IdFornecedor;
                        comando.Parameters.Add("@id_local_armazenamento", SqlDbType.Int).Value = this.IdLocalArmazenamento;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                        ret = (int)comando.ExecuteScalar();
                    }
                    else
                    {
                        comando.CommandText =
                            "update produto set codigo=@codigo, nome=@nome, preco_custo=@preco_custo, " +
                            "preco_venda=@preco_venda, quant_estoque=@quant_estoque, id_unidade_medida=@id_unidade_medida, " +
                            "id_grupo=@id_grupo, id_marca=@id_marca, id_fornecedor=@id_fornecedor, " +
                            "id_local_armazenamento=@id_local_armazenamento, ativo=@ativo where id = @id";

                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
                        comando.Parameters.Add("@codigo", SqlDbType.VarChar).Value = this.Codigo;
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@preco_custo", SqlDbType.Decimal).Value = this.PrecoCusto;
                        comando.Parameters.Add("@preco_venda", SqlDbType.Decimal).Value = this.PrecoVenda;
                        comando.Parameters.Add("@quant_estoque", SqlDbType.Int).Value = this.QuantEstoque;
                        comando.Parameters.Add("@id_unidade_medida", SqlDbType.Int).Value = this.IdUnidadeMedida;
                        comando.Parameters.Add("@id_grupo", SqlDbType.Int).Value = this.IdGrupo;
                        comando.Parameters.Add("@id_marca", SqlDbType.Int).Value = this.IdMarca;
                        comando.Parameters.Add("@id_fornecedor", SqlDbType.Int).Value = this.IdFornecedor;
                        comando.Parameters.Add("@id_local_armazenamento", SqlDbType.Int).Value = this.IdLocalArmazenamento;
                        comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                        if (comando.ExecuteNonQuery() > 0)
                        {
                            ret = this.Id;
                        }
                    }
                }
            }

            return ret;
        }
        public static string SalvarPedidoEntrada(DateTime data, Dictionary<int, int> produtos)
        {
            var ret = "";

            try
            {
                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    var numPedido = "";
                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "select next value for sec_entrada_produto";
                        numPedido = ((int)comando.ExecuteScalar()).ToString("D10");
                    }

                    using (var transacao = conexao.BeginTransaction())
                    {
                        foreach (var produto in produtos)
                        {
                            using (var comando = new SqlCommand())
                            {
                                comando.Connection = conexao;
                                comando.Transaction = transacao;
                                comando.CommandText = "insert into entrada_produto (numero, data, id_produto, quant) values (@numero, @data, @id_produto, @quant)";

                                comando.Parameters.Add("@numero", SqlDbType.VarChar).Value = numPedido;
                                comando.Parameters.Add("@data", SqlDbType.Date).Value = data;
                                comando.Parameters.Add("@id_produto", SqlDbType.Int).Value = produto.Key;
                                comando.Parameters.Add("@quant", SqlDbType.Int).Value = produto.Value;

                                comando.ExecuteNonQuery();
                            }

                            using (var comando = new SqlCommand())
                            {
                                comando.Connection = conexao;
                                comando.Transaction = transacao;
                                comando.CommandText = "update produto set quant_estoque = quant_estoque + @quant_estoque where (id = @id)";

                                comando.Parameters.Add("@id", SqlDbType.Int).Value = produto.Key;
                                comando.Parameters.Add("@quant_estoque", SqlDbType.Int).Value = produto.Value;

                                comando.ExecuteNonQuery();
                            }
                        }

                        transacao.Commit();

                        ret = numPedido;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return ret;
        }
    }
}