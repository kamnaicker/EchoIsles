using System.Collections.Generic;
using UnityEngine;

public class LevelDefinition : MonoBehaviour
{
	[SerializeField] private string levelId;
	[SerializeField] private string unlockLevelId;
	[SerializeField] private string nextSceneName;
	[SerializeField] private bool autoLoadNextScene = true;
	[SerializeField] private float nextSceneLoadDelay = 1.5f;

	[SerializeField] private bool requireAllScenePuzzlesSolved = false;
	[SerializeField] private bool requireAtLeastOnePuzzleWhenUsingSceneScan = true;
	[SerializeField] private bool autoCompleteWhenReady = false;
	[SerializeField] private List<string> requiredPuzzleIds = new List<string>();

	public string SceneName => gameObject.scene.name;
	public string LevelId => string.IsNullOrWhiteSpace(levelId) ? SceneName : levelId;
	public string UnlockLevelId => unlockLevelId;
	public string NextSceneName => nextSceneName;
	public bool AutoLoadNextScene => autoLoadNextScene;
	public float NextSceneLoadDelay => Mathf.Max(0f, nextSceneLoadDelay);

	public bool RequireAllScenePuzzlesSolved => requireAllScenePuzzlesSolved;
	public bool RequireAtLeastOnePuzzleWhenUsingSceneScan => requireAtLeastOnePuzzleWhenUsingSceneScan;
	public bool AutoCompleteWhenReady => autoCompleteWhenReady;
	public IReadOnlyList<string> RequiredPuzzleIds => requiredPuzzleIds;

	private void OnEnable()
	{
		if (LevelManager.Instance != null)
			LevelManager.Instance.RegisterLevel(this);
	}

	private void Start()
	{
		if (LevelManager.Instance != null)
			LevelManager.Instance.RegisterLevel(this);
	}

	private void OnDisable()
	{
		if (LevelManager.Instance != null)
			LevelManager.Instance.UnregisterLevel(this);
	}

	public bool TryCompleteLevel()
	{
		return LevelManager.Instance != null && LevelManager.Instance.TryCompleteLevel(this);
	}

	public bool TryCompleteLevel(bool bypassRequirementChecks)
	{
		return LevelManager.Instance != null && LevelManager.Instance.TryCompleteLevel(this, bypassRequirementChecks);
	}
}
