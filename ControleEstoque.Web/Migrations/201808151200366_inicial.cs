namespace ControleEstoque.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class inicial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.cidade",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 30),
                        ativo = c.Boolean(nullable: false),
                        id_estado = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.estado", t => t.id_estado)
                .Index(t => t.id_estado);
            
            CreateTable(
                "dbo.estado",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 30),
                        uf = c.String(nullable: false, maxLength: 2),
                        ativo = c.Boolean(nullable: false),
                        id_pais = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.pais", t => t.id_pais)
                .Index(t => t.id_pais);
            
            CreateTable(
                "dbo.pais",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 30),
                        codigo = c.String(nullable: false, maxLength: 3),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.entrada_produto",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        numero = c.String(nullable: false, maxLength: 10),
                        data = c.DateTime(nullable: false),
                        quant = c.Int(nullable: false),
                        id_produto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.produto", t => t.id_produto)
                .Index(t => t.id_produto);
            
            CreateTable(
                "dbo.produto",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        codigo = c.String(nullable: false, maxLength: 10),
                        nome = c.String(nullable: false, maxLength: 50),
                        preco_custo = c.Decimal(nullable: false, precision: 7, scale: 2),
                        preco_venda = c.Decimal(nullable: false, precision: 7, scale: 2),
                        quant_estoque = c.Int(nullable: false),
                        id_unidade_medida = c.Int(nullable: false),
                        id_grupo = c.Int(nullable: false),
                        id_marca = c.Int(nullable: false),
                        id_fornecedor = c.Int(nullable: false),
                        id_local_armazenamento = c.Int(nullable: false),
                        ativo = c.Boolean(nullable: false),
                        imagem = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.fornecedor", t => t.id_fornecedor)
                .ForeignKey("dbo.grupo_produto", t => t.id_grupo)
                .ForeignKey("dbo.local_armazenamento", t => t.id_local_armazenamento)
                .ForeignKey("dbo.marca_produto", t => t.id_marca)
                .ForeignKey("dbo.unidade_medida", t => t.id_unidade_medida)
                .Index(t => t.id_unidade_medida)
                .Index(t => t.id_grupo)
                .Index(t => t.id_marca)
                .Index(t => t.id_fornecedor)
                .Index(t => t.id_local_armazenamento);
            
            CreateTable(
                "dbo.fornecedor",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 60),
                        razao_social = c.String(maxLength: 100),
                        num_documento = c.String(maxLength: 20),
                        tipo = c.Int(nullable: false),
                        telefone = c.String(nullable: false, maxLength: 20),
                        contato = c.String(nullable: false, maxLength: 60),
                        logradouro = c.String(nullable: false, maxLength: 100),
                        numero = c.String(nullable: false, maxLength: 20),
                        complemento = c.String(maxLength: 100),
                        cep = c.String(maxLength: 10),
                        id_pais = c.Int(nullable: false),
                        id_estado = c.Int(nullable: false),
                        id_cidade = c.Int(nullable: false),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.cidade", t => t.id_cidade)
                .ForeignKey("dbo.estado", t => t.id_estado)
                .ForeignKey("dbo.pais", t => t.id_pais)
                .Index(t => t.id_pais)
                .Index(t => t.id_estado)
                .Index(t => t.id_cidade);
            
            CreateTable(
                "dbo.grupo_produto",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 50),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.local_armazenamento",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 50),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.marca_produto",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 50),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.unidade_medida",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 30),
                        sigla = c.String(nullable: false, maxLength: 3),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.inventario_estoque",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        data = c.DateTime(nullable: false),
                        motivo = c.String(maxLength: 100),
                        quant_estoque = c.Int(nullable: false),
                        quant_inventario = c.Int(nullable: false),
                        id_produto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.produto", t => t.id_produto)
                .Index(t => t.id_produto);
            
            CreateTable(
                "dbo.perfil",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false, maxLength: 20),
                        ativo = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.usuario",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        login = c.String(nullable: false, maxLength: 50),
                        senha = c.String(nullable: false, maxLength: 32),
                        nome = c.String(nullable: false, maxLength: 100),
                        email = c.String(nullable: false, maxLength: 150),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.saida_produto",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        numero = c.String(nullable: false, maxLength: 10),
                        data = c.DateTime(nullable: false),
                        quant = c.Int(nullable: false),
                        id_produto = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.produto", t => t.id_produto)
                .Index(t => t.id_produto);
            
            CreateTable(
                "dbo.perfil_usuario",
                c => new
                    {
                        id_perfil = c.Int(nullable: false),
                        id_usuario = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.id_perfil, t.id_usuario })
                .ForeignKey("dbo.perfil", t => t.id_perfil, cascadeDelete: true)
                .ForeignKey("dbo.usuario", t => t.id_usuario, cascadeDelete: true)
                .Index(t => t.id_perfil)
                .Index(t => t.id_usuario);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.saida_produto", "id_produto", "dbo.produto");
            DropForeignKey("dbo.perfil_usuario", "id_usuario", "dbo.usuario");
            DropForeignKey("dbo.perfil_usuario", "id_perfil", "dbo.perfil");
            DropForeignKey("dbo.inventario_estoque", "id_produto", "dbo.produto");
            DropForeignKey("dbo.entrada_produto", "id_produto", "dbo.produto");
            DropForeignKey("dbo.produto", "id_unidade_medida", "dbo.unidade_medida");
            DropForeignKey("dbo.produto", "id_marca", "dbo.marca_produto");
            DropForeignKey("dbo.produto", "id_local_armazenamento", "dbo.local_armazenamento");
            DropForeignKey("dbo.produto", "id_grupo", "dbo.grupo_produto");
            DropForeignKey("dbo.produto", "id_fornecedor", "dbo.fornecedor");
            DropForeignKey("dbo.fornecedor", "id_pais", "dbo.pais");
            DropForeignKey("dbo.fornecedor", "id_estado", "dbo.estado");
            DropForeignKey("dbo.fornecedor", "id_cidade", "dbo.cidade");
            DropForeignKey("dbo.cidade", "id_estado", "dbo.estado");
            DropForeignKey("dbo.estado", "id_pais", "dbo.pais");
            DropIndex("dbo.perfil_usuario", new[] { "id_usuario" });
            DropIndex("dbo.perfil_usuario", new[] { "id_perfil" });
            DropIndex("dbo.saida_produto", new[] { "id_produto" });
            DropIndex("dbo.inventario_estoque", new[] { "id_produto" });
            DropIndex("dbo.fornecedor", new[] { "id_cidade" });
            DropIndex("dbo.fornecedor", new[] { "id_estado" });
            DropIndex("dbo.fornecedor", new[] { "id_pais" });
            DropIndex("dbo.produto", new[] { "id_local_armazenamento" });
            DropIndex("dbo.produto", new[] { "id_fornecedor" });
            DropIndex("dbo.produto", new[] { "id_marca" });
            DropIndex("dbo.produto", new[] { "id_grupo" });
            DropIndex("dbo.produto", new[] { "id_unidade_medida" });
            DropIndex("dbo.entrada_produto", new[] { "id_produto" });
            DropIndex("dbo.estado", new[] { "id_pais" });
            DropIndex("dbo.cidade", new[] { "id_estado" });
            DropTable("dbo.perfil_usuario");
            DropTable("dbo.saida_produto");
            DropTable("dbo.usuario");
            DropTable("dbo.perfil");
            DropTable("dbo.inventario_estoque");
            DropTable("dbo.unidade_medida");
            DropTable("dbo.marca_produto");
            DropTable("dbo.local_armazenamento");
            DropTable("dbo.grupo_produto");
            DropTable("dbo.fornecedor");
            DropTable("dbo.produto");
            DropTable("dbo.entrada_produto");
            DropTable("dbo.pais");
            DropTable("dbo.estado");
            DropTable("dbo.cidade");
        }
    }
}
