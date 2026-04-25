using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace RaizesDigitais.Pages
{
    public partial class index : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarExperienciasDestaque();
                CarregarTestemunhos();
            }
        }

        private void CarregarTestemunhos()
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_avaliacoes_publicas", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    dt.Columns.Add("estrelas_html", typeof(string));
                    dt.Columns.Add("data_formatada", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        int estrelas = Convert.ToInt32(row["estrelas"]);
                        row["estrelas_html"] = GetEstrelas(estrelas);

                        DateTime data = Convert.ToDateTime(row["data_avaliacao"]);
                        row["data_formatada"] = data.ToString("dd MMM yyyy");
                    }
                }

                rpt_testemunhos.DataSource = dt;
                rpt_testemunhos.DataBind();
            }
        }

        private string GetEstrelas(int valor)
        {
            string estrelas = "";
            for (int i = 1; i <= 5; i++)
            {
                estrelas += (i <= valor)
                    ? "<i class='bi bi-star-fill'></i> "
                    : "<i class='bi bi-star'></i> ";
            }
            return estrelas;
        }

        private void CarregarExperienciasDestaque()
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                // ADDED: uso de SP em vez de query directa (professor exige 100% SP)
                SqlCommand cmd = new SqlCommand("sp_listar_experiencias_destaque", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rpt_experiencias.DataSource = dt;
                rpt_experiencias.DataBind();
            }
        }
    }
}
