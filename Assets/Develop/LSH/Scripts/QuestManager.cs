using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;
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
    [SerializeField] public Quest currentQuest;
    [SerializeField] public int maxItemCount;
    [SerializeField] List<int> listNums = new List<int>();

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
        maxItemCount = 0;
        if (PhotonNetwork.IsMasterClient)
        {
            //int rand = Random.Range(1, itemPrefabs.Length);
            int rand = Random.Range(2, 3);

            List<int> randomPrefabIndexes = new List<int>();
            int[] choseIndex = new int[rand];

            // 아이템 목록화
            for (int i = 0; i < itemPrefabs.Length; i++)
            {
                randomPrefabIndexes.Add(i);
            }

            // 아이템 개수 설정
            int checkItemLength = 0;
            int[] maxItemCounts = new int[rand];
            for (int i = 0; i < maxItemCounts.Length; i++)
            {
                //maxItemCounts[i] = Random.Range(1, 15);
                maxItemCounts[i] = 1;
                maxItemCount += maxItemCounts[i];
                checkItemLength++;
                if (maxItemCount >= 30)
                {
                    int deleteCount = maxItemCounts.Max();
                    maxItemCount -= 30;

                    for (int j = 0; j < maxItemCounts.Length; j++)
                    {
                        if (maxItemCounts[j] == deleteCount)
                        {
                            if (maxItemCounts[j] > maxItemCount)
                            {
                                maxItemCounts[j] -= maxItemCount;
                            }
                        }
                    }
                    break;
                }

            }

            int[] itemCounts = new int[checkItemLength];
            for (int i = 0; i < itemCounts.Length; i++)
            {
                itemCounts[i] = maxItemCounts[i];
            }

            // 아이템 선정
            for (int i = 0; i < itemCounts.Length; i++)
            {
                int randomIndex = Random.Range(0, randomPrefabIndexes.Count);
                choseIndex[i] = randomPrefabIndexes[randomIndex];
                randomPrefabIndexes.RemoveAt(randomIndex);

            }

            photonView.RPC(nameof(SetQuest), RpcTarget.AllBuffered, "택배포장", itemCounts.Length, choseIndex, itemCounts);
        }
    }

    [PunRPC]
    public void SetQuest(string questName, int count, int[] itemIndexes, int[] itemCounts)
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

    private void UpdateUI()
    {
        UIManager.Instance.UpdateQuestUI(questsList, questsList.Count - 1);
    }

    public void CountUpdate(int[] id, int[] number, int[] count)
    {
        Debug.Log("카운트 업데이트");
        photonView.RPC(nameof(CountCheck), RpcTarget.AllBuffered, id, number, count);
    }

    [PunRPC]
    private void CountCheck(int[] id, int[] number, int[] count)
    {
        Debug.Log("카운트 감소");

        for (int i = 0; i < id.Length; i++)
        {
            questsList[id[i]].requiredItems[number[i]].requiredcount -= count[i];
        }
        //questsList[id].requiredItems[number].requiredcount -= count;

        for (int i = 0; i < id.Length; i++)
        {
            if (questsList[id[i]].requiredItems[number[i]].requiredcount <= 0)
            {
                Debug.Log("납품완료");
                Debug.Log("퀘스트 성공 여부 동기화!");
                questsList[id[i]].requiredItems[number[i]].isSuccess = true;
            }
        }

        /* if (questsList[id].requiredItems[number].requiredcount <= 0)
         {
             */
        //SuccessQuest(id, number);

        //questsList[id].requiredItems[number].isSuccess = true;

        int listNum = 0;
        
        foreach (QuestManager.Quest list in questsList)
        {
            for (int i = 0; i < list.requiredItems.Count; i++)
            {
                if (list.requiredItems[i].isSuccess == false)
                    break;

                Debug.Log($"i의 값 : {i} Count의 값 : {list.requiredItems.Count}");
                if (i == list.requiredItems.Count - 1)
                {
                    Debug.Log("퀘스트 완료");
                    list.isSuccess = true;
                    listNums.Add(listNum);
                    Debug.Log($"{listNums.Count}");
                    Debug.Log($"{listNums[0]}");
                }
            }
            listNum++;
        }

        if (listNums.Count != 0)
        {
            int[] listArray = listNums.ToArray();

            IsQuestComplete(listArray);
        }

        UpdateUI();
        //  }
    }

    public void IsQuestComplete(int[] listNums)
    {
        for (int i = 0; i < listNums.Length; i++)
        {
            questsList.RemoveAt(listNums[i]);
        }

        if (questsList.Count == 0)
        {
            //GameSpawn gameSpawn = FindObjectOfType<GameSpawn>();
            //gameSpawn.ReturnToFusion();
            //SceneLoader.LoadSceneWithLoading("03_FusionLobby");
        }

        UpdateUI();
    }
}