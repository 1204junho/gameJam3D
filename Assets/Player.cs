using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform cam;
    public GameManager GM;
    
    GameObject seeObject;//, holdObject;
    [Range(0f, 20f)]
    public float speed = 10f;
    public float[] sensitivity;
    [Range(1f, 10f)]
    float heightRate;
    public bool isHide;
    Rigidbody rigid;
    RaycastHit hit;
    void Start()
    {
        sensitivity[0] = sensitivity[1] = 5;
        rigid = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    IEnumerator CamShake(float duraition)
    {
        float nowTime = 0f;
        while(nowTime < duraition)
        {
            cam.localPosition = Vector3.up + Random.insideUnitSphere*0.125f;
            nowTime += Time.deltaTime;
            yield return null;
        }
        cam.localPosition = Vector3.up;
    }
    
    void Update()
    {
        if(Input.GetKeyDown("escape")){
            GM.Phone();
        }
        if (Input.GetKeyDown("e"))
        {
            if (isHide)
            {
                transform.position += transform.rotation* Vector3.forward;
                GetComponent<Collider>().enabled = true;
                GM.ScriptFlush();
                isHide = false;
            }
            else if (GM.HasDrink)
            {
                StartCoroutine(GM.SpeedBuff(2.5f));
            }
        }
        else if (Physics.Raycast(cam.position, cam.TransformDirection(Vector3.forward), out hit, 10f))
        {
            if (hit.collider.gameObject.tag == "InterActiveObject" || hit.collider.gameObject.tag == "Item")
            {
                if (seeObject == null)
                {
                    seeObject = hit.collider.gameObject;
                    seeObject.GetComponent<Outline>().OutlineWidth = 10f;
                }
                else if (seeObject != hit.collider.gameObject)
                {
                    seeObject.GetComponent<Outline>().OutlineWidth = 0f;
                    seeObject = hit.collider.gameObject;
                    seeObject.GetComponent<Outline>().OutlineWidth = 10f;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    switch (hit.collider.gameObject.tag)
                    {
                        case "InterActiveObject":
                            if (seeObject.name == "MoneyMaker")
                                StartCoroutine(GM.MakeMoney(3));
                            else if (seeObject.name == "HidePlace")
                            {
                                isHide = true;
                                GetComponent<Collider>().enabled = false;
                                transform.position = seeObject.transform.position;
                                transform.rotation = seeObject.transform.rotation;
                                GM.ShowScript("press E to get out", Color.green);
                            }
                            break;
                        case "Item":
                            switch (seeObject.name)
                            {
                                case "Battery":
                                    GM.BatteryCharge(30);
                                    break;
                                case "Drink":
                                    if (GM.HasDrink) return;
                                    GM.HasDrink = true;
                                    break;
                            }
                            seeObject.SetActive(false);
                            break;
                    }
                }
            }
        }
        else
        {
            if (seeObject != null) {
                seeObject.GetComponent<Outline>().OutlineWidth = 0f;
                seeObject = null;
            }
        }
        if(Cursor.lockState != CursorLockMode.Confined)
        {
            transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity[0], 0);
            heightRate -= Input.GetAxis("Mouse Y") * sensitivity[1];
            if (heightRate > 45f) heightRate = 45f;
            else if (heightRate < -75f) heightRate = -75f;
            cam.localEulerAngles = Vector3.right * heightRate;
            rigid.velocity = isHide ? Vector3.zero : transform.rotation * (Vector3.right * Input.GetAxis("Horizontal") + Vector3.forward * Input.GetAxis("Vertical")) * speed;
        }        
        /*
        if (Input.GetMouseButtonDown(1))
        {
            if (holdObject == null) {  
                if (seeObject != null) {
                    holdObject = seeObject;
                    holdObject.GetComponent<Rigidbody>().useGravity = false;
                }
            }
            else {
                    holdObject.GetComponent<Rigidbody>().useGravity = true;
                    holdObject.GetComponent<Rigidbody>().velocity = cam.rotation * Vector3.forward * 10;
                    holdObject = null;
                } 
        }
        if (holdObject != null) holdObject.transform.position = transform.position + cam.rotation * Vector3.forward * 1.5f + Vector3.up;
        */
        //viewVec = Quaternion.Euler(0, target.eulerAngles.y, 0) * Vector3.forward;//legnth is 1.
        //sideVec = Quaternion.Euler(0, target.eulerAngles.y, 0) * Vector3.right;
    }
}
