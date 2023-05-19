using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static QuestManager;

public class UI_Quest : UI_Base
{
    public List<UI_Quest_Item> QuestSlot { get; } = new List<UI_Quest_Item>();

	GameObject _questGrid;

	public override void Init()
    {
        QuestSlot.Clear();

		_questGrid = transform.Find("Quest/Viewport/QuestGrid").gameObject;
        foreach (Transform child in _questGrid.transform)
            Destroy(child.gameObject);


        RefreshUI();
    }


	public void RefreshUI()
	{
        foreach (Transform child in _questGrid.transform)
            Destroy(child.gameObject);

        foreach(Quest quest in Managers.Quest.PlayerQuests.Values.ToList())
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Quest_Item", _questGrid.transform);
            UI_Quest_Item questItem = go.GetOrAddComponent<UI_Quest_Item>();

            questItem.SetQuestItem(quest);
            QuestSlot.Add(questItem);
        }

    }

}
