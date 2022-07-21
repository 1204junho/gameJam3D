using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform[] movePoint;
    public Player player;
    public GameManager GM;
    public Light enemySight;
    public AudioSource[] enemySounds;
    [Range(0.06f, .5f)]
    public float delayTime;
    public float sightLength;
    float SightLength { get { return sightLength; } set{ sightLength = value; enemySight.spotAngle = value * 3 + 10; } }
    public bool isTrace;
    WaitForSeconds delay;
    NavMeshAgent navi;
    public float speed { get { return navi.speed; } set { navi.speed = value; } }
    int nowPhase = 0;
    public int NowPhase
    {
        set
        {
            if(nowPhase < value)
            {
                nowPhase = value;
                switch (nowPhase)
                {
                    case 1:
                        enemySounds[0].Play();
                        SightLength *= 0.95f;
                        speed = player.speed;
                        break;
                    case 2:
                        enemySounds[1].Play();
                        speed = player.speed * 1.1f;
                        break;
                    default:
                        Debug.LogError("Enemy Error Phase");
                        break;
                }
            }
        }
    }
    void Start()
    {
        delay = new(delayTime);
        SightLength = sightLength;
        navi = GetComponent<NavMeshAgent>();
        StartCoroutine(ChangeDestination());
        StartCoroutine(SeekPlayer());
    }
    float distanceToPlayer { get { return Mathf.Sqrt(Mathf.Pow(player.transform.position.x - transform.position.x,2) + Mathf.Pow(player.transform.position.z - transform.position.z, 2)); } }
    bool isPlayerClose { get { return distanceToPlayer < SightLength; } }
    bool isPlayerHit { get { return distanceToPlayer < 2f; } }
    IEnumerator ChangeDestination()
    {
        int i = Random.Range(0, movePoint.Length);
        navi.SetDestination(movePoint[i].position);
        while (true)
        {
            if (isTrace || nowPhase == 2)//trace
            {
                if (isPlayerHit || (GM.IsPlayerHide && isPlayerClose)) {
                    GM.EnemyHitPlayer();
                    navi.SetDestination(movePoint[i].position);
                    isTrace = false;
                }
                else if (GM.IsPlayerHide && !isPlayerClose) { //sucess hide
                    navi.SetDestination(movePoint[i].position);
                    isTrace = false;
                }
                else navi.SetDestination(player.transform.position);
            }
            else
            {
                if ((movePoint[i].position - transform.position).magnitude < 2f)
                {
                    i = ++i % movePoint.Length;
                    navi.SetDestination(movePoint[i].position);
                }
            }
            yield return delay;
        }
    }
    IEnumerator SeekPlayer()
    {
        RaycastHit hit;
        float i;
        WaitForSeconds quarterSec = new(0.25f);
        while (true)
        {
            if (!GM.IsPlayerHide)
            {
                for ( i = -1; i <= 1; i += 0.25f )
                    if (Physics.Raycast(transform.position, transform.rotation * (Vector3.forward + Vector3.right * i), out hit, SightLength))
                    {
                        if (hit.collider.gameObject.tag == "Player") isTrace = true;
                    }
                Debug.DrawRay(transform.position, transform.rotation * Vector3.forward*SightLength,Color.cyan,0.25f);
            }
            yield return quarterSec;
        }
    }
}
