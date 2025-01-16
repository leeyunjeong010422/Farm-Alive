using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcTextView : MonoBehaviour
{
    [SerializeField] GameObject textPanel;
    [SerializeField] public Text text;
    private float outputTime = 5f;
    private float outputSpeed = 0.05f;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] soundList;

    private Coroutine textCoroutine;

    public void NpcText()
    {
        //text.text = "거북이 조상님이 와도\n이거보다는 빠르겠어!!";
        ShowText("거북이 조상님이 와도\n이거보다는 빠르겠어!!");
        anim.SetBool("isField", true);
        SoundManager.Instance.PlaySFX("SFX_NPCFail");
    }

    public void NpcText(bool check)
    {
        if (check)
        {
            //text.text = "완벽하군!!\n 다음에도 잘 부탁하겠네!";
            ShowText("완벽하군!!\n 다음에도 잘 부탁하겠네!");
            anim.SetBool("isSuccess", true);
            SoundManager.Instance.PlaySFX("SFX_NPCSuccess");
        }
        else
        {
            //text.text = "내가 원하던 작물이 아니잖아!!\n일을 어떻게 하는거야!";
            ShowText("내가 원하던 작물이 아니잖아!!\n일을 어떻게 하는거야!");
            anim.SetBool("isField", true);
            SoundManager.Instance.PlaySFX("SFX_NPCWrongCrop");
        }
    }

    public void NpcText(int count)
    {
        if (count > 0)
        {
            //text.text = "흠 주문하지 않은 작물이 섞여있는데\n이건 선물인가?\n 고맙게 잘 받겠네";
            ShowText("흠 주문하지 않은 작물이 섞여있는데\n이건 선물인가?\n 고맙게 잘 받겠네");
            anim.SetBool("isSuccess", true);
            SoundManager.Instance.PlaySFX("SFX_NPCManyCrop");
        }

        if (count <= 0)
        {
            //text.text = "고맙네\n남은 작물들도 빠르게 부탁하겠네!";
            ShowText("고맙네\n남은 작물들도 빠르게 부탁하겠네!");
            SoundManager.Instance.PlaySFX("SFX_NPCCorrect");
        }
    }

    private void ShowText(string npcText)
    {
        if (textCoroutine != null)
        {
            StopCoroutine(textCoroutine);
        }

        textPanel.SetActive(true);
        textCoroutine = StartCoroutine(TypingText(npcText));
        StartCoroutine(ShowPanelTime(outputTime));
    }

    private IEnumerator TypingText(string npcText)
    {
        text.text = "";
        foreach (char textcul in npcText)
        {
            text.text += textcul;
            yield return new WaitForSeconds(outputSpeed);
        }
    }

    private IEnumerator ShowPanelTime(float outputTiem)
    {
        yield return new WaitForSeconds(outputTiem);

        anim.SetBool("isSuccess", false);
        anim.SetBool("isField", false);
        textPanel.SetActive(false);
    }
}