using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;


public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    public Dictionary<int, Data.Skill> SkillDict { get; private set; } = new Dictionary<int, Data.Skill>();
    public Dictionary<int, Data.ItemData> ItemDict { get; private set; } = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.BossMonsterData> BossMonsterDict { get; private set; } = new Dictionary<int, Data.BossMonsterData>();


    static readonly string aes_key = "AXe8YwuIn1zxt3FPWTZFlAa14EHdPAdN9FaZ9RQWihc="; //44자
    static readonly string aes_iv = "bsxnWolsAyO7kCfWuyrnqg=="; //24자

    static string[] encryptedFilePaths = new string[] { "ItemData.json" };

    public void Init()
    {
        // SkillDict = LoadJson<Data.SkillData, int, Data.Skill>("SkillData").MakeDict();
        ItemDict = LoadJson<Data.ItemLoader, int, Data.ItemData>("ItemData.encrypted").MakeDict();
        // BossMonsterDict = LoadJson<Data.BossMonsterLoader, int, Data.BossMonsterData>("BossMonsterData").MakeDict();
    }

    Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
        // 암호화된 파일 복호화
        string encryptedFilePath = Application.dataPath + "/Resources/Data/EcryptedData/" + path;
        string encryptedJsonFile = File.ReadAllText(encryptedFilePath);

        string decryptedJsonFile = DecryptAES(encryptedJsonFile);
        Debug.Log(decryptedJsonFile);

        // Json 파일 역직렬화
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Loader>(decryptedJsonFile);
    }

    // 복호화 함수
    public static string DecryptAES(string encryptedText)
    {
        string decrypted = null;
        byte[] cipher = Convert.FromBase64String(encryptedText);

        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
        {
            aes.KeySize = 256; //AES256
            aes.BlockSize = 128;
            aes.Key = Convert.FromBase64String(aes_key);
            aes.IV = Convert.FromBase64String(aes_iv);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(cipher))
            {
                using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        decrypted = sr.ReadToEnd();
                    }
                }
            }
        }

        return decrypted;
    }
}
