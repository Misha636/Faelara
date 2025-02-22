using System;
using UnityEngine;
using UnityEngine.Events;

public class ShapeShift : MonoBehaviour
{
    public int currentShapeIndex;
    public int currentActiveShape;

    public GameObject defaultShape;
    public ShapeTypes[] shapes;
    public GameObject[] shapeObjects;

    private Vector3 lastPosition;
    private PlayerController playerController;
    private CameraFollow cameraFollow; // Reference to CameraFollow script

    [SerializeField] private GameObject shapeShiftEffect;

    public UnityEvent[] changeShape;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        cameraFollow = Camera.main.GetComponent<CameraFollow>(); // Get the main camera follow script

        currentShapeIndex = 0;
        currentActiveShape = -1;

        lastPosition = defaultShape.transform.position;
        ToggleShape(currentActiveShape);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeCurrentShape(1);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeCurrentShape(-1);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            ShiftShape(currentShapeIndex);
        }
    }

    public void ChangeCurrentShape(int direction)
    {
        currentShapeIndex += direction;
        currentShapeIndex = Mathf.Clamp(currentShapeIndex, 0, shapes.Length - 1);
        changeShape[currentShapeIndex].Invoke();
    }

    public void ShiftShape(int shapeIndex)
    {
        if (currentActiveShape == shapeIndex) return;

        lastPosition = defaultShape.activeSelf ? defaultShape.transform.position : shapeObjects[currentActiveShape].transform.position;

        GameManager.Instance.ChangeState(shapes[shapeIndex]);
        ToggleShape(shapeIndex);
        currentActiveShape = shapeIndex;

        playerController.UpdateCollisionComponent(shapeObjects[currentActiveShape].GetComponent<CollisionComponent>());

        cameraFollow.UpdateTarget(shapeObjects[currentActiveShape].transform);

        Instantiate(shapeShiftEffect, shapeObjects[currentActiveShape].transform);
    }

    private void ToggleShape(int activeIndex)
    {
        if (activeIndex == -1)
        {
            defaultShape.SetActive(true);
            defaultShape.transform.position = lastPosition;

            foreach (GameObject shape in shapeObjects)
            {
                shape.SetActive(false);
            }

            playerController.UpdateCollisionComponent(defaultShape.GetComponent<CollisionComponent>());
            cameraFollow.UpdateTarget(defaultShape.transform);
        }
        else
        {
            defaultShape.SetActive(false);

            for (int i = 0; i < shapeObjects.Length; i++)
            {
                shapeObjects[i].SetActive(i == activeIndex);
                if (shapeObjects[i].activeSelf)
                {
                    shapeObjects[i].transform.position = lastPosition;
                }
            }

            cameraFollow.UpdateTarget(shapeObjects[activeIndex].transform);
        }
    }
}
