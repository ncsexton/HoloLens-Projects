using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonTest : MonoBehaviour
{
    public GameObject tmpGO;
    public TextMeshPro tmp;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Test");
    }

    public void Pressed()
    {
        Debug.Log(tmpGO.GetComponent<TextMeshPro>());
        tmp = tmpGO.GetComponent<TextMeshPro>() ?? tmpGO.AddComponent<TextMeshPro>();
        tmp.text = "Pressed";
    }
}
