var salvar_customizado = null;

function marcar_ordenacao_campo(coluna) {
    var ordem_crescente = true,
        ordem = coluna.find('i');

    if (ordem.length > 0) {
        ordem_crescente = ordem.hasClass('glyphicon-arrow-down');
        if (ordem_crescente) {
            ordem.removeClass('glyphicon-arrow-down');
            ordem.addClass('glyphicon glyphicon-arrow-up');
        }
        else {
            ordem.removeClass('glyphicon-arrow-up');
            ordem.addClass('glyphicon-arrow-down');
        }
    }
    else {
        $('.coluna-ordenacao i').remove();
        coluna.append('&nbsp;<i class="glyphicon glyphicon-arrow-down" style="color: #000000"></i>');
    }
}

function obter_ordem_grid() {
    var colunas_grid = $('.coluna-ordenacao'),
        ret = '';

    colunas_grid.each(function (index, item) {
        var coluna = $(item),
            ordem = coluna.find('i');

        if (ordem.length > 0) {
            ordem_crescente = ordem.hasClass('glyphicon-arrow-down');
            ret = coluna.attr('data-campo') + (ordem_crescente ? '' : ' desc');
            return true;
        }
    });

    return ret;
}

function add_anti_forgery_token(data) {
    data.__RequestVerificationToken = $('[name=__RequestVerificationToken]').val();
    return data;
}

function formatar_mensagem_aviso(mensagens) {
    var template =
        '<ul>' +
        '{{ #. }}' +
        '<li>{{ . }}</li>' +
        '{{ /. }}' +
        '</ul>';

    return Mustache.render(template, mensagens);
}

function abrir_form(dados) {
    set_dados_form(dados);

    var modal_cadastro = $('#modal_cadastro');

    $('#msg_mensagem_aviso').empty();
    $('#msg_aviso').hide();
    $('#msg_mensagem_aviso').hide();
    $('#msg_erro').hide();

    bootbox.dialog({
        title: 'Cadastro de ' + tituloPagina,
        message: modal_cadastro,
        className: 'dialogo',
    })
    .on('shown.bs.modal', function () {
        modal_cadastro.show(0, function () {
            set_focus_form();
        });
    })
    .on('hidden.bs.modal', function () {
        modal_cadastro.hide().appendTo('body');
    });
}

function criar_linha_grid(dados) {
    var template = $('#template-grid').html();
    return Mustache.render(template, dados);
}

function salvar_ok(response, param) {
    if (response.Resultado == "OK") {
        if (param.Id == 0) {
            param.Id = response.IdSalvo;
            var table = $('#grid_cadastro').find('tbody'),
                linha = criar_linha_grid(param);

            table.append(linha);
            $('#grid_cadastro').removeClass('invisivel');
            $('#mensagem_grid').addClass('invisivel');
        }
        else {
            var linha = $('#grid_cadastro').find('tr[data-id=' + param.Id + ']').find('td');
            preencher_linha_grid(param, linha);
        }

        $('#modal_cadastro').parents('.bootbox').modal('hide');
    }
    else if (response.Resultado == "ERRO") {
        $('#msg_aviso').hide();
        $('#msg_mensagem_aviso').hide();
        $('#msg_erro').show();
    }
    else if (response.Resultado == "AVISO") {
        $('#msg_mensagem_aviso').html(formatar_mensagem_aviso(response.Mensagens));
        $('#msg_aviso').show();
        $('#msg_mensagem_aviso').show();
        $('#msg_erro').hide();
    }
}

function salvar_erro() {
    swal('Aviso', 'Não foi possível salvar. Tente novamente em instantes.', 'warning');
}

