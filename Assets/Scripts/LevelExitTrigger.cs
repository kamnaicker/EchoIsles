using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class LevelExitTrigger : MonoBehaviour
{
	[FormerlySerializedAs("levelDefinition")]
	[SerializeField] private LevelDefinition levelConfig;
	[FormerlySerializedAs("requiredTag")]
	[SerializeField] private string triggeringTag = "Player";
	[FormerlySerializedAs("ignoreRequirements")]
	[SerializeField] private bool bypassRequirementChecks = true;
	[FormerlySerializedAs("oneShot")]
	[SerializeField] private bool triggerOnce = true;
	[FormerlySerializedAs("logBlockedAttempts")]
	[SerializeField] private bool logBlockedCompletions = true;

	private bool _hasTriggered;

	private void Reset()
	{
		Collider colliderComponent = GetComponent<Collider>();
		if (colliderComponent != null)
			colliderComponent.isTrigger = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_hasTriggered && triggerOnce)
			return;

		if (!string.IsNullOrWhiteSpace(triggeringTag) && !other.CompareTag(triggeringTag))
			return;

		LevelDefinition activeLevelConfig = levelConfig;
		if (activeLevelConfig == null)
			activeLevelConfig = FindFirstObjectByType<LevelDefinition>();

		bool didCompleteLevel = activeLevelConfig != null
			? activeLevelConfig.TryCompleteLevel(bypassRequirementChecks)
			: (LevelManager.Instance != null && LevelManager.Instance.TryCompleteActiveLevel(bypassRequirementChecks));

		if (!didCompleteLevel)
		{
			if (logBlockedCompletions)
				Debug.Log("Level exit reached, but completion was blocked.");
			return;
		}

		_hasTriggered = true;
	}
}
