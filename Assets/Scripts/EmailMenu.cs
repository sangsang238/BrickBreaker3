using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using TMPro;

public class EmailMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField email_InputField;
    [SerializeField] private TMP_InputField password_InputField;
    [SerializeField] private TMP_InputField subject_InputField;
    [SerializeField] private TMP_InputField body_InputField;
    // in4 recipient:
    string recipientEmail = "sang23820011@gmail.com";

    // btn Send licked:
    public void SendEmail()
    {
        // Sender's Email & Password:
        string senderEmail = email_InputField.text;
        string senderPassword = password_InputField.text;
        // Subject & Message:
        string emailSubject = subject_InputField.text;
        string emailBody = body_InputField.text;

        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(senderEmail);
        mail.To.Add(recipientEmail);
        mail.Subject = emailSubject;
        mail.Body = emailBody;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential(senderEmail, senderPassword);
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };

        try
        {
            smtpServer.Send(mail);
            Debug.Log("Email sent successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to send email: " + e.Message);
        }
    }
}