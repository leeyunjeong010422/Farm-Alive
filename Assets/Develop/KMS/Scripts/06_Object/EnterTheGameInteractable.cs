using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EnterTheGameInteractable : XRGrabInteractable
{
    [Header("이동할 씬 이름")]
    public string targetSceneName = "AssetScene";

    private PhotonView _photonView;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    protected override void Awake()
    {
        base.Awake();

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        _photonView = GetComponent<PhotonView>();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("게임시작 문이 선택 되었습니다.");

        if(PhotonNetwork.IsMasterClient)
        {
            Debug.Log("마스터 클라언트 씬로드가 선택되었습니다.");
            StartCoroutine(GameStartCountDown(5));
        }
        else
        {
            MessageDisplayManager.Instance.ShowMessage($"방장만 선택이 가능합니다.", 1f, 3f);
        }
    }

    private IEnumerator GameStartCountDown(float countdown)
    {
        float remainingTime = countdown;
        while (remainingTime > 0)
        {
            // 메시지 갱신
            _photonView.RPC("DisplayMessageRPC", RpcTarget.All, $"{(int)remainingTime}");

            Debug.Log($"After {(int)remainingTime} seconds, you enter the room.");
            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        Debug.Log("게임 씬 이동 중...");
        SceneLoader.LoadNetworkSceneWithLoading(targetSceneName);
    }

    [PunRPC]
    public void DisplayMessageRPC(string message)
    {
        MessageDisplayManager.Instance.ShowMessage($"{message}초 후, 게임으로 입장 합니다.", 1f, 3f);
    }


}
