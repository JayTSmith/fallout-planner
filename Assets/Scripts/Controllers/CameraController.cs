using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private float outOfBoundsPercent = .05f;
    private float movementSpeed = 8.0f;

    private float zoomScalar = 0.2f;
    private float minZoom = 1.0f;
    private float maxZoom = 100.0f;

    [Range(1.0f, 100.0f), SerializeField]
    private float startZoom = 10.0f;

    [SerializeField]
    private GameObject homeObject;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.orthographicSize = startZoom;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 moveVec = new Vector3(0,0,0);
        Rect bounds = Screen.safeArea;

        if (Input.GetKeyDown(KeyCode.Home))
        {
            FrameObject(homeObject);
        }

        // Movement of camera
        if (mousePos.x >= bounds.xMin && mousePos.x <= outOfBoundsPercent * bounds.xMax)
        {
            moveVec.x = -movementSpeed * Time.deltaTime;
        }
        else if (mousePos.x >= (1 - outOfBoundsPercent) * bounds.xMax && mousePos.x <= bounds.xMax)
        {
            moveVec.x = movementSpeed * Time.deltaTime;
        }

        if (mousePos.y >= bounds.yMin && mousePos.y <= outOfBoundsPercent * bounds.yMax)
        { 
            moveVec.y = -movementSpeed * Time.deltaTime;
        }
        else if (mousePos.y >= (1 - outOfBoundsPercent) * bounds.yMax && mousePos.y <= bounds.yMax)
        {
            moveVec.y = movementSpeed * Time.deltaTime;
        }

        transform.Translate(moveVec);


        float newZoomLevel = Camera.main.orthographicSize + (zoomScalar * -Input.mouseScrollDelta.y);
        // Bounds checking on the zoom.
        newZoomLevel = (newZoomLevel < minZoom) ? minZoom : ((newZoomLevel > maxZoom) ? maxZoom : newZoomLevel);
        Camera.main.orthographicSize = newZoomLevel;

    }

    public void FrameObject(GameObject go) {
        Vector3 newPos = go.transform.position;
        newPos.z -= 10;
        transform.position = newPos;
    }
}
