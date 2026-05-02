
using Microsoft.Extensions.Configuration;
using MovieTicketAPI.Application.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace MovieTicketAPI.Infrastructure
{
    public class MailService : IMailService
    {
        readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendMailAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            // 1. Mailin Taslağını Oluşturma (MimeMessage)
            var message = new MimeMessage();

            // Kimden
            message.From.Add(new MailboxAddress("BiletiniAl", _configuration["Mail:Username"]));

            // Kime (Çoklu eklenti)
            foreach (var to in tos)
            {
                message.To.Add(MailboxAddress.Parse(to));
            }

            // Konu
            message.Subject = subject;

            // İçerik (BodyBuilder ile HTML veya Düz Metin ayarı)
            var bodyBuilder = new BodyBuilder();
            if (isBodyHtml)
                bodyBuilder.HtmlBody = body;
            else
                bodyBuilder.TextBody = body;

            message.Body = bodyBuilder.ToMessageBody();

            // 2. Maili Gönderme İşlemi (MailKit SmtpClient)
            // using bloğu işimiz bitince postacıyı bellekten temizler
            using var smtp = new SmtpClient();

            // Sunucuya bağlan (587 portu ve StartTLS güvenliği ile)
            await smtp.ConnectAsync(_configuration["Mail:Host"], 587, SecureSocketOptions.StartTls);

            // Kimlik doğrulaması yap
            await smtp.AuthenticateAsync(_configuration["Mail:Username"], _configuration["Mail:Password"]);

            // Maili gönder
            await smtp.SendAsync(message);

            // Bağlantıyı güvenli bir şekilde kapat
            await smtp.DisconnectAsync(true);
        }
        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            StringBuilder mail = new();
            mail.AppendLine("Merhaba<br>Eğer yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirsiniz.<br><strong><a target=\"_blank\" href=\"");
            mail.AppendLine(_configuration["AngularClientUrl"]);
            mail.AppendLine("/update-password/");
            mail.AppendLine(userId);
            mail.AppendLine("/");
            mail.AppendLine(resetToken);
            mail.AppendLine("\">Yeni şifre talebi için tıklayınız...</a></strong><br><br><span style=\"font-size:12px;\">NOT : Eğer ki bu talep tarafınızca gerçekleştirilmemişse lütfen bu maili ciddiye almayınız.</span><br>Saygılarımızla...<br><br><br>NG - Mini|E-Ticaret");

            await SendMailAsync(to, "Şifre Yenileme Talebi", mail.ToString());
        }

        public async Task SendCompletedOrderMailAsync(string to, string orderCode, DateTime orderDate, string userName)
        {
            string mail = $"Sayın {userName} Merhaba<br>" +
                $"{orderDate} tarihinde vermiş olduğunuz {orderCode} kodlu siparişiniz tamamlanmış ve kargo firmasına verilmiştir.<br>Hayrını görünüz efendim...";

            await SendMailAsync(to, $"{orderCode} Sipariş Numaralı Siparişiniz Tamamlandı", mail);

        }
    }


}

