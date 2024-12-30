using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using static QuestManager;

public class QuestManager : MonoBehaviourPun
{
    public static QuestManager Instance;

    [System.Serializable]
    public class RequiredItem
    {
        public GameObject itemPrefab;
        public int requiredcount;
        public bool isSuccess;

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
        public List<RequiredItem> requiredItems;
        public bool isSuccess;
    }

    [SerializeField] public List<Quest> questsList = new List<Quest>();
    [SerializeField] public List<TruckQuest> truckList = new List<TruckQuest>();
    [SerializeField] public GameObject[] itemPrefabs;
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
        if (PhotonNetwork.IsMasterClient)
        {
            int rand = Random.Range(1, itemPrefabs.Length);

            List<int> randomPrefabIndexes = new List<int>();
            int[] choseIndex = new int[rand];


            for (int i = 0; i < itemPrefabs.Length; i++)
            {
                randomPrefabIndexes.Add(i);
            }

            for (int i = 0; i < rand; i++)
            {
                int randomIndex = Random.Range(0, randomPrefabIndexes.Count);
                choseIndex[i] = randomPrefabIndexes[randomIndex];
                randomPrefabIndexes.RemoveAt(randomIndex);

            }

            int[] itemCounts = new int[rand];
            for (int i = 0; i < itemCounts.Length; i++)
            {
                itemCounts[i] = Random.Range(1, 15);
            }

            photonView.RPC(nameof(SetQuest), RpcTarget.AllBuffered, "택배포장", rand, choseIndex, itemCounts);
        }
    }

    [PunRPC]
    public void SetQuest(string questName,int count, int[] itemIndexes, int[] itemCounts)
    {
        currentQuest = new Quest
        {
            questName = questName,
            requiredItems = new List<RequiredItem>()
        };

        for (int i = 0; i < count; i++)
        {
            GameObject itemPrefab = itemPrefabs[itemIndexes[i]];            
            int requiredCount = itemCounts[i];
            currentQuest.requiredItems.Add(new RequiredItem(itemPrefab, requiredCount));
        }

        questsList.Add(currentQuest);
        UpdateUI();
    }

    public void NewQuest(string questName, GameObject[] itemPrefabs, int[] itemCounts)
    {
        /*currentQuest = new Quest(questName, itemPrefabs, itemCounts);

        questsList.Add(currentQuest);
        UpdateUI();*/
    }

   /* public void UpdateCount(int x, int y)
    {
        Debug.Log("업데이트");
        photonView.RPC(nameof(UpdateItemCount), RpcTarget.AllBuffered, x, y);
    }*/

   /* [PunRPC]
    public void UpdateItemCount(int x, int y)
    {
        Debug.Log(currentQuest.currentCount);
        questsList[x].requiredItems[y].count++;
        UpdateUI();

        foreach (Quest item in questsList)
        {
            for (int i = 0; i < item.requiredItems.Count; i++)
            {

            }
        }
        if (currentQuest.currentCount >= currentQuest.requiredItems.Count)
        {
            QuestComplete();
        }
    }*/

   /* [PunRPC]
    public void ExitItemCount()
    {
        currentQuest.currentCount--;
        Debug.Log(currentQuest.currentCount);
        UpdateUI();

        *//*if (currentQuest.currentCount >= currentQuest.requiredCount)
        {
            QuestComplete();
        }*//*
    }*/

    private void UpdateUI()
    {
        /*UIManager.Instance.UpdateQuestUI(currentQuest.questName, currentQuest.currentCount, currentQuest.requiredCount);*/
    }

    public void CountUpdate(int id, int number, int count)
    {
        Debug.Log("카운트 업데이트");
        photonView.RPC(nameof(CountCheck), RpcTarget.AllBuffered, id, number, count);
    }

    [PunRPC]
    private void CountCheck(int id, int number, int count)
    {
        Debug.Log("카운트 감소");
        questsList[id].requiredItems[number].requiredcount -= count;

        if (questsList[id].requiredItems[number].requiredcount <= 0)
        {
            Debug.Log("납품완료");
            QuestManager.Instance.SuccessQuest(id, number);
        }
    }

    public void SuccessQuest(int id, int number)
    {
        Debug.Log("퀘스트 완료!");
        photonView.RPC(nameof(SuccessCheck), RpcTarget.AllBuffered, id, number);
    }

    [PunRPC]
    private void SuccessCheck(int id, int number)
    {
        Debug.Log("퀘스트 성공 여부 동기화!");
        questsList[id].requiredItems[number].isSuccess = true;

        foreach (QuestManager.Quest list in questsList)
        {
            for (int i = 0; i < list.requiredItems.Count; i++)
            {
                //if (list.requiredItems[i].isSuccess)
            }
        }
    }

    public bool IsQuestComplete()
    {
        /*return currentQuest != null && currentQuest.currentCount >= currentQuest.requiredCount;*/
        return true;
    }
}