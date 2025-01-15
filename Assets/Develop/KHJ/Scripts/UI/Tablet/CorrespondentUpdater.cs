using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorrespondentUpdater : UIBinder
{
    [SerializeField] Sprite[] _npcSprites;
    private Dictionary<int, Sprite> _npcSpriteDict = new();

    private void Start()
    {
        int i = 0;
        foreach (var corrID in CSVManager.Instance.Correspondents.Keys)
        {
            _npcSpriteDict.Add(corrID, _npcSprites[i++]);
        }

        SubscribeTruck();
    }

    private void SubscribeTruck()
    {
        QuestManager.Instance.OnTruckUpdated.AddListener(UpdateUI);
    }

    private void UpdateUI(List<QuestManager.Quest> questList, List<TruckQuest> truckList)
    {
        for (int i = 0; i < questList.Count; i++)
        {
            Debug.Log($"{i + 1}번째 거래처와 UI 연동");

            string limitTime = $"Info{i + 1}_LimitTime";
            string NPCImage = $"Info{i + 1}_NPCImage";
            string NPCName = $"Info{i + 1}_NPCName";

            GetUI<TextMeshProUGUI>(limitTime).text = GetLimitTime(questList[i].questTimer);
            GetUI<Image>(NPCImage).sprite = _npcSpriteDict[truckList[i].correspondentId];
            GetUI<TextMeshProUGUI>(NPCName).text = CSVManager.Instance.Correspondents[truckList[i].correspondentId].correspondent_name;
            for (int cropIdx = 0; cropIdx < questList[i].requiredItems.Count; cropIdx++)
                GetUI<QuestItemSlot>($"Info{i + 1}_Slot{cropIdx + 1}").OnUpdate(questList, i, cropIdx);
        }
    }

    private string GetLimitTime(float seconds)
    {
        int min = (int)(seconds / 60f);
        int sec = (int)(seconds % 60f);

        return $"{min:00}:{sec:00}";
    }
}
