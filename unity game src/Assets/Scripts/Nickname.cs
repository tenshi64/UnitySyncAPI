using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Nickname : MonoBehaviour
{
    Transform MainCamera;
    TextMeshPro TextMesh;

    private void Start()
    {
        TextMesh = GetComponent<TextMeshPro>();
    }

    void Update()
    {
        if(MainCamera == null)
        {
            MainCamera = GameObject.Find("Main Camera").transform;
        }
        else
        {
            transform.LookAt(MainCamera.transform.position);
            transform.eulerAngles = new Vector3(MainCamera.transform.eulerAngles.x, MainCamera.transform.eulerAngles.y, 0);
        }

        if(TextMesh.text == "Nickname")
        {
            TextMesh.text = transform.parent.parent.GetComponent<NetworkObject>().Nickname;
        }
    }
}
