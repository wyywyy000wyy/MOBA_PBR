using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;


public class ECSBornSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        jobTT.Add(1);
        base.OnCreate();
        EntityManager.CreateEntity();
        //EntityManager.SetSharedComponentData()
    }


    List<int> jobTT = new List<int>();
    struct TestJob : IJob
    {
        [NativeDisableContainerSafetyRestrictionAttribute]
        public List<int> tt;
        public void Execute()
        {
            int i = 0;
            i += tt.Count;

            int j = i;
            j++;
        }
    }

    protected override void OnUpdate()
    {
    }

    protected  JobHandle OnUpdate2(JobHandle inputDeps)
    {
        
        var job = new TestJob { tt = jobTT};
        inputDeps = job.Schedule(inputDeps);
        return inputDeps;
    }

}
