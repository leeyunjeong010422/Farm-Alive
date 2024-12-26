using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taping : MonoBehaviour
{
    private BoxTrigger currentBox;
    private bool isTaping = false;
    private float tapeProgress = 0f;
    public float requiredTapeProgress = 1f;

    private void Update()
    {
        if (isTaping && currentBox != null)
        {
            BoxTaping();
        }
    }

    public void StartTaping(BoxTrigger box)
    {
        if (box == null || !box.IsCoverClosed()) return;

        currentBox = box;
        isTaping = true;
        tapeProgress = 0f;
        Debug.Log($"테이핑 시작: {box.name}");
    }

    public void StopTaping()
    {
        isTaping = false;
        currentBox = null;
        Debug.Log("테이핑 중단");
    }

    private void BoxTaping()
    {
        if (Input.GetMouseButton(0))
        {
            tapeProgress += Time.deltaTime;
            Debug.Log($"테이핑 진행 중: {tapeProgress / requiredTapeProgress * 100}%");

            if (tapeProgress >= requiredTapeProgress)
            {
                CompleteTaping();
            }
        }
    }

    private void CompleteTaping()
    {
        isTaping = false;
        if (currentBox != null)
        {
            currentBox.SealBox();
            Debug.Log($"테이핑 완료: {currentBox.name}");
        }
        currentBox = null;
    }
}