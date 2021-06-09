using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;


public class BeatBackSystem : SystemBase
{
    protected override void OnUpdate()
    {
       
        float deltaTime = Time.DeltaTime;
        Entities.
            ForEach((ref BeatBack beatBack,ref Translation translation ) =>
        {
            
            if (beatBack.velocity <0.1f)
            {
                beatBack.velocity = 0;
                beatBack.timer = 0;
                beatBack.curVelocity = 0;
                return;
               
            }
            float temp = beatBack.velocity;
            beatBack.timer += 2*deltaTime;
         
            temp = math.lerp(beatBack.velocity, 0,beatBack.timer);
            if (temp < 0.1f)
            {
                beatBack.velocity = 0;
            }
            beatBack.curVelocity = temp;

            translation.Value += beatBack.velocity * deltaTime * math.forward(beatBack.rotation.Value);
        }).Run();
    }
}
