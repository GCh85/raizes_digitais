using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RaizesDigitais
{
    public partial class MasterSite : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Ano dinâmico no footer
            lit_ano.Text = DateTime.Now.Year.ToString();
        }
    }
}
