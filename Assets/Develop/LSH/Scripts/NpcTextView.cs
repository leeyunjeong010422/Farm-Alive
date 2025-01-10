using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcTextView : MonoBehaviour
{
    [SerializeField] Transform myCamera;
    [SerializeField] public Text text;

    private void OnEnable()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PhotonView photonView = player.GetComponent<PhotonView>();

            if (photonView != null && photonView.IsMine)
            {
                myCamera = player.transform.Find("Main Camera");
                break;
            }
        }
    }
    private void Update()
    {
        if (myCamera != null)
        {
            this.transform.LookAt(myCamera);
        }
    }

    public void NpcText()
    {
        text.text = "내가 원하던 작물이 아니잖아!!\n일을 어떻게 하는거야!";
    }

    public void NpcText(int count)
    {
        if (count > 0)
            text.text = "흠 주문하지 않은 작물이 섞여있는데\n이건 선물인가?\n 고맙게 잘 받겠네";

        if (count <= 0)
            text.text = "완벽하군!!\n다음에도 잘 부탁하겠네!";
    }
}