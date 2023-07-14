using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public enum Grade
{
    Normal,
    Uncommon,
    Rare,
    Legend,
}

public enum Size
{
    POT64,
    POT128,
    POT256,
    POT512,
    POT1024,
}

public enum TypeOfItem
{
    Weapon,
    Armor,
    Consumable,
}

public class IconCreator : MonoBehaviour
{
    public Camera mainCam;
    public RenderTexture renderTexture;
    public Image backGround;

    [SerializeField]
    public string FileName = "";
    private string FilePath = "";

    public Grade grade;
    public Size size;
    public TypeOfItem type;



    private void Start()
    {
        mainCam = Camera.main;
        SettingColor();
        SettingSize();
        SettingPath();
    }

    public void CreateIcon()
    {
        StartCoroutine(CaptureImage());
    }

    IEnumerator CaptureImage()
    {
        // 버그 방지를 위해 한 프레임씩 쉬어야 한다.
        yield return null;

        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height,
            TextureFormat.ARGB32, false, true);
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);

        yield return null;

        var data = tex.EncodeToPNG();
        
        string extention = ".png"; // png 파일로 인코딩

        Debug.Log($"{FilePath} 에 아이콘 이미지가 생성되었습니다.");

        if (!Directory.Exists(FilePath)) 
            Directory.CreateDirectory(FilePath);

        if (FileName == "")
            FileName = "Thumbnail";

        File.WriteAllBytes(FilePath + FileName + extention, data);

        yield return null;
    }

    void SettingColor()
    {
        switch(grade)
        {
            case Grade.Normal:
                mainCam.backgroundColor = Color.white;
                // backGround.color = Color.white;
                break;
            case Grade.Uncommon:
                mainCam.backgroundColor = Color.green;
                // backGround.color = Color.green;
                break;
            case Grade.Rare:
                mainCam.backgroundColor = Color.blue;
                // backGround.color = Color.blue;
                break;
            case Grade.Legend:
                mainCam.backgroundColor = Color.yellow;
                // backGround.color = Color.yellow;
                break;
        }
    }

    void SettingSize()
    {
        switch (size)
        {
            case Size.POT64:
                renderTexture.width = 64;
                renderTexture.height = 64;
                break;
            case Size.POT128:
                renderTexture.width = 128;
                renderTexture.height = 128; 
                break;
            case Size.POT256:
                renderTexture.width = 256;
                renderTexture.height = 256; 
                break;
            case Size.POT512:
                renderTexture.width = 512;
                renderTexture.height = 512;
                break;
            case Size.POT1024:
                renderTexture.width = 1024;
                renderTexture.height = 1024; 
                break;
        }
    }

    void SettingPath()
    {
        switch (type)
        {
            case TypeOfItem.Weapon:
                FilePath = Application.dataPath + "/Resources/Textures/Item/Weapon/";
                break;
            case TypeOfItem.Armor:
                FilePath = Application.dataPath + "/Resources/Textures/Item/Armor/";
                break;
            case TypeOfItem.Consumable:
                FilePath = Application.dataPath + "/Resources/Textures/Item/Consumable/";
                break;
        }
    }
}
