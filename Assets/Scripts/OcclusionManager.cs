using UnityEngine;
using System.Collections.Generic;

//Manages the camera occlusion using raycast between camera and player
public class OcclusionManager : MonoBehaviour
{
    //track player object and wall layer
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask Walls;

    [SerializeField] private float playerWidth = 0.7f;
    [SerializeField] private float padding = 0.2f;

    //track current faded walls
    private HashSet<WallSectionOcclusion> currentlyOccluded = new ();
    //track walls faded this frame
    private HashSet<WallSectionOcclusion> occludedThisFrame = new ();
    
    //called after update before handling occlusions
    void LateUpdate()
        {
            HandleOcclusion();
        }

    private void HandleOcclusion()
    {
        //clears last frames stored walls
        occludedThisFrame.Clear();

        //calculate the vector between cam and player, then normalise the value to be used in SphereCast
        Vector3 camPos = transform.position;
        Vector3 targetPos = player.position;
        Vector3 direction = targetPos - camPos;
        float distance = direction.magnitude;
        direction.Normalize();

        float radius = (playerWidth * 0.5f) + padding;

        //create raycast between cam and player that detects the walls, using spherecast for better wall detection
        RaycastHit[] hits = Physics.SphereCastAll(camPos, radius, direction, distance, Walls);

        //loops through each hit wall and if it has the occluder script, fades out
        foreach (RaycastHit hit in hits)
        {
            WallSectionOcclusion section =
                hit.collider.GetComponentInParent<WallSectionOcclusion>();
            if (section == null)
            {
                continue;
            }

            section.FadeOutSection();

            occludedThisFrame.Add(section);
            currentlyOccluded.Add(section);
        }

        //list to store walls to be faded back in
        List<WallSectionOcclusion> toRemove = new();

        //loops through walls to be faded in, and does so
        foreach(WallSectionOcclusion section in currentlyOccluded)
        {
            if(!occludedThisFrame.Contains(section)) 
            {
                section.FadeInSection();
                toRemove.Add(section);
            }
        }

        foreach(WallSectionOcclusion section in toRemove) 
        { 
            currentlyOccluded.Remove(section); 
        }
    }
}
