using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class BattleStage : MonoBehaviour
{
    public static BattleStage Instance { get; private set; }

    [Header("Cameras")]
    public Camera worldCamera;
    public Camera battleCamera;

    [Header("Battle Stage Positions")]
    public Transform   playerBattleSlot;
    public Transform[] enemyBattleSlots;

    [Header("References")]
    public GameObject playerObject;

    [Header("Lunge Settings")]
    public float lungeDistance = 1.5f;
    public float lungeSpeed    = 8f;
    public float returnSpeed   = 5f;

    [Header("Video Player")]
    [Tooltip("VideoPlayer component ที่ใช้เล่น Skill / Ultimate")]
    public VideoPlayer videoPlayer;

    [Tooltip("Panel ที่ครอบ RawImage สำหรับแสดง Video (ซ่อนไว้ก่อน)")]
    public GameObject videoPanel;

    [Tooltip("Battle UI Canvas ที่จะซ่อนตอนเล่น Ultimate Video")]
    public Canvas battleUICanvas;

    [Header("Hero Video Table")]
    [Tooltip("ลาก HeroData ทุกตัวมาใส่ที่นี่ — ใช้ lookup VideoClip ตาม heroName")]
    public HeroData[] heroDataList;

    // ── Runtime ───────────────────────────────
    Vector3          _playerOriginInBattle;
    List<GameObject> _activeEnemies = new List<GameObject>();
    Vector3[]        _enemyOrigins;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // =========================================
    public void EnterBattleStage(List<GameObject> enemyInGroup)
    {
        if (worldCamera  != null) worldCamera.gameObject.SetActive(false);
        if (battleCamera != null) battleCamera.gameObject.SetActive(true);

        if (playerObject != null && playerBattleSlot != null)
        {
            playerObject.transform.position = playerBattleSlot.position;
            playerObject.transform.rotation = playerBattleSlot.rotation;
            _playerOriginInBattle = playerBattleSlot.position;
        }

        _activeEnemies = enemyInGroup;
        _enemyOrigins  = new Vector3[_activeEnemies.Count];

        for (int i = 0; i < _activeEnemies.Count; i++)
        {
            if (_activeEnemies[i] == null) continue;

            Vector3 slot = (i < enemyBattleSlots.Length && enemyBattleSlots[i] != null)
                           ? enemyBattleSlots[i].position
                           : transform.position + Vector3.right * (i * 2f);

            _activeEnemies[i].transform.position = slot;

            if (i < enemyBattleSlots.Length && enemyBattleSlots[i] != null)
                _activeEnemies[i].transform.rotation = enemyBattleSlots[i].rotation;

            _enemyOrigins[i] = slot;
        }
    }

    public void ExitBattleStage()
    {
        if (battleCamera != null) battleCamera.gameObject.SetActive(false);
        if (worldCamera  != null) worldCamera.gameObject.SetActive(true);
    }

    // =========================================
    //  Animations
    // =========================================

    /// <summary>Attack: lunge ปกติ</summary>
    public IEnumerator PlayerLunge(int targetIndex)
    {
        if (playerObject == null) yield break;

        Vector3 dir    = GetEnemyLungeDirection(targetIndex);
        Vector3 origin = _playerOriginInBattle;
        Vector3 dest   = origin + dir * lungeDistance;

        yield return MoveTo(playerObject.transform, dest, lungeSpeed);
        yield return new WaitForSeconds(0.05f);
        yield return MoveTo(playerObject.transform, origin, returnSpeed);
    }

    /// <summary>Skill: เล่น Video ของ Hero นั้น (ถ้ายังไม่มี Video จะ lunge แทน)</summary>
    public IEnumerator PlayerSkillAnimation(string heroName, int targetIndex)
    {
        VideoClip clip = FindSkillVideo(heroName);

        if (clip != null)
            yield return PlayVideo(clip);
        else
            yield return PlayerLunge(targetIndex);  // fallback ถ้ายังไม่ได้ใส่ Video
    }

    /// <summary>Ultimate: เล่น Video ของ Hero นั้น (ถ้ายังไม่มี Video จะ lunge แทน)</summary>
    public IEnumerator PlayerUltimateAnimation(string heroName, int targetIndex)
    {
        VideoClip clip = FindUltimateVideo(heroName);

        if (clip != null)
            yield return PlayVideo(clip, hideBattleUI: true);
        else
        {
            // fallback: lunge ยาวกว่าปกติ
            float old = lungeDistance;
            lungeDistance = old * 1.5f;
            yield return PlayerLunge(targetIndex);
            lungeDistance = old;
        }
    }

    // =========================================
    //  Video
    // =========================================
    IEnumerator PlayVideo(VideoClip clip, bool hideBattleUI = false)
    {
        if (videoPlayer == null || clip == null) yield break;

        // ซ่อน Battle UI ถ้าต้องการ
        if (hideBattleUI && battleUICanvas != null)
            battleUICanvas.gameObject.SetActive(false);

        if (videoPanel != null) videoPanel.SetActive(true);

        videoPlayer.Stop();
        videoPlayer.clip = clip;
        videoPlayer.Prepare();

        yield return new WaitUntil(() => videoPlayer.isPrepared);

        videoPlayer.Play();

        // รอ 1 frame ให้ isPlaying เป็น true ก่อน
        yield return null;

        yield return new WaitUntil(() => !videoPlayer.isPlaying);

        if (videoPanel != null) videoPanel.SetActive(false);

        // คืน Battle UI
        if (hideBattleUI && battleUICanvas != null)
            battleUICanvas.gameObject.SetActive(true);
    }

    // =========================================
    //  Lookup VideoClip จาก heroName
    // =========================================
    VideoClip FindSkillVideo(string heroName)
    {
        if (heroDataList == null) return null;
        foreach (var h in heroDataList)
            if (h != null && h.heroName == heroName) return h.skillVideo;
        return null;
    }

    VideoClip FindUltimateVideo(string heroName)
    {
        if (heroDataList == null) return null;
        foreach (var h in heroDataList)
            if (h != null && h.heroName == heroName) return h.ultimateVideo;
        return null;
    }

    // =========================================
    //  Enemy
    // =========================================
    public IEnumerator EnemyLunge(int enemyIndex)
    {
        if (enemyIndex >= _activeEnemies.Count || _activeEnemies[enemyIndex] == null)
            yield break;

        Vector3 dir = playerObject != null
            ? (_activeEnemies[enemyIndex].transform.position - playerObject.transform.position).normalized * -1f
            : Vector3.left;

        dir.y = 0;
        dir.Normalize();

        Vector3 origin = _enemyOrigins[enemyIndex];
        Vector3 dest   = origin + dir * lungeDistance;

        yield return MoveTo(_activeEnemies[enemyIndex].transform, dest, lungeSpeed);
        yield return new WaitForSeconds(0.05f);
        yield return MoveTo(_activeEnemies[enemyIndex].transform, origin, returnSpeed);
    }

    // =========================================
    //  Helpers
    // =========================================
    IEnumerator MoveTo(Transform t, Vector3 target, float speed)
    {
        while (Vector3.Distance(t.position, target) > 0.01f)
        {
            t.position = Vector3.MoveTowards(t.position, target, speed * Time.deltaTime);
            yield return null;
        }
        t.position = target;
    }

    Vector3 GetEnemyLungeDirection(int targetIndex)
    {
        if (targetIndex < _activeEnemies.Count && _activeEnemies[targetIndex] != null && playerObject != null)
        {
            Vector3 d = (_activeEnemies[targetIndex].transform.position - playerObject.transform.position);
            d.y = 0;
            return d.normalized;
        }
        return Vector3.right;
    }
}