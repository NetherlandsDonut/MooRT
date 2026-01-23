using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Mail;
using System.Diagnostics;
using System.Net.Security;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using UnityEngine;

using Newtonsoft.Json;

using static MusicRelease;

using static Newtonsoft.Json.Formatting;
using static Newtonsoft.Json.JsonConvert;

class Serialization
{
    //Mail to send
    public static MailMessage mail;

    //Smtp server to send mails with
    public static SmtpClient smtpServer;

    public static async void SendMail()
    {
        var content = StringFromPackage(new(musicRelease, musicRelease.artist, musicRelease.country, Root.newCoverURL));
        smtpServer = new("smtp-relay.brevo.com");
        smtpServer.Timeout = 10000;
        smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
        smtpServer.UseDefaultCredentials = false;
        smtpServer.Port = 587;
        mail = new();
        mail.From = new MailAddress("moort.box@gmail.com");
        mail.To.Add(new MailAddress("moort.box@gmail.com"));
        mail.Subject = musicRelease.artist + " - " + musicRelease.name;
        mail.Body = content;
        smtpServer.Credentials = new System.Net.NetworkCredential("a0a9c9001@smtp-brevo.com", SMTPPass.password) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
        mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
        BackupAlbumCreation(content, mail.Subject);
        try
        {
            await smtpServer.SendMailAsync(mail);
        }
        catch
        {
            Root.SpawnDesktopBlueprint("SendingMailFailure");
        }
        Root.CloseDesktop("SendingMail");
    }

    //Prefix for serialisation
    public static string prefix = "";

    //Indicates whether game tries to load data from unity the folder
    public static bool useUnityData = false;

    //Indicates whether the program allows for library expansion
    public static bool libraryExpansion = false;

