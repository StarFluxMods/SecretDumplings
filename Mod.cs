using System.Collections.Generic;
using KitchenLib;
using KitchenLib.Logging;
using KitchenLib.Logging.Exceptions;
using KitchenMods;
using System.Linq;
using System.Reflection;
using Kitchen;
using KitchenData;
using KitchenLib.References;
using KitchenLib.Utils;
using UnityEngine;

namespace KitchenSecretDumplings
{
    public class Mod : BaseMod, IModSystem
    {
        public const string MOD_GUID = "com.starfluxgames.secretdumplings";
        public const string MOD_NAME = "Secret Dumplings";
        public const string MOD_VERSION = "0.1.1";
        public const string MOD_AUTHOR = "StarFluxGames";
        public const string MOD_GAMEVERSION = ">=1.1.7";

        public static AssetBundle Bundle;
        public static KitchenLogger Logger;

        public Mod() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly())
        {
        }

        protected override void OnInitialise()
        {
            Logger.LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            Item cookedDumplings = GameData.Main.Get<Item>(ItemReferences.CookedDumplings);
            Item preparedDumplings = GameData.Main.Get<Item>(ItemReferences.PreparedDumplings);
            Dish dumplingsDish = GameData.Main.Get<Dish>(DishReferences.Dumplings);
            dumplingsDish.IconPrefab = MaterialUtils.AssignMaterialsByNames(Bundle.LoadAsset<GameObject>("Dumpling - Icon"));
            dumplingsDish.DisplayPrefab = MaterialUtils.AssignMaterialsByNames(Bundle.LoadAsset<GameObject>("Dumpling - Icon"));
            cookedDumplings.Prefab = MaterialUtils.AssignMaterialsByNames(Bundle.LoadAsset<GameObject>("Dumplings Cooked"));
            preparedDumplings.Prefab = MaterialUtils.AssignMaterialsByNames(Bundle.LoadAsset<GameObject>("Dumplings Raw"));
            ItemGroup platedDumpling = GameData.Main.Get<ItemGroup>(ItemGroupReferences.DumplingsPlated);
            platedDumpling.Prefab = MaterialUtils.AssignMaterialsByNames(Bundle.LoadAsset<GameObject>("Dumpling Plated"));
            ItemGroupView view = platedDumpling.Prefab.AddComponent<ItemGroupView>();
            ItemGroupViewUtils.AddSideContainer(GameData.Main, platedDumpling, view);

            ItemGroupView.ComponentGroup dumplings = new()
            {
                Item = GameData.Main.Get<Item>(ItemReferences.CookedDumplings),
                Objects = new List<GameObject>
                {
                    GameObjectUtils.GetChildObject(platedDumpling.Prefab, "Dumpling Cooked/Dumplings/Sphere"),
                    GameObjectUtils.GetChildObject(platedDumpling.Prefab, "Dumpling Cooked/Dumplings/Sphere (1)")
                },
                DrawAll = true
            };

            ItemGroupView.ComponentGroup seaweed = new()
            {
                Objects = new List<GameObject>
                {
                    GameObjectUtils.GetChildObject(platedDumpling.Prefab, "Seaweed - Cooked/Seaweed"),
                    GameObjectUtils.GetChildObject(platedDumpling.Prefab, "Seaweed - Cooked/Seaweed (1)")
                },
                DrawAll = true
            };

            List<ItemGroupView.ComponentGroup> groups = new() { dumplings, seaweed };
            Dictionary<int, ItemGroupView.ComponentGroup> draw = new()
            {
                { ItemReferences.CookedDumplings, dumplings },
                { ItemReferences.SeaweedCooked, seaweed }
            };
            view.ComponentGroups = groups;
            view.DrawComponents = draw;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnPostActivate(KitchenMods.Mod mod)
        {
            Bundle = mod.GetPacks<AssetBundleModPack>().SelectMany(e => e.AssetBundles).FirstOrDefault() ?? throw new MissingAssetBundleException(MOD_GUID);
            Logger = InitLogger();
        }
    }
}