using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text moneyText;
    public TMP_Text script;
    public void MoneyChange(int money)
    {
        moneyText.text = "Money - " + money;
    }
    public IEnumerator ShowScript(string saying)
    {
        script.text = saying;
        yield return new WaitForSeconds(1f);
        script.text = "";
    }
}
