using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameData;
using Unity.Collections.LowLevel.Unsafe;

public class CSVManager : MonoBehaviour
{
    const string cropCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=815678089&format=csv"; // 작물 시트
    const string facilityCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1924872652&format=csv"; // 시설 시트
    const string correspondentCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1446733082&format=csv"; // 거래처 시트
    const string corNeedCropCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1954559638&format=csv"; // 거래처 요구작물 시트
    const string corCropTypeCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1014353564&format=csv"; // 거래처별 작물종류개수 시트
    const string corCountCropCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=760081684&format=csv"; // 거래처별 요구작물개수 시트
    const string eventCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1411103725&format=csv"; // 돌발 이벤트 시트
    const string eventSeasonCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=446557917&format=csv"; // 이벤트 계절 시트
    const string stageCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=1766045491&format=csv"; // 스테이지 시트
    const string stageCorCSVPath = "https://docs.google.com/spreadsheets/d/1Sd5x4Mt1fnIVY-X97tvYx1lTE8bh-nQ0/export?gid=169149848&format=csv"; // 스테이지별 거래처 시트

    public List<CROP> Crops;
    public bool downloadCheck;
    public static CSVManager Instance;    

    private void Start()
    {
        downloadCheck = false;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(DownloadRoutine());
    }

    IEnumerator DownloadRoutine()
    {
        UnityWebRequest request = UnityWebRequest.Get(cropCSVPath); // 링크를 통해서 웹사이트에 다운로드 요청
        yield return request.SendWebRequest();                  // 링크를 진행하고 완료될 때까지 대기

        string receiveText = request.downloadHandler.text;      // 다운로드 완료한 파일을 텍스트로 읽기

        Debug.Log(receiveText);

        string[] lines = receiveText.Split('\n');
        for (int y = 2; y < lines.Length; y++)
        {
            CROP crop = new CROP();

            string[] values = lines[y].Split(',', '\t');

            crop.cropID = int.Parse(values[0]);
            crop.cropName = values[1];
            crop.maxShovelCount = int.Parse(values[2]);
            crop.maxWaterCount = int.Parse(values[3]);
            crop.maxNutrientCount = int.Parse(values[4]);
            crop.maxGrowthTime = float.Parse(values[5]);
            crop.droughtMaxWaterCount = int.Parse(values[6]);
            crop.droughtMaxNutrientCount = int.Parse(values[7]);
            crop.damagePercent = float.Parse(values[8]);
            crop.damageTime = float.Parse(values[9]);
            crop.lowTemperTime = float.Parse(values[10]);
            crop.highTemperTime = float.Parse(values[11]);

            Crops.Add(crop);
        }
    }
}