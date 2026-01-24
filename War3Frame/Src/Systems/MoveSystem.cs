using Friflo.Engine.ECS;
using Friflo.Engine.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace War3Frame.Src.Systems
{
/*    public class MoveSystem : QuerySystem<Position>
    {
        protected override void OnUpdate()
        {
            Query.ForEachEntity((ref Position position, Entity entity) =>
            {
                var unit = entity.GetComponent<Velocity>().unit;
                var ux = JassApi.GetUnitX(unit);
                var uy = JassApi.GetUnitY(unit);
                position.value = new Vector3(ux, uy, 0);
                Console.WriteLine($"更新单位位置: x: {ux}, y: {uy}");
            });
        }
    }*/
}
