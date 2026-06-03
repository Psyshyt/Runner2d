using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Project.Scripts.Game
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "GameScene";

        private void Awake()
        {
            Time.timeScale = 1f;
        }

        public void StartGame()
        {
            PlayerPrefs.SetInt("SelectedLevel", 1);
            PlayerPrefs.Save();

            SceneManager.LoadScene(gameSceneName);
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}