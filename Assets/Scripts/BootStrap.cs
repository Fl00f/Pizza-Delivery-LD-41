using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;
using System.Linq;

public class BootStrap
{
    public static MeshInstanceRenderer PlayerLook;
    public static MeshInstanceRenderer ArrowLook;

    public static EntityArchetype PlayerArchetype { get; private set; }
    public static EntityArchetype ArrowArchetype { get; private set; }

    public static EntityArchetype MouseDataArchetype { get; private set; }

    public static float2 PlayerSpawnPosition;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        DefineArchetypes(entityManager);
        PlayerSpawnPosition = new float2(0, 0);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeWithScene()
    {
        PlayerLook = GetLookFromPrototype("PlayerRenderPrototype");
        ArrowLook = GetLookFromPrototype("ArrowRenderPrototype");
        NewGame();
    }

    private static void NewGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        CreatePlayer(entityManager);

        var mouseEntity = entityManager.CreateEntity(MouseDataArchetype);
        //HACK to see mouse data
        entityManager.AddSharedComponentData(mouseEntity, PlayerLook);
    }

    private static void DefineArchetypes(EntityManager entityManager)
    {
        PlayerArchetype = entityManager.CreateArchetype(typeof(Player),
                                                typeof(PlayerMoveSpeed),
                                                typeof(Heading2D),
                                                typeof(Position2D),
                                                typeof(TransformMatrix),
                                                typeof(PlayerInput));

        MouseDataArchetype = entityManager.CreateArchetype(
            typeof(Mouse),
            typeof(Position2D), //world postition
            typeof(Heading2D)); //world delta

        ArrowArchetype = entityManager.CreateArchetype(typeof(ArrowSpawnData));
    }

    private static void CreatePlayer(EntityManager entityManager)
    {
        //TODO: Create some kind of settings to change in editor
        Entity playerEntity = entityManager.CreateEntity(PlayerArchetype);
        entityManager.SetComponentData(playerEntity, new PlayerInput { FireCooldown = 0 });

        entityManager.SetComponentData(playerEntity, new Position2D { Value = PlayerSpawnPosition });
        entityManager.SetComponentData(playerEntity, new Heading2D { Value = new float2(0.0f, 1.0f) });
        entityManager.SetComponentData(playerEntity, new PlayerMoveSpeed { speed = 5 });

        entityManager.AddSharedComponentData(playerEntity, PlayerLook);
    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        UnityEngine.Object.Destroy(proto);
        return result;
    }
}