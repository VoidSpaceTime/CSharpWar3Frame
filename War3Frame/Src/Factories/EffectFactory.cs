using Friflo.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;
using War3Frame.Src.Factories;

namespace War3Frame
{
    public static class EntityStoreExtensions
    {
        public static Entity CreateEffect(this EntityStore store,
                           string modelAlias,
                           float x, float y, float z)
        {
  
            // 创建 Entity + 基础组件
            var entity = store.CreateEntity(
                new UnitState { isAlive = true },
                new Position { x = x, y = y }
            );
            return entity;
        }
    }
}
