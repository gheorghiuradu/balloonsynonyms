using KinderPlay.Extensions;
using KinderPlay.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace KinderPlay.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;
        private List<Game> games;

        public HomeController(IConfiguration config)
        {
            this.configuration = config;

            var json = System.IO.File.ReadAllText("games.json".ToAppPath());
            this.games = JsonConvert.DeserializeObject<List<Game>>(json);
        }

        public IActionResult Index()
        {
            return View(this.games);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Play(string slug)
        {
            var game = this.games.Find(g => g.Slug == slug);

            if (game is null)
                return this.NotFound();

            return View(game);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageVM message)
        {
            using (var client = new SmtpClient(this.configuration["SES:Server"], 587))
            {
                client.EnableSsl = true;
                client.Credentials =
                    new NetworkCredential(this.configuration["SES:User"], this.configuration["SES:Password"]);

                var mail = new MailMessage
                {
                    From = new MailAddress(this.configuration["SES:ToAddress"]),
                    Subject = $"{message.Subject} - Contact Form",
                    Body = message.Text
                };
                mail.ReplyToList.Add(message.From);
                mail.To.Add(this.configuration["SES:ToAddress"]);

                await client.SendMailAsync(mail);
            }

            return RedirectToAction(nameof(this.ThankYouForMessage));
        }

        public IActionResult ThankYouForMessage()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}