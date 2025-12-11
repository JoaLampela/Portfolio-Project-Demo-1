using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class MoveAuthoring : MonoBehaviour
{
    public int moveSpeed;
    
    public class MoveBaker : Baker<MoveAuthoring>
    {
        public override void Bake(MoveAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Move
            {
                moveSpeed = authoring.moveSpeed,
                targetPosition = new float3(0f, 0f, 0f)
            });
        }
    }
}

public partial struct Move : IComponentData
{
    public int moveSpeed;
    public float3 targetPosition;
}
