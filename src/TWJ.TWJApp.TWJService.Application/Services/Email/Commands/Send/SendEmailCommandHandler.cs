using MediatR;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System;
using TWJ.TWJApp.TWJService.Application.Interfaces;
using TWJ.TWJApp.TWJService.Application.Services.Base;
using TWJ.TWJApp.TWJService.Application.Services.Email.Commands.Send;
using System.Linq;
using TWJ.TWJApp.TWJService.Application.Dto.Models;
using Microsoft.Extensions.Configuration;
using TWJ.TWJApp.TWJService.Application.Helpers.Interfaces;
using HtmlAgilityPack;

public class SendEmailCommandHandler : BaseService, IRequestHandler<SendEmailCommand, Response<bool>>
{
    private readonly ITWJAppDbContext _context;
    private readonly IOpenAiService _openAiService;
    private readonly string _SMTPUserName;
    private readonly string _SMTPPassword;
    private readonly string _SMTPHost;
    private readonly string _senderName;
    private readonly string _senderEmail;
    private readonly string _webURL;
    private readonly int _SMTPPort;

    public SendEmailCommandHandler(ITWJAppDbContext context, IConfiguration configuration, IGlobalHelperService globalHelperService, IOpenAiService openAiService) : base(configuration, globalHelperService, openAiService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _SMTPUserName = _configuration["Amazon:SES:SMTP:SMTPUserName"];
        _SMTPPassword = _configuration["Amazon:SES:SMTP:SMTPPassword"];
        _senderName = _configuration["Amazon:SES:SMTP:SenderName"];
        _senderEmail = _configuration["Amazon:SES:SMTP:SenderEmail"];
        _SMTPHost = _configuration["Amazon:SES:SMTP:Host"];
        _webURL = _configuration["AppSettings:WebURL"];
        _SMTPPort = int.Parse(_configuration["Amazon:SES:SMTP:Port"]);
        className = GetType().Name;
        _openAiService = openAiService;
    }

    public async Task<Response<bool>> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var apiDirectory = GetApiDirectory();
            var templatePath = Path.Combine(apiDirectory, "Email Templates", "Newsletter", "email-template-1.html");
            var emailTemplate = await File.ReadAllTextAsync(templatePath);

            emailTemplate = emailTemplate.Replace("{year}", DateTime.Now.Year.ToString());
            var response = new Response<bool>
            {
                Data = false
            };

            var latestPost = _context.BlogPosts
                .Where(x => x.Published == true)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefault();

            if (latestPost == null)
            {
                response.Message = "No blog posts found.";
                return response;
            }

            using (var smtpClient = new SmtpClient(_SMTPHost, _SMTPPort))
            {
                smtpClient.Credentials = new NetworkCredential(_SMTPUserName, _SMTPPassword);
                smtpClient.EnableSsl = true;

                var subscribers = _context.NewsLetterSubscribers
                    .Where(x => x.Subscribed == true)
                    .ToList();

                if (subscribers == null || !subscribers.Any())
                {
                    response.Message = "No subscribers found.";
                    return response;
                }

                int emailsSent = 0;
                const int maxEmailsPerSecond = 10;

                foreach (var subscriber in subscribers)
                {
                    var email = subscriber.Email?.Trim();

                    if (!_IsValidEmail(email))
                    {
                        continue;
                    }

                    var unsubscribeLink = _webURL.Replace("post", "email-unsubscribe") + $"{subscriber.Id}";

                    var personalizedTemplate = emailTemplate.Replace("{unsubscribe_link}", unsubscribeLink);

                    string updatedTemplate = InsertContentIntoTemplate(personalizedTemplate, latestPost.Content, latestPost.Image);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_senderEmail, _senderName),
                        Subject = "Latest Health News",
                        Body = updatedTemplate,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(new MailAddress(email));

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
                        emailsSent++;

                        if (emailsSent % maxEmailsPerSecond == 0)
                        {
                            await Task.Delay(1000, cancellationToken);  // Pause for 1 second every 10 emails
                        }
                    }
                    catch (Exception ex)
                    {
                        await _globalHelperService.Log(ex, className);
                    }
                }
            }

            response.Data = true;
            response.Success = true;
            response.Message = "Emails sent successfully.";
            return response;
        }
        catch (Exception ex)
        {
            await _globalHelperService.Log(ex, className);
            throw new ApplicationException("An error occurred while sending emails: ", ex);
        }
    }


    private string InsertContentIntoTemplate(string template, string content, string imageUrl)
    {
        var contentDocument = new HtmlDocument();
        contentDocument.LoadHtml(content);

        var nodesToRemove = contentDocument.DocumentNode.SelectNodes("//*[@class='yu5kvzao1k']");
        if (nodesToRemove != null)
        {
            foreach (var node in nodesToRemove)
            {
                node.Remove();
            }
        }

        var nodesToRemove2 = contentDocument.DocumentNode.SelectNodes("//*[@class='pJDcMLp9xo']");
        if (nodesToRemove != null)
        {
            foreach (var node in nodesToRemove2)
            {
                node.Remove();
            }
        }

        var nodesToRemove3 = contentDocument.DocumentNode.SelectNodes("//*[@class='sQxpAj1VQF']");
        if (nodesToRemove != null)
        {
            foreach (var node in nodesToRemove3)
            {
                node.Remove();
            }
        }

        var cleanedContent = contentDocument.DocumentNode.OuterHtml;

        var templateDocument = new HtmlDocument();
        templateDocument.LoadHtml(template);

        var emailContainer = templateDocument.GetElementbyId("email-body");
        if (emailContainer != null)
        {
            emailContainer.InnerHtml = cleanedContent;
        }

        var blogPostImageContainer = templateDocument.GetElementbyId("blog-post-image");
        if (blogPostImageContainer != null)
        {
            var imageTag = $"<img src=\"{imageUrl}\" style=\"width: 100%;max-height:500px;\" alt=\"Blog Post Image\" />";
            blogPostImageContainer.InnerHtml = imageTag;
        }

        return templateDocument.DocumentNode.OuterHtml;
    }


    private bool _IsValidEmail(string email)
    {
        try
        {
            email = email?.Trim();

            if (string.IsNullOrEmpty(email))
                return false;

            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch (FormatException)
        {
            return false;
        }
    }


}
