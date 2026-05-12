using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalTrigger : MonoBehaviour
{
    [Header("Portal Settings")]
    [SerializeField] private string sceneToLoad = "SceneName"; // Set in Inspector
    [SerializeField] private string playerTag = "Player";

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;

        if (other.CompareTag(playerTag))
        {
            isTransitioning = true;
            TeleportToScene(other.gameObject);
        }
    }

    private void TeleportToScene(GameObject player)
    {
        // Persist the player across scenes
        DontDestroyOnLoad(player);

        // Find and persist the camera too so ThirdPersonController doesn't lose it
        Camera playerCamera = Camera.main;
        if (playerCamera != null)
        {
            // If the camera is NOT already a child of the player, persist it separately
            if (!playerCamera.transform.IsChildOf(player.transform))
            {
                DontDestroyOnLoad(playerCamera.gameObject);
            }
            // If it IS a child of the player, DontDestroyOnLoad on the player covers it already
        }

        // Also persist any AudioListener that lives outside the player
        AudioListener[] audioListeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if (audioListeners.Length > 0 && !audioListeners[0].transform.IsChildOf(player.transform))
        {
            DontDestroyOnLoad(audioListeners[0].gameObject);
        }

        // Subscribe to scene loaded to reposition player
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Unsubscribe so this only fires once
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Find the player that persisted
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        // Find spawn point in the new scene
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

        if (player != null)
        {
            if (spawnPoint != null)
            {
                player.transform.position = spawnPoint.transform.position;
                player.transform.rotation = spawnPoint.transform.rotation;
            }
            else
            {
                player.transform.position = Vector3.zero;
                Debug.LogWarning("PortalTrigger: No SpawnPoint found in scene. Player placed at origin.");
            }

            // Destroy any duplicate camera the new scene spawned
            // since we brought our camera from the previous scene
            Camera[] allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            if (allCameras.Length > 1)
            {
                foreach (Camera cam in allCameras)
                {
                    // Cameras in the new scene will have the new scene's name; destroy those duplicates
                    if (cam.gameObject.scene == scene)
                    {
                        Debug.Log("PortalTrigger: Destroying duplicate camera from new scene: " + cam.name);
                        Destroy(cam.gameObject);
                    }
                }
            }
        }

        isTransitioning = false;
    }
}