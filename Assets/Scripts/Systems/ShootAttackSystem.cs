using SF.EntitiesModule.Combat;

using Unity.Burst;
using Unity.Entities;

namespace SF.EntitiesModule
{
    partial struct ShootAttackSystem : ISystem
    {

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
           foreach((
                RefRW<ShootAttack> shootAttack, 
                RefRO<Target> target) 
                in SystemAPI.Query
                    <RefRW<ShootAttack>, 
                    RefRO<Target>>())
            {
                // If the TargetEntity is not null continue to next interation
                if(target.ValueRO.TargetEntity == Entity.Null)
                    continue;

                shootAttack.ValueRW.Timer -= SystemAPI.Time.DeltaTime;

                // If not enough time has passed to shoot go to next interation.
                if(shootAttack.ValueRO.Timer > 0f)
                    continue;

                shootAttack.ValueRW.Timer = shootAttack.ValueRO.TimerMax;

                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.TargetEntity);
                int damageAmount = 1;
                targetHealth.ValueRW.HealthAmount -= damageAmount;

            } // End of Shoot Attack/Target foreach
        }
    }
}
