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

    [SerializeField] GameObject[] _images;

    private void Start()
    {
        StageManager.Instance.OnGameStarted.AddListener(SubscribeGround);

        _images = new GameObject[(int)E_CropStateUI.SIZE];
        for (int i = 0; i < _images.Length; i++)
        {
            _images[i] = transform.GetChild(i).gameObject;
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
                plantGround.OnMyPlantUpdated.AddListener(UpdateCropState);
        }
    }

    private void UpdateCropState(Crop.E_CropState cropState)
    {
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
        foreach (var image in _images)
        {
            image.SetActive(image.name == cropStateUI.ToString());
        }
    }
}
