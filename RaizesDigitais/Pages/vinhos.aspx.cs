using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais.Pages
{
    public partial class vinhos : Page
    {
        readonly string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CarregarVinhos();
        }

        private void CarregarVinhos()
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Vinhos
                SqlCommand cmdVinhos = new SqlCommand("sp_listar_vinhos", con);
                cmdVinhos.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(cmdVinhos);
                DataTable dtVinhos = new DataTable();
                da.Fill(dtVinhos);

                // Médias
                SqlCommand cmdMedias = new SqlCommand("sp_media_avaliacoes_vinhos", con);
                cmdMedias.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter daMedias = new SqlDataAdapter(cmdMedias);
                DataTable dtMedias = new DataTable();
                daMedias.Fill(dtMedias);

                // Favoritos do cliente (se logado)
                DataTable dtFavoritos = new DataTable();
                dtFavoritos.Columns.Add("id_vinho", typeof(int));

                if (Session["cliente_id"] != null)
                {
                    SqlCommand cmdFav = new SqlCommand("sp_listar_favoritos_cliente", con);
                    cmdFav.CommandType = CommandType.StoredProcedure;
                    cmdFav.Parameters.AddWithValue("@id_cliente", Convert.ToInt32(Session["cliente_id"]));
                    SqlDataAdapter daFav = new SqlDataAdapter(cmdFav);
                    daFav.Fill(dtFavoritos);
                }

                // Juntar ao DataTable de vinhos
                dtVinhos.Columns.Add("media_estrelas", typeof(double));
                dtVinhos.Columns.Add("total_avaliacoes", typeof(int));
                dtVinhos.Columns.Add("is_favorito", typeof(bool));

                foreach (DataRow row in dtVinhos.Rows)
                {
                    int idVinho = Convert.ToInt32(row["id_vinho"]);

                    DataRow[] mediaRow = dtMedias.Select("id_vinho = " + idVinho);
                    row["media_estrelas"] = mediaRow.Length > 0 ? Convert.ToDouble(mediaRow[0]["media"]) : 0.0;
                    row["total_avaliacoes"] = mediaRow.Length > 0 ? Convert.ToInt32(mediaRow[0]["total"]) : 0;

                    DataRow[] favRow = dtFavoritos.Select("id_vinho = " + idVinho);
                    row["is_favorito"] = favRow.Length > 0;
                }

                // ADDED: filter only active wines (visible on website)
                DataTable vinhosActivos = dtVinhos.Select("activo = true").CopyToDataTable();
                rpt_vinhos.DataSource = vinhosActivos;
                rpt_vinhos.DataBind();
            }
        }

        protected void rpt_vinhos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "ToggleFavorito") return;
            if (Session["cliente_id"] == null)
            {
                Response.Redirect("~/Pages/conta_login.aspx");
                return;
            }

            int idVinho = int.Parse(e.CommandArgument.ToString());
            int idCliente = Convert.ToInt32(Session["cliente_id"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_toggle_favorito", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_cliente", idCliente);
                cmd.Parameters.AddWithValue("@id_vinho", idVinho);

                SqlParameter outRetorno = new SqlParameter("@retorno", SqlDbType.Int);
                outRetorno.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outRetorno);

                con.Open();
                cmd.ExecuteNonQuery();
            }

            CarregarVinhos();
        }

        protected string GetEstrelas(object mediaObj, object totalObj)
        {
            double media = Convert.ToDouble(mediaObj);
            int total = Convert.ToInt32(totalObj);

            if (total == 0)
                return "<small class='text-muted'>Sem avaliações</small>";

            string estrelas = "";
            for (int i = 1; i <= 5; i++)
            {
                if (i <= Math.Floor(media))
                    estrelas += "<i class='bi bi-star-fill' style='color:#c8a84b;'></i>";
                else if (i - media < 1 && i - media > 0)
                    estrelas += "<i class='bi bi-star-half' style='color:#c8a84b;'></i>";
                else
                    estrelas += "<i class='bi bi-star' style='color:#c8a84b;'></i>";
            }

            return estrelas + " <small class='text-muted'>(" + total + ")</small>";
        }

        protected string GetVinhoImagem(object imagemUrl, object nomeVinho)
        {
            string url = imagemUrl?.ToString();
            if (!string.IsNullOrEmpty(url))
                return ResolveUrl(url);

            string nome = nomeVinho?.ToString() ?? "";
            string nomeLower = nome.ToLower();
            if (nomeLower.Contains("magnum") || nomeLower.Contains("grande reserva tinto"))
                return ResolveUrl("~/Images/QtaAzenha_Magnum2014.png");
            if (nomeLower.Contains("bruto") || nomeLower.Contains("espumante"))
                return ResolveUrl("~/Images/QtaAzenhaBruto_2019.png");
            if (nomeLower.Contains("grande reserva branco"))
                return ResolveUrl("~/Images/QtaAzenhaGrdReserva.png");
            return ResolveUrl("~/Images/QtaAzenha2019.png");
        }

        protected string GetBadgeStyle(string tipo)
        {
            switch (tipo)
            {
                case "Branco": return "background-color: #d4edda; color: #155724; font-weight: 400;";
                case "Tinto": return "background-color: #f8d7da; color: #721c24; font-weight: 400;";
                case "Espumante": return "background-color: #d1ecf1; color: #0c5460; font-weight: 400;";
                default: return "background-color: #e2e3e5; color: #383d41; font-weight: 400;";
            }
        }
    }
}