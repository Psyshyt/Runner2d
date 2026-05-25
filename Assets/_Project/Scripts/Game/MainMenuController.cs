using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Game
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Scenes")]
        [SerializeField] private string gameSceneName = "GameScene";

        private void Awake()
        {
            Time.timeScale = 1f;
        }

        public void StartGame()
        {
            if (string.IsNullOrEmpty(gameSceneName))
            {
                Debug.LogError("Не указано название игровой сцены");
                return;
            }

            SceneManager.LoadScene(gameSceneName);
        }
    }
}