using UnityEngine;
using System.Security.Cryptography;
using System;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * ���̽� ���� ��ȣȭ �� �ڵ����� ������ ��η� ��ȣȭ�� ������
 * ����, �����ϴ� ������ �� ��ũ��Ʈ�̴�.
 * 
 * 
 */




public static class GameDataEncryptor
{
#if UNITY_EDITOR

    static readonly string aes_key = "AXe8YwuIn1zxt3FPWTZFlAa14EHdPAdN9FaZ9RQWihc="; //44��
    static readonly string aes_iv = "bsxnWolsAyO7kCfWuyrnqg=="; //24��

    static string[] rawFilePaths = new string[] { "ItemData.json", "StatData.json", "SkillData.json" }; // 1. ������, 2. ���� 3. ��ų

    // ��ȣȭ ���̽� ���� ���� �Լ�
    public static void EncryptJsonFile(List<string> buildFileNames)
    {
        
        if (EditorUtility.DisplayDialog("���� DataFile ����", "��ȣȭ�� DataFile�� �����մϴ�.", "Encrypt", "Cancel"))
        {
            List<string> encryptedFilePaths_Build = new List<string>();

            foreach (string path in rawFilePaths)
            {
                Debug.Log(Application.dataPath);
                string rawFilePath = Application.dataPath + "/Resources/Data/" + path;

                // ���߿� ���
                string encryptedFilePath_Develope = Application.dataPath + "/Resources/Data/EcryptedData/" + path;


                // ����� ���
                foreach (string buildFileName in buildFileNames)
                {
                    string encryptedFilePath_Build = Application.dataPath + 
                        "/../Builds/Win64/" + buildFileName + "/" + buildFileName + "_Data/Resources/Data/EcryptedData/" + path;

                    encryptedFilePaths_Build.Add(encryptedFilePath_Build);
                }
                


                if (File.Exists(encryptedFilePath_Develope))
                {
                    // ��� ������ �̹� �����ϴ� ��� ����
                    File.Delete(encryptedFilePath_Develope);
                }

                foreach(string encryptedFilePath_Build in encryptedFilePaths_Build)
                {
                    if (File.Exists(encryptedFilePath_Build))
                    {
                        // ��� ������ �̹� �����ϴ� ��� ����
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

    // ��ȣȭ�� ���̽� ���� ������
    public static string EncryptAES(string plainText)
    {
        byte[] encrypted;

        // Base64��� ��ȣȭ ���
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

    // ��ȣȭ�� ���̽� ���� ��ȣȭ�� �Լ�
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

// ����� ������ ��η� ��ȣȭ�� ���̽� ������ ������Ű�� ���� ������ ��
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
        EditorGUILayout.LabelField("���� ���� �̸�1 : ");
        BuildFileName1 = EditorGUILayout.TextField(BuildFileName1);
        EditorGUILayout.LabelField("���� ���� �̸�2 : ");
        BuildFileName2 = EditorGUILayout.TextField(BuildFileName2);
        EditorGUILayout.LabelField("���� ���� �̸�3 : ");
        BuildFileName3 = EditorGUILayout.TextField(BuildFileName3);
        EditorGUILayout.LabelField("���� ���� �̸�4 : ");
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
                Debug.Log("������ ���� ��η� ��ȣȭ ���� ���� ����");
                GameDataEncryptor.EncryptJsonFile(buildFileNames);
            }

            buildFileNames.Clear();
            Close();
        }
    }
}