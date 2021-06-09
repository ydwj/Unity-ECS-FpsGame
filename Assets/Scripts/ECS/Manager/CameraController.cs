using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;//相机相对于玩家的位置

    private Vector3 pos;
    public float speed;

    private EntityManager _manager;
    private float3 tempPos;
    public Entity targetEntity;
    void Start()
    {
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var queryDescription = new EntityQueryDesc
        {
            None = new ComponentType[] { },
            All = new ComponentType[] { ComponentType.ReadOnly<Character>(), ComponentType.ReadOnly<Translation>() }
        };
        EntityQuery players = _manager.CreateEntityQuery(queryDescription);
        if (players.CalculateEntityCount() != 0)
        {
            NativeArray<Entity> temp1 = new NativeArray<Entity>(1, Allocator.Temp);
            temp1 = players.ToEntityArray(Allocator.Temp);
            targetEntity = temp1[0];
            temp1.Dispose();
        }
        players.Dispose();

    }

    void Update()
    {
        if (targetEntity != Entity.Null)
        {
            if (_manager.HasComponent<Translation>(targetEntity))
            {
                tempPos = _manager.GetComponentData<Translation>(targetEntity).Value;
            }
        }

        transform.position = Vector3.Lerp(transform.position, (Vector3)tempPos + offset, speed * Time.deltaTime);//调整相机与玩家之间的距离
    }
}
