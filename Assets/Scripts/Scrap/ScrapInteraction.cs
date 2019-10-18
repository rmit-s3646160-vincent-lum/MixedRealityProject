using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Events;
using static ScrapConstants;

public class ScrapInteraction : BaseInputHandler, IMixedRealityInputHandler<Vector2>, IMixedRealityPointerHandler, IMixedRealityFocusHandler
{
	[SerializeField] private ScrapState state;

	public float defaultOffsetDistance = 0.5f;
    [SerializeField] private float offsetDistance;
	public float smoothing = 0.001f;
    public bool shoot = false;
    public Color colorOnClip = Color.blue;
    public FocusEvent OnHoverEnter = new FocusEvent();
    public FocusEvent OnHoverExit = new FocusEvent();
    public PointerUnityEvent OnGrab = new PointerUnityEvent();
    public PointerUnityEvent OnRelease = new PointerUnityEvent();
    public PointerUnityEvent OnClick = new PointerUnityEvent();
    public UnityEvent OnPlacement = new UnityEvent();


    private struct PointerData
    {
        public IMixedRealityPointer pointer;
        private Vector3 initialGrabPointInPointer;

        public PointerData(IMixedRealityPointer pointer, Vector3 initialGrabPointInPointer) : this()
        {
            this.pointer = pointer;
            this.initialGrabPointInPointer = initialGrabPointInPointer;
        }

        public bool IsNearPointer()
        {
            return (pointer is IMixedRealityNearPointer);
        }

        /// Returns the grab point on the manipulated object in world space
        public Vector3 GrabPoint
        {
            get
            {
                return (pointer.Rotation * initialGrabPointInPointer) + pointer.Position;
            }
        }
    }

    Dictionary<uint, PointerData> pointerIdToPointerMap = new Dictionary<uint, PointerData>();
	TwoHandMoveLogic moveLogic;
    TwoHandScaleLogic scaleLogic;
    TwoHandRotateLogic rotateLogic;
    Quaternion objectToHandRotation;
    Quaternion targetRotationTwoHands;
    TransformScaleHandler scaleHandler;
    Vector3 dragPos;
    Vector3 dragVelocity;

    Rigidbody rb;
    MeshRenderer[] meshRenderers;
    Color[] defaultColors;

    private int collideCount;

	void Awake()
	{
		moveLogic = new TwoHandMoveLogic();
        rotateLogic = new TwoHandRotateLogic();
        scaleLogic = new TwoHandScaleLogic();
        scaleHandler = GetComponent<TransformScaleHandler>();
        rb = GetComponentInChildren<Rigidbody>();
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        defaultColors = new Color[meshRenderers.Length];
        for(int i = 0; i < meshRenderers.Length; i++)
        {
            defaultColors[i] = meshRenderers[i].material.color;
        }

        offsetDistance = defaultOffsetDistance;
	}

