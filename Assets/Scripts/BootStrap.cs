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
    public static Settings GameSettings;

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
        var settingsGO = GameObject.Find("Settings");
        GameSettings = settingsGO.GetComponent<Settings>();

        GetPrototypes();

        NewGame();
    }

    private static void GetPrototypes()
    {
        PlayerLook = GetLookFromPrototype("PlayerRenderPrototype");
        ArrowLook = GetLookFromPrototype("ArrowRenderPrototype");
        CannonLook = GetLookFromPrototype("IngredientCannonPrototype");
        PizzaLook = GetLookFromPrototype("PizzaPrototype");

        GetIngridientPrototypes();
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
        Entity playerEntity = entityManager.CreateEntity(PlayerArchetype);
        entityManager.SetComponentData(playerEntity, new PlayerInput { FireCooldown = 0 });

        entityManager.SetComponentData(playerEntity, new Position2D { Value = PlayerSpawnPosition });
        entityManager.SetComponentData(playerEntity, new Heading2D { Value = new float2(0.0f, 1.0f) });
        entityManager.SetComponentData(playerEntity, new PlayerMoveSpeed { speed = GameSettings.PlayerMovementSpeed });

        entityManager.AddSharedComponentData(playerEntity, PlayerLook);
    }

    private static void CreateCannons(EntityManager entityManager)
    {
        //TODO: Create some kind of settings to change in editor
        Entity cannonRight = entityManager.CreateEntity(CannonArchetype);
        entityManager.SetComponentData(cannonRight, new IngridientCannon { FireCooldown = GameSettings.CannonFireRate, CannonOn = GameSettings.CannonsOnByDefault });

        entityManager.SetComponentData(cannonRight, new Position2D { Value = new float2(GameSettings.CannonPositions[0].x, GameSettings.CannonPositions[0].y) });
        entityManager.SetComponentData(cannonRight, new Heading2D { Value = new float2(GameSettings.CannonHeadings[0].x, GameSettings.CannonHeadings[0].y) });

        entityManager.AddSharedComponentData(cannonRight, CannonLook);

        //***********************************************************

        Entity cannonLeft = entityManager.CreateEntity(CannonArchetype);
        entityManager.SetComponentData(cannonLeft, new IngridientCannon { FireCooldown = GameSettings.CannonFireRate, CannonOn = GameSettings.CannonsOnByDefault });

        entityManager.SetComponentData(cannonLeft, new Position2D { Value = new float2(GameSettings.CannonPositions[1].x, GameSettings.CannonPositions[1].y) });
        entityManager.SetComponentData(cannonLeft, new Heading2D { Value = new float2(GameSettings.CannonHeadings[1].x, GameSettings.CannonHeadings[1].y) });

        entityManager.AddSharedComponentData(cannonLeft, CannonLook);
    }

    private static void CreatePizzas(EntityManager entityManager)
    {
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