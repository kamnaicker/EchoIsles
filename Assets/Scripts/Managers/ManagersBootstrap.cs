using UnityEngine;

public static class ManagersBootstrap
{
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void BootstrapManagers()
	{
		EnsureManager<GameManager>("GameManager");
		EnsureManager<SaveManager>("SaveManager");
		EnsureManager<PuzzleManager>("PuzzleManager");
		EnsureManager<AudioManager>("AudioManager");
		EnsureManager<InteractionManager>("InteractionManager");
		EnsureManager<PlayerManager>("PlayerManager");
		EnsureManager<EchoManager>("EchoManager");
		EnsureManager<LevelManager>("LevelManager");
	}

	private static void EnsureManager<T>(string managerName) where T : MonoBehaviour
	{
		if (Object.FindFirstObjectByType<T>() != null)
			return;

		GameObject managerObject = new GameObject(managerName);
		managerObject.AddComponent<T>();
	}
}
