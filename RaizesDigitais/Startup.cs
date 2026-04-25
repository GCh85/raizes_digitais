using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;


// Startup.cs — Configuração OWIN para Google OAuth
// O atributo OwinStartup diz ao servidor qual a classe que inicializa o middleware OWIN.


[assembly: OwinStartup(typeof(RaizesDigitais.Startup))]

namespace RaizesDigitais
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cookie middleware — necessário para o Google OAuth
            // guardar a sessão de autenticação externa.
            // O LoginPath aponta para a página de login do projecto.
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ExternalCookie" 
            });
            app.SetDefaultSignInAsAuthenticationType("ExternalCookie");

          
            // Google OAuth — Client ID e Secret vêm do Web.config.
            // O Google redireciona para /signin-google após autenticação
            // (este caminho está registado na Google Cloud Console).
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["GoogleClientId"],
                ClientSecret = ConfigurationManager.AppSettings["GoogleClientSecret"]
            });
        }
    }
}