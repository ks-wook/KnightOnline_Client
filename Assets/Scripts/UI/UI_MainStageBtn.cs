using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_MainStageBtn : UI_Base
{
    enum Texts
    {
        ClearText,
    }

    enum Images
    {

    }

    GameObject clearText;

    public override void Init()
    {
        Bind<Text>(typeof(Texts));

        clearText = GetText((int)Texts.ClearText).gameObject;
        gameObject.BindEvent(OnClickMainStageBtn);

        RefreshUI();
    }

    public void OnClickMainStageBtn(PointerEventData evt)
    {
        Debug.Log("���� �������� : " + gameObject.name);

        // ���� �������� �� �ε�

        // ������ C_EnterGame ����
        C_EnterGame enterGamePacket = new C_EnterGame();
        enterGamePacket.Name = Managers.Object.MyPlayer.name;
        enterGamePacket.RoomNum = 0;
        Managers.Network.Send(enterGamePacket);




        // �� �ε�
        Managers.Scene.LoadScene(gameObject.name);



    }

    // �������� Ŭ���� ������ ���� UI ����� �޶���
    public void RefreshUI()
    {
        Debug.Log(gameObject.name);

        if(Managers.Object.ClearedStages.Contains(gameObject.name)) // ���� ���������� Ŭ���� �ߴ� �� Ȯ��
            clearText.SetActive(true);
        else
            clearText.SetActive(false);
    }

}
