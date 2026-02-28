using UnityEngine;
using System.Collections;

public class EnviromentOcclusion : MonoBehaviour
{
    private Material material;
    private Coroutine fadeRoutine;

    [SerializeField] private float fadedAlpha = 0.25f;
    [SerializeField] private float fadeDuration = 0.15f;

    //Stores object material once object instance created
    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    public void FadeOut()
    {
        StartFade(fadedAlpha);
    }

    public void FadeIn()
    {
        Debug.Log($"Fading in {gameObject.name}");
        StartFade(1f);
    }

    //Checks if fade is currently active, then starts the fade routine on object
    private void StartFade(float targetAlpha)
    {
        if(fadeRoutine != null)
            StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    //fade animation
    private IEnumerator FadeRoutine(float targetAlpha)
    {
        //sets initial value and counter for time duration
        float startAlpha = material.color.a;
        float time = 0f;

        //runs length of fadeDuration, then uses time to gradually transition alpha value
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            Color c = material.color;
            c.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            material.color = c;
            yield return null;
        }

        //set final values
        Color final = material.color;
        final.a = targetAlpha;
        material.color = final;
    }
}
