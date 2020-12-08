using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Utils
{
    public class SendmailProcessing
    {
        #region Properties
        private string SMTP;
        private int Port;
        private bool IsSSL;
        private string AccountName;
        private string AccountUser;
        private string AccountPassword;
        private string EmailReceipt;

        private string[] EmailTo;
        private string[] EmailCC;
        private string[] EmailBCC;
        private string Subject;
        private string Content;
        private string[] FileList;

        MailMessage message;
        ToolFolder folder;
        ToolDAO toolDAO;
        #endregion

        #region Method Contruction
        public SendmailProcessing(string _SMTP, int _Port, bool _IsSSL, 
            string _AccountName, string _EmailReceipt, string _AccountUser, string _AccountPassword,
            string[] _EmailTo, string[] _EmailCC, string[] _EmailBCC,
            string _Subject, string _Content, string[] _FileList)
        {
            SMTP = _SMTP; Port = _Port; IsSSL = _IsSSL;
            AccountName = _AccountName; AccountUser = _AccountUser; AccountPassword = _AccountPassword; EmailReceipt = _EmailReceipt;
            EmailTo = _EmailTo; EmailCC = _EmailCC; EmailBCC = _EmailBCC;
            Subject = _Subject; Content = _Content; FileList = _FileList;
            folder = new ToolFolder();
            toolDAO = new ToolDAO();
            folder.WriteLogService("Start mail [" + Subject + "] processing!");
        }
        #endregion

        #region Method
        public bool EmailSending()
        {
            bool IsSendOk = false;
            try
            {
                folder.WriteLogService("Send [Subject: " + Subject + " Content:" + Content + "]");
                message = new MailMessage();
                message.Subject = Subject;
                message.Body = Content;
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                folder.WriteLogService("Send [From: " + AccountUser + " Name:" + AccountName + "]");
                message.From = new MailAddress(AccountUser, AccountName);
                try
                {
                    folder.WriteLogService("Sender [" + EmailReceipt + " " + AccountName + "]");
                    message.Sender = new MailAddress(EmailReceipt, AccountName);
                }
                catch
                {
                    message.Sender = new MailAddress(AccountUser, AccountName);
                    folder.WriteLogService("Sender [" + EmailReceipt + " " + AccountName + "] fail");
                }

                if (!EmailToSet()) return false; // ko co email to tu choi
                EmailCCSet();
                EmailBCCSet();
                AttachmentSet();

                SmtpClient client = new SmtpClient(SMTP, Port); //Gmail smtp    
                NetworkCredential basicCredential1 = new NetworkCredential(AccountUser, AccountPassword);
                client.EnableSsl = IsSSL;
                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;

                client.Send(message);

                IsSendOk = true;
                folder.WriteLogService("Sendmail [" + Subject + "] - OK");
            }
            catch(Exception ex)
            {
                IsSendOk = false;
                folder.WriteLogService("Sendmail [" + Subject + "] - Error: " + ex.ToString());
            }
            return IsSendOk;
        }
        #endregion

        #region Private Method
        private bool EmailToSet()
        {
            bool IsAddOk = false;
            if (EmailTo != null)
            {
                if (EmailTo.Length > 0)
                {
                    for (int i = 0; i < EmailTo.Length; i++)
                    {
                        try
                        {
                            message.To.Add(new MailAddress(EmailTo[i]));
                            IsAddOk = true;
                            folder.WriteLogService("Sendmail [" + Subject + "] Add mail-to: " + EmailTo[i]);
                        }
                        catch (Exception ex)
                        {
                            folder.WriteLogService("Sendmail [" + Subject + "] Add mail-to: " + EmailTo[i] + "; Error: " + ex.ToString());
                        }
                    }
                }
            }
            return IsAddOk;
        }
        private bool EmailCCSet()
        {
            bool IsAddOk = false;
            if (EmailCC != null)
            {
                if (EmailCC.Length > 0)
                {
                    for (int i = 0; i < EmailCC.Length; i++)
                    {
                        try
                        {
                            message.CC.Add(new MailAddress(EmailCC[i]));
                            IsAddOk = true;
                            folder.WriteLogService("Sendmail [" + Subject + "] Add mail-cc: " + EmailCC[i]);
                        }
                        catch (Exception ex)
                        {
                            folder.WriteLogService("Sendmail [" + Subject + "] Add mail-cc: " + EmailCC[i] + "; Error: " + ex.ToString());
                        }
                    }
                }
            }
            return IsAddOk;
        }
        private bool EmailBCCSet()
        {
            bool IsAddOk = false;
            if (EmailBCC != null)
            {
                if (EmailBCC.Length > 0)
                {
                    for (int i = 0; i < EmailBCC.Length; i++)
                    {
                        try
                        {
                            message.Bcc.Add(new MailAddress(EmailBCC[i]));
                            IsAddOk = true;
                            folder.WriteLogService("Sendmail [" + Subject + "] Add mail-bcc: " + EmailBCC[i]);
                        }
                        catch (Exception ex)
                        {
                            folder.WriteLogService("Sendmail [" + Subject + "] Add mail-bcc: " + EmailBCC[i] + "; Error: " + ex.ToString());
                        }
                    }
                }
            }
            return IsAddOk;
        }
        private bool AttachmentSet()
        {
            bool IsAddOk = false;
            if (FileList != null)
            {
                if (FileList.Length > 0)
                {
                    for (int i = 0; i < FileList.Length; i++)
                    {
                        try
                        {
                            message.Attachments.Add(new Attachment(FileList[i]));
                            IsAddOk = true;
                            folder.WriteLogService("Sendmail [" + Subject + "] Attachment-file: " + FileList[i]);
                        }
                        catch (Exception ex)
                        {
                            folder.WriteLogService("Sendmail [" + Subject + "] Attachment-file: " + FileList[i] + "; Error: " + ex.ToString());
                        }
                    }
                }
            }
            return IsAddOk;
        }
        #endregion
    }
}
