using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace RaizesDigitais.Pages
{
    public partial class experiencias : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarExperiencias();
            }
        }

        private void CarregarExperiencias()
        {
            string connStr = ConfigurationManager.ConnectionStrings["RaizesDB"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                SqlCommand cmd = new SqlCommand("sp_listar_experiencias", con);
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
