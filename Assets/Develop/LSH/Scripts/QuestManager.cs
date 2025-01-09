using GameData;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
using UnityEngine.UIElements;
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
        public float questTimer;
    }

    [SerializeField] public List<Quest> questsList = new List<Quest>();
    [SerializeField] public List<TruckQuest> truckList = new List<TruckQuest>();

    [SerializeField] public GameObject[] itemPrefabs;
    [SerializeField] public Quest currentQuest;
    [SerializeField] TruckController truckController;
    [SerializeField] QuestController questController;

    [SerializeField] public int maxRequiredCount;
    [SerializeField] public int questCount;
    [SerializeField] public int itemTypeCount;
    [SerializeField] public int clearQuestCount;
    [SerializeField] public int totalQuestCount;

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
        int stageID = 511;
        if (questsList.Count < 4)
        {
            totalQuestCount = CSVManager.Instance.Stages_Correspondents[stageID].stage_corCount;
            photonView.RPC(nameof(QuestStart), RpcTarget.AllBuffered, stageID);
        }
    }

    [PunRPC]
    public void QuestStart(int stageID)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < CSVManager.Instance.Stages_Correspondents[stageID].stage_corCount; i++)
            {
                maxRequiredCount = CSVManager.Instance.Correspondents_CropsCount[i].correspondent_stage[stageID];
                Debug.Log($"max아이템갯수는 : {maxRequiredCount}");

                int rand = CSVManager.Instance.Stages_Correspondents[stageID].stage_corCount;

                List<int> randomPrefabIndexes = new List<int>();
                int[] choseIndex = new int[rand];

                // 아이템 목록화
                for (int j = 0; j < 3; j++)
                {
                    randomPrefabIndexes.Add(j);
                }

                // 아이템 개수 설정
                int checkItemLength = 0;
                int[] curItemCounts = new int[CSVManager.Instance.Correspondents_CropsType[i].correspondent_stage[stageID]];
                int curCount = 0;
                for (int j = 0; j < curItemCounts.Length; j++)
                {
                    curItemCounts[j] = Random.Range(1, 8);
                    curCount += curItemCounts[j];
                    checkItemLength++;
                    if (curCount >= maxRequiredCount)
                    {
                        int deleteCount = curItemCounts.Max();
                        curCount -= maxRequiredCount;

                        for (int a = 0; a < curItemCounts.Length; a++)
                        {
                            if (curItemCounts[a] == deleteCount)
                            {
                                if (curItemCounts[a] > curCount)
                                {
                                    curItemCounts[a] -= curCount;
                                }
                            }
                        }
                        break;
                    }

                    if (j == curItemCounts.Length - 1 && curCount < maxRequiredCount)
                    {
                        int temp = maxRequiredCount - curCount;
                        curItemCounts[j] += temp;
                    }
                }

                int[] itemCounts = new int[checkItemLength];
                for (int j = 0; j < itemCounts.Length; j++)
                {
                    itemCounts[j] = curItemCounts[j];
                }

                // 아이템 선정
                for (int j = 0; j < itemCounts.Length; j++)
                {
                    int randomIndex = Random.Range(0, randomPrefabIndexes.Count);
                    choseIndex[j] = randomPrefabIndexes[randomIndex];
                    randomPrefabIndexes.RemoveAt(randomIndex);
                }

                int corID = CSVManager.Instance.Stages_Correspondents[stageID].stage_corList[i];
                float qTimer = CSVManager.Instance.Correspondents[corID].correspondent_timeLimit;

                photonView.RPC(nameof(SetQuest), RpcTarget.AllBuffered, "택배포장", itemCounts.Length, choseIndex, itemCounts, corID, qTimer);
            }

        }
    }

    [PunRPC]
    public void SetQuest(string questName, int count, int[] itemIndexes, int[] itemCounts, int corID, float qTimer)
    {
        currentQuest = new Quest
        {
            questName = questName,
            requiredItems = new List<RequiredItem>(),
            questTimer = qTimer
        };

        for (int i = 0; i < count; i++)
        {
            int y = CSVManager.Instance.Correspondents_RequireCrops[corID].correspondent_cropID[itemIndexes[i]];
            y = 4 * ((y % 100 - y % 10) / 10 - 1) + y % 10;


            GameObject itemPrefab = itemPrefabs[y - 1];
            int requiredCount = itemCounts[i];
            currentQuest.requiredItems.Add(new RequiredItem(itemPrefab, requiredCount));
        }

        questsList.Add(currentQuest);

        StartCoroutine(questController.QuestCountdown(currentQuest));
        

        if (PhotonNetwork.IsMasterClient)
            truckController.CreateTruck(corID);
    }

    public void UpdateUI()
    {
        UIManager.Instance.UpdateQuestUI(questsList);
    }

    public void CountUpdate(int[] questId, int[] number, int[] count, int boxView)
    {
        Debug.Log("카운트 업데이트");
        photonView.RPC(nameof(CountCheck), RpcTarget.AllBuffered, questId, number, count, boxView);
    }

    [PunRPC]
    private void CountCheck(int[] questId, int[] number, int[] count, int boxView)
    {
        Debug.Log("카운트 감소");

        for (int i = 0; i < questId.Length; i++)
        {
            questsList[questId[i]].requiredItems[number[i]].requiredcount -= count[i];

            if (questsList[questId[i]].requiredItems[number[i]].requiredcount <= 0)
            {
                Debug.Log("납품완료");
                Debug.Log("퀘스트 성공 여부 동기화!");
                questsList[questId[i]].requiredItems[number[i]].isSuccess = true;
            }
        }

        List<int> completedIndexes = new List<int>();
        for (int i = 0; i < questsList.Count; i++)
        {
            bool allCompleted = true;

            for (int j = 0; j < questsList[i].requiredItems.Count; j++)
            {
                if (!questsList[i].requiredItems[j].isSuccess)
                {
                    allCompleted = false;
                    break;
                }
            }

            if (allCompleted)
            {
                questsList[i].isSuccess = true;
                completedIndexes.Add(i);
            }
        }

        PhotonView box = PhotonView.Find(boxView);
        if (box != null && box.IsMine)
        {
            PhotonNetwork.Destroy(box.gameObject);
        }

        if (completedIndexes.Count > 0)
        {
            int[] listArray = completedIndexes.ToArray();

            IsQuestComplete(listArray);
        }

        UpdateUI();
    }

    public void IsQuestComplete(int[] completedIndexes)
    {
        foreach (int index in completedIndexes.OrderByDescending(x => x))
        {
            //questsList.RemoveAt(index);
            PhotonNetwork.Destroy(truckList[index].gameObject);
        }

        if (questsList.Count == 0)
        {
            // TODO 로비 복귀ㅣ 함수
        }
        else
        {
            UpdateUI();
        }
    }
}