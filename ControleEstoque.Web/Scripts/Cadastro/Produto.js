function set_dados_form(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_codigo').val(dados.Codigo);
    $('#txt_nome').val(dados.Nome);
    $('#txt_preco_custo').val(dados.PrecoCusto);
    $('#txt_preco_venda').val(dados.PrecoVenda);
    $('#txt_quant_estoque').val(dados.QuantEstoque);
    $('#ddl_unidade_medida').val(dados.IdUnidadeMedida);
    $('#ddl_grupo').val(dados.IdGrupo);
    $('#ddl_marca').val(dados.IdMarca);
    $('#ddl_fornecedor').val(dados.IdFornecedor);
    $('#ddl_local_armazenamento').val(dados.IdLocalArmazenamento);
    $('#cbx_ativo').prop('checked', dados.Ativo);
    //$('#txt_imagem').val(dados.Imagem);
}

function set_focus_form() {
    var alterando = (parseInt($('#id_cadastro').val()) > 0);
    $('#txt_quant_estoque').attr('readonly', alterando);

    $('#txt_codigo').focus();
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Codigo: '',
        Nome: '',
        PrecoCusto: 0,
        PrecoVenda: 0,
        QuantEstoque: 0,
        IdUnidadeMedida: 0,
        IdGrupo: 0,
        IdMarca: 0,
        IdFornecedor: 0,
        IdLocalArmazenamento: 0,
        Ativo: true,
        Imagem: '',
    };
}

function get_dados_form() {
    var form = new FormData();
    form.append('Id', $('#id_cadastro').val());
    form.append('Codigo', $('#txt_codigo').val());
    form.append('Nome', $('#txt_nome').val());
    form.append('PrecoCusto', $('#txt_preco_custo').val());
    form.append('PrecoVenda', $('#txt_preco_venda').val());
    form.append('QuantEstoque', $('#txt_quant_estoque').val());
    form.append('IdUnidadeMedida', $('#ddl_unidade_medida').val());
    form.append('IdGrupo', $('#ddl_grupo').val());
    form.append('IdMarca', $('#ddl_marca').val());
    form.append('IdFornecedor', $('#ddl_fornecedor').val());
    form.append('IdLocalArmazenamento', $('#ddl_local_armazenamento').val());
    form.append('Ativo', $('#cbx_ativo').prop('checked'));
    form.append('Imagem', $('#txt_imagem').prop('files')[0]);
    form.append('__RequestVerificationToken', $('[name=__RequestVerificationToken]').val());
    return form;
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Codigo).end()
        .eq(1).html(param.Nome).end()
        .eq(2).html(param.QuantEstoque).end()
        .eq(3).html(param.Ativo ? 'SIM' : 'NÃO');
}

function salvar_customizado(url, param, salvar_ok, salvar_erro) {
    $.ajax({
        type: 'POST',
        processData: false,
        contentType: false,
        data: param,
        url: url,
        dataType: 'json',
        success: function (response) {
            salvar_ok(response, get_param());
        },
        error: function () {
            salvar_erro();
        }
    });
}

function get_param() {
    return {
        Id: $('#id_cadastro').val(),
        Codigo: $('#txt_codigo').val(),
        Nome: $('#txt_nome').val(),
        QuantEstoque: $('#txt_quant_estoque').val(),
        Ativo: $('#cbx_ativo').prop('checked'),
        Imagem: $('#txt_imagem').prop('files')[0]
    };
}

$(document)
.ready(function () {
    $('#txt_preco_custo,#txt_preco_venda').mask('#.##0,00', { reverse: true });
    $('#txt_quant_estoque').mask('00000');
})
.on('click', '.btn-exibir-imagem', function () {
    var nome_imagem = $(this).closest('tr').attr('data-imagem'),
        modal_imagem = $('#modal_imagem'),
        template_imagem = $('#template-imagem'),
        conteudo_modal_imagem = Mustache.render(template_imagem.html(), { Imagem: nome_imagem });

    modal_imagem.html(conteudo_modal_imagem);

    bootbox.dialog({
        title: `Imagem ${nome_imagem}`,
        message: modal_imagem,
        className: 'dialogo'
    })
    .on('shown.bs.modal', function () {
        modal_imagem.show();
    })
    .on('hidden.bs.modal', function () {
        modal_imagem.hide().appendTo('body');
    });
});