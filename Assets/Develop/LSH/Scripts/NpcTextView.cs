using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcTextView : MonoBehaviour
{
    [SerializeField] public Text text;

    public void NpcText()
    {
            text.text = "거북이 조상님이 와도\n이거보다는 빠르겠어!!";
    }

    public void NpcText(bool check)
    {
        if (check)
        {
            text.text = "완벽하군!!\n 다음에도 잘 부탁하겠네!";
        }
        else
        {
            text.text = "내가 원하던 작물이 아니잖아!!\n일을 어떻게 하는거야!";
        }
    }

    public void NpcText(int count)
    {
        if (count > 0)
            text.text = "흠 주문하지 않은 작물이 섞여있는데\n이건 선물인가?\n 고맙게 잘 받겠네";

        if (count <= 0)
            text.text = "고맙네\n남은 작물들도 빠르게 부탁하겠네!";
    }
}