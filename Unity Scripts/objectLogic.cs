using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;

public class objectLogic : MonoBehaviour
{
    public bool grabed = false;
    //public UdpServer server;
    public GameObject playSpace;
    private void Awake()
    {
        //find the needed objects and scripts
        //server = GameObject.Find("Manager").GetComponent<UdpServer>();
        playSpace = GameObject.Find("SceneContent");
    }
    public void SetGrabed(bool b)
    {
        //logic to send whether or not the object grabed = b;
        if (b) { 
            //server.StartServer("G0");
            //server.StartServer("G0");
        }
        else if (!b)
        {
            //server.StartServer("G1");
            //server.StartServer("G1");
        }
    }
    private void Update()
    {
        if (grabed)
        {
            //converts position to postiion relative to the playSpace
            Vector3 val = playSpace.transform.worldToLocalMatrix.MultiplyPoint3x4(gameObject.transform.position);
            //converts vector to string then sends that data to the other hololens
            //server.StartServer("V" + val.x.ToString() + "," + val.y.ToString() + "," + val.z.ToString());
        }
    }
}
