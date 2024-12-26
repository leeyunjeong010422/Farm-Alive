using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SectionManager : MonoBehaviour
{
    private const int SECTION_NUM = 3;

    public static SectionManager Instance { get; private set; }

    [SerializeField] private List<Crop>[] _crops;
    [SerializeField] private int _curSection;

    public List<Crop>[] Crops { get { return _crops; } }
    public int CurSection { get { return _curSection; } set { _curSection = value; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += Init;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= Init;
    }

    private void Init(Scene scene, LoadSceneMode mode)
    {
        _curSection = 0;

        _crops = new List<Crop>[SECTION_NUM];
        for (int i = 0; i < SECTION_NUM; i++)
            _crops[i] = new List<Crop>();
    }

    public void IncreaseMoisture()
    {
        foreach (Crop crop in _crops[_curSection])
        {
            crop.IncreaseMoisture();
        }
    }

    public void IncreaseNutrient()
    {
        foreach (Crop crop in _crops[_curSection])
        {
            crop.IncreaseNutrient();
        }
    }
}
