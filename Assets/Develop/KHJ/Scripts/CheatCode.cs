using Photon.Pun;
using UnityEngine;

public class CheatCode : MonoBehaviour
{
    // 모든 작물 즉시 성장
    public void GrowAllCrops()
    {
        GameObject[] cropGameObjects = GameObject.FindGameObjectsWithTag("Crop");
        foreach (var item in cropGameObjects)
        {
            item.GetComponent<Crop>().CompleteGrowth();
        }
    }

    public void HarvestAllCrops()
    {
        GameObject[] cropGameObjects = GameObject.FindGameObjectsWithTag("Crop");
        foreach (var item in cropGameObjects)
        {
            var interactors = item.GetComponent<CropInteractable>().interactorsSelecting;
            interactors.Clear();
        }
    }

    public void ExitGame()
    {
        GameSpawn gameSpawn = FindObjectOfType<GameSpawn>();
        gameSpawn.ReturnToFusion();
    }
}
