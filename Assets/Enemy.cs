using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public Transform[] movePoint;
    public Player player;
    public Light enemySight;
    [Range(0, 5f)]
    public float delayTime;
    [Range(5f,15f)]
    public float range;
    [SerializeField]
    bool isTrace;
    WaitForSeconds delay, oneSec;
    NavMeshAgent navi;
    
    // Start is called before the first frame update
    
    void Start()
    {
        delay = new WaitForSeconds(delayTime);
        oneSec = new WaitForSeconds(1f);
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
            if ((movePoint[i].position - transform.position).magnitude < 0.5f)
            {
                i = ++i % movePoint.Length;
                navi.SetDestination(movePoint[i].position);
            }
            yield return delay;
            
        }
    }
    IEnumerator TimeCheck()
    {
        StopCoroutine(ChangeDestination());
        isTrace = true;
        int time = 0;
        while (time < 2)
        {
            if (!player.isHide) time = 0;
            Debug.Log(time);
            yield return oneSec;
            time++;
        }
        StartCoroutine(StopTrase());
    }
    IEnumerator StopTrase()
    {
        navi.isStopped = true;
        isTrace = false;
        enemySight.color = Color.cyan;
        yield return oneSec;
        navi.isStopped = false;
        enemySight.color = Color.red;
        StartCoroutine(ChangeDestination());
    }
    private void Update()
    {
        if (isTrace) navi.SetDestination(player.transform.position);
        else if (isPlayerClose && !player.isHide) StartCoroutine(TimeCheck());
    }
}
