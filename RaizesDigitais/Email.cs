// Copyright (c) 2026 GCh85. All rights reserved.
// This code is part of the Raizes Digitais project.
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.

using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;


namespace RaizesDigitais
{
    public class Email
    {
        
        private static string HtmlBase(string conteudo)
        {
            return @"
            <html>
            <head>
            <meta charset='utf-8'/>
            <meta name='viewport' content='width=device-width, initial-scale=1'/>
            <link href='https://fonts.googleapis.com/css2?family=Playfair+Display:wght@400;600;700&family=Inter:wght@300;400;500&display=swap' rel='stylesheet'/>
            </head>
            <body style='background:#FDFDFB; padding:40px 16px; margin:0; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background:#FDFDFB;'>
            <tr><td align='center'>
            <table role='presentation' width='560' cellpadding='0' cellspacing='0' style='max-width:560px; width:100%;'>
            <tr><td style='padding:24px 0;'>
            <!-- Logo / nome da marca, simples e centrado -->
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='text-align:center; margin-bottom:8px;'>
            <tr><td style='font-family:Playfair Display,Georgia,serif; font-size:20px; font-weight:700; color:#2F3E1E; letter-spacing:0.04em;'>
            Quinta da Azenha
            </td></tr>
            <tr><td style='font-size:11px; color:#6B7A5C; letter-spacing:0.15em; text-transform:uppercase; padding-top:2px;'>
            Bucelas · DOC Arinto
            </td></tr>
            </table>
            <!-- Separador sutil -->
            <table role='presentation' cellpadding='0' cellspacing='0' width='40' style='margin:12px auto 28px;'>
            <tr><td style='height:2px; background:#4A7C2F; width:40px;'></td></tr>
            </table>
            <!-- Conteudo da mensagem -->
            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background:#ffffff; border:1px solid #E8E8E4; border-radius:6px;'>
            <tr><td style='padding:32px 36px;'>
            " + conteudo + @"
            </td></tr>
            </table>
            <!-- Footer  -->
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='margin-top:24px; text-align:center;'>
            <tr><td style='font-size:11px; color:#6B7A5C; line-height:1.6;'>
            Quinta da Azenha · Bucelas, 2670-632 · Loures, Portugal<br/>
            <a href='mailto:info.quintadaazenha@gmail.com' style='color:#4A7C2F; text-decoration:none;'>info.quintadaazenha@gmail.com</a>
            </td></tr>
            </table>
            </td></tr>
            </table>
            </td></tr>
            </table>
            </body>
            </html>";
        }

        // Separador de seccao dentro do email
        private static string SeparadorSeccao(string titulo)
        {
            return @"
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='margin:24px 0 16px;'>
            <tr><td style='font-family:Inter,Segoe UI,Arial,sans-serif; font-size:10px; font-weight:500; text-transform:uppercase; letter-spacing:0.18em; color:#4A7C2F;'>" + titulo + @"</td></tr>
            <tr><td style='height:1px; background:#E8E8E4; margin-top:8px;'><div style='height:1px; background:#E8E8E4;'></div></td></tr>
            </table>";
        }

        // Botao CTA
        private static string BotaoCTA(string texto, string link)
        {
            return @"
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='margin:28px 0 4px;'>
            <tr><td align='center'>
            <a href='" + link + @"'
               style='background:#2F3E1E; color:#FDFDFB; padding:13px 40px;
                      border-radius:2px; text-decoration:none; font-size:13px;
                      font-weight:500; letter-spacing:0.08em; text-transform:uppercase;
                      display:inline-block; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            " + texto + @"
            </a>
            </td></tr>
            </table>";
        }

        // Nota de rodape
        private static string NotaRodape(string texto, string notaSeguranca)
        {
            string notaHtml = "";
            if (!string.IsNullOrEmpty(notaSeguranca))
            {
                notaHtml = @"
                <p style='margin:4px 0 0; color:#6B7A5C; font-size:11px;'>" + notaSeguranca + @"</p>";
            }
            return @"
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='margin-top:20px;'>
            <tr><td style='font-size:12px; color:#6B7A5C; line-height:1.6;'>
            <p style='margin:0;'>" + texto + @"</p>
            " + notaHtml + @"
            </td></tr>
            </table>";
        }

