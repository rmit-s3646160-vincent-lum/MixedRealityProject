using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class Vacuum : AttachToController
{
    public enum StateEnum
    {
        Uninitialized,
        Idle,
        Switching,
        Spawning
    }

    [SerializeField] private GameObject objectToProject;
    [SerializeField] private Transform shotOrigin;

    private GameObject beam;

    private StateEnum state = StateEnum.Uninitialized;

    // Start is called before the first frame update
    void Start()
    {
        // Remove this later
        state = StateEnum.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (state != StateEnum.Idle)
        {
            return;
        }

        // Input test with mouse controls
        if (Input.GetMouseButtonDown(0)) // Select button down
        {
            Debug.Log("Left click");
            SpawnObject();
        } else if (Input.GetMouseButtonUp(0))
        {
            DespawnObject();
        }

        if(beam != null)
        {
            //beam.transform.rotation = shotOrigin.rotation;
        }
    }



    private void SpawnObject()
    {
        // Instantiate the beam
        if (beam != null)
            return;

        beam = Instantiate(objectToProject, shotOrigin);
        //beam.transform.rotation = shotOrigin.rotation;
    }

    private void DespawnObject()
    {
        // Destroy he beam
        if(beam != null)
        {
            Destroy(beam);
        }
    }

    protected override void OnAttachToController()
    {
        //#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        // Subscribe to input now that we're parented under the controller
        InteractionManager.InteractionSourcePressed += InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased += InteractionSourceReleased;
        //#endif

        state = StateEnum.Idle;
    }

    protected override void OnDetachFromController()
    {
        //#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        // Unsubscribe from input now that we've detached from the controller
        InteractionManager.InteractionSourcePressed -= InteractionSourcePressed;
        InteractionManager.InteractionSourceReleased -= InteractionSourceReleased;
        //#endif

        state = StateEnum.Uninitialized;
    }

    //#if UNITY_WSA && UNITY_2017_2_OR_NEWER
    private void InteractionSourcePressed(InteractionSourcePressedEventArgs obj)
    {
        // Check handedness, see if it is right controller
        if (obj.state.source.handedness == InteractionSourceHandedness.Right)
        {
            switch (obj.pressType)
            {
                // If it is Select button event, spawn object
                case InteractionSourcePressType.Select:
                    if (state == StateEnum.Idle)
                    {
                        // We've pressed select - enter spawning state
                        state = StateEnum.Spawning;
                        SpawnObject();
                    }
                    break;

                // If it is Grasp button event
                case InteractionSourcePressType.Grasp:

                    // Increment the index of current mesh type (sphere, cube, cylinder)
                    /*meshIndex++;
                    if (meshIndex >= NumAvailableMeshes)
                    {
                        meshIndex = 0;
                    }*/
                    break;

                default:
                    break;
            }
        }
    }

    private void InteractionSourceReleased(InteractionSourceReleasedEventArgs obj)
    {
        if (obj.state.source.handedness == InteractionSourceHandedness.Right)
        {
            switch (obj.pressType)
            {
                case InteractionSourcePressType.Select:
                    if (state == StateEnum.Spawning)
                    {
                        DespawnObject();
                        // We've released select - return to idle state
                        state = StateEnum.Idle;
                    }
                    break;

                default:
                    break;
            }
        }
    }
    //#endif
}
