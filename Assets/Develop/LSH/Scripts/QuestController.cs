using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public void MaxItemCountReturn(int stageLevel, out int questCount, out int itemTypeCount, out int itemCount)
    {
        switch (stageLevel)
        {
            case 1:
                questCount = 1;
                itemTypeCount = 1;
                itemCount = 10;
                break;
            case 2:
                questCount = 1;
                itemTypeCount = 1;
                itemCount = 15;
                break;
            case 3:
                questCount = 1;
                itemTypeCount = 1;
                itemCount = 20;
                break;
            case 4:
                questCount = 1;
                itemTypeCount = 1;
                itemCount = 25;
                break;
            case 5:
                questCount = 2;
                itemTypeCount = 1;
                itemCount = 10;
                break;
            case 6:
                questCount = 2;
                itemTypeCount = 1;
                itemCount = 10;
                break;
            case 7:
                questCount = 2;
                itemTypeCount = 1;
                itemCount = 10;
                break;
            case 8:
                questCount = 5;
                itemTypeCount = 1;
                itemCount = 10;
                break;
            case 9:
                questCount = 5;
                itemTypeCount = 1;
                itemCount = 10;
                break;
            case 10:
                questCount = 5;
                itemTypeCount = 1;
                itemCount = 10;
                break;
            default:
                questCount = -1;
                itemTypeCount = -1;
                itemCount = -1;
                break;
        }
    }
}
