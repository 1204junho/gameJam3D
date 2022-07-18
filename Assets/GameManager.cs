using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public AudioSource[] sounds;
    public TMP_Text moneyText;
    public TMP_Text script;
    public Image batteryUI, drinkUI, buffUI, makeMoneyUI;
    public Slider[] sensitivitySlider;
    public GameObject phone;
    public Gradient batteryState;
    public Light flashlight;
    public Enemy enemy;
    public Player player;
    [SerializeField, Range(0, 100)]
    int battery;
    int BatteryAmount { 
        get { return battery; }
        set { battery = value; batteryUI.fillAmount = (float)value / 100; batteryUI.color = batteryState.Evaluate((float)value / 100); } }
    int Money;
    public int money
    {
        get { return Money; }
        set
        {
            if (Money < 2000 && value >= 2000)
            {
                sounds[0].Play();
                EnemyEnhance(0);
            }

            else if (Money < 5000 && value >= 5000)
            {
                sounds[1].Play();
                EnemyEnhance(1);
            }

            Money = value;
        }
    }
    [SerializeField]
    bool hasDrink;
    public bool HasDrink { get { return hasDrink; } set { hasDrink = value; drinkUI.color = value ? Color.white : Color.gray*0.5f; } }
    private void Start()
    {
        StartCoroutine(BatteryDecrease());
        HasDrink = false;
    }
    public void Phone()
    {
        phone.SetActive(!phone.activeSelf);
        Time.timeScale = phone.activeSelf ? 0f : 1f;
        Cursor.lockState = phone.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
    public void ChangeSensitivity(int num)
    {
        player.sensitivity[num] = sensitivitySlider[num].value;
    }
    public IEnumerator MakeMoney(float time)
    {
        float nowTime = 0;
        ShowScript("searching...", Color.green);
        sounds[2].Play();
        while (time > nowTime)
        {
            nowTime += Time.deltaTime;
            yield return null;
            makeMoneyUI.fillAmount = nowTime / time;
            if (!Input.GetMouseButton(0))
            {
                makeMoneyUI.fillAmount = 0;
                ScriptFlush();
                yield break;
            }
        }
        makeMoneyUI.fillAmount = 0;
        ScriptFlush();
        if (Random.value > 0.5f)
        {
            sounds[3].Play();
            money += Random.Range(10, 21) * 100;
            MoneyChange(money);
        }
        else
        {
            sounds[4].Play();
            ShowScript("Fail", Color.red, 1f);
        }
    }
    public IEnumerator SpeedBuff(float dur)
    {
        HasDrink = false;
        float nowTime = 0;
        player.speed += 10;
        while (dur > nowTime) {
            nowTime += Time.deltaTime;
            buffUI.fillAmount = 1 - nowTime / dur;
            yield return null;
        }
        player.speed -= 10;
    }
    public void EnemyEnhance(int phase)
    {
        switch (phase)
        {
            case 0:
                enemy.range *= 0.95f;
                enemy.navi.speed = player.speed;
                break;
            case 1:
                enemy.isTrace = true;
                enemy.navi.speed = player.speed * 1.1f;
                break;
        }
    }
    IEnumerator BatteryDecrease()
    {
        WaitForSeconds oneSec = new(1f);
        while (true)
        {
            if(BatteryAmount > 0)
            {
                BatteryAmount--;
                if (BatteryAmount == 0) flashlight.range = 0;
                else flashlight.range = BatteryAmount / 2 + 15;
            }
            yield return oneSec;
        }
    }
    public void BatteryCharge(int amount)
    {
        BatteryAmount = (BatteryAmount + amount > 100) ? 100 : BatteryAmount + amount;
    }
    public void MoneyChange(int money)
    {
        moneyText.text = money+" won";
        moneyText.color = money > 2000 ? Color.yellow : Color.red;
    }
    public void ScriptFlush()
    {
        script.text = "";
    }
    public void ShowScript(string saying, Color textColor, float time = 0f)
    {
        script.text = saying;
        script.color = textColor;
        if(time > 0)
            Invoke("ScriptFlush", time);
    }
}
