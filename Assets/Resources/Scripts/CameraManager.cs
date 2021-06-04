using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    public float xSpeed = 200f;
    public float ySpeed = 200f;
    public float yMin = -80;
    public float yMax = 80;
    public float distance = 5f;
    public float zoomDampening = 5f;
    public float panSpeed = 0.3f;
    public float[] minPos = { -25f, -25f, -50f };
    public float[] maxPos = { 25f, 25f, 50f };
    public float maxDistance = 50;
    public float minDistance = 3;
    public int zoomRate = 40;

    GameObject CameraRig;
    public DockManager dock;


    void Start()
    {
        if (dock == null) {
            dock = GameObject.FindObjectOfType<DockManager>();
        }
        CameraRig = transform.parent.gameObject;
        distance = Vector3.Distance(transform.localPosition, CameraRig.transform.position);
        currentDistance = -distance;
        desDistance = -distance;
        transform.localPosition = new Vector3(0, 0, desDistance);

        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        curRotation = transform.rotation;
        desRotation = transform.rotation;

        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private Quaternion desRotation;
    private Quaternion curRotation;
    private Quaternion rotation;
    private float desDistance;
    private float currentDistance;
    private Vector3 position;


    void LateUpdate()
    {
        if (dock.focus)
        {
            if (Input.GetMouseButton(1))
            {
                xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                yDeg = ClampAngle(yDeg, yMin, yMax);

                desRotation = Quaternion.Euler(yDeg, xDeg, 0);
                curRotation = CameraRig.transform.rotation;

                CameraRig.transform.rotation = desRotation;
            }
            else if (Input.GetMouseButton(2))
            {
                CameraRig.transform.Translate(Vector3.right * -Input.GetAxis("Mouse X") * panSpeed);
                CameraRig.transform.Translate(transform.up * -Input.GetAxis("Mouse Y") * panSpeed, Space.World);

                CameraRig.transform.position = ClampPosition(CameraRig.transform.position, minPos, maxPos);
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                CameraRig.transform.Translate(new Vector3(Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate, 0, 0), Space.World);
                CameraRig.transform.position = ClampPosition(CameraRig.transform.position, minPos, maxPos);
            }
            else if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                CameraRig.transform.Translate(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate), Space.World);
                CameraRig.transform.position = ClampPosition(CameraRig.transform.position, minPos, maxPos);
            }
            else if (Input.GetKey(KeyCode.LeftControl) && Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                CameraRig.transform.Translate(new Vector3(0, Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate, 0), Space.World);
                CameraRig.transform.position = ClampPosition(CameraRig.transform.position, minPos, maxPos);
            }
            else if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                desDistance += Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate;
                desDistance = Mathf.Clamp(desDistance, -maxDistance, -minDistance);
                transform.localPosition = new Vector3(0, 0, desDistance);
            }
        }
    }

    private static Vector3 ClampPosition(Vector3 position, float[] min, float[] max)
    {
        float x = Mathf.Clamp(position.x, min[0], max[0]);
        float y = Mathf.Clamp(position.y, min[1], max[1]);
        float z = Mathf.Clamp(position.z, min[2], max[2]);
        Vector3 newPosition = new Vector3(x, y, z);

        return newPosition;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
        {
            angle += 360;
        }
        if (angle > 360)
        {
            angle -= 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}