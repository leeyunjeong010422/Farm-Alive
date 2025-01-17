using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnToLobbyInteractable : XRGrabInteractable
{
    [Header("카운트 다운")]
    public float countdownTime = 3f;

    [Header("안내문 텍스트 오브젝트")]
    public TMP_Text text;


    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Transform player;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        text.text = "로비로 돌아갈 시 문을 Select 하세요.";
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        SoundManager.Instance.PlaySFX("SFX_Lobby_CropSelected");
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 선택 되었습니다.");
#endif
        StartCoroutine(ReturnToLobby(args.interactableObject.transform.gameObject, countdownTime));
    }

    private IEnumerator ReturnToLobby(GameObject targetObject, float countdown)
    {
        float remainingTime = countdown;

        if (targetObject)
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        while (remainingTime > 0)
        {
            MessageDisplayManager.Instance.ShowMessage($"{(int)remainingTime} 초 후 , 로비로 이동 합니다..", 1f, 3f);
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        PunPlayerSpawn pps = FindObjectOfType<PunPlayerSpawn>();
        if (pps)
            pps.ReturnToFusion();
        else
            Debug.LogWarning("pps가 없습니다.");
    }
}
