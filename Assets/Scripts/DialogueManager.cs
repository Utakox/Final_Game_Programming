using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("UI References")]
    public GameObject      dialogueBox;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI npcNameText;
    public Button          nextButton;

    Rigidbody _playerRb;

    string[] _lines;
    int      _index;

    void Awake()
    {
        instance = this;
        dialogueBox.SetActive(false);
        nextButton.onClick.AddListener(OnNextClicked);

        // Grab the player's Rigidbody
        _playerRb = FindFirstObjectByType<ThirdPersonController>().GetComponent<Rigidbody>();
    }

    public void StartDialogue(string npcName, string[] lines)
    {
        _lines = lines;
        _index = 0;

        npcNameText.text = npcName;
        dialogueBox.SetActive(true);
        ThirdPersonController.dialouge = true;

        // Freeze the player completely so they don't fall
        if (_playerRb != null)
        {
            _playerRb.linearVelocity        = Vector3.zero;
            _playerRb.constraints   = RigidbodyConstraints.FreezeAll;
        }

        ShowLine();
    }

    void ShowLine()
    {
        dialogueText.text = _lines[_index];
    }

    void OnNextClicked()
    {
        _index++;

        if (_index < _lines.Length)
            ShowLine();
        else
            EndDialogue();
    }

    public void EndDialogue()
    {
        dialogueBox.SetActive(false);
        ThirdPersonController.dialouge = false;

        // Unfreeze the player when dialogue ends
        if (_playerRb != null)
            _playerRb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}