using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace SpellCreationNamespace{
    enum SpellWrapperBasePrefabTypeEnum
    {
        EMPTY,
        CUBE,
        SPHERE,
        TETRAHEDRON,
        OCTAHEDRON,
    }

    interface ISpellWrapperBasePrefabSettings
    {
        Entity Prefab { get; set; }
        SpellWrapperBasePrefabTypeEnum WrapperType { get; set; }
    }

    struct SpellWrapperBasePrefabSettings : IComponentData, ISpellWrapperBasePrefabSettings
    {
        public Entity Prefab { get; set; }
        public SpellWrapperBasePrefabTypeEnum WrapperType { get; set; }
    }


    class SpellWrapperBasePrefabSettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
#pragma warning disable 649
        public SpellWrapperBasePrefabTypeEnum wrapperType;
        public GameObject prefab;
#pragma warning restore 649

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var prefabSettings = new SpellWrapperBasePrefabSettings
            {
                Prefab = conversionSystem.GetPrimaryEntity(prefab),
                WrapperType = wrapperType,
            };
            Configure(ref prefabSettings);
            dstManager.AddComponentData(entity, prefabSettings);
        }

        internal virtual void Configure(ref SpellWrapperBasePrefabSettings prefabSettings) { }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs) => referencedPrefabs.Add(prefab);
    }

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    class SpellWrapperPrefabSpawningSystem : SystemBase
    {
        public static SpellWrapperPrefabSpawningSystem inst;

        //cached prefab spawning
        public static Dictionary<SpellWrapperBasePrefabTypeEnum, Entity> cachedSpellPrefabSystems = new Dictionary<SpellWrapperBasePrefabTypeEnum, Entity>();

        protected void CacheSpellPrefabs(bool warnOnDuplicate)
		{
            using (var entities = GetEntityQuery(new ComponentType[] { typeof(SpellWrapperBasePrefabSettings) }).ToEntityArray(Allocator.TempJob))
            {
                for (int j = 0; j < entities.Length; j++)
                {
                    var prefabSettings = EntityManager.GetComponentData<SpellWrapperBasePrefabSettings>(entities[j]);

                    if (cachedSpellPrefabSystems.ContainsKey(prefabSettings.WrapperType))
                    {
                        if(warnOnDuplicate)
                            Debug.LogError("DUPLICATE SPELL WRAPPER BASE ENUM TYPE");
                    }
                    else
                    {
                        SpellWrapperPrefabSpawningSystem.cachedSpellPrefabSystems.Add(prefabSettings.WrapperType, prefabSettings.Prefab);
                    }
                }
            }
        }

        protected override void OnCreate()
        {
            SpellWrapperPrefabSpawningSystem.inst = this;
            base.OnCreate();
            CacheSpellPrefabs(true);
        }


        internal virtual int GetRandomSeed(SpellWrapperBasePrefabSettings spawnSettings)
        {
            var seed = 0;
            return seed;
        }

        internal virtual void OnBeforeInstantiatePrefab(ref SpellWrapperBasePrefabSettings spawnSettings) { }

        internal virtual void ConfigureInstance(Entity instance, ref SpellWrapperBasePrefabSettings spawnSettings) { }

        protected override void OnUpdate() { }


        public static Entity InstantiateNewSpellWrapperEntity(EntityManager entityMananager, SpellWrapperBasePrefabTypeEnum type)
		{
            if(cachedSpellPrefabSystems.ContainsKey(type))
			{
                return entityMananager.Instantiate(cachedSpellPrefabSystems[type]);
            }
            else
			{
                SpellWrapperPrefabSpawningSystem.inst.CacheSpellPrefabs(false);
                if (cachedSpellPrefabSystems.ContainsKey(type))
                {
                    return entityMananager.Instantiate(cachedSpellPrefabSystems[type]);
                }

                Debug.LogError("No Cached Entity Found with that SpellWrapperBasePrefabTypeEnum value: " + type);
			}

            return new Entity();
        }

    }

}