        // ═══════════════════════════════════════════════════════
        // ENVIAR (SMTP)
        // ═══════════════════════════════════════════════════════
        private static void Enviar(string destinatario, string assunto, string corpo)
        {
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"]);
            smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            smtp.Credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["SmtpUser"],
                ConfigurationManager.AppSettings["SmtpPassword"]
            );
            smtp.EnableSsl = true;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["SmtpFrom"]);
            msg.To.Add(destinatario);
            msg.Subject = assunto;
            msg.Body = corpo;
            msg.IsBodyHtml = true;

            try
            {
                smtp.Send(msg);
            }
            catch (SmtpException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao enviar email: " + ex.Message);
            }
            finally
            {
                msg.Dispose();
                smtp.Dispose();
            }
        }

        // ═══════════════════════════════════════════════════════
        // EnviarActivacao
        // ═══════════════════════════════════════════════════════
        public static void EnviarActivacao(string destinatario, string nomeUtilizador, string token)
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            string link = baseUrl + "/activar.aspx?token=" + token;

            string assunto = "Quinta da Azenha — Activar a sua conta";
            string conteudo = @"
            <p style='margin:0 0 16px; font-size:15px; color:#2F3E1E; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            Olá <strong>" + nomeUtilizador + @"</strong>,</p>
            <p style='color:#6B7A5C; line-height:1.7; font-size:13px; margin:0 0 8px;'>
            A sua conta foi criada com sucesso na <strong>Quinta da Azenha</strong>.<br/>
            Para a activar, clique no botão abaixo:</p>
            " + BotaoCTA("Activar conta", link) +
            NotaRodape("Este link é válido durante <strong>24 horas</strong>.",
            "Se não criou esta conta, ignore este email.");

            Enviar(destinatario, assunto, HtmlBase(conteudo));
        }

        // ═══════════════════════════════════════════════════════
        // EnviarCodigo2FA
        // ═══════════════════════════════════════════════════════
        public static void EnviarCodigo2FA(string destinatario, string nomeUtilizador, string codigo)
        {
            string assunto = "Quinta da Azenha — Código de verificação";
            string conteudo = @"
            <p style='margin:0 0 20px; font-size:15px; color:#2F3E1E; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            Olá <strong>" + nomeUtilizador + @"</strong>,</p>
            <p style='color:#6B7A5C; line-height:1.7; font-size:13px; margin:0 0 4px;'>
            Para completar o início de sessão, insira o código abaixo na página de verificação:</p>
            <!-- Código grande, centrado, fundo claro — sem amarelo -->
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='margin:28px 0;'>
            <tr><td align='center'>
            <span style='font-size:36px; font-weight:600; letter-spacing:8px;
            color:#2F3E1E; padding:16px 28px; display:inline-block;
            font-family:Inter,Segoe UI,Arial,sans-serif;
            border:1px solid #E8E8E4; border-radius:4px; background:#FDFDFB;'>
            " + codigo + @"
            </span>
            </td></tr>
            </table>
            " + NotaRodape("Código válido por <strong>10 minutos</strong>.",
            "Se não tentou fazer login, ignore este email.");

            Enviar(destinatario, assunto, HtmlBase(conteudo));
        }

        // ═══════════════════════════════════════════════════════
        // EnviarResetPassword
        // ═══════════════════════════════════════════════════════
        public static void EnviarResetPassword(string destinatario, string nomeUtilizador, string token)
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            string link = baseUrl + "/reset_password.aspx?token=" + token;

            string assunto = "Quinta da Azenha — Recuperar palavra-passe";
            string conteudo = @"
            <p style='margin:0 0 16px; font-size:15px; color:#2F3E1E; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            Olá <strong>" + nomeUtilizador + @"</strong>,</p>
            <p style='color:#6B7A5C; line-height:1.7; font-size:13px; margin:0 0 8px;'>
            Recebemos um pedido para redefinir a palavra-passe da sua conta.<br/>
            Clique no botão abaixo para criar uma nova:</p>
            " + BotaoCTA("Redefinir palavra-passe", link) +
            NotaRodape("Este link expira em <strong>1 hora</strong>.",
            "Se não fez este pedido, ignore este email.");

            Enviar(destinatario, assunto, HtmlBase(conteudo));
        }

        // ═══════════════════════════════════════════════════════
        // EnviarConfirmacaoReserva
        // Layout baseado no confirmacao_reserva_template.pdf
        // ═══════════════════════════════════════════════════════
        public static void EnviarConfirmacaoReserva(string destinatario, string nomeCliente,
                                                     string experiencia, DateTime data,
                                                     int numPessoas, decimal precoTotal,
                                                     string numReserva, byte[] pdfBytes)
        {
            string assunto = "Quinta da Azenha - Confirmação de Reserva " + numReserva;

            // Separador com o numero de reserva
            string resumoReserva = @"
            <!-- Numero de reserva em destaque -->
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='margin-bottom:20px;'>
            <tr>
            <td style='font-size:10px; text-transform:uppercase; letter-spacing:0.18em; color:#6B7A5C; font-family:Inter,Segoe UI,Arial,sans-serif;'>Nº Reserva</td>
            <td style='text-align:right; font-size:15px; font-weight:600; color:#2F3E1E; font-family:Consolas,monospace,Inter,sans-serif; letter-spacing:0.05em;'>" + numReserva + @"</td>
            </tr>
            <tr><td colspan='2' style='height:1px; background:#E8E8E4; padding-top:8px;'><div style='height:1px; background:#E8E8E4;'></div></td></tr>
            </table>
            <!-- Dados da reserva — tabela limpa, como no template PDF -->
            <table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='margin-bottom:16px;'>
            <tr>
            <td style='padding:8px 0; font-size:12px; color:#6B7A5C; font-family:Inter,Segoe UI,Arial,sans-serif;'>Experiência</td>
            <td style='padding:8px 0; font-size:13px; color:#2F3E1E; text-align:right; font-weight:500; font-family:Inter,Segoe UI,Arial,sans-serif;'>" + experiencia + @"</td>
            </tr>
            <tr><td colspan='2' style='height:1px; background:#F0F0EC;'><div style='height:1px; background:#F0F0EC;'></div></td></tr>
            <tr>
            <td style='padding:8px 0; font-size:12px; color:#6B7A5C; font-family:Inter,Segoe UI,Arial,sans-serif;'>Data</td>
            <td style='padding:8px 0; font-size:13px; color:#2F3E1E; text-align:right; font-weight:500; font-family:Inter,Segoe UI,Arial,sans-serif;'>" + data.ToString("dd/MM/yyyy") + @"</td>
            </tr>
            <tr><td colspan='2' style='height:1px; background:#F0F0EC;'><div style='height:1px; background:#F0F0EC;'></div></td></tr>
            <tr>
            <td style='padding:8px 0; font-size:12px; color:#6B7A5C; font-family:Inter,Segoe UI,Arial,sans-serif;'>Participantes</td>
            <td style='padding:8px 0; font-size:13px; color:#2F3E1E; text-align:right; font-weight:500; font-family:Inter,Segoe UI,Arial,sans-serif;'>" + numPessoas + @"</td>
            </tr>
            <tr><td colspan='2' style='height:1px; background:#F0F0EC;'><div style='height:1px; background:#F0F0EC;'></div></td></tr>
            <!-- Total  -->
            <tr>
            <td style='padding:14px 0 0; font-size:12px; color:#6B7A5C; font-family:Inter,Segoe UI,Arial,sans-serif;'>Total</td>
            <td style='padding:14px 0 0; font-size:18px; color:#4A7C2F; text-align:right; font-weight:600; font-family:Inter,Segoe UI,Arial,sans-serif;'>" + precoTotal.ToString("0.00") + @" €</td>
            </tr>
            </table>";

            string conteudo = @"
            <p style='margin:0 0 16px; font-size:15px; color:#2F3E1E; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            Olá <strong>" + nomeCliente + @"</strong>,</p>
            <p style='color:#6B7A5C; line-height:1.7; font-size:13px; margin:0 0 4px;'>
            O seu pedido de reserva foi recebido com sucesso.<br/>
            A confirmação em PDF encontra-se em anexo a este email.</p>
            " + resumoReserva + @"
            <p style='color:#6B7A5C; font-size:12px; line-height:1.6; margin-top:16px;'><br/>
            Em caso de dúvida: <a href='mailto:info.quintadaazenha@gmail.com' style='color:#4A7C2F; text-decoration:none; font-weight:500;'>info.quintadaazenha@gmail.com</a>
            </p>";

            // Envio com anexo PDF (duplicado aqui para nao depender da classe Email original)
            SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SmtpHost"]);
            smtp.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
            smtp.Credentials = new NetworkCredential(
                ConfigurationManager.AppSettings["SmtpUser"],
                ConfigurationManager.AppSettings["SmtpPassword"]
            );
            smtp.EnableSsl = true;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["SmtpFrom"]);
            msg.To.Add(destinatario);
            msg.Subject = assunto;
            msg.Body = HtmlBase(conteudo);
            msg.IsBodyHtml = true;

            try
            {
                if (pdfBytes != null && pdfBytes.Length > 0)
                {
                    System.IO.MemoryStream ms = new System.IO.MemoryStream(pdfBytes);
                    msg.Attachments.Add(new Attachment(ms, "reserva_" + numReserva + ".pdf", "application/pdf"));
                }
                smtp.Send(msg);
            }
            catch (SmtpException ex)
            {
                System.Diagnostics.Debug.WriteLine("Erro ao enviar email de confirmação: " + ex.Message);
            }
            finally
            {
                msg.Dispose();
                smtp.Dispose();
            }
        }

        // ═══════════════════════════════════════════════════════
        // EnviarBoasVindas
        // Sem fundo escuro, sem password a amarelo — card limpo
        // ═══════════════════════════════════════════════════════
        public static void EnviarBoasVindas(string destinatario, string nomeCliente)
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            string assunto = "Quinta da Azenha — Bem-vindo à sua área pessoal";

            // Credenciais em tabela
            string credenciais = SeparadorSeccao("Credenciais de acesso") + @"
            <table role='presentation' width='100%' cellpadding='0' cellspacing='0'>
            <tr>
            <td style='padding:6px 0; font-size:13px; color:#6B7A5C; font-family:Inter,Segoe UI,Arial,sans-serif;'>Email</td>
            <td style='padding:6px 0; font-size:13px; color:#2F3E1E; text-align:right; font-weight:500; font-family:Inter,Segoe UI,Arial,sans-serif;'>" + destinatario + @"</td>
            </tr>
            <tr><td colspan='2' style='height:1px; background:#F0F0EC;'><div style='height:1px; background:#F0F0EC;'></div></td></tr>
            <tr>
            <td style='padding:8px 0; font-size:13px; color:#6B7A5C; font-family:Inter,Segoe UI,Arial,sans-serif;'>Password temporária</td>
            <td style='padding:8px 0; font-size:13px; color:#2F3E1E; text-align:right; font-weight:600; font-family:Consolas,monospace,Inter,sans-serif; letter-spacing:0.04em;'>Raizes2026!</td>
            </tr>
            </table>";

            string conteudo = @"
            <p style='margin:0 0 16px; font-size:15px; color:#2F3E1E; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            Olá <strong>" + nomeCliente + @"</strong>,</p>
            <p style='color:#6B7A5C; line-height:1.7; font-size:13px; margin:0 0 4px;'>
            A sua reserva foi criada e, com ela, a sua conta pessoal na <strong>Quinta da Azenha</strong>.<br/>
            Na sua área pessoal pode consultar reservas, pontos de fidelização e favoritos.</p>
            " + credenciais + @"
            <!-- Nota de seguranca -->
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='margin:16px 0 0;'>
            <tr><td style='font-size:11px; color:#C9A84C; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            Recomendamos que altere a password após o primeiro acesso.
            </td></tr>
            </table>
            " + BotaoCTA("Aceder à minha conta", baseUrl + "/Pages/conta_login.aspx");

            Enviar(destinatario, assunto, HtmlBase(conteudo));
        }

        // ═══════════════════════════════════════════════════════
        // EnviarResetPasswordCliente
        // ═══════════════════════════════════════════════════════
        public static void EnviarResetPasswordCliente(string destinatario, string nomeCliente, string token)
        {
            string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];
            string link = baseUrl + "/Pages/conta_reset_password.aspx?token=" + token;

            string assunto = "Quinta da Azenha — Redefinir password";
            string conteudo = @"
            <p style='margin:0 0 16px; font-size:15px; color:#2F3E1E; font-family:Inter,Segoe UI,Arial,sans-serif;'>
            Olá <strong>" + nomeCliente + @"</strong>,</p>
            <p style='color:#6B7A5C; line-height:1.7; font-size:13px; margin:0 0 8px;'>
            Recebemos um pedido para redefinir a password da sua conta.<br/>
            Clique no botão abaixo para criar uma nova:</p>
            " + BotaoCTA("Redefinir password", link) +
            NotaRodape("Este link expira em <strong>1 hora</strong>.",
            "Se não fez este pedido, ignore este email.");

            Enviar(destinatario, assunto, HtmlBase(conteudo));
        }
    }
}
