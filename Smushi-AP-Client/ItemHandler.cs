using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Archipelago.MultiClient.Net.Models;
using CMF;
using UnityEngine;

namespace Smushi_AP_Client
{
    public enum SCHItem
    {
        EnergySpore = 0x1,
        WindEssence = 0x2,
        LeafGlider = 0x3,
        ProgressiveHooks = 0x4,
        ToolOfMining = 0x5,
        ToolOfWriting = 0x6, 
        BandOfElasticity = 0x7,
        BladeOfPower = 0x8,
        AncientRelic = 0x9,
        Blueberry = 0xA,
        MycologyJournal = 0xB,
        FirestarterKit = 0xC,
        SecretPassword = 0xD,
        ExplosivePowder = 0xE,
        ContainerOfLight = 0xF,
        Headlamp = 0x10,
        EssenceOfWater = 0x11,
        SecretOpener = 0x13,
        OldString = 0x14,
        Screwdriver = 0x15,
        BandAid = 0x16,
        RingOfLove = 0x17,
        RingOfYouth = 0x18,
        RingOfTruth = 0x19,
        RingOfProsperity = 0x1A,
        RingOfSpirit = 0x1B,
        ConchShell = 0x1C,
        SacredStreamer = 0x1D,
        SuperSpore = 0x1E,
        SuperEssence = 0x1F,
        LotusFlower = 0x20,
        AmethystShroomie = 0x101,
        StrawberryShroomie = 0x102,
        FlowerShroomie = 0x103,
        SHOBowlShroomie = 0x104,
        VerdantShroomie = 0x105,
        PelagicShroomie = 0x106,
        HoneyShroomie = 0x107,
        SparkleShroomie = 0x108,
        ClavariaShroomie = 0x109,
        InkcapShroomie = 0x10A,
        SharpShroomie = 0x10B,
        PreciousShroomie = 0x10C,
        RainbowShroomie = 0x10D,
        VeiledShroomie = 0x10E,
        SacredShroomie = 0x10F,
        PurpleCrystal = 0x200,
        SuperSecretFaceThing = 0x201
    }
    
    public class ItemHandler : MonoBehaviour
    {
        private Sprite apSprite = LoadSprite();
        private Queue<(int, ItemInfo)> cachedItems;
        
        private static Sprite LoadSprite()
        {
            var dllDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var logoPath = Path.Combine(dllDir, "APLogo.png");
            var data = File.ReadAllBytes(logoPath);
            var tex = new Texture2D(2, 2);
            tex.LoadImage(data);
            return Sprite.Create(
                tex,
                new Rect(0, 0, tex.width, tex.height),
                new Vector2(0.5f, 0.5f),
                100f
            );
        }

        private bool TryGetHandlers(
            out SaveLoadManager manager, 
            out PlayerData pd,
            out AdvancedWalkerController tpc,
            out InventoryManager inv)
        {
            manager = FindObjectOfType<SaveLoadManager>();
            inv = FindObjectOfType<InventoryManager>();
            if (manager == null)
            {
                pd = null;
                tpc = null;
                return false;
            }
            pd = manager.pd;
            tpc = manager.tpc;
            return pd != null && inv != null && tpc != null;
        }

        void Awake()
        {
            cachedItems = new Queue<(int, ItemInfo)>();
        }
        
        public void HandleItem(int index, ItemInfo item)
        {
            try
            {
                if (index < PluginMain.SaveDataHandler.CustomPlayerData.ItemIndex) 
                    return;
                
                if (!TryGetHandlers(out var manager, out var playerData, out var movement, out var inventory))
                {
                    cachedItems.Enqueue((index, item));
                    return;
                }
                
                if (cachedItems.Count > 0)
                    FlushQueue(manager, playerData, movement, inventory);
                HandleItem(index, item, manager, playerData, movement, inventory);
                
            }
            catch (Exception ex)
            {
                APConsole.Instance.Log($"[HandleItem ERROR] {ex}");
                throw;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                FlushQueue();
            }
        }
        
        private void FlushQueue()
        {
            if (!TryGetHandlers(out var manager, out var playerData, out var movement, out var inventory))
                return;
            FlushQueue(manager, playerData, movement, inventory);
        }

        private void FlushQueue(
            SaveLoadManager manager,
            PlayerData playerData,
            AdvancedWalkerController movement,
            InventoryManager inventory)
        {
            while (cachedItems.Count > 0)
            {
                var (index, item) = cachedItems.Dequeue();
                HandleItem(index, item, manager, playerData, movement, inventory);
            }
        }

