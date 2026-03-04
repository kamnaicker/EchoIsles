using UnityEngine;

public class EchoArea : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) 
		{
			//Replayer.Instance.ToggleRecordingAvailability(true);
            AudioManager.Instance.HandleEchoPlateStateChanged(EchoPlateState.On, gameObject.GetComponent<AudioSource>());
        }
	}

		private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) 
        {
			//Replayer.Instance.ToggleRecordingAvailability(false);
            AudioManager.Instance.HandleEchoPlateStateChanged(EchoPlateState.Off, gameObject.GetComponent<AudioSource>());
        }
    }
}