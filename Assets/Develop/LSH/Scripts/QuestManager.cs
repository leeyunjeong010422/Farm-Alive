using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class QuestManager : MonoBehaviourPun
{
    public static QuestManager Instance;

    [System.Serializable]
    public class RequiredItem
    {
        public GameObject itemPrefab;
        public int requiredcount;
        public int count = 0;

        public RequiredItem(GameObject prefab, int itemCount)
        {
            itemPrefab = prefab;
            requiredcount = itemCount;
        }
    }

    [System.Serializable]
    public class Quest
    {
        public string questName;
        public GameObject[] itemPrefabs;
        public List<RequiredItem> requiredItems;
        public int currentCount = 0;

        public Quest(string name, GameObject[] prefabs, int[] counts)
        {
            questName = name;
            itemPrefabs = prefabs;
            requiredItems = new List<RequiredItem>();

            int rand = Random.Range(1, itemPrefabs.Length);

            List<int> randomPrefabIndexes = new List<int>();
            for (int i = 0; i < prefabs.Length; i++)
            {
                randomPrefabIndexes.Add(i);
            }

            for (int i = 0; i < rand; i++)
            {
                int randomIndex = Random.Range(0, randomPrefabIndexes.Count);
                int chosenIndex = randomPrefabIndexes[randomIndex];
                randomPrefabIndexes.RemoveAt(randomIndex);

                requiredItems.Add(new RequiredItem(prefabs[chosenIndex], counts[i]));
            }
        }
    }

    [SerializeField] public List<Quest> questsList = new List<Quest>();
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
        int[] itemCounts = new int[5];
        for (int i = 0; i < itemCounts.Length; i++)
        {
            itemCounts[i] = Random.Range(1, itemCounts.Length);
        }

        NewQuest("택배포장", currentQuest.itemPrefabs, itemCounts);
    }

    public void NewQuest(string questName, GameObject[] itemPrefabs, int[] itemCounts)
    {
        currentQuest = new Quest(questName, itemPrefabs, itemCounts);

        questsList.Add(currentQuest);
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

        if (currentQuest.currentCount >= currentQuest.requiredItems.Count)
        {
            QuestComplete();
        }
    }

    [PunRPC]
    public void ExitItemCount()
    {
        currentQuest.currentCount--;
        Debug.Log(currentQuest.currentCount);
        UpdateUI();

        /*if (currentQuest.currentCount >= currentQuest.requiredCount)
        {
            QuestComplete();
        }*/
    }

    private void UpdateUI()
    {
        /*UIManager.Instance.UpdateQuestUI(currentQuest.questName, currentQuest.currentCount, currentQuest.requiredCount);*/
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
        /*return currentQuest != null && currentQuest.currentCount >= currentQuest.requiredCount;*/
        return true;
    }
}