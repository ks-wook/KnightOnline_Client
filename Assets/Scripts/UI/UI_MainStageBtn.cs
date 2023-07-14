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
        Debug.Log("메인 스테이지 : " + gameObject.name);

        // 메인 스테이지 씬 로딩

        // 서버로 C_EnterGame 전송
        C_EnterGame enterGamePacket = new C_EnterGame();
        enterGamePacket.Name = Managers.Object.MyPlayer.name;
        enterGamePacket.RoomNum = 0;
        Managers.Network.Send(enterGamePacket);




        // 씬 로드
        Managers.Scene.LoadScene(gameObject.name);



    }

    // 스테이지 클리어 유무에 따라 UI 출력이 달라짐
    public void RefreshUI()
    {
        Debug.Log(gameObject.name);

        if(Managers.Object.ClearedStages.Contains(gameObject.name)) // 현재 스테이지를 클리어 했는 지 확인
            clearText.SetActive(true);
        else
            clearText.SetActive(false);
    }

}
