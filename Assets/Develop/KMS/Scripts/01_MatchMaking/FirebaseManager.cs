using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Threading.Tasks;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; } // 싱글톤

    private FirebaseApp app;
    public static FirebaseApp App { get { return Instance.app; } }

    private Firebase.Auth.FirebaseAuth auth;
    public static Firebase.Auth.FirebaseAuth Auth { get { return Instance.auth; } }

    private FirebaseDatabase dataBase;
    public static FirebaseDatabase DataBase { get { return Instance.dataBase; } }

    private string userId; // Firebase UID

    public event Action OnFirebaseInitialized; // Firebase 초기화 완료 이벤트

    private string _highStage = "";

    private string _nickName;
    public class StageData
    {
        public int stars;
        public float playTime;

        public StageData(int stars, float playTime)
        {
            this.stars = stars;
            this.playTime = playTime;
        }
    }
    private Dictionary<int, StageData> cachedStageData = new Dictionary<int, StageData>();


    /// <summary>
    /// 플레이어의 저장된 UID를 호출.
    /// </summary>
    /// <returns></returns>
    public string GetUserId()
    {
        if (string.IsNullOrEmpty(userId))
        {
            userId = PlayerPrefs.GetString("firebase_uid", string.Empty);
        }
        return userId;
    }

    private void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeFirebase();
    }

    /// <summary>
    /// 파이어베이스 초기화 메서드.
    /// </summary>
    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                dataBase = FirebaseDatabase.DefaultInstance;

                Debug.Log("Firebase 초기화 완료!");
                CheckAndInitializeUserData();
            }
            else
            {
                Debug.LogError("Firebase 의존성 해결 실패!");
                app = null;
                auth = null;
                dataBase = null;
            }
        });
    }

    /// <summary>
    /// 익명 로그인 생성 메서드.
    /// </summary>
    private void AnonymouslyLogin()
    {
        ClearLocalUid();

        auth.SignOut();
        Debug.Log("기존 인증 로그아웃 완료. 새로운 익명 로그인 시도...");

        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("익명 로그인 작업이 취소되었습니다.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("익명 로그인 작업 중 오류 발생: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            if (result?.User == null)
            {
                Debug.LogError("로그인 결과에서 User 정보를 가져올 수 없습니다.");
                return;
            }

            userId = result.User.UserId; // Firebase UID 저장
            SaveUidLocally(userId); // UID를 로컬 저장
            Debug.LogFormat($"익명 로그인 성공! UID: {userId}");

            // uid 및 데이터 저장.
            SaveUserData();
            // 마지막 로그인 시간 저장.
            UpdateLastLogin();
        });
    }

    /// <summary>
    /// 유저 UID 검색 후 해당 데이터가 없을 시 익명 로그인으로 생성.
    /// </summary>
    private void CheckAndInitializeUserData()
    {
        string localUid = LoadUidFromLocal();

        if (!string.IsNullOrEmpty(localUid))
        {
            Debug.Log($"로컬 저장된 UID 발견: {localUid}");
            VerifyUserExistsInFirebase(localUid);
        }
        else
        {
            Debug.Log("로컬 UID가 없습니다. 새로 로그인 진행...");
            AnonymouslyLogin();
        }
    }

    /// <summary>
    /// 파이어베이스 데이터가 존재시 해당 아이디로 로그인
    /// 없을시 익명 로그인으로 실행하는 메서드.
    /// </summary>
    /// <param name="uid"></param>
    private void VerifyUserExistsInFirebase(string uid)
    {
        DatabaseReference userRef = dataBase.GetReference($"users/{uid}");

        userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Permission Denied 처리
                Debug.LogError("Firebase 접근 실패: " + task.Exception);
                Debug.Log("Permission Denied 또는 다른 오류 발생. 로컬 UID 초기화 후 새 로그인 진행...");
                AnonymouslyLogin();
                return;
            }

            if (task.IsCompleted && task.Result.Exists)
            {
                Debug.Log($"Firebase에서 UID {uid} 발견!");
                userId = uid;
                UpdateLastLogin();
                LoadNickName();
                LoadHighStage();
                NotifyInitializationComplete();
            }
            else
            {
                Debug.Log("Firebase에 해당 UID 없음. 로컬 UID 삭제 후 새 로그인 진행...");
                AnonymouslyLogin();
            }
        });
    }

    /// <summary>
    /// 생성시 파이어 베이스에 유저 데이터 저장 메서드.
    /// </summary>
    private void SaveUserData()
    {
        DatabaseReference userRef = dataBase.GetReference($"users/{userId}");

        Dictionary<string, object> userData = new Dictionary<string, object>()
        {
            { "uid", userId },
            { "nickname", "" },
            { "createdAt", DateTime.Now.ToString("o") },
            { "lastLogin", DateTime.Now.ToString("o") },
            { "settings", new Dictionary<string, object>()
                {
                    { "sound", 0 }
                }
            },
            { "highStage", _highStage },
            { "achievements", new List<string>() { "first_login" } }
        };

        userRef.SetValueAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("사용자 데이터 저장 완료!");
                UpdateLastLogin();
                NotifyInitializationComplete();
                return;
            }
            else
            {
                Debug.LogError("사용자 데이터 저장 실패: " + task.Exception);
            }
        });
    }

    /// <summary>
    /// 스테이지 결과 파이어 베이스에 저장 메서드. 
    /// </summary>
    public void SaveStageResult(int stageID, float playedTime, int starCount)
    {
        if (string.IsNullOrEmpty(userId))
            return;

        int stageIDX = CSVManager.Instance.Stages[stageID].idx + 1;
        string highstageID = "Stage" + stageIDX;
        
        DatabaseReference stageRef = dataBase.GetReference($"users/{userId}/stageResults/{stageID}");

        Dictionary<string, object> resultData = new Dictionary<string, object>()
        {
            { "stageID", stageID },
            { "playTime", playedTime },
            { "stars" , starCount },
            {"timeStamp" , DateTime.Now.ToString("o") },
        };

        stageRef.SetValueAsync(resultData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("스테이지 데이터 저장 완료!");
                return;
            }
            else
            {
                Debug.LogError("스테이지 데이터 저장 실패: " + task.Exception);
            }
        });

        dataBase.GetReference($"users/{userId}/highStage").SetValueAsync(highstageID).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("최고레벨 저장 완료!");
                return;
            }
            else
            {
                Debug.LogError("최고레벨 저장 실패: " + task.Exception);
            }
        });
    }

    /// <summary>
    /// Firebase에서 HighStage를 가져와 캐싱합니다.
    /// </summary>
    public void LoadHighStage()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("유저 ID가 없습니다. HighStage를 가져올 수 없습니다.");
            return;
        }

        DatabaseReference highStageRef = dataBase.GetReference($"users/{userId}/highStage");

        highStageRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result != null && task.Result.Value != null)
            {
                _highStage = task.Result.Value.ToString();
                Debug.Log($"HighStage 불러오기 성공: {_highStage}");

                // 캐싱 시작
                CacheStageData();
            }
            else
            {
                Debug.LogError("HighStage 불러오기 실패: " + task.Exception);
                _highStage = "Stage1";
            }
        });
    }

    /// <summary>
    /// 현재 저장된 HighStage를 반환합니다.
    /// </summary>
    public string GetHighStage()
    {
        if (string.IsNullOrEmpty(_highStage))
        {
            Debug.LogWarning("HighStage가 아직 로드되지 않았습니다.");
        }

        return _highStage;
    }

    /// <summary>
    /// stageID를 이용한 stars, playTime을 캐싱하기.
    /// </summary>
    private void CacheStageData()
    {
        if (!Enum.TryParse(_highStage, out E_StageMode highStageEnum))
        {
            Debug.LogError("HighStage를 파싱할 수 없습니다.");
            return;
        }

        // 비연속적이기에 Enum.GetValues를 사용하여 순회
        foreach (E_StageMode stage in Enum.GetValues(typeof(E_StageMode)))
        {
            // 유효한 스테이지만 처리
            if (stage == E_StageMode.None || stage == E_StageMode.SIZE_MAX || stage > highStageEnum)
            {
                continue;
            }

            int stageID = (int)stage;
            DatabaseReference stageRef = dataBase.GetReference($"users/{userId}/stageResults/{stageID}");

            stageRef.GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && task.Result != null && task.Result.HasChildren)
                {
                    int stars = 0;
                    float playTime = 0f;

                    if (task.Result.Child("stars").Value != null)
                    {
                        stars = int.Parse(task.Result.Child("stars").Value.ToString());
                    }

                    if (task.Result.Child("playTime").Value != null)
                    {
                        playTime = float.Parse(task.Result.Child("playTime").Value.ToString());
                    }

                    cachedStageData[stageID] = new StageData(stars, playTime);
                    Debug.Log($"Stage {stage} 데이터 캐싱 완료: Stars {stars}, PlayTime {playTime}");
                }
                else
                {
                    Debug.LogWarning($"Stage {stage} 데이터가 없습니다.");
                    cachedStageData[stageID] = new StageData(0, 0f);
                }
            });
        }
    }

    public StageData GetCachedStageData(int stageID)
    {
        if (cachedStageData.ContainsKey(stageID))
        {
            return cachedStageData[stageID];
        }
        Debug.LogWarning($"Stage {stageID} 데이터가 캐싱되지 않았습니다.");
        return null;
    }

    public void SaveNickName(string playerName)
    {
        if (string.IsNullOrEmpty(userId)) return;

        DatabaseReference nickNameRef = dataBase.GetReference($"users/{userId}/nickname");

        nickNameRef.SetValueAsync(playerName).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"닉네임 '{playerName}' 저장 완료!");
                // 닉네임 캐싱.
                _nickName = playerName;
            }
            else
            {
                Debug.LogError($"닉네임 저장 실패: {task.Exception}");
            }
        });

    }

    /// <summary>
    /// Firebase에서 닉네임을 가져와 캐싱합니다.
    /// </summary>
    public void LoadNickName()
    {
        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("유저 ID가 없습니다. 닉네임을 불러올 수 없습니다.");
            return;
        }

        DatabaseReference nickNameRef = dataBase.GetReference($"users/{userId}/nickname");

        nickNameRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result != null && task.Result.Value != null)
            {
                _nickName = task.Result.Value.ToString();
                Debug.Log($"닉네임 불러오기 성공: {_nickName}");
            }
            else
            {
                Debug.LogError("닉네임 불러오기 실패: " + task.Exception);
                _nickName = null;
            }
        });
    }

    /// <summary>
    /// 현재 저장된 닉네임을 반환합니다.
    /// </summary>
    public string GetNickName()
    {
        if (string.IsNullOrEmpty(_nickName))
        {
            Debug.LogWarning("닉네임이 아직 로드되지 않았습니다.");
        }

        return _nickName;
    }

    /// <summary>
    /// 최근 로그인한 기록을 저장하는 메서드.
    /// </summary>
    public void UpdateLastLogin()
    {
        Debug.Log("캐릭터 마지막 로그인 갱신!");
        Debug.Log("지금은 데이터를 잠시 갱신안함 - (코드 막아둠!)");

        DatabaseReference userRef = dataBase.GetReference($"users/{userId}/lastLogin");

        userRef.SetValueAsync(DateTime.Now.ToString("o")).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("마지막 로그인 시간 업데이트 성공!");
            }
            else
            {
                Debug.LogError("마지막 로그인 시간 업데이트 실패: " + task.Exception);
            }
        });
    }

    /// <summary>
    /// 로그인 완료 시작을 알리는 메서드.
    /// </summary>
    public void NotifyInitializationComplete()
    {
        OnFirebaseInitialized?.Invoke();
    }

    /// <summary>
    /// 로컬에 유저의 UID를 저장.
    /// </summary>
    /// <param name="uid"></param>
    private void SaveUidLocally(string uid)
    {
        PlayerPrefs.SetString("firebase_uid", uid);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 로컬에 저장된 UID를 불러오는 메서드.
    /// 없을시 Empty로 반환.
    /// </summary>
    /// <returns></returns>
    private string LoadUidFromLocal()
    {
        return PlayerPrefs.GetString("firebase_uid", string.Empty);
    }

    /// <summary>
    /// 로컬에 저장된 UID를 삭제하는 메서드.
    /// </summary>
    private void ClearLocalUid()
    {
        PlayerPrefs.DeleteKey("firebase_uid");
        PlayerPrefs.Save();
        userId = string.Empty;
    }
}
