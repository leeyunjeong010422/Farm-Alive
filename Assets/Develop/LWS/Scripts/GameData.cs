using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [System.Serializable]
    public struct CROP
    {
        public int cropID, maxShovelCount, maxWaterCount, maxNutrientCount,
                   droughtMaxWaterCount, droughtMaxNutrientCount;
        public float maxGrowthTime, damagePercent, damageTime, lowTemperTime, highTemperTime;
        public string cropName;
    }
}