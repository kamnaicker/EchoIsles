using UnityEngine;

public class WallSectionOcclusion : MonoBehaviour
{
    private EnviromentOcclusion[] walls;
    private bool isOccluded;

    private void Awake()
    {
        //gets all the walls for the section
        walls = GetComponentsInChildren<EnviromentOcclusion>();
    }

    public void FadeOutSection()
    {
        if (isOccluded) return;
        isOccluded = true;

        foreach (var wall in walls) 
        {
            wall.FadeOut();
        }
    }

    public void FadeInSection()
    {
        Debug.Log("fadeoutsection called");
        if (!isOccluded) return;
        isOccluded = false;

        foreach (var wall in walls)
        {
            Debug.Log("fading walls loop");
            wall.FadeIn(); 
        }
    }
}
