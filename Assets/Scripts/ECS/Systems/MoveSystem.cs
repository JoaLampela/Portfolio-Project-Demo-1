using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

partial struct MoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Called when the system is created
    }
    
    // Called on every system loop
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        new MoveJob().ScheduleParallel();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        // Called when the system is destroyed
    }
}

[BurstCompile]
public partial struct MoveJob : IJobEntity
{
    public void Execute(ref LocalTransform localTransform, in Move move, ref PhysicsVelocity physicsVelocity, in Player player)
    {
        float3 moveDirection = move.targetPosition - localTransform.Position;

        if (math.lengthsq(moveDirection) < 0.0001f)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }
        
        physicsVelocity.Linear = math.normalize(moveDirection) * move.moveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}
