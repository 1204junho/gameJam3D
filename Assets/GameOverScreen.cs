using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public GameObject[] bloods;
    public GameObject[] buttons;
    WaitForSecondsRealtime delayTime;
    [SerializeField, Range(0.125f, 1f)]
    float delay;

    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(GameOverEffect());
    }
    void Start()
    {
        delayTime = new(delay);
    }
    IEnumerator GameOverEffect()
    {
        foreach(var blood in bloods)
        {
            blood.SetActive(true);
            yield return delayTime;
        }
        foreach (var blood in bloods)
            blood.SetActive(false);
        GetComponent<Image>().color = Color.red;
        foreach (var button in buttons)
            button.SetActive(true);
    }
}
