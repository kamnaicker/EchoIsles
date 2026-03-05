using UnityEngine;

public class EchoArea : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) 
		{
			Replayer.Instance.ToggleRecordingAvailability(true);

			if (gameObject.GetComponent<AudioSource>() == null) gameObject.AddComponent<AudioSource>();
            AudioManager.Instance.HandleEchoPlateStateChanged(EchoPlateState.On, gameObject.GetComponent<AudioSource>());
        }
	}

		private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) 
        {
			Replayer.Instance.ToggleRecordingAvailability(false);

			if (gameObject.GetComponent<AudioSource>() == null) gameObject.AddComponent<AudioSource>();
            AudioManager.Instance.HandleEchoPlateStateChanged(EchoPlateState.Off, gameObject.GetComponent<AudioSource>());
        }
    }
}