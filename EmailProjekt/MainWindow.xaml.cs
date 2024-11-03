using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace EmailProjekt
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary> 
    public class EmailAttachment
    {
        public string FilePath { get; set; }
        public Attachment Attachment { get; set; }

        public EmailAttachment(string filePath)
        {
            FilePath = filePath;
            Attachment = new Attachment(filePath);
        }
    }

    public class EmailSender
    {
        private SmtpClient SmtpClient = new SmtpClient("smtp.gmail.com");
        private MailAddress mailSender = new MailAddress("denickyyy.social@gmail.com");
        public List<MailAddress> mailReceivers = new List<MailAddress>();
        public List<EmailAttachment> attachments = new List<EmailAttachment>();
        public int ReceiverCount => mailReceivers.Count;
        public int AttachmentCount => attachments.Count;

        public EmailSender()
        {
            SmtpClient.Port = 587;
            SmtpClient.Credentials = new NetworkCredential("denickyyy.social@gmail.com", "ewzy qcjq viro valp");
            SmtpClient.EnableSsl = true;
        }

        public void AddReceiver(string receiver)
        {
            try
            {
                var newReceiver = new MailAddress(receiver);

                if (mailReceivers.Any(existingReceiver => existingReceiver.Address == newReceiver.Address))
                {
                    var duplicateMessageBox = new Wpf.Ui.Controls.MessageBox
                    {
                        Title = "BŁĄD",
                        Content = "Ten adres e-mail jest już dodany",
                        CloseButtonAppearance = ControlAppearance.Secondary,
                        CloseButtonText = "Close"
                    };
                    duplicateMessageBox.ShowDialogAsync();
                    return;
                }
                else
                {
                    mailReceivers.Add(newReceiver);
                }

                
            }
            catch (FormatException)
            {
                var invalidMessageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "BŁĄD",
                    Content = "Niepoprawny format adresu e-mail",
                    CloseButtonAppearance = ControlAppearance.Secondary,
                    CloseButtonText = "Close"
                };
                invalidMessageBox.ShowDialogAsync();
            }
        }

        public void AddAttachment(string filePath)
        {
            var attachment = new EmailAttachment(filePath);
            attachments.Add(attachment);

            Console.WriteLine($"Dodano załącznik: {attachment.FilePath}. Aktualna liczba załączników: {attachments.Count}");
        }

        public async Task<bool> MessageAsync(string messageSubject, string messageBody)
        {
            try
            {
                foreach (var receiver in mailReceivers)
                {
                    MailMessage newMail = new MailMessage
                    {
                        From = mailSender,
                        Subject = messageSubject,
                        Body = messageBody
                    };

                    newMail.To.Add(receiver);

                    foreach (var emailAttachment in attachments)
                    {
                        newMail.Attachments.Add(emailAttachment.Attachment);
                    }

                    await SmtpClient.SendMailAsync(newMail);
                }

                foreach (var emailAttachment in attachments)
                {
                    emailAttachment.Attachment.Dispose();
                }
                attachments.Clear();
                mailReceivers.Clear();

                Console.WriteLine("Message sent, attachments and receivers cleared.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }



        public void ShowAttachments()
        {
            Console.WriteLine("Lista załączników:");
                foreach (var attachment in attachments)
                {
                    Console.WriteLine($"- {attachment.FilePath}");
                }
            Console.WriteLine($"Liczba załączników: {attachments.Count}");
        }
    }

    public partial class MainWindow : FluentWindow
    {
        private EmailSender emailSend;
        public MainWindow()
        {
            DataContext = this;

            Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                 Wpf.Ui.Appearance.ApplicationTheme.Dark,
                 Wpf.Ui.Controls.WindowBackdropType.Mica,
                 true
            );

            Globals.theme = 1;

            InitializeComponent();
            emailSend = new EmailSender();
        }

        public void CreateAttachmentBox(string fileName)
        {
            var card = new Card()
            {
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 5, 0),
                Tag = fileName
            };
            var sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0)
            };
            var symbol = new SymbolIcon()
            {
                Symbol = SymbolRegular.Document24
            };
            var label = new Label()
            {
                Content = fileName,
                Padding = new Thickness(0),
                Margin = new Thickness(5, 0, 0, 0),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Fonts/#Montserrat Medium"),
                FontSize = 12
            };
            var button = new Wpf.Ui.Controls.Button()
            {
                Icon = new SymbolIcon() { Symbol = SymbolRegular.Dismiss16 },
                Padding = new Thickness(1),
                Appearance = ControlAppearance.Secondary,
                Margin = new Thickness(10, 0, 0, 0),
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Tag = fileName
            };

            button.Click += (s, e) => DeleteAttachment(card, fileName);

            sp.Children.Add(symbol);
            sp.Children.Add(label);
            sp.Children.Add(button);
            card.Content = sp;
            zalacznikiPanel.Children.Add(card);
        }

        public void CreateReceiverBox(string receiver)
        {
            var card = new Card()
            {
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 5, 0),
                Tag = receiver
            };
            var sp = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0)
            };
            var symbol = new SymbolIcon()
            {
                Symbol = SymbolRegular.Person28
            };
            var label = new Label()
            {
                Content = receiver,
                Padding = new Thickness(0),
                Margin = new Thickness(5, 0, 0, 0),
                FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Fonts/#Montserrat Medium"),
                FontSize = 12
            };
            var button = new Wpf.Ui.Controls.Button()
            {
                Icon = new SymbolIcon() { Symbol = SymbolRegular.Dismiss16 },
                Padding = new Thickness(1),
                Appearance = ControlAppearance.Secondary,
                Margin = new Thickness(10, 0, 0, 0),
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                Tag = receiver
            };

            button.Click += (s, e) => DeleteReceiver(card, receiver);

            sp.Children.Add(symbol);
            sp.Children.Add(label);
            sp.Children.Add(button);
            card.Content = sp;
            adresaciPanel.Children.Add(card);
        }

        private void DeleteReceiver(Card card, string receiver)
        {
            var userToRemove = emailSend.mailReceivers.FirstOrDefault(r => r.Address == receiver);
            if (userToRemove != null)
            {
                emailSend.mailReceivers.Remove(userToRemove);
            }

            adresaciPanel.Children.Remove(card);
        }

        private void DeleteAttachment(Card card, string filePath)
        {
            var attachmentToRemove = emailSend.attachments
                .FirstOrDefault(a => a.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));

            if (attachmentToRemove != null)
            {
                emailSend.attachments.Remove(attachmentToRemove);
                attachmentToRemove.Attachment.Dispose();

                Console.WriteLine($"Załącznik '{filePath}' usunięty. Aktualna liczba załączników: {emailSend.attachments.Count}");
            }
            else
            {
                Console.WriteLine($"Załącznik '{filePath}' nie został znaleziony do usunięcia.");
            }

            emailSend.ShowAttachments();
            zalacznikiPanel.Children.Remove(card);
        }

        private void AddReceiverButtonClick(object sender, RoutedEventArgs e)
        {
            if (adresatField.Text.Contains("@"))
            {
                int originalCount = emailSend.ReceiverCount;

                emailSend.AddReceiver(adresatField.Text);

                if (emailSend.ReceiverCount > originalCount)
                {
                    CreateReceiverBox(adresatField.Text);
                    adresatField.Text = string.Empty;
                }
            }
        }

        private async void SendMessageButtonClick(object sender, RoutedEventArgs e)
        {
            if (emailSend.ReceiverCount == 0)
            {
                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "BŁĄD",
                    Content = "Nie wprowadzono adresata",
                    CloseButtonAppearance = ControlAppearance.Secondary,
                    CloseButtonText = "OK",
                };
                await messageBox.ShowDialogAsync();

                return;
            }

            if (string.IsNullOrWhiteSpace(tematField.Text))
            {
                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "BŁĄD",
                    Content = "Nie wprowadzono tematu wiadomości",
                    CloseButtonAppearance = ControlAppearance.Secondary,
                    CloseButtonText = "OK"
                };
                await messageBox.ShowDialogAsync();

                return;
            }

            if (string.IsNullOrWhiteSpace(wiadomoscField.Text) && emailSend.AttachmentCount == 0)
            {
                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "BŁĄD",
                    Content = "Nie wprowadzono treści wiadomości",
                    CloseButtonAppearance = ControlAppearance.Secondary,
                    CloseButtonText = "OK"
                };
                await messageBox.ShowDialogAsync();

                return;
            }

            progressBar.Visibility = Visibility.Visible;
            progressBar.IsIndeterminate = true;

            bool isSent = await emailSend.MessageAsync(tematField.Text, wiadomoscField.Text);

            progressBar.IsIndeterminate = false;
            progressBar.Visibility = Visibility.Hidden;

            if (isSent)
            {
                var snackbar = new Snackbar(snackbarPresenter)
                {
                    Title = "SUKCES!",
                    Content = "Wysłano wiadomość",
                    Appearance = ControlAppearance.Success,
                    Icon = new SymbolIcon() { Symbol = SymbolRegular.MailCheckmark16 },
                };

                await snackbar.ShowAsync();

                adresaciPanel.Children.Clear();
                zalacznikiPanel.Children.Clear();
                tematField.Text = string.Empty;
                wiadomoscField.Text = string.Empty;
            }
            else
            {
                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "BŁĄD",
                    Content = "Wystąpił błąd podczas wysyłania wiadomości",
                    CloseButtonAppearance = ControlAppearance.Secondary,
                    CloseButtonText = "OK"
                };
                await messageBox.ShowDialogAsync();
            }
        }

        private void MenuThemeChangeButtonClick(object sender, RoutedEventArgs e)
        {
            switch (Globals.theme)
            {
                case 1:
                    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                    Wpf.Ui.Appearance.ApplicationTheme.Light,
                    Wpf.Ui.Controls.WindowBackdropType.Mica,
                    true
                    );
                    Globals.theme = 2;
                    break;
                case 2:
                    Wpf.Ui.Appearance.ApplicationThemeManager.Apply(
                    Wpf.Ui.Appearance.ApplicationTheme.Dark,
                    Wpf.Ui.Controls.WindowBackdropType.Mica,
                    true
                    );
                    Globals.theme = 1;
                    break;
            }
        }

        private void AddAttachmentButtonClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    emailSend.AddAttachment(fileName);
                    CreateAttachmentBox(fileName);
                }

                var messageBox = new Wpf.Ui.Controls.MessageBox
                {
                    Title = "Załączniki",
                    Content = "Dodano załącznik(i) do wiadomości.",
                    CloseButtonAppearance = ControlAppearance.Primary,
                    CloseButtonText = "OK"
                };
                messageBox.ShowDialogAsync();
            }
        }
    }
}
