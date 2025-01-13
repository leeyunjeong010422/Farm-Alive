using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropStateUpdater : MonoBehaviour
{
    public enum E_CropStateUI
    {
        Empty, Stopped, Growing, Completed, Damaged, SIZE
    }

    public int section;
    public int ground;

    [SerializeField] TabletUIController tabletUIController;
    [SerializeField] GameObject[] _cropStateImages;
    [SerializeField] Dictionary<int, GameObject> _cropImagesDict;

    private void Start()
    {
        StageManager.Instance.OnGameStarted.AddListener(SubscribeGround);

        _cropStateImages = new GameObject[(int)E_CropStateUI.SIZE];
        for (int i = 0; i < _cropStateImages.Length; i++)
        {
            _cropStateImages[i] = transform.GetChild(0).GetChild(i).gameObject;
        }

        _cropImagesDict = new Dictionary<int, GameObject>();
        for (int i = 0; i < CSVManager.Instance.cropDatas.Count; i++)
        {
            _cropImagesDict.Add(CSVManager.Instance.cropDatas[i].ID, transform.GetChild(1).GetChild(i).gameObject);
        }
    }

    private void SubscribeGround()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject gameObject in gameObjects)
        {
            PlantGround plantGround = gameObject.GetComponent<PlantGround>();

            // 표시할 대응되는 경작지의 작물 상태 갱신 이벤트 구독
            if (plantGround.section == section && plantGround.ground == ground)
            {
                Debug.Log($"{section + 1}, {ground + 1} 구독");
                plantGround.OnMyPlantUpdated.AddListener(UpdateCropState);

                if (section == SectionManager.Instance.SectionNum - 1 && ground == SectionManager.Instance.GroundPerSection - 1)
                {
                    tabletUIController.ReturnToMainPanel();
                }
            }
        }
    }

    private void UpdateCropState(int cropID, Crop.E_CropState cropState)
    {
        // Crop Image
        ChangeCropImage(cropID);

        // Crop States
        switch (cropState)
        {
            case Crop.E_CropState.Growing:
                ChangeStateImage(E_CropStateUI.Growing);
                break;
            case Crop.E_CropState.GrowStopped:
                ChangeStateImage(E_CropStateUI.Stopped);
                break;
            case Crop.E_CropState.GrowCompleted:
                ChangeStateImage(E_CropStateUI.Completed);
                break;
            case Crop.E_CropState.Waste:
                ChangeStateImage(E_CropStateUI.Damaged);
                break;
            case Crop.E_CropState.SIZE:
                ChangeStateImage(E_CropStateUI.Empty);
                break;
            default:
                break;
        }
    }

    private void ChangeStateImage(E_CropStateUI cropStateUI)
    {
        foreach (var stateImage in _cropStateImages)
        {
            stateImage.SetActive(stateImage.name == cropStateUI.ToString());
        }
    }

    private void ChangeCropImage(int cropID)
    {
        foreach (var cropImage in _cropImagesDict)
        {
            cropImage.Value.SetActive(cropID == cropImage.Key);
        }
    }
}
