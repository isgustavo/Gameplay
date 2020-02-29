using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
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
	}

	private void Update()
	{
		//AnimationState animState = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<AnimationState>(entityReference);

		////transfer the values to the Animator state machine
		//animator.SetFloat("Speed", animState.Speed);
		//animator.SetBool("IsWalking", animState.IsWalking);
		//if (animState.TriggerAttack) animator.SetTrigger("Attack");
		//if (animState.TriggerTakeDamage) animator.SetTrigger("TakeDamage");
		//if (animState.TriggerIsDead) animator.SetTrigger("IsDead");

		//if (animState.TriggerIsDead) this.enabled = false;
	}

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
		entityReference = entity;//conversionSystem.GetPrimaryEntity(this.transform);

		//dstManager.AddComponent(entity, typeof(PlayerTag));
		//dstManager.AddComponent(entity, typeof(Score));
		//dstManager.AddComponent(entity, typeof(AnimationState));
		dstManager.AddComponent(entity, typeof(CopyTransformToGameObject));

		dstManager.AddComponentData(entity, new PlayableCharacterDeviceInputComponentData
		{
			DeviceInputId = deviceInputId,
			AdditionalDeviceInputId = addicitionId
		});

        dstManager.AddComponentData(entity, new PlayableCharacterInputsComponentData { });

        //dstManager.AddComponentData(entity, new MovementInput { MoveAmount = new float3() });
        //dstManager.AddComponentData(entity, new Speed { Value = speed });
        //float atkAnimLength = attackClip.length;
        //dstManager.AddComponentData(entity, new AttackInput { Attack = false, AttackLength = atkAnimLength, AttackStrength = attackStrength });
        //dstManager.AddComponentData(entity, new AttackRange { Range = attackRange });
        //dstManager.AddComponentData(entity, new AlertRange { Range = attackRange });
        //dstManager.AddComponentData(entity, new Health { Current = initialHealth, FullHealth = initialHealth });
        //dstManager.AddBuffer<Damage>(entity);
    }

    public void SetupPlayableCharacterDeviceInput(int deviceInputId, int AdditionalDeviceInputId = -1)
    {
		this.deviceInputId = deviceInputId;
		this.addicitionId = AdditionalDeviceInputId;
		//EntityManager dstManager = World.DefaultGameObjectInjectionWorld.EntityManager;

  //      dstManager.AddComponentData(entityReference, new PlayableCharacterDeviceInputComponentData
  //      {
  //          DeviceInputId = deviceInputId,
  //          AdditionalDeviceInputId = AdditionalDeviceInputId
		//});

  //      dstManager.AddComponentData(entityReference, new PlayableCharacterInputsComponentData { });

    }
}
