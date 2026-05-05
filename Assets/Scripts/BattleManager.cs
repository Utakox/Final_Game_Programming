using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    // ── Player ──────────────────────────────
    [Header("Player Stats")]
    public int playerMaxHP = 120;
    public int playerAttack = 15;
    public int playerSkill = 30;

    [Header("Skill Point")]
    public int maxSP = 5;
    public int spCost = 2;
    public int spGain = 1;

    // ── Player Camera ─────────────────────────────

    // ── Enemies ─────────────────────────────
    [Header("Enemies")]
    public List<EnemyData> enemies;

    public List<GameObject> enemyInGroup;

    // ── UI ─────────────────────────────
    [Header("Player UI")]
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI spText;
    public TextMeshProUGUI enemyHealthUI;
    public TextMeshProUGUI logText;

    public Button attackButton;
    public Button skillButton;

    // [Header("Camera")]
    // public Camera battleCamera;
    // public Camera playerCamera;

    [Header("Enemy HP Labels")]
    public List<HPLabel> enemyHPLabels;

    [Header("Canvas After Win")]

    public Canvas battleUICanvas;

    // [Header("Target Buttons")]
    // public List<Button> enemySelectBtns;

    // ── Runtime ─────────────────────────────
    int playerHP; // เลือด
    int skillPoint; // skill point
    int _target; // target enemy เล็งตัวไหนอยู่

    List<int> _enemyHP = new List<int>();

    bool _playerTurn;

    // ─────────────────────────────
    void Start()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyHPLabels[i].SetHP(enemies[i].maxHP, enemies[i].maxHP);
        }
    // รอ BattleTrigger เรียก InitBattle() แทน
    }

    public void InitBattle()
    {
    playerHP   = playerMaxHP;
    skillPoint = 0;
    _target    = 0;

    _enemyHP.Clear();
    foreach (EnemyData e in enemies)
        _enemyHP.Add(e.maxHP);

    RefreshUI();
    StartCoroutine(BattleStart());
    }

    void NextTarget()
    {
    _target++;

    if (_target >= enemies.Count)
        _target = 0;

    if (_enemyHP[_target] <= 0)
    {
        NextTarget();
        return;
    }

    RefreshUI();

    Log("Target : " + enemies[_target].enemyName);
    }

    IEnumerator BattleStart()
    {
        SetActionButtons(false);

        Log("Battle Start!");

        yield return new WaitForSeconds(1.5f);

        PlayerTurn();
    }

    // ─────────────────────────────
    void PlayerTurn()
    {
        _playerTurn = true;

        SetActionButtons(true);

        Log("Your Turn!");
    }

    void Update()
    {
        if (!_playerTurn) return;

        if (Input.GetKeyDown(KeyCode.Q))
            OnAttack();

        if (Input.GetKeyDown(KeyCode.E))
            OnSkill();
        
        if (Input.GetKeyDown(KeyCode.Tab))
            NextTarget();
    }

    // ─────────────────────────────
    public void OnAttack()
    {
        if (!_playerTurn) return;

        _playerTurn = false;

        SetActionButtons(false);

        int dmg = playerAttack;

        _enemyHP[_target] -= dmg;

        if (_enemyHP[_target] < 0)
            _enemyHP[_target] = 0;

        skillPoint += spGain;

        if (skillPoint > maxSP)
            skillPoint = maxSP;

        Log(enemies[_target].enemyName + " took " + dmg + " damage!");

        RefreshUI();

        StartCoroutine(AfterPlayer());
    }

    public void OnSkill()
    {
        if (!_playerTurn) return;

        if (skillPoint < spCost)
        {
            Log("Not enough SP!");
            return;
        }

        _playerTurn = false;

        SetActionButtons(false);

        int dmg = playerSkill;

        _enemyHP[_target] -= dmg;

        if (_enemyHP[_target] < 0)
            _enemyHP[_target] = 0;

        skillPoint -= spCost;

        Log("Skill hit " + enemies[_target].enemyName + " for " + dmg);

        RefreshUI();

        StartCoroutine(AfterPlayer());
    }

    // ─────────────────────────────
    // void SelectTarget(int idx)
    // {
    //     if (!_playerTurn) return;

    //     if (_enemyHP[idx] <= 0) return;

    //     _target = idx;

    //     RefreshUI();

    //     Log("Target : " + enemies[_target].enemyName);
    // }

    // ─────────────────────────────
    IEnumerator AfterPlayer()
    {
        yield return new WaitForSeconds(1f);

        bool allDead = true;

        for (int i = 0; i < _enemyHP.Count; i++)
        {
            if (_enemyHP[i] > 0)
            {
                allDead = false;
                break;
            }
        }

        if (_enemyHP[_target] <= 0 )
        {
            Log(enemies[_target].enemyName + " is defeated!");
            
            Destroy(enemyInGroup[_target]);
            enemyInGroup[_target] = null;
        }
            yield return new WaitForSeconds(1f);

            FindNextTarget();
            RefreshUI();

        
        if (allDead)
        {
            StartCoroutine(Victory());
            yield break;
        }

    StartCoroutine(EnemyTurn());
    }

    // ─────────────────────────────
    IEnumerator EnemyTurn()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (_enemyHP[i] <= 0)
                continue;

            Log(enemies[i].enemyName + "'s Turn");

            yield return new WaitForSeconds(1f);

            int dmg = enemies[i].attackPower;

            playerHP -= dmg;

            if (playerHP < 0)
                playerHP = 0;

            Log(enemies[i].enemyName + " attacked for " + dmg);

            RefreshUI();

            yield return new WaitForSeconds(1f);

            if (playerHP <= 0)
            {
                Log("You Lost...");
                yield break;
            }
        }

        PlayerTurn();
    }

    void FindNextTarget()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (_enemyHP[i] > 0)
            {
                _target = i;
                return;
            }
        }
    }

    // ─────────────────────────────
    void RefreshUI() // ใช้ update UI (พึ่งมารู้ว่า UI ไม่อัปเดตอัตโนมัติ!!!!)
    {
        if (playerHPText != null)
            playerHPText.text = $"HP: {playerHP} / {playerMaxHP}";

        if (spText != null)
            spText.text = $"SP: {skillPoint} / {maxSP}";

        if (enemyHealthUI != null)
            enemyHealthUI.text = $"Enemy: {enemies[_target].enemyName}\n HP: {_enemyHP[_target]} / {enemies[_target].maxHP}";

        for (int i = 0; i < enemies.Count; i++)
        {
            if (i >= enemyHPLabels.Count) continue;
            if (enemyHPLabels[i] == null) continue;
                enemyHPLabels[i].SetHP(_enemyHP[i], enemies[i].maxHP);
        }
    }

    void SetActionButtons(bool on)
    {
        if (attackButton != null)
            attackButton.interactable = on;

        if (skillButton != null)
            skillButton.interactable = on;
    }

    void Log(string msg)
    {
        if (logText != null)
            logText.text = msg;
    }

 
    IEnumerator Victory()
    {
        Log("You Won!");
        yield return new WaitForSeconds(1.5f);
        battleUICanvas.gameObject.SetActive(false);
        

    }

    
}
