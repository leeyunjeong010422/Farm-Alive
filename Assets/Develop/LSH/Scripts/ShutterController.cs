using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutterController : MonoBehaviour
{
    //[SerializeField] public bool isUpperLowerShutterOpen = false;
    [SerializeField] public bool isLeftRightShutterOpen = false;

    /*[SerializeField] Transform upperShutter;
    [SerializeField] Transform lowerShutter;
    [SerializeField] Vector3 upperClosedPosition;
    [SerializeField] Vector3 upperOpenPosition;
    [SerializeField] Vector3 lowerClosedPosition;
    [SerializeField] Vector3 lowerOpenPosition;*/

    [SerializeField] Transform leftShutter;
    [SerializeField] Transform rightShutter;
    [SerializeField] Vector3 leftClosedPosition;
    [SerializeField] Vector3 leftOpenPosition;
    [SerializeField] Vector3 rightClosedPosition;
    [SerializeField] Vector3 rightOpenPosition;

    [SerializeField] float moveSpeed = 5f;

    void Update()
    {
        /*if (upperShutter != null && lowerShutter != null)
        {
            if (isUpperLowerShutterOpen)
            {
                upperShutter.localPosition = Vector3.MoveTowards(upperShutter.localPosition, upperOpenPosition, moveSpeed * Time.deltaTime);
                lowerShutter.localPosition = Vector3.MoveTowards(lowerShutter.localPosition, lowerOpenPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                upperShutter.localPosition = Vector3.MoveTowards(upperShutter.localPosition, upperClosedPosition, moveSpeed * Time.deltaTime);
                lowerShutter.localPosition = Vector3.MoveTowards(lowerShutter.localPosition, lowerClosedPosition, moveSpeed * Time.deltaTime);
            }
        }*/

        if (leftShutter != null && rightShutter != null)
        {
            if (isLeftRightShutterOpen)
            {
                leftShutter.localPosition = Vector3.MoveTowards(leftShutter.localPosition, leftOpenPosition, moveSpeed * Time.deltaTime);
                rightShutter.localPosition = Vector3.MoveTowards(rightShutter.localPosition, rightOpenPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                leftShutter.localPosition = Vector3.MoveTowards(leftShutter.localPosition, leftClosedPosition, moveSpeed * Time.deltaTime);
                rightShutter.localPosition = Vector3.MoveTowards(rightShutter.localPosition, rightClosedPosition, moveSpeed * Time.deltaTime);
            }
        }
    }
}
