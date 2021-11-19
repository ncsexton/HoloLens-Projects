using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PT : MonoBehaviour
{
    //public UdpServer server;
    public GameObject playArea;

    private void Awake()
    {
        //get necessary script
        //server = GameObject.Find("Manager").GetComponent<UdpServer>();
    }
    private void Update()
    {
        //get the headsets position
        Vector3 loc = playArea.transform.worldToLocalMatrix.MultiplyPoint3x4(gameObject.transform.position);
        //send the headsets position as string to other device
        //server.StartServer("C" + loc.x.ToString() + "," + loc.y.ToString() + "," + loc.z.ToString());

    }
}
