using UnityEngine;
using System.Security.Cryptography;
using System;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * 제이슨 파일 암호화 후 자동으로 지정된 경로로 암호화된 파일을
 * 저장, 생성하는 에디터 툴 스크립트이다.
 * 
 * 
 */




public static class GameDataEncryptor
{
#if UNITY_EDITOR

    static readonly string aes_key = "AXe8YwuIn1zxt3FPWTZFlAa14EHdPAdN9FaZ9RQWihc="; //44자
    static readonly string aes_iv = "bsxnWolsAyO7kCfWuyrnqg=="; //24자

    static string[] rawFilePaths = new string[] { "ItemData.json", "StatData.json", "SkillData.json" }; // 1. 아이템, 2. 스탯 3. 스킬

    // 암호화 제이슨 파일 저장 함수
    public static void EncryptJsonFile(List<string> buildFileNames)
    {
        
        if (EditorUtility.DisplayDialog("게임 DataFile 생성", "암호화된 DataFile을 생성합니다.", "Encrypt", "Cancel"))
        {
            List<string> encryptedFilePaths_Build = new List<string>();

            foreach (string path in rawFilePaths)
            {
                Debug.Log(Application.dataPath);
                string rawFilePath = Application.dataPath + "/Resources/Data/" + path;

                // 개발용 경로
                string encryptedFilePath_Develope = Application.dataPath + "/Resources/Data/EcryptedData/" + path;


                // 빌드용 경로
                foreach (string buildFileName in buildFileNames)
                {
                    string encryptedFilePath_Build = Application.dataPath + 
                        "/../Builds/Win64/" + buildFileName + "/" + buildFileName + "_Data/Resources/Data/EcryptedData/" + path;

                    encryptedFilePaths_Build.Add(encryptedFilePath_Build);
                }
                


                if (File.Exists(encryptedFilePath_Develope))
                {
                    // 대상 파일이 이미 존재하는 경우 삭제
                    File.Delete(encryptedFilePath_Develope);
                }

                foreach(string encryptedFilePath_Build in encryptedFilePaths_Build)
                {
                    if (File.Exists(encryptedFilePath_Build))
                    {
                        // 대상 파일이 이미 존재하는 경우 삭제
                        File.Delete(encryptedFilePath_Build);
                    }
                }
                
                string jsonData = File.ReadAllText(rawFilePath);

                string encryptedDataStr = EncryptAES(jsonData);
                encryptedFilePath_Develope = Path.ChangeExtension(encryptedFilePath_Develope, ".encrypted");
                File.WriteAllText(encryptedFilePath_Develope, encryptedDataStr);

                foreach (string encryptedFilePath_Build in encryptedFilePaths_Build)
                {
                    string encryptedFilePath_BuildEx = Path.ChangeExtension(encryptedFilePath_Build, ".encrypted");
                    File.WriteAllText(encryptedFilePath_BuildEx, encryptedDataStr);
                }

                encryptedFilePaths_Build.Clear();
            }
        }
    }

    // 암호화된 제이슨 파일 생성기
    public static string EncryptAES(string plainText)
    {
        byte[] encrypted;

        // Base64기반 암호화 방식
        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
        {
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Key = Convert.FromBase64String(aes_key);
            aes.IV = Convert.FromBase64String(aes_iv);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    encrypted = ms.ToArray();
                }
            }
        }

        return Convert.ToBase64String(encrypted);
    }

    // 암호화된 제이슨 파일 복호화용 함수
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

#endif
}

// 빌드시 지정된 경로로 암호화된 제이슨 파일을 생성시키기 위한 에디터 툴
public class MyEditorWindow : EditorWindow
{
    List<string> buildFileNames = new List<string>();

    private string BuildFileName1 = "MMO_Portfolio1";
    private string BuildFileName2 = "MMO_Portfolio2";
    private string BuildFileName3 = "";
    private string BuildFileName4 = "";

    [MenuItem("Tools/Create Encrypt DataFile")]
    public static void OpenPopupWindow()
    {
        MyEditorWindow window = EditorWindow.GetWindow<MyEditorWindow>("Popup Window");
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("빌드 파일 이름1 : ");
        BuildFileName1 = EditorGUILayout.TextField(BuildFileName1);
        EditorGUILayout.LabelField("빌드 파일 이름2 : ");
        BuildFileName2 = EditorGUILayout.TextField(BuildFileName2);
        EditorGUILayout.LabelField("빌드 파일 이름3 : ");
        BuildFileName3 = EditorGUILayout.TextField(BuildFileName3);
        EditorGUILayout.LabelField("빌드 파일 이름4 : ");
        BuildFileName4 = EditorGUILayout.TextField(BuildFileName4);

        if (GUILayout.Button("OK"))
        {
            Debug.Log(Application.dataPath);
            if (!string.IsNullOrEmpty(BuildFileName1))
                buildFileNames.Add(BuildFileName1);
            if (!string.IsNullOrEmpty(BuildFileName2))
                buildFileNames.Add(BuildFileName2);
            if (!string.IsNullOrEmpty(BuildFileName3))
                buildFileNames.Add(BuildFileName3);
            if (!string.IsNullOrEmpty(BuildFileName4))
                buildFileNames.Add(BuildFileName4);

            if (buildFileNames.Count > 0)
            {
                Debug.Log("지정된 파일 경로로 암호화 파일 생성 시작");
                GameDataEncryptor.EncryptJsonFile(buildFileNames);
            }

            buildFileNames.Clear();
            Close();
        }
    }
}