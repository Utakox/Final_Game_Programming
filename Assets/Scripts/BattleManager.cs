using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    // ── UI ───────────────────────────────────
    [Header("Player UI")]
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI spText;
    public TextMeshProUGUI ultimateText;
    public TextMeshProUGUI enemyHealthUI;
    public TextMeshProUGUI logText;

    public Button attackButton;
    public Button skillButton;
    public Button ultimateButton;

    [Header("Canvas")]
    public Canvas battleUICanvas;

    // ── Runtime ──────────────────────────────
    HeroSaveData             _hero;
    int                      _playerHP;
    int                      _sp;
    int                      _ultimate;
    int                      _target;
    List<EnemyData>          _enemies        = new List<EnemyData>();
    List<GameObject>         _enemyInGroup   = new List<GameObject>();
    List<int>                _enemyHP        = new List<int>();
    List<EnemyHPDisplay>     _enemyDisplays  = new List<EnemyHPDisplay>();
    bool                     _playerTurn;

    // ═════════════════════════════════════════
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // ═════════════════════════════════════════
    //  BattleTrigger เรียกอันนี้
    // ═════════════════════════════════════════
    public void StartBattleFromTrigger(List<EnemyData> enemies, List<GameObject> enemyInGroup)
    {
        _enemies      = enemies;
        _enemyInGroup = enemyInGroup;

        _hero = HeroSaveManager.Load();

        if (_hero == null)
        {
            Debug.LogError("ไม่พบข้อมูล Hero!");
            return;
        }

        _playerHP = _hero.maxHP;
        _sp       = 0;
        _ultimate = 0;
        _target   = 0;

        _enemyHP.Clear();
        foreach (EnemyData e in _enemies)
            _enemyHP.Add(e.maxHP);

        // หา EnemyHPDisplay จากตัวศัตรูแต่ละตัว
        _enemyDisplays.Clear();
        foreach (GameObject obj in _enemyInGroup)
        {
            if (obj != null)
                _enemyDisplays.Add(obj.GetComponent<EnemyHPDisplay>());
            else
                _enemyDisplays.Add(null);
        }

        if (battleUICanvas != null)
            battleUICanvas.gameObject.SetActive(true);

        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;

        if (BattleStage.Instance != null)
            BattleStage.Instance.EnterBattleStage(_enemyInGroup);

        RefreshUI();
        StartCoroutine(BattleStart());
    }

    // ═════════════════════════════════════════
    IEnumerator BattleStart()
    {
        SetAllButtons(false);
        Log("Battle Start!");
        yield return new WaitForSeconds(1.5f);
        PlayerTurn();
    }

    void PlayerTurn()
    {
        _playerTurn = true;
        SetAllButtons(true);

        if (ultimateButton != null)
            ultimateButton.interactable = _ultimate >= _hero.ultimateCost;

        Log("Your Turn!  [" + _hero.heroName + "]  Target: " + _enemies[_target].enemyName);
    }

    void Update()
    {
        if (!_playerTurn) return;
        if (Input.GetKeyDown(KeyCode.Q))   OnAttack();
        if (Input.GetKeyDown(KeyCode.E))   OnSkill();
        if (Input.GetKeyDown(KeyCode.R))   OnUltimate();
        if (Input.GetKeyDown(KeyCode.Tab)) NextTarget();
    }

    // ═════════════════════════════════════════
    //  Actions
    // ═════════════════════════════════════════

    public void OnAttack()
    {
        if (!_playerTurn) return;

        _playerTurn = false;
        SetAllButtons(false);

        int dmg = _hero.attackPower;
        _enemyHP[_target] -= dmg;

        if (_enemyHP[_target] < 0)
            _enemyHP[_target] = 0;

        _sp += _hero.spGainAttack;
        if (_sp > _hero.maxSP)
            _sp = _hero.maxSP;

        _ultimate += _hero.ultGainAttack;
        if (_ultimate > _hero.ultimateCost)
            _ultimate = _hero.ultimateCost;

        Log(_hero.heroName + " attacks " + _enemies[_target].enemyName + " for " + dmg + "!");
        RefreshUI();
        StartCoroutine(DoAttack());
    }

    IEnumerator DoAttack()
    {
        if (BattleStage.Instance != null)
            yield return BattleStage.Instance.PlayerLunge(_target);
        else
            yield return new WaitForSeconds(1.5f);

        yield return AfterPlayer();
    }

    public void OnSkill()
    {
        if (!_playerTurn) return;

        if (_sp < _hero.spCost)
        {
            Log("Not enough SP!");
            return;
        }

        _playerTurn = false;
        SetAllButtons(false);

        int dmg = _hero.skillPower;
        _enemyHP[_target] -= dmg;

        if (_enemyHP[_target] < 0)
            _enemyHP[_target] = 0;

        _sp -= _hero.spCost;
        _sp += _hero.spGainSkill;

        if (_sp < 0)
            _sp = 0;

        _ultimate += _hero.ultGainSkill;
        if (_ultimate > _hero.ultimateCost)
            _ultimate = _hero.ultimateCost;

        Log(_hero.heroName + " uses Skill on " + _enemies[_target].enemyName + " for " + dmg + "!");
        RefreshUI();
        StartCoroutine(DoSkill());
    }

    IEnumerator DoSkill()
    {
        if (BattleStage.Instance != null)
            yield return BattleStage.Instance.PlayerSkillAnimation(_hero.heroName, _target);
        else
            yield return new WaitForSeconds(2.8f);

        yield return AfterPlayer();
    }

    public void OnUltimate()
    {
        if (!_playerTurn) return;

        if (_ultimate < _hero.ultimateCost)
        {
            Log("Ultimate not ready!");
            return;
        }

        _playerTurn = false;
        SetAllButtons(false);

        int dmg = _hero.ultimatePower;
        _enemyHP[_target] -= dmg;

        if (_enemyHP[_target] < 0)
            _enemyHP[_target] = 0;

        _ultimate = 0;

        Log(_hero.heroName + " ULTIMATE! " + _enemies[_target].enemyName + " took " + dmg + "!");
        RefreshUI();
        StartCoroutine(DoUltimate());
    }

    IEnumerator DoUltimate()
    {
        if (BattleStage.Instance != null)
            yield return BattleStage.Instance.PlayerUltimateAnimation(_hero.heroName, _target);
        else
            yield return new WaitForSeconds(2.8f);

        yield return AfterPlayer();
    }

    // ═════════════════════════════════════════
    void NextTarget()
    {
        _target++;

        if (_target >= _enemies.Count)
            _target = 0;

        if (_enemyHP[_target] <= 0)
        {
            NextTarget();
            return;
        }

        RefreshUI();
        Log("Target: " + _enemies[_target].enemyName);
    }

    // ═════════════════════════════════════════
    IEnumerator AfterPlayer()
    {
        yield return new WaitForSeconds(0.5f);

        if (_enemyHP[_target] <= 0)
        {
            Log(_enemies[_target].enemyName + " is defeated!");

            if (_target < _enemyInGroup.Count && _enemyInGroup[_target] != null)
            {
                Destroy(_enemyInGroup[_target]);
                _enemyInGroup[_target] = null;
            }
        }

        yield return new WaitForSeconds(1.5f);

        bool allDead = true;
        for (int i = 0; i < _enemyHP.Count; i++)
        {
            if (_enemyHP[i] > 0)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            StartCoroutine(Victory());
            yield break;
        }

        FindNextTarget();
        RefreshUI();
        StartCoroutine(EnemyTurn());
    }

    // ═════════════════════════════════════════
    IEnumerator EnemyTurn()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemyHP[i] <= 0)
                continue;

            Log(_enemies[i].enemyName + "'s Turn");
            yield return new WaitForSeconds(0.8f);

            bool heavy = Random.value > _enemies[i].normalAttackChance;

            int rawDmg;
            if (heavy)
                rawDmg = _enemies[i].skillPower;
            else
                rawDmg = _enemies[i].attackPower;

            int dmg = rawDmg;
            if (dmg < 1)
                dmg = 1;

            if (BattleStage.Instance != null)
                yield return BattleStage.Instance.EnemyLunge(i);

            _playerHP -= dmg;
            if (_playerHP < 0)
                _playerHP = 0;

            if (heavy)
                Log(_enemies[i].enemyName + " Heavy Attack! You took " + dmg);
            else
                Log(_enemies[i].enemyName + " Normal Attack! You took " + dmg);

            RefreshUI();
            yield return new WaitForSeconds(1.5f);

            if (_playerHP <= 0)
            {
                StartCoroutine(Defeat());
                yield break;
            }
        }

        PlayerTurn();
    }

    // ═════════════════════════════════════════
    void FindNextTarget()
    {
        for (int i = 0; i < _enemyHP.Count; i++)
        {
            if (_enemyHP[i] > 0)
            {
                _target = i;
                return;
            }
        }
    }

    IEnumerator Victory()
    {
        Log("You Won!");
        yield return new WaitForSeconds(1.5f);

        if (BattleStage.Instance != null)
            BattleStage.Instance.ExitBattleStage();

        if (battleUICanvas != null)
            battleUICanvas.gameObject.SetActive(false);

        Cursor.visible   = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    IEnumerator Defeat()
    {
        Log("You Lost...");
        yield return new WaitForSeconds(1.5f);
        Log("Game Over! Restarting...");
        yield return new WaitForSeconds(2.5f);

        if (BattleStage.Instance != null)
            BattleStage.Instance.ExitBattleStage();

        if (battleUICanvas != null)
            battleUICanvas.gameObject.SetActive(false);

        Cursor.visible   = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("HeroSelect");
    }

    // ═════════════════════════════════════════
    void RefreshUI()
    {
        if (playerHPText != null)
            playerHPText.text = _playerHP + " / " + _hero.maxHP;

        if (spText != null)
            spText.text = "SP: " + _sp + " / " + _hero.maxSP;

        if (ultimateText != null)
            ultimateText.text = "Ult: " + _ultimate + " / " + _hero.ultimateCost;

        if (enemyHealthUI != null)
            enemyHealthUI.text = "Targetting: " + _enemies[_target].enemyName + "  " + _enemyHP[_target] + " / " + _enemies[_target].maxHP;

        if (ultimateButton != null)
            ultimateButton.interactable = _playerTurn && _ultimate >= _hero.ultimateCost;

        if (skillButton != null)
        {
            ColorBlock c = skillButton.colors;

            if (_sp >= _hero.spCost)
                c.normalColor = Color.white;
            else
                c.normalColor = Color.gray;

            skillButton.colors = c;
        }

        // อัปเดต HP ผ่าน EnemyHPDisplay ของศัตรูแต่ละตัว
        for (int i = 0; i < _enemyDisplays.Count; i++)
        {
            if (_enemyDisplays[i] != null)
                _enemyDisplays[i].UpdateHP(_enemyHP[i]);
        }
    }

    void SetAllButtons(bool on)
    {
        if (attackButton != null)
            attackButton.interactable = on;

        if (skillButton != null)
            skillButton.interactable = on;

        if (ultimateButton != null)
        {
            if (on && _ultimate >= _hero.ultimateCost)
                ultimateButton.interactable = true;
            else
                ultimateButton.interactable = false;
        }
    }

    void Log(string msg)
    {
        if (logText != null)
            logText.text = msg;
    }
}