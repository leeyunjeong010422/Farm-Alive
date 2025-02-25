using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    [System.Serializable]
    public struct CROP
    {
        public int crop_ID, crop_maxShovelCount,
                   crop_droughtMaxWaterCount, crop_droughtMaxNutrientCount;
        public float crop_maxWaterCount, crop_maxNutrientCount, crop_maxGrowthTime,
                     crop_damagePercent, crop_damageTime, crop_lowTemperTime, crop_highTemperTime;
        public string crop_Name;
    }

    [System.Serializable]
    public struct FACILITY
    {
        public int facility_maxHammeringCount, facility_maxBootCount;
        public float facility_symptomPercent, facility_stormSymptomPercent, facility_timeLimit;
        public string facility_name;
    }

    [System.Serializable]
    public struct CORRESPONDENT
    {
        public string correspondent_name;
        public float correspondent_timeLimit;
    }

    [System.Serializable]
    public struct CORRESPONDENT_REQUIRECROPS
    {
        public int[] correspondent_cropID;
    }

    [System.Serializable]
    public struct CORRESPONDENT_CROPSTYPECOUNT
    {
        public int[] correspondent_stage;
    }

    [System.Serializable]
    public struct CORRESPONDENT_CROPSCOUNT
    {
        public int[] correspondent_stage;
    }

    [System.Serializable]
    public struct EVENT
    {
        public string event_name;
        public float event_occurPercent, event_occurPlusPercent, event_continueTime;
    }

    [System.Serializable]
    public struct EVENT_SEASON
    {
        public int[] event_seasonID;
    }

    [System.Serializable]
    public struct STAGE
    {
        public int idx, stage_seasonID, stage_allowSymptomFacilityCount;
    }

    [System.Serializable]
    public struct STAGE_CORRESPONDENT
    {
        public int stage_corCount;
        public int[] stage_corList;
    }
}