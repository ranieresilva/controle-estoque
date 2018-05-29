using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ControleEstoque.Web.Models
{
    public class PerfilModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public List<UsuarioModel> Usuarios { get; set; }

        public PerfilModel()
        {
            this.Usuarios = new List<UsuarioModel>();
        }

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
                    comando.CommandText = "select count(*) from perfil";
                    ret = (int)comando.ExecuteScalar();
                }
            }

            return ret;
        }

        public static List<PerfilModel> RecuperarLista(int pagina, int tamPagina, string ordem = "")
        {
            var ret = new List<PerfilModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    var pos = (pagina - 1) * tamPagina;

                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        "select *" +
                        " from perfil" +
                        " order by " + (!string.IsNullOrEmpty(ordem) ? ordem : "nome") +
                        " offset {0} rows fetch next {1} rows only",
                        pos > 0 ? pos - 1 : 0, tamPagina);
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }

            return ret;
        }

        public void CarregarUsuarios()
        {
            this.Usuarios.Clear();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText =
                        "select u.* " +
                        "from perfil_usuario pu, usuario u " +
                        "where (pu.id_perfil = @id_perfil) and (pu.id_usuario = u.id)";

                    comando.Parameters.Add("@id_perfil", SqlDbType.Int).Value = this.Id;

                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        this.Usuarios.Add(new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Login = (string)reader["login"]
                        });
                    }
                }
            }
        }

        public static List<PerfilModel> RecuperarListaAtivos()
        {
            var ret = new List<PerfilModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format("select * from perfil where ativo=1 order by nome");
                    var reader = comando.ExecuteReader();
                    while (reader.Read())
                    {
                        ret.Add(new PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]
                        });
                    }
                }
            }

            return ret;
        }

        public static PerfilModel RecuperarPeloId(int id)
        {
            PerfilModel ret = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();
                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select * from perfil where (id = @id)";

                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    var reader = comando.ExecuteReader();
                    if (reader.Read())
                    {
                        ret = new PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
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
                        comando.CommandText = "delete from perfil where (id = @id)";

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

                using (var transacao = conexao.BeginTransaction())
                {
                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.Transaction = transacao;

                        if (model == null)
                        {
                            comando.CommandText = "insert into perfil (nome, ativo) values (@nome, @ativo); select convert(int, scope_identity())";

                            comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                            comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);

                            ret = (int)comando.ExecuteScalar();
                            this.Id = ret;
                        }
                        else
                        {
                            comando.CommandText = "update perfil set nome=@nome, ativo=@ativo where id = @id";

                            comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                            comando.Parameters.Add("@ativo", SqlDbType.VarChar).Value = (this.Ativo ? 1 : 0);
                            comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;

                            if (comando.ExecuteNonQuery() > 0)
                            {
                                ret = this.Id;
                            }
                        }
                    }

                    if (this.Usuarios != null && this.Usuarios.Count > 0)
                    {
                        using (var comandoExclusaoPerfilUsuario = new SqlCommand())
                        {
                            comandoExclusaoPerfilUsuario.Connection = conexao;
                            comandoExclusaoPerfilUsuario.Transaction = transacao;

                            comandoExclusaoPerfilUsuario.CommandText = "delete from perfil_usuario where (id_perfil = @id_perfil)";
                            comandoExclusaoPerfilUsuario.Parameters.Add("@id_perfil", SqlDbType.Int).Value = this.Id;

                            comandoExclusaoPerfilUsuario.ExecuteScalar();
                        }

                        if (this.Usuarios[0].Id != -1)
                        {
                            foreach (var usuario in this.Usuarios)
                            {
                                using (var usuarioInclusaoPerfilUsuario = new SqlCommand())
                                {
                                    usuarioInclusaoPerfilUsuario.Connection = conexao;
                                    usuarioInclusaoPerfilUsuario.Transaction = transacao;

                                    usuarioInclusaoPerfilUsuario.CommandText = "insert into perfil_usuario (id_perfil, id_usuario) values (@id_perfil, @id_usuario)";
                                    usuarioInclusaoPerfilUsuario.Parameters.Add("@id_perfil", SqlDbType.Int).Value = this.Id;
                                    usuarioInclusaoPerfilUsuario.Parameters.Add("@id_usuario", SqlDbType.Int).Value = usuario.Id;

                                    usuarioInclusaoPerfilUsuario.ExecuteScalar();
                                }
                            }
                        }
                    }

                    transacao.Commit();
                }
            }

            return ret;
        }
    }
}