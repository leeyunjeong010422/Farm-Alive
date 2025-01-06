using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionMover : MonoBehaviourPun
{
    [SerializeField] GameObject mainSection;
    [SerializeField] GameObject[] targetSection;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float waitSteps = 0.5f;

    [SerializeField] ShutterController ShutterController;

    [SerializeField] GameObject selectedSection;
    [SerializeField] Vector3[] firstPosition;
    [SerializeField] Vector3[] middlePosition;
    [SerializeField] Vector3[] endPosition;
    [SerializeField] Vector3[] middleBackPosition;
    [SerializeField] Vector3[] endBackPosition;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] bool isMoving = false;
    [SerializeField] bool isStartMoving = false;
    [SerializeField] bool isMiddleMoving = false;
    [SerializeField] bool isEndMoving = false;
    [SerializeField] bool[] wayPoint1;
    [SerializeField] bool[] wayPoint2;


    //private bool isUpperLowerShutterOpen = false;
    private bool isLeftRightShutterOpen = false;

    private void Start()
    {
        wayPoint1 = new bool[targetSection.Length];
        wayPoint2 = new bool[targetSection.Length];
        firstPosition = new Vector3[targetSection.Length];
        for (int i = 0; i < targetSection.Length; i++)
        {
            firstPosition[i] = targetSection[i].transform.position;
        }

        middlePosition = new Vector3[targetSection.Length];
        for (int i = 0; i < targetSection.Length; i++)
        {
            middlePosition[i] = new Vector3(
                targetSection[i].transform.position.x,
                targetSection[i].transform.position.y + 14,
                targetSection[i].transform.position.z
                );

            wayPoint1[i] = false;
        }

        endPosition = new Vector3[targetSection.Length];
        for (int i = 0; i < targetSection.Length; i++)
        {
            endPosition[i] = new Vector3(
                targetPosition.x,
                targetPosition.y + 14,
                targetPosition.z
                );

            wayPoint2[i] = false;
        }

        middleBackPosition = new Vector3[targetSection.Length];
        for (int i = 0; i < targetSection.Length; i++)
        {
            middleBackPosition[i] = new Vector3(
                targetSection[i].transform.position.x,
                targetSection[i].transform.position.y + 28,
                targetSection[i].transform.position.z
                );
        }

        endBackPosition = new Vector3[targetSection.Length];
        for (int i = 0; i < targetSection.Length; i++)
        {
            endBackPosition[i] = new Vector3(
                targetPosition.x,
                targetPosition.y + 28,
                targetPosition.z
                );
        }
    }
    void Update()
    {
        if (selectedSection != null)
        {
            int selectedIndex = System.Array.IndexOf(targetSection, selectedSection);

            if (!wayPoint1[selectedIndex])
            {
                selectedSection.transform.position = Vector3.MoveTowards(selectedSection.transform.position, middlePosition[selectedIndex], moveSpeed * Time.deltaTime);

                if (Vector3.Distance(selectedSection.transform.position, middlePosition[selectedIndex]) < 0.01f)
                {
                    wayPoint1[selectedIndex] = true;
                }
            }
            else if (!wayPoint2[selectedIndex])
            {
                selectedSection.transform.position = Vector3.MoveTowards(selectedSection.transform.position, endPosition[selectedIndex], moveSpeed * Time.deltaTime);

                if (Vector3.Distance(selectedSection.transform.position, endPosition[selectedIndex]) < 0.01f)
                {
                    wayPoint2[selectedIndex] = true;
                }
            }
            else
            {
                selectedSection.transform.position = Vector3.MoveTowards(selectedSection.transform.position, targetPosition, moveSpeed * Time.deltaTime);

                if (Vector3.Distance(selectedSection.transform.position, targetPosition) < 0.01f)
                {
                    if (/*!isUpperLowerShutterOpen ||*/ !isLeftRightShutterOpen)
                    {
                        SectorDistance();
                    }
                }
            }
        }

        for (int i = 0; i < targetSection.Length; i++)
        {
            if (targetSection[i] != selectedSection)
            {
                if(Vector3.Distance(targetSection[i].transform.position, firstPosition[i]) > 0.01f)
                {
                    if (!wayPoint1[i])
                    {
                        targetSection[i].transform.position = Vector3.MoveTowards(targetSection[i].transform.position, endBackPosition[i], moveSpeed * Time.deltaTime);

                        if (Vector3.Distance(targetSection[i].transform.position, endBackPosition[i]) < 0.01f)
                        {
                            wayPoint1[i] = true;
                        }
                    }
                    else if (!wayPoint2[i])
                    {
                        targetSection[i].transform.position = Vector3.MoveTowards(targetSection[i].transform.position, middleBackPosition[i], moveSpeed * Time.deltaTime);

                        if (Vector3.Distance(targetSection[i].transform.position, middleBackPosition[i]) < 0.01f)
                        {
                            wayPoint2[i] = true;
                        }
                    }
                    else
                    {
                        targetSection[i].transform.position = Vector3.MoveTowards(targetSection[i].transform.position, firstPosition[i], moveSpeed * Time.deltaTime);
                    }
                }
            }
        }
    }

    public void sel1()
    {
        Debug.Log("버튼클릭");
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

    public void sel4()
    {
        OnSelectCube(3);
    }

    public void OnSelectCube(int cubeIndex)
    {
        if (!isMoving)
        {
            Debug.Log("큐브선택");

            photonView.RPC(nameof(SelectCubeRPC), RpcTarget.AllBuffered, cubeIndex);
        }
    }

    [PunRPC]
    private void SelectCubeRPC(int cubeIndex)
    {
        for (int i = 0; i < wayPoint1.Length; i++)
        {
            wayPoint1[i] = false;
            wayPoint2[i] = false;
        }
        Debug.Log("큐브선택RPc");
        selectedSection = targetSection[cubeIndex];
        isMoving = true;
        SectionManager.Instance.CurSection = cubeIndex;

        photonView.RPC(nameof(CloseAllShutters), RpcTarget.AllBuffered);
    }

    public void SectorDistance()
    {
        isMoving = false;
        photonView.RPC(nameof(OpenAllShutters), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void OpenAllShutters()
    {
        /*if (!isUpperLowerShutterOpen)
        {
            if (ShutterController != null)
                ShutterController.isUpperLowerShutterOpen = true;

            isUpperLowerShutterOpen = true;
        }*/
        if (!isLeftRightShutterOpen)
        {
            if (ShutterController != null)
                ShutterController.isLeftRightShutterOpen = true;

            isLeftRightShutterOpen = true;
        }
    }

    [PunRPC]
    public void CloseAllShutters()
    {
        Debug.Log("닫힘");
        if (ShutterController != null)
        {
            //ShutterController.isUpperLowerShutterOpen = false;
            ShutterController.isLeftRightShutterOpen = false;
        }

        //isUpperLowerShutterOpen = false;
        isLeftRightShutterOpen = false;
    }
}
