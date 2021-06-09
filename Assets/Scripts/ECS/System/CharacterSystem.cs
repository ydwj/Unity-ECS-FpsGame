using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;

public class CharacterSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        float3 input;
        string h = "Horizontal";
        string v = "Vertical";

        Entities.
            WithoutBurst().
            WithName("Player").
            ForEach((ref Translation translation, ref Rotation rotation, in Character character) =>
            {

                input.x = Input.GetAxis(h);
                input.y = 0;
                input.z = Input.GetAxis(v);
                var dir = character.speed * deltaTime * input;
                dir.y = 0;
                //令角色前方和移动方向一致
                if (math.length(input) > 0.1f)
                {
                    //Debug.Log("Dir " + dir);
                    rotation.Value = quaternion.LookRotation(math.normalize(dir), math.up());

                }
                translation.Value += dir;

            }).Run();
    }
}