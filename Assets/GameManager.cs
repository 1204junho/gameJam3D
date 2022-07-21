using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public AudioSource[] sounds;
    public TMP_Text moneyText, script, atmMoneyText;
    public TMP_Text[] coinTexts;
    public Image batteryUI, drinkUI, buffUI, makeMoneyUI;
    public Slider[] sensitivitySlider;
    public GameObject phone, PlayerHideUI,gameOverScreen,ATM;
    public GameObject[] ATMUI;
    public Gradient batteryState;
    public Light flashlight;
    Enemy enemy;
    Player player;
    public Vector3 playerSpawn, enemySpawn;
    [SerializeField, Range(0, 100)]
    int battery, money, atmAimMoney, atmNowMoney;
    public int[] coinPrice;
    int BatteryAmount {
        get { return battery; }
        set { battery = value; batteryUI.fillAmount = (float)value / 100; batteryUI.color = batteryState.Evaluate((float)value / 100); } }
    public int Money
    {
        get { return money; }
        set
        {
            money = value;
            moneyText.text = value +"";
            moneyText.color = value > 2000 ? Color.yellow : Color.red;
            if(value >= 2000)
            {
                if (value < 5000)
                    enemy.NowPhase = 1;
                else 
                    enemy.NowPhase = 2;
            }
        }
    }
    [SerializeField]
    bool hasDrink, isPlayerHide;
    float leftBuffTime;
    public bool IsPlayerHide{ get { return isPlayerHide; } set {
            isPlayerHide = value;
            PlayerHideUI.SetActive(value);
            if (value)
            {
                player.GetComponent<Collider>().enabled = false;
                ShowScript("press E to get out", Color.green);
            }
            else
            {
                player.transform.position += player.transform.rotation * Vector3.forward;
                player.GetComponent<Collider>().enabled = true;
                ScriptFlush();
            }
        }
    }
    public bool HasDrink {
        get { return hasDrink; }
        set {
            hasDrink = value;
            drinkUI.color = Color.white * (value ? 1f : 0.25f);
            if (value) sounds[0].Play();
            else
            {
                if (leftBuffTime > 0) StopCoroutine(SpeedBuff());
                StartCoroutine(SpeedBuff());
            }
                
        }
    }
    public int curserLockState { set { Cursor.lockState = (CursorLockMode)value; } }
    //-----------------------------------------------------------------------------------
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        enemy = GameObject.Find("Enemy").GetComponent<Enemy>();
        player = GameObject.Find("Player").GetComponent<Player>();
        StartCoroutine(BatteryDecrease());
        hasDrink = false;
        drinkUI.color = Color.white * 0.25f;
        StartCoroutine(RandomCoinPrice());
        coinPrice = new int[] { Random.Range(300, 1500), Random.Range(300, 1500),Random.Range(300, 1500)};
    }
    public void GoTitle()
    {
        SceneManager.LoadScene(0);
    }
    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
    IEnumerator RandomCoinPrice()
    {
        int i;
        WaitForSeconds oneMin = new(1f);
        while(true)
        {
            for( i = 0; i < 3; i++)
            {
                coinPrice[i] = (int)((Random.value+0.5f) * coinPrice[i]);
                coinTexts[i].text = coinPrice[i] + "";
                Debug.Log(i + " : " + coinPrice[i]);
            }
            yield return oneMin;
        }
    }
    
    public void UseATM(int menu)
    {
        Cursor.lockState = CursorLockMode.Confined;
        ATMUI[menu].SetActive(true);
        switch (menu)
        {
            case 1:
                ATMDeposit();
            break;
            default:
                break;
        }
        
    }
    public void ATMDeposit()
    {
        if (Money <= 2000)
            ShowScript("you dont have enough money", Color.red, 1f);
        else
        {
            atmNowMoney += Money - 2000;
            Money = 2000;
            atmMoneyText.text = atmNowMoney + " / " + atmAimMoney;
            if (atmNowMoney > atmAimMoney) Destroy(ATM);
        }
        
    }
    public void UseVending(Vending_machine mach)
    {
        if (mach.price < Money)
        {
            Money -= mach.price;
            mach.DropItem();
        }
        else
            ShowScript("you dont have enough money",Color.red,1f);
            
    }
    public void Phone()
    {
        phone.SetActive(!phone.activeSelf);
        Time.timeScale = phone.activeSelf ? 0f : 1f;
        Cursor.lockState = phone.activeSelf ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
    public void EnemyHitPlayer()
    {
        if (Money <= 2000)
        {
            gameOverScreen.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Money = (int)(0.85f * Money);
            player.transform.position = playerSpawn;
            enemy.transform.position = enemySpawn;
            if(IsPlayerHide)
                IsPlayerHide = false;
        }
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
            Money += Random.Range(10, 21) * 100;
        }
        else
        {
            sounds[4].Play();
            ShowScript("Fail", Color.red, 1f);
        }
    }
    public IEnumerator SpeedBuff()
    {
        leftBuffTime = 3;
        sounds[1].Play();
        player.speed += 10;
        while (leftBuffTime > 0) {
            leftBuffTime -= Time.deltaTime;
            buffUI.fillAmount = leftBuffTime / 3;
            yield return null;
        }
        player.speed -= 10;
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
    void ScriptFlush(string saying = null)
    {
        if(saying == null || script.text == saying)
            script.text = "";
    }
    IEnumerator ScriptFlushDelay(float time, string saying = null)
    {
        yield return new WaitForSeconds(time);
        if (saying == null || script.text == saying)
            script.text = "";
    }
    public void ShowScript(string saying, Color textColor, float time = 0f)
    {
        script.text = saying;
        script.color = textColor;
        if (time > 0.1f)
            StartCoroutine(ScriptFlushDelay(time,saying));
    }
}
