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
    Rigidbody rigid;
    RaycastHit hit;
    Vector3 moveVec;
    AudioSource walkSound;
    float walkSoundSpeed { set { walkSound.pitch = value; } }
    void Start()
    {
        sensitivity[0] = sensitivity[1] = 5;
        rigid = GetComponent<Rigidbody>();
        walkSound = GetComponent<AudioSource>();
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
            if (GM.IsPlayerHide)
                GM.IsPlayerHide = false;
            else if (GM.HasDrink)
            {
                GM.HasDrink = false;
            }
        }
        else if (Physics.Raycast(cam.position, cam.TransformDirection(Vector3.forward), out hit, 10f))
        {
            if (hit.collider.gameObject.tag == "InterActiveObject")
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
                    if (hit.collider.gameObject.tag == "InterActiveObject")
                    {
                        switch (seeObject.name)
                        {
                            //object
                            case "MoneyMaker":
                                GM.StartCoroutine(GM.MakeMoney(3));
                                break;
                            case "HidePlace":
                                GM.IsPlayerHide = true;
                                rigid.velocity = Vector3.zero;
                                transform.position = seeObject.transform.position;
                                transform.rotation = seeObject.transform.rotation;
                                walkSoundSpeed = 0f;
                                break;
                            case "Vending":
                                GM.UseVending(seeObject.GetComponent<Vending_machine>());
                                break;
                            case "ATM":
                                GM.UseATM(0);
                                break;


                        }
                    }
                }
                if (Input.GetMouseButton(0))
                {
                    //item
                    switch (seeObject.name)
                    {
                        case "Battery(Clone)":
                            GM.BatteryCharge(30);
                            Destroy(seeObject);
                            break;
                        case "Drink(Clone)":
                            if (!GM.HasDrink) { 
                                GM.HasDrink = true;
                                Destroy(seeObject);
                            }
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
        if(Cursor.lockState != CursorLockMode.Confined && !GM.IsPlayerHide)
        {
            transform.rotation = Quaternion.Euler(0, transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity[0], 0);
            heightRate -= Input.GetAxis("Mouse Y") * sensitivity[1];
            if (heightRate > 45f) heightRate = 45f;
            else if (heightRate < -75f) heightRate = -75f;
            cam.localEulerAngles = Vector3.right * heightRate;
            moveVec = transform.rotation * (Vector3.right * Input.GetAxis("Horizontal") + Vector3.forward * Input.GetAxis("Vertical"));
            if (moveVec.magnitude > 1.4f) moveVec.Normalize();
            rigid.velocity = moveVec * speed;
            walkSoundSpeed = (rigid.velocity.magnitude) / 20f;
        }        
    }
}
