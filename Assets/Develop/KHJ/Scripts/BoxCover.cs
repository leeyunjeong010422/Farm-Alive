using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCover : MonoBehaviour
{
    [SerializeField] private GameObject _front;
    [SerializeField] private GameObject _back;
    [SerializeField] private GameObject _right;
    [SerializeField] private GameObject _left;

    public bool IsOpen {  get; private set; }
    public bool IsPackaged { get; private set; }


}
