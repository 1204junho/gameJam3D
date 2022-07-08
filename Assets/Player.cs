using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody rigid;
    public Transform cam;
    float heightRate;
    RaycastHit hit;
    GameObject seeObject, holdObject;
    [SerializeField, Range(0f, 20f)]
    private float speed = 5f;
    [SerializeField,Range(1f, 10f)]
    private float sensitivity_X;
    [SerializeField, Range(1f, 10f)]
    private float sensitivity_Y;
    // Start is called before the first frame update
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
    // Update is called once per frame
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
                    seeObject.GetComponent<Rigidbody>().velocity = transform.rotation * Vector3.forward * 20;
                    StartCoroutine(CamShake(1));
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
