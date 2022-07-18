using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform[] movePoint;
    public Player player;
    public Light enemySight;
    [Range(0, .5f)]
    public float delayTime;
    [Range(5f,15f)]
    public float range;
    public bool isTrace;
    WaitForSeconds delay;
    public NavMeshAgent navi;
    
    // Start is called before the first frame update
    
    void Start()
    {
        delay = new(delayTime);
        enemySight.range = range * 1.75f;
        navi = GetComponent<NavMeshAgent>();
        StartCoroutine(ChangeDestination());
    }
    bool isPlayerClose { get { return (player.transform.position - transform.position).magnitude < range; } }
    IEnumerator ChangeDestination()
    {
        int i = Random.Range(0, movePoint.Length);
        navi.SetDestination(movePoint[i].position);
        while (true)
        {
            if (isTrace)
            {
                navi.SetDestination(player.transform.position);
                if (player.isHide && !isPlayerClose) { 
                    isTrace = false; 
                    navi.SetDestination(movePoint[i].position);
                }
            }
            else
            {
                if ((movePoint[i].position - transform.position).magnitude < 2f)
                {
                    i = ++i % movePoint.Length;
                    navi.SetDestination(movePoint[i].position);
                }
                if (!player.isHide && isPlayerClose) isTrace = true;
            }

            yield return delay;
        }
    }
}
