using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static QuestManager;

public class DataSheetConfig : MonoBehaviour
{
    const string ScriptURL = "https://docs.google.com/spreadsheets/d/1qziJnNzfDgjzyyR8C4YE3sum7pICll0ATPerMF94LZw/export?format=tsv&gid=0&range=A2:C";
    const string QuestURL = "https://docs.google.com/spreadsheets/d/1qziJnNzfDgjzyyR8C4YE3sum7pICll0ATPerMF94LZw/export?format=tsv&gid=268034950&range=A2:G";


    public static Dictionary<(int, int), (string[], int)> NpcSriptDict = new Dictionary<(int, int), (string[], int)>();
    public static Dictionary<int, Quest> QuestDict = new Dictionary<int, Quest>();

    enum DataSheet
    {
        Script,
        Quest,
    }

    void Awake()
    {
        if (Managers.Data.isDataSheetInitialized == false)
        {
            StartCoroutine(GetDataSheet(ScriptURL, DataSheet.Script));
            StartCoroutine(GetDataSheet(QuestURL, DataSheet.Quest));
        }

        Managers.Data.isDataSheetInitialized = true;

    }


    IEnumerator GetDataSheet(string URL, DataSheet type)
    {
        UnityWebRequest www = UnityWebRequest.Get(URL);
        yield return www.SendWebRequest();

        // 요청 완료된 후 다시 실행
        if(Enum.GetName(typeof(DataSheet), type) == "Script")
        {
            string data = www.downloadHandler.text;
            ParseScript(data);
            Debug.Log($"{data}");
        }
        else if (Enum.GetName(typeof(DataSheet), type) == "Quest")
        {
            string data = www.downloadHandler.text;
            ParseQuest(data);
            Debug.Log($"{data}");
        }

    }

    // NPC script 파싱
    void ParseScript(string data)
    {
        foreach(string scriptItem in data.Split("\t\tEOS"))
        {
            string[] script = scriptItem.Split("\t\t");
            int NPCID = int.Parse(script[0].Split("\t")[0]);
            int ScriptID = int.Parse(script[0].Split("\t")[1]);

            script[0] = script[0].Split("\t")[2];

            int QuestNum = 0; // 기본 값은 0 -> 퀘스트 없음
            if (script[script.Length - 1].Split("@").Length > 1)
            {
                string[] dialougeAndQuest = script[script.Length - 1].Split("@");
                script[script.Length - 1] = dialougeAndQuest[0];
                QuestNum = int.Parse(dialougeAndQuest[1]);
            }

            // 퀘스트 번호가 0일 때, 퀘스트가 없는 일반 스크립트
            // 0 이 아닐 때, 대화시 해당 퀘스트를 받는 대화
            NpcSriptDict.Add((NPCID, ScriptID), (script, QuestNum));
        }

        Managers.Dialouge.DialogueDict = NpcSriptDict;
    }


    // Quest 관련 정보 파싱
    void ParseQuest(string data)
    {
        foreach(string QuestItem in data.Split("\n"))
        {
            string[] quest = QuestItem.Split("\t");
            int questID = int.Parse(quest[0]);
            string questType = quest[1];
            string questName = quest[2];
            string questTip = quest[3];
            int ownerNPCId = int.Parse(quest[4]);
            int questExp = int.Parse(quest[5]);
            string questObject = quest[6].TrimEnd('\r');

            QuestDict.Add(questID, new Quest(questID, questType, questName, questTip, ownerNPCId, questExp, questObject, false, false));
        }

        Managers.Quest.AllQuests = QuestDict;
    }

}


