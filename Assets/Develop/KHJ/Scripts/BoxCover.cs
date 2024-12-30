using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCover : MonoBehaviour
{
    [Header("박스 커버")]
    [SerializeField] private GameObject _front;
    [SerializeField] private GameObject _back;
    [SerializeField] private GameObject _right;
    [SerializeField] private GameObject _left;

    [field: SerializeField]
    public bool IsOpen {  get; private set; }
    [field: SerializeField]
    public bool IsPackaged { get; private set; }
}
