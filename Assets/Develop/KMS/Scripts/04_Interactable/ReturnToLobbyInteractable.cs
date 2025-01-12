using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ReturnToLobbyInteractable : XRGrabInteractable
{
    public float countdownTime = 3f;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private Transform player;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
#if UNITY_EDITOR
        Debug.Log($"{args.interactableObject.transform.name}가 선택 되었습니다.");
#endif
        var player = args.interactorObject.transform;
        var playerPhotonView = player.GetComponentInParent<PhotonView>();

        if (playerPhotonView != null)
        {
            Debug.Log($"이벤트를 동작시킨 플레이어: {playerPhotonView.Owner.NickName} (ID: {playerPhotonView.Owner.ActorNumber})");
        }
        else
        {
            Debug.LogWarning("PhotonView를 찾을 수 없습니다. 로컬 플레이어가 아닐 수 있습니다.");
        }

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
            Debug.LogWarning("pps가 없음!");
    }
}