    //Reads a text file and returns all lines of text
    public static string[] ReadTXT(string file, string prefix = "")
    {
        if (useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
        if (!Directory.Exists(prefix + "MooRT_Data_2"))
            Directory.CreateDirectory(prefix + "MooRT_Data_2");
        if (!File.Exists(prefix + "MooRT_Data_2/" + file + ".txt")) return null;
        var content = File.ReadAllLines(prefix + "MooRT_Data_2/" + file + ".txt");
        return content;
    }

    //Starts a process of opening a text file
    public static void OpenTXT(string file, string prefix = "")
    {
        if (useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
        if (!Directory.Exists(prefix + "MooRT_Data_2"))
            Directory.CreateDirectory(prefix + "MooRT_Data_2");
        if (useUnityData) Process.Start(prefix + "MooRT_Data_2/" + file + ".txt");
        else Process.Start(Application.dataPath + "_2/" + file + ".txt");
    }

    public static string urlContent = "";

    public static void DeserializeFromURL<T>(ref T target, bool encoded = false)
    {
        var content = urlContent;
        if (encoded) content = Decrypt(content);
        target = DeserializeObject<T>(content);
    }

    public static void Deserialize<T>(ref T target, string file, bool encoded = false, string prefix = "")
    {
        if (useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
        if (!Directory.Exists(prefix + "MooRT_Data_2"))
            Directory.CreateDirectory(prefix + "MooRT_Data_2");
        if (!File.Exists(prefix + "MooRT_Data_2/" + file + (encoded ? "" : ".json"))) return;
        var content = File.ReadAllText(prefix + "MooRT_Data_2/" + file + (encoded ? "" : ".json"));
        if (encoded) content = Decrypt(content);
        target = DeserializeObject<T>(content);
    }

    public static void Serialize(object what, string where, bool backup = false, bool encoded = false, string prefix = "")
    {
        if (useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
        if (!Directory.Exists(prefix + "MooRT_Data_2"))
            Directory.CreateDirectory(prefix + "MooRT_Data_2");
        var date = DateTime.Now.ToString("dd.MM.yyyy - HH.mm");
        if (backup)
        {
            if (backup && !Directory.Exists(prefix + "MooRT_Data_2/Backup"))
                Directory.CreateDirectory(prefix + "MooRT_Data_2/Backup");
            if (backup && !Directory.Exists(prefix + "MooRT_Data_2/Backup/" + date))
                Directory.CreateDirectory(prefix + "MooRT_Data_2/Backup/" + date);
        }
        if (backup && File.Exists(prefix + "MooRT_Data_2/" + (backup ? "Backup/" + date + "/" : "") + where + (encoded ? "" : ".json"))) return;
        var sett = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
        var data = SerializeObject(what, encoded ? None : Indented, sett);
        if (encoded) data = Encrypt(data);
        File.WriteAllText(prefix + "MooRT_Data_2/" + (backup ? "Backup/" + date + "/" : "") + where + (encoded ? "" : ".json"), data);
    }

    public static void BackupAlbumCreation(string data, string where, string prefix = "")
    {
        if (useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
        if (!Directory.Exists(prefix + "MooRT_Data_2"))
            Directory.CreateDirectory(prefix + "MooRT_Data_2");
        if (!Directory.Exists(prefix + "MooRT_Data_2/Created albums"))
            Directory.CreateDirectory(prefix + "MooRT_Data_2/Created albums");
        if (File.Exists(prefix + "MooRT_Data_2/" + "Created albums/" + where + ".txt")) return;
        File.WriteAllText(prefix + "MooRT_Data_2/" + "Created albums/" + where + ".txt", data);
    }

    public static string IV = "1a1a1a1a1a1a1a1a";
    public static string Key = "1a1a1a1a1a1a1a1a1a1a1a1a1a1a1a13";

    public static string Encrypt(string what)
    {
        byte[] textbytes = Encoding.UTF8.GetBytes(what);
        var endec = new AesCryptoServiceProvider()
        {
            BlockSize = 128,
            KeySize = 256,
            IV = Encoding.UTF8.GetBytes(IV),
            Key = Encoding.UTF8.GetBytes(Key),
            Padding = PaddingMode.Zeros,
            Mode = CipherMode.ECB
        };
        ICryptoTransform icrypt = endec.CreateEncryptor(endec.Key, endec.IV);
        byte[] enc = icrypt.TransformFinalBlock(textbytes, 0, textbytes.Length);
        icrypt.Dispose();
        return Convert.ToBase64String(enc);
    }

    public static string Decrypt(string what)
    {
        byte[] textbytes = Convert.FromBase64String(what);
        var endec = new AesCryptoServiceProvider()
        {
            BlockSize = 128,
            KeySize = 256,
            IV = Encoding.UTF8.GetBytes(IV),
            Key = Encoding.UTF8.GetBytes(Key),
            Padding = PaddingMode.Zeros,
            Mode = CipherMode.ECB
        };
        ICryptoTransform icrypt = endec.CreateDecryptor(endec.Key, endec.IV);
        byte[] enc = icrypt.TransformFinalBlock(textbytes, 0, textbytes.Length);
        icrypt.Dispose();
        return Encoding.UTF8.GetString(enc);
    }

    public static string CompressToBase64(string text)
    {
        byte[] compressedBytes;
        using (var output = new MemoryStream())
        {
            using (var gzip = new GZipStream(output, System.IO.Compression.CompressionLevel.Optimal))
            using (var writer = new StreamWriter(gzip, Encoding.UTF8)) { writer.Write(text); }
            compressedBytes = output.ToArray();
        }
        return Convert.ToBase64String(compressedBytes);
    }

    public static string DecompressFromBase64(string base64)
    {
        Span<byte> compressedBytes = new byte[base64.Length];
        if (Convert.TryFromBase64String(base64, compressedBytes, out int bytesWritten))
        {
            using var input = new MemoryStream(compressedBytes.ToArray());
            using var gzip = new GZipStream(input, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip, Encoding.UTF8);
            return reader.ReadToEnd();
        }
        else return "Failed";
    }

    public static string StringFromPackage(ReleasePackage package)
    {
        var sett = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore };
        return CompressToBase64(SerializeObject(package, None, sett));
    }

    public static ReleasePackage PackageFromString(string data)
    {
        var uncompressed = DecompressFromBase64(data);
        if (uncompressed == "Failed") return null;
        return DeserializeObject<ReleasePackage>(uncompressed);
    }
}
