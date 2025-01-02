using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    [Tooltip("이동할 씬 이름을 저장하는 정적 변수")]
    public static string TargetScene { get; set; }

    /// <summary>
    /// 로딩씬으로 전환시 이동할 씬 이름을 설정
    /// </summary>
    /// <param name="targetSceneName">이동할 씬 이름</param>
    public static void LoadSceneWithLoading(string targetSceneName)
    {
        TargetScene = targetSceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    /// <summary>
    /// 네트워크 씬 전환: PhotonNetwork를 통해 모든 클라이언트 이동
    /// </summary>
    public static void LoadNetworkSceneWithLoading(string targetSceneName)
    {
        PhotonNetwork.LoadLevel(targetSceneName);
    }
}
