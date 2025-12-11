using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Security.Cryptography;

using Newtonsoft.Json;

using static Newtonsoft.Json.JsonConvert;
using static Newtonsoft.Json.Formatting;

class Serialization
{
    //Prefix for serialisation
    public static string prefix = "";

    //Indicates whether game tries to load data from unity the folder
    public static bool useUnityData = true;

    //Indicates whether the program allows for library expansion
    public static bool libraryExpansion = true;

    public static string[] ReadTXT(string file, string prefix = "")
    {
        if (useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
        if (!Directory.Exists(prefix + "MooRT_Data_2"))
            Directory.CreateDirectory(prefix + "MooRT_Data_2");
        if (!File.Exists(prefix + "MooRT_Data_2/" + file + ".txt")) return null;
        var content = File.ReadAllLines(prefix + "MooRT_Data_2/" + file + ".txt");
        return content;
    }

    public static void OpenTXT(string file, string prefix = "")
    {
        if (useUnityData) prefix = @"C:\Users\ragan\Documents\Projects\Unity\MooRT\";
        if (!Directory.Exists(prefix + "MooRT_Data_2"))
            Directory.CreateDirectory(prefix + "MooRT_Data_2");
        if (!File.Exists(prefix + "MooRT_Data_2/" + file + ".txt"))
        {
            using var sw = new StreamWriter(prefix + "MooRT_Data_2/" + file + ".txt", true);
            sw.WriteLine("Artist name:");
            sw.WriteLine("");
            sw.WriteLine("here");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("Artist country:");
            sw.WriteLine("");
            sw.WriteLine("here");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("Release name:");
            sw.WriteLine("");
            sw.WriteLine("here");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("Release cover:");
            sw.WriteLine("");
            sw.WriteLine("here");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("Release type: Studio album | Live album | Extended play | Compilation album | Demo recording | Soundtrack | Remix album");
            sw.WriteLine("");
            sw.WriteLine("here");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("Genres:");
            sw.WriteLine("");
            sw.WriteLine("here, here, here, here");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("");
            sw.WriteLine("Tracklist:");
            sw.WriteLine("");
            sw.WriteLine("here");
            sw.WriteLine("here");
            sw.WriteLine("here");
        }
        Process.Start(prefix + "MooRT_Data_2/" + file + ".txt");
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
}
