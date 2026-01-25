using Friflo.Engine.ECS;
using System;
using System.Collections.Generic;
using System.Text;

namespace War3Frame.Src.Factories
{
    public static class EntityStoreExtensions
    {

        public static Entity CreateUnit(this EntityStore store,
                                   string templateName,
                                   JPlayer player,
                                   float x, float y)
        {
            var template = UnitTemplates.Get(templateName);
            if (template == null)

                throw new Exception($"模板不存在: {templateName}");
            // 创建 War3 原生单位
            var native = JassApi.CreateUnit(player, JassApi.C2I("hfoo"), x, y, template.facing);
            // 创建 Entity + 基础组件
            var entity = store.CreateEntity(
                new UnitNative { unit = native },
                new UnitState { isAlive = true },
                new Position { x = x, y = y }
            );
            // 应用模板配置的组件
            template.Configure(entity);
            return entity;
        }
        /// <summary>
        /// 从模板创建 + 额外配置
        /// </summary>
        public static Entity CreateUnit(this EntityStore store,
                                       string templateName,
                                       JPlayer player,
                                       float x, float y,
                                       Action<Entity> extraConfig)
        {
            var entity = store.CreateUnit(templateName, player, x, y);
            extraConfig(entity);
            return entity;
        }
    }
    /// <summary>
    /// 单位模板 - 使用委托配置组件
    /// </summary>
    public class UnitTemplate
    {
        public float facing = 270;

        /// <summary>
        /// 组件配置委托
        /// </summary>
        public Action<Entity> Configure { get; set; }
        public UnitTemplate(Action<Entity> configure)
        {
            Configure = configure;
        }
    }

    /// <summary>
    /// 模板注册表
    /// </summary>
    public static class UnitTemplates
    {
        private static readonly Dictionary<string, UnitTemplate> _templates = new();
        static UnitTemplates()
        {
            // ========== 步兵 ==========
            Register("footman", new UnitTemplate(
                e =>
                {
                    e.AddComponent(new Health { max = 420, current = 420, regen = 0.25f });
                    e.AddComponent(new Attack { damage = 12, speed = 1.0f });
                }
            ));
        }
        public static void Register(string name, UnitTemplate template)
            => _templates[name] = template;
        public static UnitTemplate? Get(string name)
            => _templates.TryGetValue(name, out var t) ? t : null;
    }
}