    // Unused
    private void HandleKeyboardInput()
    {
        if (state == ScrapState.manipulating)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                offsetDistance += 0.1f;
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                offsetDistance -= 0.1f;
            }
        }
    }

    public void OnPointerDown(MixedRealityPointerEventData eventData)
	{
        SetState(ScrapState.manipulating);

        uint id = eventData.Pointer.PointerId;

        // Add the pointer to the dictionary
        if (!eventData.used && !pointerIdToPointerMap.ContainsKey(eventData.Pointer.PointerId))
        {
            Vector3 initialGrabPoint = Quaternion.Inverse(eventData.Pointer.Rotation) * (eventData.Pointer.Result.Details.Point - eventData.Pointer.Position);
            pointerIdToPointerMap.Add(id, new PointerData(eventData.Pointer, initialGrabPoint));

            // If this is the first pointer...
            if (pointerIdToPointerMap.Count == 1)
            {
                // Call grab event
                OnGrab.Invoke(eventData);

                // Set up move logic using first pointer data
                PointerData pointerData = pointerIdToPointerMap.Values.First();
                IMixedRealityPointer pointer = pointerData.pointer;

                Quaternion worldToPalmRotation = Quaternion.Inverse(pointer.Rotation); // Calculate relative transform from object to hand.
                objectToHandRotation = worldToPalmRotation * transform.rotation;
                MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
                MixedRealityPose hostPose = new MixedRealityPose(transform.position, transform.rotation);
                moveLogic.Setup(pointerPose, pointerData.GrabPoint, hostPose, transform.localScale);

                dragPos = transform.position;
            }
            else if(pointerIdToPointerMap.Count > 1)
            {
                // Set up scale and rotate logic
                var handPositionMap = GetHandPositionMap();
                targetRotationTwoHands = transform.rotation;
                rotateLogic.Setup(handPositionMap, transform, RotationConstraintType.None);
                scaleLogic.Setup(handPositionMap, transform);
            }
        }

        if (pointerIdToPointerMap.Count > 0)
        {
            eventData.Use();
        }
            
    }

	public void OnPointerDragged(MixedRealityPointerEventData eventData)
	{
        if(pointerIdToPointerMap.Count == 0)
        {
            return;
        }
        else if(pointerIdToPointerMap.Count == 1)
        {
            // Handle movement
            Debug.Assert(pointerIdToPointerMap.Count == 1);
            PointerData pointerData = pointerIdToPointerMap.Values.First();
            IMixedRealityPointer pointer = pointerData.pointer;
            Quaternion targetRotation = transform.rotation;

            // Update target position
            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            Vector3 targetPosition = moveLogic.Update(pointerPose, targetRotation, transform.localScale, false, true, MovementConstraintType.None);

            // Update target position using offset distance from controller
            Vector3 grabOffset = transform.position - eventData.Pointer.Result.Details.Point;
            Vector3 offsetFromController = Vector3.Normalize(targetPosition - eventData.Pointer.Position) * offsetDistance;
            Vector3 distanceVector = targetPosition - eventData.Pointer.Position;
            targetPosition = targetPosition - distanceVector + offsetFromController + grabOffset;

            // Smooth and move object to target position
            float lerpAmount = 1.0f - Mathf.Pow(smoothing, Time.deltaTime);
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpAmount);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, lerpAmount);
            transform.SetPositionAndRotation(smoothedPosition, smoothedRotation);

            // Update velocity
            dragVelocity = (transform.position - dragPos) / Time.deltaTime;
            dragPos = transform.position;

        }
        else
        {
            // Handle rotation and scaling
            var targetPosition = transform.position;
            var targetScale = transform.localScale;
            var handPositionMap = GetHandPositionMap();

            targetScale = scaleLogic.UpdateMap(handPositionMap);
            targetRotationTwoHands = rotateLogic.Update(handPositionMap, targetRotationTwoHands, RotationConstraintType.None);

            // Two-handed Rotate
            float lerpAmount = 1.0f - Mathf.Pow(smoothing, Time.deltaTime);
            Quaternion smoothedRotation = Quaternion.Lerp(transform.rotation, targetRotationTwoHands, lerpAmount);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, lerpAmount);
            transform.SetPositionAndRotation(smoothedPosition, smoothedRotation);

            // Two-handed Scale
            if (scaleHandler != null)
            {
                // Apply TransformScaleHandler contraints
                targetScale = scaleHandler.ClampScale(targetScale);
            }
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpAmount);
        }
    }

	public void OnPointerUp(MixedRealityPointerEventData eventData)
	{
        uint id = eventData.Pointer.PointerId;

        if (pointerIdToPointerMap.ContainsKey(id))
        {
            pointerIdToPointerMap.Remove(id);
        }
		eventData.Use();

        // Call release event if scrap has no pointers
        if(pointerIdToPointerMap.Count == 0)
        {
            SetState(ScrapState.notPlaced);

            if (shoot)
                ShootScrap(eventData);
            else
                ReleaseScrap(eventData);

            OnRelease.Invoke(eventData);
        }
    }

	public void OnPointerClicked(MixedRealityPointerEventData eventData)
	{
        OnClick.Invoke(eventData);
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        if (OnHoverEnter != null)
            OnHoverEnter.Invoke(eventData);
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        if (OnHoverExit != null)
            OnHoverExit.Invoke(eventData);
    }

    public void ShootScrap(MixedRealityPointerEventData eventData)
    {
        if (rb != null)
        {
            if (eventData.Pointer.Controller != null)
            {
                float speed = 20;
                Vector3 direction = eventData.Pointer.Result.Details.Point - eventData.Pointer.Position;
                rb.velocity = direction.normalized * speed;
            }
        }
    }

    public void ReleaseScrap(MixedRealityPointerEventData eventData)
    {
        if (rb != null)
        {
            if (eventData.Pointer.Controller != null)
            {
                rb.velocity = dragVelocity;
                offsetDistance = defaultOffsetDistance;
            }
        }
    }

    public void HandleDualAxisInput(Vector2 inputData)
    {
        float yUpper = 0.5f;
        float yLower = -0.5f;
        float xUpper = 0.5f;
        float xLower = -0.5f;
        float rotateAmount = 10;
        float pullAmount = 1;

        // Adjust translation with y-axis input
        if (inputData.y > yUpper)
        {
            offsetDistance += (pullAmount);
            Mathf.Clamp(offsetDistance, defaultOffsetDistance, 100);
        }
        else if(inputData.y < yLower)
        {
            offsetDistance -= (pullAmount);
            Mathf.Clamp(offsetDistance, defaultOffsetDistance, 100);
        }

        // Rotate with x-axis input
        if(inputData.x > xUpper)
        {
            transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, rotateAmount, 0);
        }
        else if (inputData.x < xLower)
        {
            transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, -rotateAmount, 0);
        }
    }

	public void SetState(ScrapState newState)
	{
        ScrapState oldState = state;

        // Return if no state change
        if (newState == oldState)
            return;

        state = newState;

        // Call state transition exit function
        switch (oldState)
        {
            case ScrapState.initial:
                // Unparent from scrappool
                transform.SetParent(null);
                GetComponent<Rotator>().enabled = false;
                break;
            case ScrapState.manipulating:
                ResetColor();
                if (rb != null)
                {
                    rb.constraints = RigidbodyConstraints.None;
                }
                if (IsColliding())
                {
                    state = ScrapState.beingPlaced;
                    OnPlacement.Invoke();
                }
                break;
            case ScrapState.beingPlaced:
                UpdateColor();
                if (rb != null)
                    rb.constraints = RigidbodyConstraints.None;
                break;
            default:
                break;
        }

        // Call state transition enter function
        switch (state)
        {
            case ScrapState.initial:
                rb.constraints = RigidbodyConstraints.FreezePosition;
                GetComponent<Rotator>().enabled = true;
                break;
            case ScrapState.manipulating:
                if(rb != null)
                {
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                break;
            case ScrapState.beingPlaced:
                OnPlacement.Invoke();
                if (rb != null)
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                break;
            default:
                break;
        }
    }

	public ScrapState GetState()
	{
		return state;
	}

    public void OnInputChanged(InputEventData<Vector2> eventData)
    {
        // Check if input was from first focus controller
        if(state == ScrapState.manipulating)
        {
            if (eventData.MixedRealityInputAction.Description == "Scroll")
            {
                //OnScroll(eventData.InputData);
            }

            if(eventData.MixedRealityInputAction.Description == "Teleport Direction")
            {
                // Change offset distance with scrolling
                HandleDualAxisInput(eventData.InputData);
            }
        }
    }

    protected override void RegisterHandlers()
    {
        InputSystem?.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    protected override void UnregisterHandlers()
    {
        InputSystem?.UnregisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    private Dictionary<uint, Vector3> GetHandPositionMap()
    {
        var handPositionMap = new Dictionary<uint, Vector3>();
        foreach (var item in pointerIdToPointerMap)
        {
            handPositionMap.Add(item.Key, item.Value.pointer.Position);
        }

        return handPositionMap;
    }

    private void OnCollisionEnter(Collision collision)
    {
        collideCount++;

        if(state == ScrapState.manipulating)
        {
            UpdateColor();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        collideCount--;
        if(!IsColliding() && state == ScrapState.beingPlaced)
        {
            SetState(ScrapState.notPlaced);
        }

        if (state == ScrapState.manipulating)
        {
            UpdateColor();
        }
    }

    public bool IsColliding()
    {
        return collideCount != 0;
    }

    public void ToggleShoot()
    {
        shoot = !shoot;
        // Need to move this logic to the controller instead of scrap
    }

    private void UpdateColor()
    {
        if(collideCount == 0)
        {
            ResetColor();
        }
        else
        {
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                meshRenderers[i].material.color = colorOnClip;
            }
        }
    }

    private void ResetColor()
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].material.color = defaultColors[i];
        }
    }

}
