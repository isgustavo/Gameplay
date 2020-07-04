using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class PlayableCharacterAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	private Entity entityReference;
	private Animator animator;

	public int deviceInputId;
	public int addicitionId;

	private void Awake()
	{
		animator = GetComponent<Animator>();

		//transform.rotation = Quaternion.Euler(0, 90, 0);
	}

	float direction;
	float leftX;
	float leftY;

	private void Update()
	{
		CharacterMoveData moveData = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<CharacterMoveData>(entityReference);

		////transfer the values to the Animator state machine

		//Debug.Log($"moveData.x {moveData.x}");
		//Debug.Log($"UnityEngine.Input.GetAxis(Horizontal) {UnityEngine.Input.GetAxis("Horizontal")}");
		//Debug.Log($"moveData.y {moveData.y}");
		//Debug.Log($"UnityEngine.Input.GetAxis(Vertical) {UnityEngine.Input.GetAxis("Vertical")}");
		leftX = moveData.x; //UnityEngine.Input.GetAxis("Horizontal");
		leftY = moveData.y; //UnityEngine.Input.GetAxis("Vertical");
		float sOut = 0f, aOut = 0f;
		direction = 0;
		//StickToWorldspace(transform, Camera.main, ref direction, ref sOut, ref aOut, false);
		//Debug.Log(direction);
		//animator.SetFloat("Speed", sOut, .05f, Time.deltaTime);
		//animator.SetFloat("Direction", direction, .25f, Time.deltaTime);
		//animator.SetFloat("Angle", aOut);

		
		//animator.SetBool("IsWalking", animState.IsWalking);
		//if (animState.TriggerAttack) animator.SetTrigger("Attack");
		//if (animState.TriggerTakeDamage) animator.SetTrigger("TakeDamage");
		//if (animState.TriggerIsDead) animator.SetTrigger("IsDead");

		//if (animState.TriggerIsDead) this.enabled = false;
	}

	//void FixedUpdate()
	//{
	//    // Rotate character model if stick is tilted right or left, but only if character is moving in that direction
	//    if ((direction >= 0 && leftX >= 0) || (direction < 0 && leftX < 0))
	//    {
	//    Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, 120 * (leftX < 0f ? -1f : 1f), 0f), Mathf.Abs(leftX));
	//    Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
	//    this.transform.rotation = (this.transform.rotation * deltaRotation);
	//    }
	//}

	public void StickToWorldspace(Transform root, Camera camera, ref float directionOut, ref float speedOut, ref float angleOut, bool isPivoting)
	{
		Vector3 rootDirection = root.forward;

		Vector3 stickDirection = new Vector3(leftX, 0, leftY);

		speedOut = stickDirection.sqrMagnitude;

		// Get camera rotation
		Vector3 CameraDirection = camera.transform.forward;
		CameraDirection.y = 0.0f; // kill Y
		Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, CameraDirection);
        
		// Convert joystick input in Worldspace coordinates
		//Vector3 moveDirection = referentialShift * stickDirection;
		//Debug.Log($"moveDirection {moveDirection}");
		Vector3 m_CamForward = Vector3.Scale(camera.transform.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 m_Move = leftY * m_CamForward + leftX * camera.transform.right;
		//Debug.Log($"m_Move {m_Move}");


		Vector3 axisSign = Vector3.Cross(m_Move, rootDirection);
		
		//Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
		//Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
		//Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);
		//Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2.5f, root.position.z), axisSign, Color.red);

		float angleRootToMove = Vector3.Angle(rootDirection, m_Move) * (axisSign.y >= 0 ? -1f : 1f);
		//Debug.Log($"Normal angleRootToMove {angleRootToMove}");
		angleOut = Mathf.Round(angleRootToMove / 90) * 90;
		//Debug.Log($"angleOut {angleOut}");
		//if (!isPivoting)
		//{
		//	angleOut = angleRootToMove;
		//}
		//angleRootToMove /= 180f;

		directionOut = angleRootToMove * 1.5f;
	}

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        entityReference = conversionSystem.GetPrimaryEntity(transform);
		//dstManager.AddComponent(entity, typeof(CopyTransformFromGameObject));
		dstManager.AddComponent(entity, typeof(CopyTransformToGameObject));
		dstManager.AddComponentData(entity, new PlayableCharacterDeviceInputComponentData
        {
            DeviceInputId = deviceInputId,
            AdditionalDeviceInputId = addicitionId
        });

        dstManager.AddComponentData(entity, new PlayableCharacterInputsComponentData { move = new float2(0, 1), look = float2.zero });

        dstManager.AddComponentData(entity, new CharacterMoveData { });
    }

    public void SetupPlayableCharacterDeviceInput(int deviceInputId, int AdditionalDeviceInputId = -1)
    {
        this.deviceInputId = deviceInputId;
        this.addicitionId = AdditionalDeviceInputId;
    }
}
