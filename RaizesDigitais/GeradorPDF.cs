using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Forms;
using iText.Forms.Fields;

namespace RaizesDigitais
{
    public class GeradorPDF
    {
        public static byte[] GerarConfirmacaoReserva(
            string numReserva,
            string experiencia,
            string dataHora,
            string participantes,
            string nome,
            string email,
            string total)
        {
            string caminhoTemplate = AppDomain.CurrentDomain.BaseDirectory
                + "Template\\confirmacao_reserva_template.pdf";

            string caminhoTemp = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Template",
                "temp_" + Guid.NewGuid().ToString("N") + ".pdf");

            PdfReader preader = new PdfReader(caminhoTemplate);
            PdfWriter pwriter = new PdfWriter(caminhoTemp);
            PdfDocument pdfDoc = new PdfDocument(preader, pwriter);

            PdfAcroForm form = PdfAcroForm.GetAcroForm(pdfDoc, false);

            if (form != null)
            {
                SetField(form, "ConfirmNumReserva", numReserva);
                SetField(form, "ConfirmExp", experiencia);
                SetField(form, "ConfirmDataHora", dataHora);
                SetField(form, "ConfirmPax", participantes);
                SetField(form, "ConfirmNome", nome);
                SetField(form, "ConfirmEmail", email);
                SetField(form, "ConfirmTotal", total);

                form.FlattenFields();
            }

            pdfDoc.Close(); // fecha o PdfDocument — liberta o PdfWriter e o PdfReader

            // Só depois do Close() é que o ficheiro está completamente escrito e libertado
            byte[] resultado = File.ReadAllBytes(caminhoTemp);

            // Tentativa de limpeza — se falhar não é problema crítico
            try { File.Delete(caminhoTemp); } catch { }

            return resultado;
        }

        private static void SetField(PdfAcroForm form, string nome, string valor)
        {
            PdfFormField field = form.GetField(nome);
            if (field != null)
                field.SetValue(valor ?? "");
        }
    }
}