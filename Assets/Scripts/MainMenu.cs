using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
	private UIDocument _uiDocument;
	private Button _startGameButton;
	private Button _tutorialGameButton;
	private Button _exitGameButton;

	private void Awake()
	{
		_uiDocument = GetComponent<UIDocument>();
		_startGameButton = _uiDocument.rootVisualElement.Q<Button>("StartButton");
        _tutorialGameButton = _uiDocument.rootVisualElement.Q<Button>("TutorialButton");
        _exitGameButton = _uiDocument.rootVisualElement.Q<Button>("QuitButton");

		_startGameButton.RegisterCallback<ClickEvent>(StartGame);
		_tutorialGameButton.RegisterCallback<ClickEvent>(StartTutorial);
        _exitGameButton.RegisterCallback<ClickEvent>(QuitGame);
	}

	private void StartGame(ClickEvent evt)
	{
		SceneManager.LoadScene(1);
	}

	private void StartTutorial(ClickEvent evt)
	{
		SceneManager.LoadScene(2);
    }

    private void QuitGame(ClickEvent evt)
	{
		Application.Quit();
	}
}