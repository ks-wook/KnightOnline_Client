using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_RaidMatch : UI_Base
{
    enum Texts
    {
        SearchingPlayerText
    }

    enum Images
    {
        RotateImg,
        CancelBtn
    }

    Image RotateImg;
    GameObject SearchingPlayerText;

    List<Sprite> RotateImgs = new List<Sprite>();
    List<string> SearchingPlayerTexts = new List<string>() { "Searching Player.", "Searching Player..", "Searching Player..." };

    public override void Init()
    {
        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        GetImage((int)Images.CancelBtn).gameObject.BindEvent(OnClickCancelBtn);

        RotateImg = GetImage((int)Images.RotateImg);
        SearchingPlayerText = GetText((int)Texts.SearchingPlayerText).gameObject;

        RotateImgs.Add(Managers.Resource.Load<Sprite>("Textures/Icon/Rotatable1"));
        RotateImgs.Add(Managers.Resource.Load<Sprite>("Textures/Icon/Rotatable2"));
        RotateImgs.Add(Managers.Resource.Load<Sprite>("Textures/Icon/Rotatable3"));
        
        StartCoroutine(CoRefreshUI());
    }

    public void OnClickCancelBtn(PointerEventData evt)
    {
        StopCoroutine(CoRefreshUI());

        // TODO : 매칭 취소 패킷 전송 및 팝업 UI OFF
        C_RaidMatch raidCancel = new C_RaidMatch();
        raidCancel.Name = PacketHandler.PlayerName;
        raidCancel.Req = false;
        Managers.Network.Send(raidCancel);


        Managers.UI.CloseAllPopupUI();
        // transform.gameObject.SetActive(false);
    }

    // 매칭창 UI 갱신
    IEnumerator CoRefreshUI()
    {
        int matchingTime = 0;
        while (true)
        {
            RotateImg.sprite = RotateImgs[matchingTime % 3];
            SearchingPlayerText.GetComponent<Text>().text = SearchingPlayerTexts[matchingTime % 3];
            matchingTime++;
            yield return new WaitForSeconds(1.0f);
        }
    }
}
