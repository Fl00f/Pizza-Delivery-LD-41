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
    public static MeshInstanceRenderer CannonLook;
    public static MeshInstanceRenderer IngredientLook;
    public static MeshInstanceRenderer PizzaLook;

    public static EntityArchetype PlayerArchetype { get; private set; }
    public static EntityArchetype ArrowArchetype { get; private set; }
    public static EntityArchetype CannonArchetype { get; private set; }
    public static EntityArchetype IngredientSpawnArchetype { get; private set; }
    public static EntityArchetype IngredientPizzaSpawnArchetype { get; private set; }

    public static EntityArchetype PizzaArchetype { get; private set; }

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
        CannonLook = GetLookFromPrototype("IngredientCannonPrototype");
        PizzaLook = GetLookFromPrototype("PizzaPrototype");

        GetIngridientPrototypes();

        NewGame();
    }

    private static void GetIngridientPrototypes()
    {
        //Default for testing
        IngredientLook = GetLookFromPrototype("IngredientPrototype");
    }

    private static void NewGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        CreatePlayer(entityManager);

        var mouseEntity = entityManager.CreateEntity(MouseDataArchetype);

        CreateCannons(entityManager);
        CreatePizzas(entityManager);
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

        CannonArchetype = entityManager.CreateArchetype(typeof(IngridientCannon),
                                                            typeof(Heading2D),
                                                            typeof(Position2D),
                                                            typeof(TransformMatrix));

        IngredientSpawnArchetype = entityManager.CreateArchetype(typeof(IngridientSpawnData));

        PizzaArchetype = entityManager.CreateArchetype(typeof(Pizza),
                                                            typeof(Heading2D),
                                                            typeof(Position2D),
                                                            typeof(TransformMatrix));

        IngredientPizzaSpawnArchetype = entityManager.CreateArchetype(typeof(IngredientSpawnOnPizzaData),
                                                           typeof(Heading2D),
                                                           typeof(Position2D),
                                                           typeof(TransformMatrix));
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

    private static void CreateCannons(EntityManager entityManager)
    {
        //TODO: Create some kind of settings to change in editor
        Entity cannonRight = entityManager.CreateEntity(CannonArchetype);
        entityManager.SetComponentData(cannonRight, new IngridientCannon { FireCooldown = 3, CannonOn = 1 });

        entityManager.SetComponentData(cannonRight, new Position2D { Value = new float2(10, 0) });
        entityManager.SetComponentData(cannonRight, new Heading2D { Value = new float2(-1, 0) });

        entityManager.AddSharedComponentData(cannonRight, CannonLook);

        //***********************************************************

        Entity cannonLeft = entityManager.CreateEntity(CannonArchetype);
        entityManager.SetComponentData(cannonLeft, new IngridientCannon { FireCooldown = 3, CannonOn = 1 });

        entityManager.SetComponentData(cannonLeft, new Position2D { Value = new float2(-10, 0) });
        entityManager.SetComponentData(cannonLeft, new Heading2D { Value = new float2(1, 0) });

        entityManager.AddSharedComponentData(cannonLeft, CannonLook);
    }

    private static void CreatePizzas(EntityManager entityManager)
    {
        //TODO: Create some kind of settings to change in editor
        Entity pizzaRight = entityManager.CreateEntity(PizzaArchetype);
        entityManager.SetComponentData(pizzaRight, new Pizza());
        entityManager.SetComponentData(pizzaRight, new Position2D { Value = new float2(2, 1) });
        entityManager.SetComponentData(pizzaRight, new Heading2D { Value = new float2(0, -1) });

        entityManager.AddSharedComponentData(pizzaRight, PizzaLook);

        //***********************************************************

        Entity pizzaLeft = entityManager.CreateEntity(PizzaArchetype);
        entityManager.SetComponentData(pizzaLeft, new Pizza());
        entityManager.SetComponentData(pizzaLeft, new Position2D { Value = new float2(-2, 1) });
        entityManager.SetComponentData(pizzaLeft, new Heading2D { Value = new float2(0, -1) });

        entityManager.AddSharedComponentData(pizzaLeft, PizzaLook);
    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        UnityEngine.Object.Destroy(proto);
        return result;
    }
}