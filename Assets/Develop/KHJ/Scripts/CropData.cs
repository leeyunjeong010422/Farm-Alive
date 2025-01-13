using UnityEngine;

[CreateAssetMenu(fileName = "Crop Data", menuName = "Scriptable Object/Crop Data", order = int.MaxValue)]
public class CropData : ScriptableObject
{
    public int ID;
    public string cropName;
    public int digCount;
    public int maxMoisture;
    public int maxNutrient;
    public float growthTime;
    public int droughtMaxMoisture;
    public int droughtMaxNutrient;
    public float damageRate;
    public float damageLimitTime;
    public float temperatureDecreaseLimitTime;
    public float temperatureIncreaseLimitTime;

    public Sprite image;
}