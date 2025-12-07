using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) // Dedects if the player collides with th Objective flag and load the next level
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Level Complete!");
            SceneManager.LoadScene("Dijkstra");
        }
    }
}
