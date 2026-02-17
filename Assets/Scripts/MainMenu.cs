using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
	private UIDocument _uiDocument;
	private Button _startGameButton;
	private Button _exitGameButton;

	private void Awake()
	{
		_uiDocument = GetComponent<UIDocument>();
		_startGameButton = _uiDocument.rootVisualElement.Q<Button>("StartButton");
		_exitGameButton = _uiDocument.rootVisualElement.Q<Button>("QuitButton");

		_startGameButton.RegisterCallback<ClickEvent>(StartGame);
		_exitGameButton.RegisterCallback<ClickEvent>(QuitGame);
	}

	private void StartGame(ClickEvent evt)
	{
		// TODO: Replace with the Main Level scene index
		SceneManager.LoadScene(0);
	}

	private void QuitGame(ClickEvent evt)
	{
		Application.Quit();
	}
}