using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance;
	private const string TutorialSceneName = "Tutorial";
	private const string MainMenuSceneName = "Main Menu";

	public LevelState State { get; private set; } = LevelState.None;
	public bool HasStateInitialized { get; private set; }
	public event System.Action<LevelState> OnLevelStateChanged;

	private LevelDefinition _activeLevel;
	private Coroutine _sceneLoadRoutine;
	private bool _isSubscribedToGameManager;
	private bool _isSubscribedToPuzzleManager;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void OnEnable()
	{
		TrySubscribeToGameManager();
		TrySubscribeToPuzzleManager();
	}

	private void Start()
	{
		TrySubscribeToGameManager();
		TrySubscribeToPuzzleManager();
	}

	private void OnDisable()
	{
		if (_isSubscribedToGameManager && GameManager.Instance != null)
			GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;

		if (_isSubscribedToPuzzleManager && PuzzleManager.Instance != null)
			PuzzleManager.Instance.OnPuzzleStateChangedDetails -= HandlePuzzleStateChanged;

		_isSubscribedToGameManager = false;
		_isSubscribedToPuzzleManager = false;
	}

	private void TrySubscribeToGameManager()
	{
		if (_isSubscribedToGameManager || GameManager.Instance == null)
			return;

		GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
		_isSubscribedToGameManager = true;

		if (GameManager.Instance.HasStateInitialized)
			HandleGameStateChanged(GameManager.Instance.State);
	}

	private void TrySubscribeToPuzzleManager()
	{
		if (_isSubscribedToPuzzleManager || PuzzleManager.Instance == null)
			return;

		PuzzleManager.Instance.OnPuzzleStateChangedDetails += HandlePuzzleStateChanged;
		_isSubscribedToPuzzleManager = true;
	}

	public void RegisterLevel(LevelDefinition definition)
	{
		if (definition == null)
			return;

		LevelDefinition previousActiveLevel = _activeLevel;

		if (previousActiveLevel != null && previousActiveLevel != definition && previousActiveLevel.SceneName == definition.SceneName)
			Debug.LogWarning($"LevelManager replacing active level definition in scene '{definition.SceneName}'.");

		_activeLevel = definition;

		// When a new scene registers its level, reset from Completed so completion can occur again.
		bool sceneChanged = previousActiveLevel == null || !string.Equals(previousActiveLevel.SceneName, definition.SceneName);
		if (sceneChanged || State != LevelState.Completed)
			UpdateLevelState(LevelState.InProgress);

		if (_activeLevel.AutoCompleteWhenReady)
			TryCompleteLevel(_activeLevel);
	}

	public void UnregisterLevel(LevelDefinition definition)
	{
		if (_activeLevel == definition)
			_activeLevel = null;
	}

	public bool TryCompleteActiveLevel()
	{
		return TryCompleteActiveLevel(bypassRequirementChecks: false);
	}

	public bool TryCompleteActiveLevel(bool bypassRequirementChecks)
	{
		LevelDefinition definition = GetOrCreateActiveLevelDefinition();
		return TryCompleteLevel(definition, bypassRequirementChecks);
	}

	public bool TryCompleteLevel(LevelDefinition definition)
	{
		return TryCompleteLevel(definition, bypassRequirementChecks: false);
	}

	public bool TryCompleteLevel(LevelDefinition definition, bool bypassRequirementChecks)
	{
		if (definition == null)
			return false;

		if (State == LevelState.Completed)
			return true;

		if (!bypassRequirementChecks && !AreCompletionRequirementsMet(definition))
			return false;

		CompleteLevel(definition);
		return true;
	}

	public bool AreCompletionRequirementsMet(LevelDefinition definition)
	{
		if (definition == null)
			return false;

		string sceneName = definition.SceneName;

		if (definition.RequireAllScenePuzzlesSolved)
		{
			if (PuzzleManager.Instance == null)
				return false;

			bool allSolved = PuzzleManager.Instance.AreAllPuzzlesSolvedInScene(
				sceneName,
				definition.RequireAtLeastOnePuzzleWhenUsingSceneScan
			);

			if (!allSolved)
				return false;
		}

		if (definition.RequiredPuzzleIds.Count > 0)
		{
			if (PuzzleManager.Instance == null)
				return false;

			for (int i = 0; i < definition.RequiredPuzzleIds.Count; i++)
			{
				string puzzleId = definition.RequiredPuzzleIds[i];
				if (string.IsNullOrWhiteSpace(puzzleId))
					continue;

				if (!PuzzleManager.Instance.TryGetPuzzleState(sceneName, puzzleId, out IPuzzle.PuzzleState state))
					return false;

				if (state != IPuzzle.PuzzleState.Solved)
					return false;
			}
		}

		return true;
	}

	public void HandleGameStateChanged(GameState newState)
	{
		switch (newState)
		{
			case GameState.Playing:
				if (_activeLevel != null && State != LevelState.Completed)
					UpdateLevelState(LevelState.InProgress);
				break;
			case GameState.LevelCompleted:
				UpdateLevelState(LevelState.Completed);
				break;
		}
	}

	private void HandlePuzzleStateChanged(string sceneName, string puzzleId, IPuzzle.PuzzleState puzzleState)
	{
		if (_activeLevel == null)
			return;

		if (!_activeLevel.AutoCompleteWhenReady)
			return;

		if (!string.Equals(sceneName, _activeLevel.SceneName))
			return;

		if (State == LevelState.Completed)
			return;

		TryCompleteLevel(_activeLevel);
	}

	private void CompleteLevel(LevelDefinition definition)
	{
		UpdateLevelState(LevelState.Completed);

		if (GameManager.Instance != null)
			GameManager.Instance.UpdateGameState(GameState.LevelCompleted);

		if (SaveManager.Instance != null)
		{
			SaveManager.Instance.SetCurrentScene(definition.SceneName);
			SaveManager.Instance.MarkLevelCompleted(definition.LevelId);

			if (!string.IsNullOrWhiteSpace(definition.UnlockLevelId))
				SaveManager.Instance.MarkLevelUnlocked(definition.UnlockLevelId);

			if (SaveManager.Instance.AutoSaveEnabled)
				SaveManager.Instance.SaveGame();
		}

		bool isTutorialLevel = string.Equals(definition.SceneName, TutorialSceneName);
		bool autoLoadNextScene = definition.AutoLoadNextScene || isTutorialLevel;
		string nextSceneName = definition.NextSceneName;

		if (string.IsNullOrWhiteSpace(nextSceneName) && isTutorialLevel)
			nextSceneName = MainMenuSceneName;

		if (!autoLoadNextScene || string.IsNullOrWhiteSpace(nextSceneName))
			return;

		if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
		{
			Debug.LogWarning($"LevelManager cannot load next scene '{nextSceneName}' because it is not in Build Settings.");
			return;
		}

		if (_sceneLoadRoutine != null)
			StopCoroutine(_sceneLoadRoutine);

		_sceneLoadRoutine = StartCoroutine(LoadNextSceneAfterDelay(nextSceneName, definition.NextSceneLoadDelay));
	}

	private IEnumerator LoadNextSceneAfterDelay(string sceneName, float delay)
	{
		if (delay > 0f)
			yield return new WaitForSeconds(delay);

		if (GameManager.Instance != null)
			GameManager.Instance.UpdateGameState(GameState.Loading);

		SceneManager.LoadScene(sceneName);
		_sceneLoadRoutine = null;
	}

	private void UpdateLevelState(LevelState newState)
	{
		State = newState;
		HasStateInitialized = true;
		OnLevelStateChanged?.Invoke(newState);
	}

	private LevelDefinition GetOrCreateActiveLevelDefinition()
	{
		if (_activeLevel != null)
			return _activeLevel;

		LevelDefinition existingDefinition = FindFirstObjectByType<LevelDefinition>();
		if (existingDefinition != null)
		{
			RegisterLevel(existingDefinition);
			return _activeLevel;
		}

		GameObject runtimeDefinitionObject = new GameObject("LevelDefinition (Runtime)");
		LevelDefinition runtimeDefinition = runtimeDefinitionObject.AddComponent<LevelDefinition>();
		RegisterLevel(runtimeDefinition);
		return _activeLevel;
	}

	public bool AreRequirementsMet(LevelDefinition definition)
	{
		return AreCompletionRequirementsMet(definition);
	}
}

public enum LevelState
{
	None,
	InProgress,
	Completed,
	Failed
}
