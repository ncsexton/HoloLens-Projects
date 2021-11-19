using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Orientator : MonoBehaviour
{
    //public GameObject message;
    //public GameObject gridSystem;
    float counter = 0;
    Vector3 last;
    public float acceptableDist;
    public float timerLength;
    public bool set = false;
    private void Update()
    {
        if (Vector3.Distance(last, gameObject.transform.position) <= acceptableDist * Time.deltaTime && !set)
        {
            counter += Time.deltaTime;
            if (counter >= timerLength)
            {
                //gridSystem.transform.position = gameObject.transform.position;
                //Debug.Log(gameObject.transform.rotation.eulerAngles.x);
                //gridSystem.transform.rotation = Quaternion.Euler(new Vector3(gameObject.transform.rotation.eulerAngles.x, gameObject.transform.rotation.eulerAngles.y, gameObject.transform.rotation.eulerAngles.z + 180));
                //message.GetComponent<TextMeshProUGUI>().text = "Worked";
                set = true;
            }
        }
        else if (!set)
        {
            counter = 0;
        }
        if (!set)
        {
            last = gameObject.transform.position;
        }
    }

}