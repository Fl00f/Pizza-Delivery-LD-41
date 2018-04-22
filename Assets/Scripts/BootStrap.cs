using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BootStrap
{
    public static Settings GameSettings;

    public static GameObject[] PizzaOrderUIs;

    #region IngredientLooks

    public static MeshInstanceRenderer IngredientDefaultLook;

    public static IngredientData[] IngredientsData;

    #endregion IngredientLooks

    public static MeshInstanceRenderer PlayerLook;
    public static MeshInstanceRenderer ArrowLook;
    public static MeshInstanceRenderer CannonLook;
    public static MeshInstanceRenderer PizzaLook;

    public static EntityArchetype PlayerArchetype { get; private set; }
    public static EntityArchetype ArrowArchetype { get; private set; }
    public static EntityArchetype CannonArchetype { get; private set; }
    public static EntityArchetype IngredientSpawnArchetype { get; private set; }
    public static EntityArchetype IngredientPizzaSpawnArchetype { get; private set; }

    public static EntityArchetype PizzaArchetype { get; private set; }

    public static EntityArchetype MouseDataArchetype { get; private set; }

    public static EntityArchetype ScoringArchetype { get; private set; }
    public static EntityArchetype AddScoreArchetype { get; private set; }
    public static EntityArchetype SubtractScoreArchetype { get; private set; }

    private static Canvas GameOverScreen;
    private static Text ScoringText;

    private static bool wasSceneGOsSet = false;
    private static bool wasProtoTypesSet = false;

    private static void GetSceneGOs()
    {
        if (wasSceneGOsSet) return;

        var settingsGO = GameObject.Find("Settings");
        GameSettings = settingsGO.GetComponent<Settings>();

        var gameOverGO = GameObject.Find("GameEndCanvas");
        GameOverScreen = gameOverGO.GetComponent<Canvas>();

        var scoringText = GameObject.Find("ScoringText");
        ScoringText = scoringText.GetComponent<Text>();

        wasSceneGOsSet = true;
    }

    private static void GetPrototypes()
    {
        if (wasProtoTypesSet) return;

        PlayerLook = GetLookFromPrototype("PlayerRenderPrototype");
        ArrowLook = GetLookFromPrototype("ArrowRenderPrototype");
        CannonLook = GetLookFromPrototype("IngredientCannonPrototype");
        PizzaLook = GetLookFromPrototype("PizzaPrototype");

        SetIngridientData();
        wasProtoTypesSet = true;
    }

    public static MeshInstanceRenderer GetIngredientLook(int ingredientType)
    {
        if (!(ingredientType >= 0 && ingredientType < IngredientsData.Length))
        {
            return IngredientDefaultLook;
        }
        return IngredientsData[ingredientType].IngredientLook;
    }

    public static string GetIngredientName(int ingredientType)
    {
        if (!(ingredientType >= 0 && ingredientType < IngredientsData.Length))
        {
            return "Default Ingredient";
        }
        return IngredientsData[ingredientType].IngredientName;
    }

    public static float GetIngredientCost(int ingredientType)
    {
        if (!(ingredientType >= 0 && ingredientType < IngredientsData.Length))
        {
            return 0;
        }
        return IngredientsData[ingredientType].IngredientCost;
    }

    private static void SetIngridientData()
    {
        IngredientDefaultLook = GetLookFromPrototype(nameof(IngredientDefaultLook)); //Default for testing

        IngredientsData = new IngredientData[]
        {
            new IngredientData(){ IngredientName = "Pepperoni", IngredientLook = GetLookFromPrototype("IngredientPeperoniLook"), IngredientCost = 1.99f },
            new IngredientData(){ IngredientName = "Onion", IngredientLook = GetLookFromPrototype("IngredientOnionLook"), IngredientCost = 1.99f },
            new IngredientData(){ IngredientName = "Chicken", IngredientLook = GetLookFromPrototype("IngredientChickenLook"), IngredientCost = 1.99f },
            new IngredientData(){ IngredientName = "Cheese", IngredientLook = GetLookFromPrototype("IngredientCheeseLook"), IngredientCost = 1.99f },
            new IngredientData(){ IngredientName = "Sausage", IngredientLook = GetLookFromPrototype("IngredientSausageLook"), IngredientCost = 1.99f },
            new IngredientData(){ IngredientName = "Mushroom", IngredientLook = GetLookFromPrototype("IngredientPeperoniLook"), IngredientCost = 1.99f }
        };
    }

    private static void SetPizzaOrderUI()
    {
        PizzaOrderUIs = new GameObject[] {
            GameObject.Find("PizzaOrderUI0"),
            GameObject.Find("PizzaOrderUI1")
        };

        foreach (var item in PizzaOrderUIs)
        {
            if (item == null)
            {
                Debug.LogWarning("One of the Pizza Order UIs came up null");
            }
        }
    }

    public static void SetPizzaOrderUIIngredients(List<int> ingredientTypes, int pizzaOrderUIIndex)
    {
        string[] ingredientNames = new string[ingredientTypes.Count];
        for (int i = 0; i < ingredientTypes.Count; i++) {
            ingredientNames[i] = IngredientsData[ingredientTypes[i]].IngredientName;
        }

        if (pizzaOrderUIIndex >= 0 && pizzaOrderUIIndex < PizzaOrderUIs.Length)
            PizzaOrderUIs[pizzaOrderUIIndex].GetComponent<PizzaOrderUI>().SetIngredientsListText(string.Join(", ", ingredientNames));
    }

    public static void SetPizzaOrderUIPrice(float price, int pizzaOrderUIIndex)
    {
        if (pizzaOrderUIIndex >= 0 && pizzaOrderUIIndex < PizzaOrderUIs.Length)
            PizzaOrderUIs[pizzaOrderUIIndex].GetComponent<PizzaOrderUI>().SetCostText($"Price: {price}");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        GetSceneGOs();

        //disable systems on scene start
        foreach (var item in World.Active.BehaviourManagers)
        {
            item.Enabled = false;
        }

        InitSomeDefaultSystemDependancies();
        SetPizzaOrderUI();
    }

    private static void InitSomeDefaultSystemDependancies()
    {
        World.Active.GetOrCreateManager<GameEndSystem>().SetGameOverScreen(GameOverScreen);
        World.Active.GetOrCreateManager<ScoringSystem>().SetScoringText(ScoringText);
    }

    public static void NewGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        InitSomeDefaultSystemDependancies();

        GetPrototypes();
        GetSceneGOs();

        DefineArchetypes(entityManager);

        CreatePlayer(entityManager);

        var mouseEntity = entityManager.CreateEntity(MouseDataArchetype);

        CreateCannons(entityManager);
        CreatePizzas(entityManager);
        CreateScoreKeeper(entityManager);

        foreach (var item in World.Active.BehaviourManagers)
        {
            item.Enabled = true;
        }

        SetPizzaOrderUIPrice(0, 0);
        SetPizzaOrderUIPrice(0, 1);

        //AddScore(66);
    }

    public static void GameOver()
    {
        GameOverScreen.enabled = true;

        foreach (var item in World.Active.BehaviourManagers)
        {
            item.Enabled = false;
        }
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        entityManager.DestroyEntity(entityManager.GetAllEntities());
    }

    public static void RestartGame()
    {
        GameOverScreen.enabled = false;

        NewGame();

        foreach (var item in World.Active.BehaviourManagers)
        {
            item.Enabled = true;
        }
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
                                                            typeof(PizzaGroup),
                                                            typeof(Heading2D),
                                                            typeof(Position2D),
                                                            typeof(TransformMatrix));

        ScoringArchetype = entityManager.CreateArchetype(typeof(ScoreKeeper));
        AddScoreArchetype = entityManager.CreateArchetype(typeof(AddScore));
        SubtractScoreArchetype = entityManager.CreateArchetype(typeof(DeductScore));
    }

    private static void CreatePlayer(EntityManager entityManager)
    {
        Entity playerEntity = entityManager.CreateEntity(PlayerArchetype);
        entityManager.SetComponentData(playerEntity, new PlayerInput { FireCooldown = 0 });

        entityManager.SetComponentData(playerEntity, new Position2D { Value = new float2(GameSettings.PlayerSpawnPosition.x, GameSettings.PlayerSpawnPosition.y) });
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
        List<int> pizzaRightIngredients = new List<int>();
        pizzaRightIngredients.Add(0);

        Entity pizzaRight = entityManager.CreateEntity();
        entityManager.AddSharedComponentData(pizzaRight, new PizzaSpawnData
        {
            PizzaGroup = new PizzaGroup { PizzaId = 1 },
            Position = new Position2D { Value = new float2(2, 1) },
            // IngredientList = pizzaRightIngredients
        });

        //***********************************************************

        List<int> pizzaLeftIngredients = new List<int>();
        pizzaLeftIngredients.Add(1);

        Entity pizzaLeft = entityManager.CreateEntity();
        entityManager.AddSharedComponentData(pizzaLeft, new PizzaSpawnData
        {
            PizzaGroup = new PizzaGroup { PizzaId = 0 },
            Position = new Position2D { Value = new float2(-2, 1) },
            // IngredientList = pizzaLeftIngredients
        });
    }

    private static void CreateScoreKeeper(EntityManager entityManager)
    {
        Entity scorer = entityManager.CreateEntity(ScoringArchetype);
        entityManager.SetComponentData(scorer, new ScoreKeeper() { Score = 0 });
        entityManager.AddSharedComponentData(scorer, new ScoringGroup() { GroupId = 0 });

        //*******
        var ent = entityManager.CreateEntity(AddScoreArchetype);
        entityManager.SetComponentData(ent, new AddScore { Value = 100 }); //Starting amount
        entityManager.AddSharedComponentData(ent, new ScoringGroup { GroupId = 0 });
    }

    private static MeshInstanceRenderer GetLookFromPrototype(string protoName)
    {
        //Can fail if not named right
        var proto = GameObject.Find(protoName);
        var result = proto.GetComponent<MeshInstanceRendererComponent>().Value;
        UnityEngine.Object.Destroy(proto);
        return result;
    }

    public static void AddScore(int score)
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var entity = entityManager.CreateEntity(AddScoreArchetype);
        entityManager.SetComponentData(entity, new AddScore { Value = score });
        entityManager.AddSharedComponentData(entity, new ScoringGroup { GroupId = 0 });
    }

    public static void SubtractScore(int score)
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        var entity = entityManager.CreateEntity(SubtractScoreArchetype);
        entityManager.SetComponentData(entity, new DeductScore { Value = score });
        entityManager.AddSharedComponentData(entity, new ScoringGroup { GroupId = 0 });
    }

    public struct IngredientData
    {
        public string IngredientName;
        public MeshInstanceRenderer IngredientLook;
        public float IngredientCost;
    }
}