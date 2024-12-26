using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager  : MonoBehaviourPun
{
    public static QuestManager Instance;

    [System.Serializable]
    public class Quest
    {
        public string questName;
        public GameObject[] requiredItems;
        public GameObject requiredItem;
        public int requiredCount;
        public int currentCount = 0;
    }

    public Quest currentQuest;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FirstStart()
    {
        photonView.RPC(nameof(QuestStart), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void QuestStart()
    {
        NewQuest("택배포장", currentQuest.requiredItems, 1);
    }

    public void NewQuest(string questName, GameObject[] requiredItems, int requiredCount)
    {
        currentQuest = new Quest
        {
            questName = questName,
            requiredItems = requiredItems,
            requiredItem = requiredItems[Random.Range(0, requiredItems.Length)],
            requiredCount = requiredCount,
            currentCount = 0
        };

        UpdateUI();
    }

    public void UpdateCount()
    {
        Debug.Log("업데이트");
        photonView.RPC(nameof(UpdateItemCount), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void UpdateItemCount()
    {
        Debug.Log(currentQuest.currentCount);
        currentQuest.currentCount++;
        UpdateUI();

        if (currentQuest.currentCount >= currentQuest.requiredCount)
        {
            QuestComplete();
        }
    }

    private void UpdateUI()
    {
        UIManager.Instance.UpdateQuestUI(currentQuest.questName, currentQuest.currentCount, currentQuest.requiredCount);
    }

    private void QuestComplete()
    {
        Debug.Log("퀘스트 완료!");
        photonView.RPC(nameof(NextQuest), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void NextQuest()
    {
        Debug.Log("다음 퀘스트를 시작합니다!");
    }

    public bool IsQuestComplete()
    {
        return currentQuest != null && currentQuest.currentCount >= currentQuest.requiredCount;
    }
}