$(document).on('click', '#btn_incluir', function () {
    abrir_form(get_dados_inclusao());
})
.on('click', '.btn-alterar', function () {
    var btn = $(this),
        id = btn.closest('tr').attr('data-id'),
        url = url_alterar,
        param = { 'id': id };

    $.post(url, add_anti_forgery_token(param), function (response) {
        if (response) {
            abrir_form(response);
        }
    })
    .fail(function () {
        swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
    });
})
.on('click', '.btn-excluir', function () {
    var btn = $(this),
        tr = btn.closest('tr'),
        id = tr.attr('data-id'),
        url = url_excluir,
        param = { 'id': id };

    bootbox.confirm({
        message: "Realmente deseja excluir o " + tituloPagina + "?",
        buttons: {
            confirm: {
                label: 'Sim',
                className: 'btn-danger'
            },
            cancel: {
                label: 'Não',
                className: 'btn-success'
            }
        },
        callback: function (result) {
            if (result) {
                $.post(url, add_anti_forgery_token(param), function (response) {
                    if (response) {
                        tr.remove();
                        var quant = $('#grid_cadastro > tbody > tr').length;
                        if (quant == 0) {
                            $('#grid_cadastro').addClass('invisivel');
                            $('#mensagem_grid').removeClass('invisivel');
                        }
                    }
                })
                .fail(function () {
                    swal('Aviso', 'Não foi possível excluir. Tente novamente em instantes.', 'warning');
                });
            }
        }
    });
})
.on('click', '#btn_confirmar', function () {
    var btn = $(this),
        url = url_confirmar,
        param = get_dados_form();

    if (salvar_customizado && typeof (salvar_customizado) == 'function') {
        salvar_customizado(url, param, salvar_ok, salvar_erro);
    }
    else {
        $.post(url, add_anti_forgery_token(param), function (response) {
            salvar_ok(response, param);
        })
        .fail(function () {
            salvar_erro();
        });
    }
})
.on('click', '.page-item', function () {
    var ordem = obter_ordem_grid(),
        btn = $(this),
        filtro = $('#txt_filtro'),
        tamPag = $('#ddl_tam_pag').val(),
        pagina = btn.text(),
        url = url_page_click,
        param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

    $.post(url, add_anti_forgery_token(param), function (response) {
        if (response) {
            var table = $('#grid_cadastro').find('tbody');

            table.empty();
            if (response.length > 0) {
                $('#grid_cadastro').removeClass('invisivel');
                $('#mensagem_grid').addClass('invisivel');

                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }
            }
            else {
                $('#grid_cadastro').addClass('invisivel');
                $('#mensagem_grid').removeClass('invisivel');
            }

            btn.siblings().removeClass('active');
            btn.addClass('active');
        }
    })
    .fail(function () {
        swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
    });
})
.on('change', '#ddl_tam_pag', function () {
    var ordem = obter_ordem_grid(),
        ddl = $(this),
        filtro = $('#txt_filtro'),
        tamPag = ddl.val(),
        pagina = 1,
        url = url_tam_pag_change,
        param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

    $.post(url, add_anti_forgery_token(param), function (response) {
        if (response) {
            var table = $('#grid_cadastro').find('tbody');

            table.empty();
            if (response.length > 0) {
                $('#grid_cadastro').removeClass('invisivel');
                $('#mensagem_grid').addClass('invisivel');

                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }
            }
            else {
                $('#grid_cadastro').addClass('invisivel');
                $('#mensagem_grid').removeClass('invisivel');
            }

            ddl.siblings().removeClass('active');
            ddl.addClass('active');
        }
    })
    .fail(function () {
        swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
    });
})
.on('keyup', '#txt_filtro', function () {
    var ordem = obter_ordem_grid(),
        filtro = $(this),
        ddl = $('#ddl_tam_pag'),
        tamPag = ddl.val(),
        pagina = 1,
        url = url_filtro_change,
        param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

    $.post(url, add_anti_forgery_token(param), function (response) {
        if (response) {
            var table = $('#grid_cadastro').find('tbody');

            table.empty();
            if (response.length > 0) {
                $('#grid_cadastro').removeClass('invisivel');
                $('#mensagem_grid').addClass('invisivel');

                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }
            }
            else {
                $('#grid_cadastro').addClass('invisivel');
                $('#mensagem_grid').removeClass('invisivel');
            }

            ddl.siblings().removeClass('active');
            ddl.addClass('active');
        }
    })
    .fail(function () {
        swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
    });
})
.on('click', '.coluna-ordenacao', function () {
    marcar_ordenacao_campo($(this));

    var ordem = obter_ordem_grid(),
        filtro = $('#txt_filtro'),
        ddl = $('#ddl_tam_pag'),
        tamPag = ddl.val(),
        pagina = 1,
        url = url_filtro_change,
        param = { 'pagina': pagina, 'tamPag': tamPag, 'filtro': filtro.val(), 'ordem': ordem };

    $.post(url, add_anti_forgery_token(param), function (response) {
        if (response) {
            var table = $('#grid_cadastro').find('tbody');

            table.empty();
            if (response.length > 0) {
                $('#grid_cadastro').removeClass('invisivel');
                $('#mensagem_grid').addClass('invisivel');

                for (var i = 0; i < response.length; i++) {
                    table.append(criar_linha_grid(response[i]));
                }
            }
            else {
                $('#grid_cadastro').addClass('invisivel');
                $('#mensagem_grid').removeClass('invisivel');
            }

            ddl.siblings().removeClass('active');
            ddl.addClass('active');
        }
    })
    .fail(function () {
        swal('Aviso', 'Não foi possível recuperar as informações. Tente novamente em instantes.', 'warning');
    });
});

$(document).ready(function () {
    var grid = $('#grid_cadastro > tbody');
    for (var i = 0; i < linhas.length; i++) {
        grid.append(criar_linha_grid(linhas[i]));
    }

    marcar_ordenacao_campo($('#grid_cadastro thead tr th:nth-child(1) span'));
});