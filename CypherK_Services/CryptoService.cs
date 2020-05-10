using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CypherK_Services
{
    public class CryptoService
    {
        private readonly byte[] _IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private readonly int _blockSize = 128;

        private static readonly Dictionary<char, string> _baconDictionary = new Dictionary<char, string> {
            {'a', "AAAAA" }, {'b', "AAAAB" }, {'c', "AAABA" }, {'d', "AAABB" }, {'e', "AABAA" },
            {'f', "AABAB" }, {'g', "AABBA" }, {'h', "AABBB" }, {'i', "ABAAA" }, {'j', "ABAAB" },
            {'k', "ABABA" }, {'l', "ABABB" }, {'m', "ABBAA" }, {'n', "ABBAB" }, {'o', "ABBBA" },
            {'p', "ABBBB" }, {'q', "BAAAA" }, {'r', "BAAAB" }, {'s', "BAABA" }, {'t', "BAABB" },
            {'u', "BABAA" }, {'v', "BABAB" }, {'w', "BABBA" }, {'x', "BABBB" }, {'y', "BBAAA" },
            {'z', "BBAAB" }, {' ', "BBBAA" }
        };

        public async Task<string> DecryptAsync(string text, string key, Algorithm selectedAlgorithm = Algorithm.undefined)
        {
            string ret;
            if (selectedAlgorithm == Algorithm.undefined)
            {
                selectedAlgorithm = await DetectUsedAlgorithmAsync(text);
            }

            switch (selectedAlgorithm)
            {
                case Algorithm.cezar:
                    ret = await DecryptCezarAsync(text, key);
                    break;
                case Algorithm.bacon:
                    ret = await DecryptBaconAsync(text, key);
                    break;
                case Algorithm.aes:
                    ret = await DecryptAesAsync(text, key);
                    break;
                default:
                    ret = "Nie rozpoznano użytego algorytmu szyfrującego.";
                    break;
            }

            return ret;
        }

        public async Task<Algorithm> DetectUsedAlgorithmAsync(string cypherText)
        {
            if (await CanBeBacon(cypherText))
            {
                return Algorithm.bacon;
            }
            else if (await CanBeCezar(cypherText))
            {
                return Algorithm.cezar;
            }
            else if(await CanBeAes(cypherText))
            {
                return Algorithm.aes;
            }
            return Algorithm.undefined;
        }


        public async Task<bool> CanBeBacon(string cypherText)
        {
            return true;
        }
        public async Task<bool> CanBeCezar(string cypherText)
        {
            return true;
        }
        public async Task<bool> CanBeAes(string cypherText)
        {
            return true;
        }



        //BACON
        public async Task<string> DecryptBaconAsync(string text, string key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in text)
            {
                if ('a' <= c && c <= 'z') sb.Append('A');
                else if ('A' <= c && c <= 'Z') sb.Append('B');
            }
            string et = sb.ToString();
            sb.Length = 0;
            for (int i = 0; i < et.Length; i += 5)
            {
                string quintet = et.Substring(i, 5);
                char k = _baconDictionary.Where(a => a.Value == quintet).First().Key;
                sb.Append(k);
            }
            return sb.ToString();
        }
        public async Task<string> EncryptBaconAsync(string text, string key)
        {
            string pt = text.ToLower();
            StringBuilder sb = new StringBuilder();
            foreach (char c in pt)
            {
                if ('a' <= c && c <= 'z') sb.Append(_baconDictionary[c]);
                else sb.Append(_baconDictionary[' ']);
            }
            string et = sb.ToString();
            string mg = text.ToLower();  // 'A's to be in lower case, 'B's in upper case
            sb.Length = 0;
            int count = 0;
            foreach (char c in mg)
            {
                if ('a' <= c && c <= 'z')
                {
                    if (et[count] == 'A') sb.Append(c);
                    else sb.Append((char)(c - 32)); // upper case equivalent
                    count++;
                    if (count == et.Length) break;
                }
                else sb.Append(c);
            }

            return sb.ToString();
        }

        //CEZAR
        public async Task<string> DecryptCezarAsync(string text, string key)
        {
            return "";
        }
        public async Task<string> EncryptCezarAsync(string text, string key)
        {
            return "";
        }

        //AES
        public async Task<string> DecryptAesAsync(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            else if (string.IsNullOrEmpty(key))
            {
                return "Nie podano klucza deszyfrującego!";
            }

            //Decrypt
            byte[] bytes = Convert.FromBase64String(text);
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(key));
            crypt.IV = _IV;
            crypt.Padding = PaddingMode.Zeros;

            using (MemoryStream memoryStream = new MemoryStream(bytes))
            {
                using (CryptoStream cryptoStream =
                   new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[bytes.Length];
                    cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    return Encoding.Unicode.GetString(decryptedBytes);
                }
            }
        }
        public async Task<string> EncryptAesAsync(string text, string key)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            else if (string.IsNullOrEmpty(key))
            {
                return "Nie podano klucza szyfrującego!";
            }

            byte[] bytes = Encoding.Unicode.GetBytes(text);
            //Encrypt
            SymmetricAlgorithm crypt = Aes.Create();
            HashAlgorithm hash = MD5.Create();
            crypt.BlockSize = _blockSize;
            crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(key));
            crypt.IV = _IV;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream =
                   new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }




        public async Task<bool> SendEmailMessage(string text, string receiver)
        {
            string[] recipients = { receiver };
            
            return await Task.Run(() => SendEmailAsync("Wiadomość z portalu CypherK", text, recipients));
            
            //return true;
        }

        public async Task<bool> SendEmailLink(string text, string receiver)
        {
            // zapisanie do bazy
            // wygenerowanie linku
            // utworzenie wiadomości i linku
            // wysyłka
            string[] recipients = { receiver };
            return await SendEmailAsync("Wiadomość z portalu CypherK", text, recipients);
        }


        public async Task<bool> SendEmailAsync(string subject, string msg, string[] recipients)
        {
            recipients = recipients.Distinct().ToArray();// żeby nie powielać odbiorców jeżeli w aplikacji błędnie skonstruujemy listę odbiorców
            var body = msg; //TODO: szablon wiadomości email
            bool isSend = false;
            try
            {
                string from_addr = "cypherk@krzepicki.pl";
                string from_acc = "cypherk@krzepicki.pl";
                string from_pass = "";
                
                string smtp_host = "smtp.krzepicki.pl";
                string smtp_port = "25";

                using (var message = new MailMessage())
                {
                    foreach (var recipent in recipients)
                        message.To.Add(new MailAddress(recipent));
                  

                    message.From = new MailAddress(from_addr);
                    message.Subject = !string.IsNullOrEmpty(subject) ? subject : "Tajna wiadomość!";
                    message.Body = body;
                    message.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                    {
                        smtp.Port = Convert.ToInt32(smtp_port);
                        smtp.Host = smtp_host;
                        smtp.EnableSsl = true;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(from_acc, from_pass);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        await smtp.SendMailAsync(message); //wysyłka maila
                        isSend = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Info  
                throw ex;
            }
            // info.  
            return isSend;
        }


    }
}
