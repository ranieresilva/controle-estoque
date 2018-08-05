using ControleEstoque.Web.Models;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
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

        [AllowAnonymous]
        public ActionResult EsqueciMinhaSenha(EsqueciMinhaSenhaViewModel model)
        {
            ViewBag.EmailEnviado = true;
            if (HttpContext.Request.HttpMethod.ToUpper() == "GET")
            {
                ViewBag.EmailEnviado = false;
                ModelState.Clear();
            }
            else
            {
                var usuario = UsuarioModel.RecuperarPeloLogin(model.Login);
                if (usuario != null)
                {
                    EnviarEmailRedefinicaoSenha(usuario);
                }
            }

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult RedefinirSenha(int id)
        {
            var usuario = UsuarioModel.RecuperarPeloId(id);
            if (usuario == null)
            {
                id = -1;
            }

            var model = new NovaSenhaViewModel() { Usuario = id };

            ViewBag.Mensagem = null;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RedefinirSenha(NovaSenhaViewModel model)
        {
            ViewBag.Mensagem = null;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = UsuarioModel.RecuperarPeloId(model.Usuario);
            if (usuario != null)
            {
                var ok = usuario.AlterarSenha(model.Senha);
                ViewBag.Mensagem = ok ? "Senha alterada com sucesso!" : "Não foi possível alterar a senha!";
            }

            return View();
        }

        private void EnviarEmailRedefinicaoSenha(UsuarioModel usuario)
        {
            var callbackUrl = Url.Action("RedefinirSenha", "Conta", new { id = usuario.Id }, protocol: Request.Url.Scheme);
            var client = new SmtpClient()
            {
                Host = ConfigurationManager.AppSettings["EmailServidor"],
                Port = Convert.ToInt32(ConfigurationManager.AppSettings["EmailPorta"]),
                EnableSsl = (ConfigurationManager.AppSettings["EmailSsl"] == "S"),
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    ConfigurationManager.AppSettings["EmailUsuario"],
                    ConfigurationManager.AppSettings["EmailSenha"])
            };

            var mensagem = new MailMessage();
            mensagem.From = new MailAddress(ConfigurationManager.AppSettings["EmailOrigem"], "Controle de Estoque - Como Programar Melhor");
            mensagem.To.Add(usuario.Email);
            mensagem.Subject = "Redefinição de senha";
            mensagem.Body = string.Format("Redefina a sua senha <a href='{0}'>aqui</a>", callbackUrl);
            mensagem.IsBodyHtml = true;

            client.Send(mensagem);
        }
    }
}