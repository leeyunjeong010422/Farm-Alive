using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionMover : MonoBehaviourPun
{
    [SerializeField] GameObject mainCube;
    [SerializeField] GameObject[] targetCubes;
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] ShutterController ShutterController;

    [SerializeField] GameObject selectedCube;
    [SerializeField] Vector3[] firstPosition;
    [SerializeField] bool isMoving = false;

    private bool isUpperLowerShutterOpen = false;
    private bool isLeftRightShutterOpen = false;

    private void Start()
    {
        firstPosition = new Vector3[targetCubes.Length];
        for (int i = 0; i < targetCubes.Length; i++)
        {
            firstPosition[i] = targetCubes[i].transform.position;
        }
    }
    void Update()
    {
        if (selectedCube != null)
        {
            Vector3 targetPosition = mainCube.transform.position + mainCube.transform.forward * 20;
            selectedCube.transform.position = Vector3.MoveTowards(selectedCube.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(selectedCube.transform.position, targetPosition) < 0.01f)
            {
                if (!isUpperLowerShutterOpen || !isLeftRightShutterOpen)
                {
                    Debug.Log("¿­¸²");
                    isMoving = false;
                    photonView.RPC(nameof(OpenAllShutters), RpcTarget.AllBuffered);
                }
                
            }
        }

        for (int i = 0; i < targetCubes.Length; i++)
        {
            if (targetCubes[i] != selectedCube)
            {
                targetCubes[i].transform.position = Vector3.MoveTowards(targetCubes[i].transform.position, firstPosition[i], moveSpeed * Time.deltaTime);
            }
        }
    }

    public void sel1()
    {
        OnSelectCube(0);
    }

    public void sel2()
    {
        OnSelectCube(1);
    }

    public void sel3()
    {
        OnSelectCube(2);
    }

    public void OnSelectCube(int cubeIndex)
    {
        if (!isMoving)
        {
            photonView.RPC(nameof(SelectCubeRPC), RpcTarget.AllBuffered, cubeIndex);
        }
    }

    [PunRPC]
    private void SelectCubeRPC(int cubeIndex)
    {
        selectedCube = targetCubes[cubeIndex];
        isMoving = true;

        photonView.RPC(nameof(CloseAllShutters), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void OpenAllShutters()
    {
        if (!isUpperLowerShutterOpen)
        {
            if (ShutterController != null)
                ShutterController.isUpperLowerShutterOpen = true;

            isUpperLowerShutterOpen = true;
        }
        else if (!isLeftRightShutterOpen)
        {
            if (ShutterController != null)
                ShutterController.isLeftRightShutterOpen = true;

            isLeftRightShutterOpen = true;
        }
    }

    [PunRPC]
    public void CloseAllShutters()
    {
        Debug.Log("´ÝÈû");
        if (ShutterController != null)
        {
            ShutterController.isUpperLowerShutterOpen = false;
            ShutterController.isLeftRightShutterOpen = false;
        }

        isUpperLowerShutterOpen = false;
        isLeftRightShutterOpen = false;
    }
}
