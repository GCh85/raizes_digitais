using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais
{
    public partial class MasterBackoffice : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // 1. Verificar sessão
            if (Session["perfil"] == null)
            {
                Response.Redirect("~/login.aspx");
                return;
            }

            // 2. Obter dados da sessão
            string utilizador = Session["utilizador"].ToString();
            string perfil = Session["perfil"].ToString();

            // 3. Preencher labels da navbar e sidebar
            lbl_utilizador_header.Text = utilizador + " (" + perfil + ")";
            lbl_utilizador_sidebar.Text = utilizador;
            lbl_perfil_sidebar.Text = perfil;
            lbl_perfil_footer.Text = perfil;

            // 4. Controlo de visibilidade (Perfis)
            bool isAdmin = (perfil == "Administrador");
            bool isGestor = (perfil == "Gestor");

            header_sistema.Visible = isAdmin;
            li_menu_utilizadores.Visible = isAdmin;
            li_menu_testemunhos.Visible = isAdmin;
            li_menu_cupoes.Visible = !isGestor;
            li_menu_ofertas_b2b.Visible = !isGestor;

        }
    }
}
