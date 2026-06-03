using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Project.Scripts.Game
{
    public class LevelSelectButton : MonoBehaviour
    {
        [SerializeField] private int levelNumber = 1;
        [SerializeField] private string gameSceneName = "GameScene";
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text buttonText;

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

            bool isUnlocked = levelNumber <= unlockedLevel;

            button.interactable = isUnlocked;

            if (buttonText != null)
            {
                buttonText.text = isUnlocked
                    ? "Уровень " + levelNumber
                    : "Закрыто";
            }

            button.onClick.AddListener(LoadLevel);
        }

        private void LoadLevel()
        {
            PlayerPrefs.SetInt("SelectedLevel", levelNumber);
            PlayerPrefs.Save();

            SceneManager.LoadScene(gameSceneName);
        }
    }
}