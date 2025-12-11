using Unity.Entities;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public class PlayerBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new Player
            {
                entityGroup = EntityGroup.Player
            });
        }
    }
}

public partial struct Player : IComponentData
{
    public EntityGroup entityGroup;
}

public enum EntityGroup
{
    Neutral = 0,
    Player = 1,
    Enemy = 2
}
