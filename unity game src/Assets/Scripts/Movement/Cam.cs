using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour
{
    public Transform cameraTarget;
    public float distanceZ = 5f;
    public float distanceY = 0f;
    private NetworkObject NetworkObject;

    void Start()
    {
        NetworkObject = transform.parent.GetComponent<NetworkObject>();
    }

    void LateUpdate()
    {
        if (!NetworkObject.IsLocalPlayer)
        {
            Destroy(gameObject);
        }

        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;
            }
        #endif

        //transform.position = new Vector3(cameraTarget.parent.position.x, transform.position.y, cameraTarget.parent.position.z - 5.273998f);
        transform.position = cameraTarget.parent.TransformPoint(-Vector3.forward * distanceZ);
        transform.position = new Vector3(transform.position.x, 8.45f + distanceY, transform.position.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, cameraTarget.parent.eulerAngles.y, transform.eulerAngles.z), 2 * Time.deltaTime);
    }
}
