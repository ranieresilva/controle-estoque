using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ControleEstoque.Web.Controllers
{
    public class ContaController : Controller
    {
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel login, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var usuario = UsuarioModel.ValidarUsuario(login.Usuario, login.Senha);

            if (usuario != null)
            {
                var tiket = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(
                    1, usuario.Nome, DateTime.Now, DateTime.Now.AddHours(12), login.LembrarMe, usuario.Id + "|" + usuario.RecuperarStringNomePerfis()));
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, tiket);
                Response.Cookies.Add(cookie);

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError("", "Login inválido.");
            }

            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult AlterarSenhaUsuario(AlteracaoSenhaUsuarioViewModel model)
        {
            ViewBag.Mensagem = null;

            if (HttpContext.Request.HttpMethod.ToUpper() == "POST")
            {
                var usuarioLogado = (HttpContext.User as AplicacaoPrincipal);
                var alterou = false;
                if (usuarioLogado != null)
                {
                    if (!usuarioLogado.Dados.ValidarSenhaAtual(model.SenhaAtual))
                    {
                        ModelState.AddModelError("SenhaAtual", "A senha atual não confere.");
                    }
                    else
                    {
                        alterou = usuarioLogado.Dados.AlterarSenha(model.NovaSenha);

                        if (alterou)
                        {
                            ViewBag.Mensagem = new string[] { "ok", "Senha alterada com sucesso." };
                        }
                        else
                        {
                            ViewBag.Mensagem = new string[] { "erro", "Não foi possível alterar a senha." };
                        }
                    }
                }

                return View();
            }
            else
            {
                ModelState.Clear();
                return View();
            }
        }
    }
}