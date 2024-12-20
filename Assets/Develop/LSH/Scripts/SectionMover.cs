using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionMover : MonoBehaviour
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
                shutterController.OpenShutter();
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
        selectedCube = targetCubes[0];
        isMoving = true;
        shutterController.CloseShutter();
    }

    public void sel2()
    {
        selectedCube = targetCubes[1];
        isMoving = true;
        shutterController.CloseShutter();
    }

    public void sel3()
    {
        selectedCube = targetCubes[2];
        isMoving = true;
        shutterController.CloseShutter();
    }
}
