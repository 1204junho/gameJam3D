using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform cam;
    public GameManager GM;
    public AudioSource[] sounds;
    int Money;
    public int money {
        get { return Money; }
        set {
            if (Money < 2000 && value >= 2000)
                sounds[0].Play();
            else if(Money < 5000 && value >= 5000)
                sounds[1].Play();
            Money = value;
        }
    }
    GameObject seeObject;//, holdObject;
    [SerializeField, Range(0f, 20f)]
    private float speed = 5f;
    [SerializeField,Range(1f, 10f)]
    private float sensitivity_X;
    [SerializeField, Range(1f, 10f)]
    private float sensitivity_Y;
    float heightRate;
    public bool isHide;
    Rigidbody rigid;
    RaycastHit hit;
    void Start()
    {
        sensitivity_X = sensitivity_Y = 5;
        rigid = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
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
    IEnumerator MakeMoney(float time)
    {
        sounds[2].Play();
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
            if (!Input.GetMouseButton(0))
                yield break;
        }
        if (Random.value > 0.5f)
        {
            sounds[3].Play();
            money += Random.Range(5, 21) * 100;
            GM.MoneyChange(money);
        }
        else
        {
            sounds[4].Play();
            StartCoroutine(GM.ShowScript("Fail"));
        }
    }
    void Update()
    {
        if (Physics.Raycast(cam.position, cam.TransformDirection(Vector3.forward), out hit, 10f))
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
                    if (seeObject.name == "MoneyMaker")
                        StartCoroutine(MakeMoney(1));
                    else if(seeObject.name == "HidePlace")
                        isHide = true;
                    //StartCoroutine(CamShake(1));
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
        heightRate -= Input.GetAxis("Mouse Y")* sensitivity_Y;
        if (heightRate > 45f) heightRate = 45f;
        else if (heightRate < -75f) heightRate = -75f;
        cam.localEulerAngles = Vector3.right * heightRate;
        transform.rotation = Quaternion.Euler( 0, transform.localEulerAngles.y + Input.GetAxis("Mouse X")*sensitivity_X, 0);
        rigid.velocity = transform.rotation*(Vector3.right * Input.GetAxis("Horizontal") + Vector3.forward * Input.GetAxis("Vertical"))*speed;
        Cursor.visible = false;

        //viewVec = Quaternion.Euler(0, target.eulerAngles.y, 0) * Vector3.forward;//legnth is 1.
        //sideVec = Quaternion.Euler(0, target.eulerAngles.y, 0) * Vector3.right;
    }
}
