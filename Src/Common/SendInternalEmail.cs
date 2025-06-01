/*
 * Copyright 2021 Sheakley Corporation.  All Rights Reserved.
 *
 * You may not copy, use, alter or otherwise utilize any of the code contained in this file
 * without the express written consent of the Sheakley Corporation.
 */
//using Microsoft.Extensions.Logging;

// TODO this method needs some hardcoding removed with variables and config settings

using SlugEnt.FluentResults;
using System.Net.Mail;

namespace SlugEnt.HR.NextGen.Common;

/// <summary>
///     Wrapper around the MailMessage 
/// </summary>
public class SendInternalEmail
{
    private readonly MailMessage _mailMessage = new();
    private static string _overrideEmailRecipient = "";
//        private ILogger<SendInternalEmail> _logger;


    /// <summary>
    ///     Constructor
    /// </summary>
    public SendInternalEmail() //ILogger<SendInternalEmail> logger)
    {
        //_logger = logger;
        _mailMessage.From = new MailAddress("NoReply@somedomain.com", "No Reply");
    }


    /// <summary>
    /// Whether the email is standard text or HTMl.
    /// </summary>
    public bool IsHTMLBody { get; set; } = true;


    /// <summary>
    ///     This allows the program to set an override recipient (for testing purposes).  Every single report produced
    ///     by this application will have its report sent to this recipient, instead of the actual recipients requested
    ///     by each report.  If not set, then the actual recipient list will be used.
    /// </summary>
    /// <param name="recipient">The recipient email address to send emails to.  Must be full email address</param>
    public static void OverrideEmailRecipient(string recipient)
    {
        _overrideEmailRecipient = recipient;
    }


    /// <summary>
    ///     Who the email is from.
    /// </summary>
    public MailAddress From
    {
        get => _mailMessage.From;
        set => _mailMessage.From = value;
    }


    /// <summary>
    ///     Subject of Email
    /// </summary>
    public string Subject
    {
        get => _mailMessage.Subject;
        set => _mailMessage.Subject = value;
    }


    /// <summary>
    ///     Body of Email
    /// </summary>
    public string Body
    {
        get => _mailMessage.Body;
        set => _mailMessage.Body = value;
    }


    /// <summary>
    ///     Add a recipient
    /// </summary>
    /// <param name="emailRecipient"></param>
    public void AddRecipient(string emailRecipient)
    {
        if (!emailRecipient.Contains("@"))
            emailRecipient += "@domain.com";
        _mailMessage.To.Add(emailRecipient);
    }


    /// <summary>
    ///     Sends the email
    /// </summary>
    public Result Send()
    {
        //SmtpClient smtpClient = new SmtpClient("mail.sheakley.com", 25);
        //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        try
        {
            _mailMessage.IsBodyHtml = IsHTMLBody;

            // If override recipient has been set, then delete the recipient list and add the override.
            if (_overrideEmailRecipient != string.Empty)
            {
                _mailMessage.To.Clear();
                _mailMessage.To.Add(_overrideEmailRecipient);

                _mailMessage.Bcc.Clear();
                _mailMessage.CC.Clear();
            }

            var smtpClient = new SmtpClient("mx.sgoc.local")
            {
                Port = 25,
                //Credentials = new NetworkCredential("username", "password"),
                //   EnableSsl = true,
            };


            smtpClient.Send(_mailMessage);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail(new ExceptionalError($"Failed to send email error: {ex.Message}",ex));
        }
    }
}