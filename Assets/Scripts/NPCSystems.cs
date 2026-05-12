using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSystems : MonoBehaviour
{
 public string       npcName  = "NPC";
    public DialogueData dialogueData;

    bool _playerNearby = false;

    void Update()
    {
        if (_playerNearby && Input.GetKeyDown(KeyCode.F))
        {
            DialogueManager.instance.StartDialogue(npcName, dialogueData.lines);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _playerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
    if (other.CompareTag("Player"))
    {
        _playerNearby = false;

        // Just close dialogue directly instead of starting a new one
        if (ThirdPersonController.dialouge)
            DialogueManager.instance.EndDialogue();
    }
   }
}