        private void HandleItem(int index, ItemInfo item, SaveLoadManager manager, PlayerData playerData, AdvancedWalkerController movement, InventoryManager inventory)
        {
            var customPlayerData = PluginMain.SaveDataHandler.CustomPlayerData;
            if (index < customPlayerData.ItemIndex) 
                return;
            PluginMain.SaveDataHandler.CustomPlayerData.ItemIndex++;
            switch ((SCHItem)item.ItemId)
            {
                case  SCHItem.EnergySpore:
                    playerData.AddSpore();
                    movement.sprinterController.CheckSporeCount();
                    var spore = Resources.Load<Item>("items/Spore");
                    inventory.AddItem(spore);
                    if (playerData.sporeCount < 2)
                        break;
                    if (playerData.sporeCount >= 2 && !playerData.hasSprint)
                        playerData.hasSprint = true;
                    break;
                case SCHItem.WindEssence:
                    ++playerData.gliderStage;
                    var windEssence = Resources.Load<Item>("items/WindEssence");
                    playerData.UpdateGliderStamina();
                    inventory.AddItem(windEssence);
                    break;
                case SCHItem.LeafGlider:
                    customPlayerData.HasLeaf = true;
                    movement.gliderEnabled = true;
                    var leaf = Resources.Load<Item>("items/Leaf");
                    inventory.AddItem(leaf);
                    break;
                case SCHItem.ProgressiveHooks:
                    if (!customPlayerData.HasHooks)
                    {
                        customPlayerData.HasHooks = true;
                        movement.hookEnabled = true;
                        movement.hookClimber.enabled = true;
                        var hooks = Resources.Load<Item>("items/Hooks");
                        inventory.AddItem(hooks);
                    }
                    else
                    {
                        playerData.climbStamina = 1f;
                        movement.hookClimber.savedStamina = 1f;
                        movement.hookClimber.stamina = 1f;
                        movement.hookClimber.hasHookUpgrade = true;
                        var hooks2 = Resources.Load<Item>("items/Hooks2");
                        inventory.AddItem(hooks2);
                    }
                    break;
                case SCHItem.ToolOfMining:
                    customPlayerData.HasHexKey = true;
                    playerData.hasHexkey = true;
                    var hexKey = Resources.Load<Item>("items/19_HexKey");
                    inventory.AddItem(hexKey);
                    break;
                case SCHItem.ToolOfWriting:
                    playerData.dialogBools["hasPencil"] = true;
                    var pencil = Resources.Load<Item>("items/Pencil");
                    inventory.AddItem(pencil);
                    break;
                case SCHItem.BandOfElasticity:
                    playerData.dialogBools["rubberBand"] = true;
                    var elastic = Resources.Load<Item>("items/RubberBand");
                    inventory.AddItem(elastic);
                    break;
                case SCHItem.BladeOfPower:
                    playerData.hasNeedle = true;
                    var needle = Resources.Load<Item>("items/Needle");
                    inventory.AddItem(needle);
                    break;
                case SCHItem.AncientRelic:
                    if (playerData.dialogBools["coin1"])
                    {
                        playerData.dialogBools["coin1"] = true;
                        var relic1 = Resources.Load<Item>("items/Relic1");
                        inventory.AddItem(relic1);
                    }
                    else
                    {
                        playerData.dialogBools["coin2"] = true;
                        var relic2 = Resources.Load<Item>("items/Relic2");
                        inventory.AddItem(relic2);
                    }
                    break;
                case SCHItem.Blueberry:
                    playerData.berryCount++;
                    var blueberry = Resources.Load<Item>("items/Blueberry");
                    inventory.AddItem(blueberry);
                    break;
                case SCHItem.MycologyJournal:
                    playerData.hasJournal = true;
                    break;
                case SCHItem.FirestarterKit:
                    playerData.hasFlint = true;
                    var firestarter = Resources.Load<Item>("items/FlintSteel");
                    inventory.AddItem(firestarter);
                    break;
                case SCHItem.SecretPassword:
                    playerData.dialogBools["knowsPass"] = true;
                    var password = Resources.Load<Item>("items/Password");
                    inventory.AddItem(password);
                    break;
                case SCHItem.ExplosivePowder:
                    playerData.AddPowderCount();
                    var powder = Resources.Load<Item>("items/ExplosivePowder");
                    inventory.AddItem(powder);
                    break;
                case SCHItem.ContainerOfLight:
                    playerData.dialogBools["lightbulb"] = true;
                    var lightContainer = Resources.Load<Item>("items/ChristmasLight");
                    inventory.AddItem(lightContainer);
                    break;
                case SCHItem.Headlamp:
                    playerData.hasHeadlamp = true;
                    var headlamp = Resources.Load<Item>("items/Headlamp");
                    inventory.AddItem(headlamp);
                    break;
                case SCHItem.EssenceOfWater:
                    playerData.hasWaterEssence = true;
                    playerData.dialogBools["hasDiving"] = true;
                    if (playerData.currentSceneName == "Zone 3")
                    {
                        var island = FindObjectOfType<IslandCaveDialogueTrigger>();
                        island.SetIslandHasDiveNow();
                    }
                    if (manager.caveIsland != null)
                        manager.caveIsland.currentIslandState = IslandCaveDialogueTrigger.CaveIslandState.HasDiveNow;
                    var waterEssence = Resources.Load<Item>("items/WaterEssence");
                    inventory.AddItem(waterEssence);
                    break;
                case SCHItem.SecretOpener:
                    playerData.dialogBools["secretKey"] = true;
                    var key = Resources.Load<Item>("items/Key");
                    inventory.AddItem(key);
                    break;
                case SCHItem.OldString:
                    playerData.dialogBools["hasString"] = true;
                    var oldString = Resources.Load<Item>("items/21_String");
                    inventory.AddItem(oldString);
                    break;
                case SCHItem.Screwdriver:
                    playerData.hasScrewdriver = true;
                    var screwdriver = Resources.Load<Item>("items/forestheartitems/Screwdriver");
                    inventory.AddItem(screwdriver);
                    break;
                case SCHItem.BandAid:
                    playerData.dialogBools["hasBandaid"] = true;
                    var bandAid = Resources.Load<Item>("items/forestheartitems/Bandaid");
                    inventory.AddItem(bandAid);
                    break;
                case SCHItem.RingOfLove:
                    playerData.rings.Add("diamond", false);
                    var ringOfLove = Resources.Load<Item>("items/forestheartitems/Ring_Diamond");
                    inventory.AddItem(ringOfLove);
                    break;
                case SCHItem.RingOfYouth:
                    playerData.rings.Add("emerald", false);
                    var ringOfYouth = Resources.Load<Item>("items/forestheartitems/Ring_Emerald");
                    inventory.AddItem(ringOfYouth);
                    break;
                case SCHItem.RingOfTruth:
                    playerData.rings.Add("sapphire", false);
                    var ringOfTruth = Resources.Load<Item>("items/forestheartitems/Ring_Sapphire");
                    inventory.AddItem(ringOfTruth);
                    break;
                case SCHItem.RingOfProsperity:
                    playerData.rings.Add("citrine", false);
                    var ringOfProsperity = Resources.Load<Item>("items/forestheartitems/Ring_Citrine");
                    inventory.AddItem(ringOfProsperity);
                    break;
                case SCHItem.RingOfSpirit:
                    playerData.rings.Add("amethyst", false);
                    var ringOfSpirit = Resources.Load<Item>("items/forestheartitems/Ring_Amethyst");
                    inventory.AddItem(ringOfSpirit);
                    break;
                case SCHItem.ConchShell:
                    customPlayerData.HasConch = true;
                    break;
                case SCHItem.SacredStreamer:
                    if (playerData.shideCountTotal >= 4)
                        break;
                    ++playerData.shideCount;
                    ++playerData.shideCountTotal;
                    playerData.forestHeart?.SetHeartState();
                    var shide = Resources.Load<Item>("items/forestheartitems/Shide");
                    inventory.AddItem(shide);
                    break;
                case SCHItem.SuperSpore:
                    movement.sprinterController.hasSprintBoost = true;
                    playerData.hasSprintBoost = true;
                    var superSpore = Resources.Load<Item>("items/forestheartitems/SuperSpore");
                    inventory.AddItem(superSpore);
                    break;
                case SCHItem.SuperEssence:
                    movement.hasGlideBoost = true;
                    playerData.hasGlideBoost = true;
                    var superEssence = Resources.Load<Item>("items/SuperEssence");
                    inventory.AddItem(superEssence);
                    break;
                case SCHItem.LotusFlower:
                    if (customPlayerData.LotusCount == 5)
                        break;
                    var flowerCount = Math.Min(5, customPlayerData.LotusCount + 1);
                    customPlayerData.LotusCount = flowerCount;
                    customPlayerData.LotusCountCurrent = Math.Min(5, customPlayerData.LotusCountCurrent + 1);
                    break;
                case SCHItem.AmethystShroomie:
                    var purpleSkin = Resources.Load<Skins>("skins/PurpleAugmenter");
                    inventory.AddSkin(purpleSkin);
                    inventory.CraftSkin(purpleSkin, purpleSkin.craftableSkin);
                    break;
                case SCHItem.StrawberryShroomie:
                    var strawberrySkin = Resources.Load<Skins>("skins/StrawberryAugmenter");
                    inventory.AddSkin(strawberrySkin);
                    inventory.CraftSkin(strawberrySkin, strawberrySkin.craftableSkin);
                    break;
                case SCHItem.FlowerShroomie:
                    var flowerSkin = Resources.Load<Skins>("skins/FlowerAugm");
                    inventory.AddSkin(flowerSkin);
                    inventory.CraftSkin(flowerSkin, flowerSkin.craftableSkin);
                    break;
                case SCHItem.SHOBowlShroomie:
                    var onionSkin = Resources.Load<Skins>("skins/OnionAugm");
                    inventory.AddSkin(onionSkin);
                    inventory.CraftSkin(onionSkin, onionSkin.craftableSkin);
                    break;
                case SCHItem.VerdantShroomie:
                    var greenSkin = Resources.Load<Skins>("skins/GreenAugmenter");
                    inventory.AddSkin(greenSkin);
                    inventory.CraftSkin(greenSkin, greenSkin.craftableSkin);
                    break;
                case SCHItem.PelagicShroomie:
                    var blueSkin = Resources.Load<Skins>("skins/BlueAugmenter");
                    inventory.AddSkin(blueSkin);
                    inventory.CraftSkin(blueSkin, blueSkin.craftableSkin);
                    break;
                case SCHItem.HoneyShroomie:
                    var honeySkin = Resources.Load<Skins>("skins/HoneyAugmenter");
                    inventory.AddSkin(honeySkin);
                    inventory.CraftSkin(honeySkin, honeySkin.craftableSkin);
                    break;
                case SCHItem.SparkleShroomie:
                    var sparkleSkin = Resources.Load<Skins>("skins/SparkleAug");
                    inventory.AddSkin(sparkleSkin);
                    inventory.CraftSkin(sparkleSkin, sparkleSkin.craftableSkin);
                    break;
                case SCHItem.ClavariaShroomie:
                    var spikeySkin = Resources.Load<Skins>("skins/SpikeyAug");
                    inventory.AddSkin(spikeySkin);
                    inventory.CraftSkin(spikeySkin, spikeySkin.craftableSkin);
                    break;
                case SCHItem.InkcapShroomie:
                    var inkcapSkin = Resources.Load<Skins>("skins/InkcapAugmenter");
                    inventory.AddSkin(inkcapSkin);
                    inventory.CraftSkin(inkcapSkin, inkcapSkin.craftableSkin);
                    break;
                case SCHItem.SharpShroomie:
                    var sharpSkin = Resources.Load<Skins>("skins/forestheartskins/SharpAug");
                    inventory.AddSkin(sharpSkin);
                    inventory.CraftSkin(sharpSkin, sharpSkin.craftableSkin);
                    break;
                case SCHItem.PreciousShroomie:
                    var crystalSkin = Resources.Load<Skins>("skins/forestheartskins/CrystalAug");
                    inventory.AddSkin(crystalSkin);
                    inventory.CraftSkin(crystalSkin, crystalSkin.craftableSkin);
                    break;
                case SCHItem.RainbowShroomie:
                    var rainbowSkin = Resources.Load<Skins>("skins/forestheartskins/RainbowAug");
                    inventory.AddSkin(rainbowSkin);
                    inventory.CraftSkin(rainbowSkin, rainbowSkin.craftableSkin);
                    break;
                case SCHItem.VeiledShroomie:
                    var veiledSkin = Resources.Load<Skins>("skins/forestheartskins/VeilAug");
                    inventory.AddSkin(veiledSkin);
                    inventory.CraftSkin(veiledSkin, veiledSkin.craftableSkin);
                    break;
                case SCHItem.SacredShroomie:
                    var sacredSkin = Resources.Load<Skins>("skins/forestheartskins/SacredAug");
                    inventory.AddSkin(sacredSkin);
                    inventory.CraftSkin(sacredSkin, sacredSkin.craftableSkin);
                    break;
                case SCHItem.PurpleCrystal:
                    playerData.AddSkin1Rock();
                    break;
                case SCHItem.SuperSecretFaceThing:
                    customPlayerData.HasFaceThing = true;
                    playerData.transform.Find("ModelRoot/shroom player/Armature/Hip/LowerSpine/UpperSpine/Head/mustache").gameObject.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            SaveSystem._instance.SaveAllData(manager, true);
        }
    }
}