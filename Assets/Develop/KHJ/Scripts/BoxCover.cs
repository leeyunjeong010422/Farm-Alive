using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCover : MonoBehaviour
{
    [Header("박스 커버")]
    [SerializeField] private BoxCoverInteractable _front;
    [SerializeField] private BoxCoverInteractable _back;
    [SerializeField] private BoxCoverInteractable _right;
    [SerializeField] private BoxCoverInteractable _left;

    [Header("포장")]
    public GameObject leftPoint;
    public GameObject rightPoint;
    public GameObject tape;

    [field: SerializeField]
    public bool IsOpen {  get; private set; }
    [field: SerializeField]
    public bool IsPackaged { get; private set; }

    public bool CheckOpen() => _front.IsOpen || _back.IsOpen || _left.IsOpen || _right.IsOpen;
}
