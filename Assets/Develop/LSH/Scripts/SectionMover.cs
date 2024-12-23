using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionMover : MonoBehaviourPun
{
    [SerializeField] GameObject mainCube;
    [SerializeField] GameObject[] targetCubes;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] ShutterController shutterController;

    [SerializeField] GameObject selectedCube;
    [SerializeField] Vector3[] firstPosition;
    [SerializeField] bool isMoving = false;

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
                isMoving = false;
                photonView.RPC("CloseShutter", RpcTarget.All);
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
            photonView.RPC("SelectCubeRPC", RpcTarget.All, cubeIndex);
        }
    }

    [PunRPC]
    private void SelectCubeRPC(int cubeIndex)
    {
        selectedCube = targetCubes[cubeIndex];
        isMoving = true;

        photonView.RPC("OpenShutter", RpcTarget.All);
    }
}
