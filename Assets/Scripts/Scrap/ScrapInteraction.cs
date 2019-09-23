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

public class ScrapInteraction : BaseInputHandler, IMixedRealityInputHandler<Vector2>, IMixedRealityPointerHandler
{
	[SerializeField] private ScrapConstants.State state;

	public float defaultOffsetDistance = 2f;
    [SerializeField] private float offsetDistance;
	public float smoothing = 0.001f;
    public bool shoot = false;
    public Color colorOnClip = Color.blue;
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

    //[SerializeField] List<uint> pointers;
    private Dictionary<uint, PointerData> pointerIdToPointerMap = new Dictionary<uint, PointerData>();

    Vector3 grabOffset;
	Vector3 rotateStartPoint;

	private TwoHandMoveLogic moveLogic;
    private TwoHandScaleLogic scaleLogic;
    private TwoHandRotateLogic rotateLogic;
    private Quaternion objectToHandRotation;
    private Quaternion targetRotationTwoHands;
    private TransformScaleHandler scaleHandler;


    private Rigidbody rb;
    private Collider collider;
    private Color defaultColor;

    private int collideCount;

	void Awake()
	{
		moveLogic = new TwoHandMoveLogic();
        rotateLogic = new TwoHandRotateLogic();
        scaleLogic = new TwoHandScaleLogic();
        scaleHandler = GetComponent<TransformScaleHandler>();
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        defaultColor = GetComponent<Renderer>().material.color;

        offsetDistance = defaultOffsetDistance;
	}

    private void Update()
    {
        HandleKeyboardInput();
    }

    private void HandleKeyboardInput()
    {
        if (state == ScrapConstants.State.manipulating)
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
		SetState(ScrapConstants.State.manipulating);

        uint id = eventData.Pointer.PointerId;

        // Add the pointer to the dictionary
        if (!eventData.used && !pointerIdToPointerMap.ContainsKey(eventData.Pointer.PointerId))
        {
            Vector3 initialGrabPoint = Quaternion.Inverse(eventData.Pointer.Rotation) * (eventData.Pointer.Result.Details.Point - eventData.Pointer.Position);
            pointerIdToPointerMap.Add(id, new PointerData(eventData.Pointer, initialGrabPoint));

            // If this is the first pointer...
            if (pointerIdToPointerMap.Count == 1)
            {
                // Set up move logic using first pointer data
                PointerData pointerData = pointerIdToPointerMap.Values.First();
                IMixedRealityPointer pointer = pointerData.pointer;

                Quaternion worldToPalmRotation = Quaternion.Inverse(pointer.Rotation); // Calculate relative transform from object to hand.
                objectToHandRotation = worldToPalmRotation * transform.rotation;
                MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
                MixedRealityPose hostPose = new MixedRealityPose(transform.position, transform.rotation);
                moveLogic.Setup(pointerPose, pointerData.GrabPoint, hostPose, transform.localScale);
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

        if (pointerIdToPointerMap.Count == 0 && rb != null)
        {

            if (shoot)
                ShootScrap(eventData);
            else
                ReleaseScrap(eventData);

            SetState(ScrapConstants.State.notPlaced);
        }
    }

	public void OnPointerClicked(MixedRealityPointerEventData eventData)
	{
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
                rb.velocity = eventData.Pointer.Controller.Velocity;
                offsetDistance = defaultOffsetDistance;
            }
        }
    }

    public void OnScroll(Vector2 inputData)
    {
        float yThreshold = 0.5f;
        float xThreshold = 0.5f;

        if(state == ScrapConstants.State.manipulating)
        {
            if(Mathf.Abs(inputData.y) > yThreshold)
            {
                // Translate toward/away from source
                offsetDistance += inputData.y * 0.1f;
                Mathf.Clamp(offsetDistance, defaultOffsetDistance, 100);
            }
            else if(Mathf.Abs(inputData.x) > xThreshold)
            {
                // Rotate object
                //transform.localEulerAngles = transform.localEulerAngles + new Vector3(0, 15, 0);
            }

            
        }
    }

	public void SetState(ScrapConstants.State newState)
	{
        ScrapConstants.State oldState = state;

        state = newState;

        // Call state transition exit function
        switch (oldState)
        {
            case ScrapConstants.State.manipulating:
                if (rb != null)
                    rb.isKinematic = false;
                GetComponent<Renderer>().material.color = defaultColor;
                if (IsColliding())
                {
                    state = ScrapConstants.State.beingPlaced;
                    OnPlacement.Invoke();
                }
                break;
            case ScrapConstants.State.beingPlaced:
                if (rb != null)
                    rb.isKinematic = false;
                break;
            default:
                break;
        }

        // Call state transition enter function
        switch (state)
        {
            case ScrapConstants.State.manipulating:
                if(rb != null)
                    rb.isKinematic = true;
                break;
            case ScrapConstants.State.beingPlaced:
                OnPlacement.Invoke();
                if (rb != null)
                    rb.isKinematic = true;
                break;
            default:
                break;
        }
    }

	public ScrapConstants.State GetState()
	{
		return state;
	}

    public void OnInputChanged(InputEventData<Vector2> eventData)
    {
        // Check if input was from first focus controller
        if(state == ScrapConstants.State.manipulating)
        {
            if (eventData.MixedRealityInputAction.Description == "Scroll")
            {
                // Change offset distance with scrolling
                OnScroll(eventData.InputData);
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

        if(state == ScrapConstants.State.manipulating)
        {
            GetComponent<Renderer>().material.color = colorOnClip;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        collideCount--;
        if(!IsColliding() && state == ScrapConstants.State.beingPlaced)
        {
            SetState(ScrapConstants.State.notPlaced);
        }

        if (state == ScrapConstants.State.manipulating)
        {
            GetComponent<Renderer>().material.color = defaultColor;
        }
    }

    public bool IsColliding()
    {
        return collideCount != 0;
    }

    public void ToggleShoot()
    {
        shoot = !shoot;
    }
}
