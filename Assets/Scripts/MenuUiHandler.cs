using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUiHandler : MonoBehaviour
{
    public InputField PlayerNameInput;
    public TextMeshProUGUI HighScoresText;
    public Text PlaceholderText;
    public AudioClip ButtonClick;
    public GameObject MainCamera;

    private Color PlaceholderNormalColor = Color.black;
    private Color PlaceholderErrorColor = Color.red;
    private string DefaultPlaceholderText = "Enter name to start";
    private string ErrorPlaceholderText = "Name Required!";
    private AudioSource AudioSource;

    public void PlayerNameChange(string name)
    {
        PlaceholderText.color = Color.black;
        GameManager.Instance.PlayerName = name;
        GameManager.Instance.SaveGameData();
    }

    private void Start()
    {
        PlayerNameInput.onEndEdit.AddListener(PlayerNameChange);
        PlayerNameInput.text = GameManager.Instance.PlayerName;
        DisplayHighScoresInMenu();
        AudioSource = MainCamera.GetComponent<AudioSource>();
    }

    public void StartGame()
    {
        AudioSource.PlayOneShot(ButtonClick);
        if (!string.IsNullOrWhiteSpace(GameManager.Instance.PlayerName))
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            ChangePlaceholderToError();
        }
    }

    public void ExitGame()
    {
        AudioSource.PlayOneShot(ButtonClick);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    void ChangePlaceholderToError()
    {
        PlaceholderText.color = PlaceholderErrorColor;
        PlaceholderText.text = ErrorPlaceholderText;
    }

    void ResetPlaceholder()
    {
        PlaceholderText.color = PlaceholderNormalColor;
        PlaceholderText.text = DefaultPlaceholderText; // Reset text to default message
    }

    void DisplayHighScoresInMenu()
    {
        List<GameManager.HighScoreEntry> highScores = GameManager.Instance.HighScores;
        StringBuilder highScoreStringBuilder = new StringBuilder();

        if (HighScoresText != null)
        {
            if (highScores != null && highScores.Count > 0)
            {
                highScoreStringBuilder.AppendLine($"HIGH SCORES TOP {highScores.Count}");

                for (int i = 0; i < highScores.Count; i++)
                {
                    highScoreStringBuilder.AppendLine($"{i + 1}: {highScores[i].Name} - {highScores[i].Score}");
                }
            }
            else
            {
                highScoreStringBuilder.AppendLine("HIGH SCORE is 0");
            }

            HighScoresText.text = highScoreStringBuilder.ToString();
        }
        else
        {
            Debug.LogError("HighScoreTextObject is not assigned in the inspector!");
        }
    }
}
