# Empyrion Scripting
[English part of this ReadMe.md](#English-Version)

## Installation
1. Downloade die EmpyrionScripting.zip Datei vom aktuellen https://github.com/GitHub-TC/EmpyrionScripting/releases
1. UnZip die Datei in dem Verzeichnis Content\\Mods directory

#### Installation für SinglePlayer
1. Downloade die EmpyrionScripting.zip Datei vom aktuellen https://github.com/GitHub-TC/EmpyrionScripting/releases
1. UnZip die Datei in dem Verzeichnis Content\\Mods directory
1. Das Spiel MUSS dann ohne EAC gestartet werden damit die Mods geladen werden

### Wofür dient diese MOD?
![](Screenshots/DemoShipScreen.png)

Echte Spielinhalte direkt auf einem LCD ausgeben

Eine dem Struktur 'LCDInfo-Demo' findest du im workshop
https://steamcommunity.com/workshop/filedetails/?id=1751409371

#### Hilfe
![](Screenshots/RedAlert.png)
![](Screenshots/LCD1.png)

YouTube video;
* https://youtu.be/8nEpEygHBu8 (danke an Olly :-) )
* https://youtu.be/8MzjdeYlzPU
* https://youtu.be/gPp5CGJusr4
* https://youtu.be/9601vpeLJAI
* https://youtu.be/V1w2A3LAZCs
* https://youtu.be/O89NQJjbQuw
* https://youtu.be/uTgXwrlCfNQ
* https://youtu.be/qhYmJWHk8ec
* https://youtu.be/IbVuzFf_ywI

* https://youtu.be/XzYKNevK0bs
* https://youtu.be/SOnZ_mzytA4
* https://youtu.be/oDOSbllwqSw
* https://youtu.be/qhOnj2D3ejo

* Änderungen mit der A11: https://youtu.be/hxvKs5U1I6I

Beginners guide (english):
* https://steamcommunity.com/workshop/filedetails/discussion/1751409371/3191368095147121750/
* https://youtu.be/IjJTNp_ZYUI

## Tutorials
* Workshop von Sephrajin: DSEV LCD Script Tutorial, https://steamcommunity.com/sharedfiles/filedetails/?id=2863240303
* Workshop von Noob: Scripting Tutorial Ship, https://steamcommunity.com/sharedfiles/filedetails/?id=2817433272
* Workshop von ASTIC, Vega AI, https://steamcommunity.com/sharedfiles/filedetails/?id=2227639387

## Beispiele
Allgemein: 
Benötigt werden mindestens 2 LCD und mindestens 1 Container
1. LCD 1 (Eingabe) wird mit der Abfrage programmiert siehe Beispiele unten. Der Namen des LCDs im ControlPanel MUSS mit "Script:" beginnen.
1. LCD 2 (Ausgabe) Muss eindeutigen Namen haben z.B. "LCD Alle Erze"
1. Jeder Kontainer der eine Information ausgeben soll, muss einen eindeutigen Namen haben

Unten stehen die ID Nummer für Erze und Barren.<br/>
Einige Funktionen benötigen ein Komma"," andere benötigen Simikolon ";".<br/>
Alles in "" sind Texte und nicht mit anzugeben.<br/>
Einzelne ' sind mit anzugeben.<br/>
Man kann eine Information auch auf 2 LCD's anzeigen lassen dannsortedeach bei Targets:Name LCD;Name LCD2<br/>
Man kann eine Information auch auf n LCD's anzeigen lassen dann bei Targets:LCDAusgabe*<br/>
Man kann eine Information auch auf n LCD's anzeigen lassen welche schon im ScriptLCD Namen angegeben sind Script:LCDAusgabe*<br/>
Man kann auf einem LCD auch den Inhalt verschiedner Kisten anzeigen lassen!<br/>
 
 ---

## Sprache, Format, Zeit der Ausgaben
Die Sprache, Uhrzeitoffset und Anzeigeformate kann man mit einem LCD einstellen welches man
'CultureInfo' benennt. Etwaige Fehler bei der Angabe werden in einem LCD mit dem Namen 'CultureInfoDebug' angezeigt.

Dabei kann man in der 'CultureInfo' folgendes angeben:
```
{
  "LanguageTag": "de-EN",
  "i18nDefault": "English",
  "UTCplusTimezone": 2
}
```
LanguageTag: https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-lcid/a9eac961-e77d-41a6-90a5-ce1a8b0cdb9c

## Was ist in der Kiste/Container/ContainerController/MunitionsKiste/Kühlschrank

Eingabe im LCD 1 (alles ohne "")
```
Targets:"NAME DES ANZUZEIGENDEN LCD"
"TEXT Optional"
{{items E.S '"Name der Kiste"'}}
{{Count}}{{Name}}
{{/items}}
```
Bsp:
```
Targets:LCD Alle Erze
Meine Erze
{{#items E.S 'Alle Erze'}}
{{Count}}{{i18 Key}}
{{/items}}
```
---
## Ausgabe aller Erze in der Basis/Schiff/HV/CV

Eingabe im LCD (alles ohne "")
```
Targets:"NAME DES ANZUZEIGENDEN LCD"
"TEXT optional"
{{#test ID in '4296,4297,4298,4299,4300,4301,4302,4317,4318,4332,4341,4345'}}
{{Count}} {{i18n Key}}
{{/test}}
{{/each}}
```
Bsp:
```
Targets:LCD Alle Erze
Meine Erze
{{#each E.S.Items}}
{{#test Id in '4296,4297,4298,4299,4300,4301,4317,4318,4332,4345,4328,4302'}}
{{Count}} {{i18n Key}}
{{/test}}
{{/each}}
```
---
## Ausgabe aller Barren in der Basis/Schiff/HV/CV

Eingabe im LCD (alles ohne "")
```
Targets:"NAME DES ANZUZEIGENDEN LCD"
"TEXT optional"
{{#each E.S.Items}}
{{#test Id in '4319,4320,4321,4322,4323,4324,4325,4326,4327,4328,4329,4333,4342,4346'}}
{{Count}} {{i18n Key}}
{{/test}}
{{/each}}
```
Bsp:
```
Targets:LCD Barren
Alle meine Barren in der Basis:
{{#each E.S.Items}}
{{#test Id in '4319,4320,4321,4322,4323,4324,4325,4326,4327,4328,4329,4333,4342,4346'}}
{{Count}} {{i18n Key}}
{{/test}}
{{/each}}
```
-----------------------------------------------------------------------------------------
## Ausgabe dieser per ID festgelegten Produkte (hier sind es alle Barren die es gibt im Spiel)
Eingabe im LCD (alles ohne "")
```
Targets:"NAME DES ANZUZEIGENDEN LCD"
"TEXT optional"
{{#itemlist E.S.Items '4319,4320,4321,4322,4323,4324,4325,4326,4327,4328,4329,4333,4342,4346'}}
{{Count}} {{i18n Key}}
{{/itemlist}}
```
Bsp:
```
Targets:LCD Alle Barren im Spiel
Alle Barren im Spiel:
{{#itemlist E.S.Items '4319,4320,4321,4322,4323,4324,4325,4326,4327,4328,4329,4333,4342,4346'}}
{{Count}} {{i18n Key}}
{{/itemlist}}
```
-----------------------------------------------------
## Anzeige eines bestimmten Produktes in der Basis/Schiff/HV/CV
```
Eingabe im LCD (alles ohne "")
Targets:"NAME DES ANZUZEIGENDEN LCD"
"TEXT optional"
{{#itemlist E.S.Items '4297'}}
{{Count}} {{i18n Key}}
{{/itemlist}}
```
Bsp:
```
Targets:LCD EISEN ERZ
Meine EisenErz und Barren
{{#itemlist E.S.Items '4297,4320'}}
{{Count}} {{i18n Key}}
{{/itemlist}}
```
------------------------------------------------------------------
## Welche Erze sind alle, bzw. nur noch X Anzahl über

Hier werden alle Erze angezeigt wo nur 1-1000 auf der Basis vorhanden ist.
```
{{#itemlist E.S.Items '4296,4297,4298,4299,4300,4301,4317,4318,4332,4345,4328,4302'}}
{{#test Count geq 1}}
{{#test Count leq 1000}}
{{Count}} {{i18n Key}}
{{/test}}
{{/test}}
{{/itemlist}}
```
---
## Hier werden alle Erze angezeigt die nicht mehr auf der Basis sind
```
{#itemlist E.S.Items '4296,4297,4298,4299,4300,4301,4317,4318,4332,4345,4328,4302'}}
{{#test Count leq 0}}
{{Count}} {{i18n Key}}
{{/test}}
{{/itemlist}}
```

## Vordefinierte ID Listen

Diese Listen können geändert werden oder durch neue Einträge erweitert werden.
Dazu kann einfach der Abschnitt "Ids" in der Datei \[EGS\]\Saves\Games\\[SaveGameName\]\Mods\EmpyrionScripting\Configuration.json
geändert werden.

Hinweis: Um den Originalzustand wieder herzustellen kann der Abschnitt "Ids" aus der Datei geöscht werden. Die Mod trägt dann hier die im Programm hinterlegte Standardkonfiguration wieder ein.

Folgende Listen können über "Ids.\[NameDerListe\] im Standard abgerufen werden.

- "Ore"                   = ",AluminiumOre,CobaltOre,CopperOre,ErestrumOre,GoldOre,IronOre,MagnesiumOre,NeodymiumOre,PentaxidOre,PromethiumOre,SiliconOre,TitanOre,ZascosiumOre,SathiumOre,",
- "Ingot"                 = ",CobaltIngot,CopperIngot,CrushedStone,ErestrumIngot,GoldIngot,IronIngot,MagnesiumPowder,NeodymiumIngot,PentaxidCrystal,PlatinBar,PromethiumPellets,RockDust,SathiumIngot,SiliconIngot,ZascosiumIngot,",
- "BlockL"                = ",AlienBlocks,AlienLargeBlocks,ConcreteArmoredBlocks,ConcreteBlocks,ConcreteDestroyedBlocks,GrowingPot,GrowingPotConcrete,GrowingPotWood,HeavyWindowBlocks,HullArmoredLargeBlocks,HullCombatFullLarge,HullCombatLargeBlocks,HullFullLarge,HullLargeBlocks,HullLargeDestroyedBlocks,HullThinLarge,LadderBlocks,PlasticLargeBlocks,StairsBlocks,StairsBlocksConcrete,StairsBlocksWood,TrussLargeBlocks,WindowArmoredLargeBlocks,WindowLargeBlocks,WindowShutterLargeBlocks,WoodBlocks,HeavyWindowDetailedBlocks,SteelRampBlocksL,HardenedRampBlocksL,CombatRampBlocksL,PassengerSeatMS,WalkwayLargeBlocks,",
- "BlockS"                = ",ArtMassBlocks,HullArmoredSmallBlocks,HullSmallBlocks,HullSmallDestroyedBlocks,ModularWingBlocks,PlasticSmallBlocks,TrussSmallBlocks,VentilatorCubeQuarter,WindowArmoredSmallBlocks,WindowShutterSmallBlocks,WindowSmallBlocks,WingBlocks,HullCombatSmallBlocks,WalkwaySmallBlocks,HeavyWindowBlocksSmall,SteelRampBlocksS,HardenedRampBlocksS,CombatRampBlocksS,",
- "Medic"                 = ",AlienParts03,AntibioticInjection,AntibioticPills,Medikit01,Medikit02,Medikit03,Medikit04,RadiationImmunityShot,RadiationPills,StomachPills,Bandages,EnergyPills,AntibioticOintment,AdrenalineShot,AntiRadiationOintment,AntiToxicOintment,AntiToxicPills,AntiParasitePills,AntiToxicInjection,AntiParasiteInjection,AntiRadiationInjection,EnergyDrink,AblativeSpray,BugSpray,Medikit05,Eden_EmergencyLifeSupport,Eden_RegenKit,Eden_StaminaRegenKit,Eden_ImmunityShield,Eden_RegenKitT2,Eden_StaminaRegenKitT2,Eden_RadiationRegenKit,Eden_Implant1,Eden_Implant2,Eden_Implant3,Eden_Implant4,Eden_Implant5,Eden_Implant6,Eden_BandagesT2,",
- "Food"                  = ",AkuaWine,AnniversaryCake,Beer,BerryJuice,Bread,Cheese,EmergencyRations,FruitJuice,FruitPie,HotBeverage,MeatBurger,Milk,Pizza,Sandwich,Steak,Stew,VegetableJuice,VeggieBurger,",
- "Ingredient"            = ",5312,AlienParts01,AlienParts02,AlienParts03,AlienThorn,AlienTooth,AloeVera,BerryJuice,Cheese,ConfettiMossScrapings,Eden_SilverIngot,Egg,ErestrumGel,Fiber,FireMossScrapings,FishMeat,Flour,Fruit,FruitJuice,Ham,HerbalLeaves,HWSFish,Meat,Milk,NCPowder,NutrientSolution,PentaxidElement,PlantProtein,PlasticMaterial,PlatinOunce,PromethiumPellets,Ratatouille,RockDust,RottenFood,Salami,Spice,TrumpetGreens,VegetableJuice,Vegetables,WaterBottle,XenoSubstrate,",
- "Sprout"                = ",AlienPalmTreeStage1,AlienPlantTube2Stage1,AlienplantWormStage1,BigFlowerStage1,BulbShroomYoungStage1,CobraLeavesPlantStage1,CoffeePlantStage1,CornStage1,DesertPlant20Stage1,DurianRoot,ElderberryStage1,InsanityPepperStage1,MushroomBellBrown01Stage1,PearthingStage1,PumpkinStage1,SnakeweedStage1,TomatoStage1,WheatStage1,",
- "Tools"                 = ",Chainsaw,ColorTool,ConcreteBlocks,ConstructorSurvival,DrillT2,Explosives,Flashlight,LightWork,LightWork02,MobileAirCon,MultiTool,MultiToolT2,OreScanner,OxygenGeneratorSmall,PlayerBike,RadarSuitT1,TextureTool,WaterGenerator,AutoMiningDeviceT1,AutoMiningDeviceT2,AutoMiningDeviceT3,Eden_AutoMiningDeviceT4,DrillEpic,TextureColorTool,NightVision,SurvivalTent,OxygenGenerator,OxygenHydrogenGenerator,Drill,SurvivalTool,DrillEpic,MedicGun,Eden_DrillVoidium,Eden_VoidiumScanner,Eden_VoidiumScannerT2,",
- "ArmorMod"              = ",ArmorBoost,ArmorBoostEpic,Eden_ArmorBoostAbyss,Eden_ArmorBoostAugmented,Eden_ColdBoostAbyss,Eden_ColdBoostAugmented,Eden_HeatBoostAbyss,Eden_HeatBoostAugmented,Eden_JetpackBoostAbyss,Eden_JetpackBoostAugmented,Eden_RadiationBoostAbyss,Eden_RadiationBoostAugmented,Eden_TransportationBoostAugmented,EVABoost,InsulationBoost,InsulationBoostEpic,JetpackBoost,JetpackBoostEpic,MobilityBoost,MobilityBoostEpic,MultiBoost,MultiBoostEpic,OxygenBoost,RadiationBoost,RadiationBoostEpic,TransportationBoost,",
- "DeviceL"               = ",AlienNPCBlocks,ArmorLocker,ATM,BlastDoorLargeBlocks,BoardingRampBlocks,CloneChamber,CockpitBlocksCV,ConstructorT0,ConstructorT1V2,ConstructorT2,ContainerAmmoControllerLarge,ContainerAmmoLarge,ContainerControllerLarge,ContainerExtensionLarge,ContainerLargeBlocks,ContainerPersonal,Core,CoreNoCPU,CPUExtenderBAT2,CPUExtenderBAT3,CPUExtenderBAT4,Deconstructor,DetectorCV,DoorArmoredBlocks,DoorBlocks,ElevatorMS,ExplosiveBlocks,ExplosiveBlocks2,Flare,FoodProcessorV2,ForcefieldEmitterBlocks,FridgeBlocks,FuelTankMSLarge,FuelTankMSLargeT2,FuelTankMSSmall,Furnace,GeneratorBA,GeneratorMS,GeneratorMST2,GravityGeneratorMS,HangarDoorBlocks,HumanNPCBlocks,LandClaimDevice,LandinggearBlocksCV,LCDScreenBlocks,LightLargeBlocks,LightPlant01,MedicalStationBlocks,OfflineProtector,OxygenStation,OxygenTankMS,OxygenTankSmallMS,PentaxidTank,Portal,RampLargeBlocks,RCSBlockMS,RCSBlockMS_T2,RemoteConnection,RepairBayBA,RepairBayBAT2,RepairBayConsole,RepairBayCVT2,RepairStation,SensorTriggerBlocks,ShieldGeneratorBA,ShieldGeneratorBAT2,ShieldGeneratorPOI,ShutterDoorLargeBlocks,SolarGenerator,SolarPanelBlocks,SolarPanelSmallBlocks,SpotlightBlocks,TeleporterBA,ThrusterMSDirectional,ThrusterMSRound2x2Blocks,ThrusterMSRound3x3Blocks,ThrusterMSRoundBlocks,VentilatorBlocks,PassengerSeatMS,CPUExtenderLargeT5,WarpDrive,RepairBayCV,TeleporterCV,ContainerHarvestControllerLarge,ShieldGeneratorCV,ShieldGeneratorCVT2,CPUExtenderCVT2,CPUExtenderCVT3,CPUExtenderCVT4,WarpDriveT2,DetectorCVT2,ShieldGeneratorT0,ShieldChargerLarge,FusionReactorLarge,ShieldCapacitorT2Large,ShieldCapacitorT3Large,ShieldChargerT2Large,ShieldChargerT3Large,Eden_LiftLargeBlocks,Eden_AuxillaryDummy,Eden_ShieldGeneratorAugmentedCV,Eden_AntimatterTank,Eden_WarpDriveAntimatter,Eden_ShieldGeneratorRegenerateCV,Eden_HydroponicsGrain,Eden_HydroponicsFruit,Eden_HydroponicsVegetables,Eden_HydroponicsNaturalStimulant,Eden_HydroponicsHerbalLeaves,Eden_HydroponicsPlantProtein,Eden_HydroponicsNaturalSweetener,Eden_HydroponicsFiber,Eden_HydroponicsMushroomBrown,Eden_HydroponicsSpice,Eden_HydroponicsBuds,Eden_HydroponicsBerries,Eden_HydroponicsPentaxid,Eden_ExplorationScannerCV,CVSmallSolarPanelBlocks,CVLargeSolarPanelBlocks,ShieldCapacitorLarge,Eden_ScienceStation,AsgardPassGen,AsgardThrusterCV,HWSLiftBlocks,AsgardExtensionLarge,AsgardExplosiveBlock,ThrusterMSRoundLarge,",
- "DeviceS"               = ",ArmorLockerSV,CloneChamberHV,ConstructorHV,ConstructorSV,Core,CPUExtenderHVT2,CPUExtenderHVT3,CPUExtenderHVT4,DetectorHVT1,DoorBlocksSV,Flare,ForcefieldEmitterBlocks,FridgeSV,FuelTankSV,FuelTankSVSmall,GeneratorSV,GeneratorSVSmall,HoverBooster,HoverEngineLarge,HoverEngineSmall,HoverEngineThruster,LightSS01,MedicStationHV,OxygenTankSV,PentaxidTankSV,RCSBlockGV,RCSBlockSV,RemoteConnection,ShieldGeneratorHV,ThrusterGVJetRound1x3x1,ThrusterGVRoundBlocks,ThrusterGVRoundLarge,ThrusterGVRoundLargeT2,ThrusterGVRoundNormalT2,ThrusterJetRound1x3x1,ThrusterJetRound2x5x2,ThrusterJetRound2x5x2V2,ThrusterJetRound3x10x3,ThrusterJetRound3x10x3V2,ThrusterJetRound3x13x3,ThrusterJetRound3x13x3V2,ThrusterJetRound3x7x3,ThrusterSVDirectional,ThrusterSVRoundBlocks,ThrusterSVRoundLarge,ThrusterSVRoundLargeT2,ThrusterSVRoundNormalT2,VentilatorBlocks,WarpDriveSV,GeneratorSVT2,ThrusterSVRoundT2Blocks,DetectorSVT2,ShieldGeneratorSVT0,ShieldGeneratorSVT2,CPUExtenderSmallT5,LargeCargoContainer,LargeHarvestContainer,ShieldCapacitorSmall,ShieldChargerSmall,FoodProcessorSmall,Eden_AuxillaryCPUSV,Eden_ShieldGeneratorAugmentedSV,Eden_WarpDriveAntimatterSV,AsgardContainerExtensionHVSV,AsgardFuelTankSVHV,AsgardExplosiveBlock,AsgardWarpDriveSV,AsgardGeneratorSVHV,ThrusterGVSuperRound2x4x2,PassengerSeatSV,PassengerSeat2SV,OxygenStationSV,ShutterDoorSmallBlocks,CockpitBlocksSV,LandinggearBlocksSV,LandinggearBlocksHeavySV,SensorTriggerBlocksSV,DetectorSVT1,ContainerControllerSmall,ContainerExtensionSmall,ContainerHarvestControllerSmall,ContainerAmmoControllerSmall,LandinggearBlocksHeavySV,RampSmallBlocks,ContainerSmallBlocks,CockpitBlocksSVT2,ShieldGeneratorSV,CPUExtenderSVT2,CPUExtenderSVT3,CPUExtenderSVT4,",
- "WeaponPlayer"          = ",AssaultRifle,AssaultRifleEpic,AssaultRifleT2,Chainsaw,ColorTool,DrillT2,Explosives,LaserPistol,LaserPistolT2,LaserRifle,LaserRifleEpic,Minigun,MinigunEpic,MultiTool,Pistol,PistolEpic,PistolT2,PulseRifle,RocketLauncher,RocketLauncherEpic,RocketLauncherT2,ScifiCannon,ScifiCannonEpic,Shotgun,Shotgun2,Shotgun2Epic,Sniper,Sniper2,Sniper2Epic,TextureTool,PulseRifleT2,SubmachineGunT1,SpecOpsRifle,SubmachineGunT2,GrenadeLauncher,TalonRepeatingCrossbow,",
- "WeaponHV"              = ",DrillAttachment,DrillAttachmentLarge,DrillAttachmentT2,SawAttachment,TurretGVArtilleryBlocks,TurretGVMinigunBlocks,TurretGVPlasmaBlocks,TurretGVRocketBlocks,TurretGVToolBlocks,WeaponSV02,TurretGVRocketBlocksT2,TurretGVArtilleryBlocksT2,WeaponSV09,WeaponSV11,",
- "WeaponSV"              = ",WeaponSV01,WeaponSV02,WeaponSV03,WeaponSV04,WeaponSV05,WeaponSV05Homing,TurretSVSmall,DrillAttachmentSVT2,TurretSVPulseLaserT2,TurretGVProjectileBlocksT2,TurretGVPlasmaBlocksT2,WeaponSV06,WeaponSV07,WeaponSV08,TurretGVBeamLaserBlocksT2, Eden_TurretVulcanSmall,Eden_ModularPulseLaserSVIR,Eden_ModularPulseLaserSVUV,Eden_ModularPulseLaserSVGamma,Eden_ShieldBoosterSV,AsgardDrillSV,",
- "WeaponCV"              = ",DrillAttachmentCV,SentryGunBlocks,TurretMSArtilleryBlocks,TurretMSLaserBlocks,TurretMSProjectileBlocks,TurretMSRocketBlocks,TurretMSToolBlocks,TurretZiraxMSLaser,TurretZiraxMSPlasma,TurretZiraxMSRocket,WeaponMS01,WeaponMS02,TurretAlien,TurretEnemyBallista,TurretMSProjectileBlocksT2,TurretMSRocketBlocksT2,TurretMSLaserBlocksT2,TurretMSArtilleryBlocksT2,WeaponMS03,TurretZiraxMSPlasmaArtillery,TurretZiraxMSLaserT2,TurretZiraxMSPlasmaT2,TurretZiraxMSRocketT2,Eden_TurretBolterCV,Eden_TurretMissileLight,Eden_TurretMissileLightT2,Eden_BlasterCV,Eden_RailgunCVSpinal_Kit,Eden_CVTorpedoRapid,Eden_TurretVulcanCV,Eden_DrillIceCV,Eden_DrillRichCV,Eden_DrillIceTurretCV,Eden_DrillTurretAutoCV,Eden_DrillTurretAutoCVT2,Eden_TurretLaserBeamCV,Eden_TurretLaserBeamCVT2,Eden_TurretLaserBeamCVT3,Eden_TurretBeamHeavyT1,Eden_TurretMissileCruiseCV,Eden_TurretMissileCruiseEMPCV,Eden_TurretMissileSwarmCV,Eden_TurretLaserT4,Eden_TurretAlienVulcan,Eden_TurretFlakClose,Eden_TurretRailgun,Eden_TurretRailgunHeavy,Eden_DrillIceTurretCVAlien,Eden_DrillTurretAutoCVAlien,AsgardDrillCV,",
- "WeaponBA"              = ",SentryGunBlocks,TurretBaseArtilleryBlocks,TurretBaseLaserBlocks,TurretBaseProjectileBlocks,TurretBaseRocketBlocks,TurretBaseProjectileBlocksT2,TurretBaseRocketBlocksT2,TurretBaseLaserBlocksT2,TurretBaseArtilleryBlocksT2,TurretBABeamLaserBlocksT2,",
- "AmmoPlayer"            = ",12.7mmBullet,5.8mmBullet,50Caliber,8.3mmBullet,DrillCharge,MultiCharge,PulseLaserChargePistol,PulseLaserChargeRifle,SciFiCannonPlasmaCharge,ShotgunShells,SlowRocket,SlowRocketHoming,",
- "AmmoHV"                = ",15mmBullet,ArtilleryRocket,FastRocket,TurretGVPlasmaCharge,",
- "AmmoSV"                = ",15mmBullet,FastRocket,FastRocketHoming,PlasmaCannonChargeSS,PulseLaserChargeSS,RailgunBullet,",
- "AmmoCV"                = ",15mmBullet,30mmBullet,5.8mmBullet,FastRocketMS,FlakRocketMS,LargeRocketMS,PulseLaserChargeMS,PulseLaserChargeMSWeapon,TurretMSPlasmaCharge,",
- "AmmoBA"                = ",15mmBullet,30mmBullet,5.8mmBullet,FastRocketBA,FlakRocket,LargeRocket,PulseLaserChargeBA,TurretBAPlasmaCharge,",
- "Gardeners"             = ",ConsoleSmallHuman,",
- "Components"            = ",AluminiumCoil,AluminiumOre,AluminiumPowder,AutoMinerCore,CapacitorComponent,Cement,CobaltAlloy,Computer,Electronics,EnergyCell,EnergyMatrix,ErestrumGel,Fiber,FluxCoil,GlassPlate,GoldIngot,HydrogenBottle,IceBlocks,LargeOptronicBridge,LargeOptronicMatrix,MagnesiumPowder,MechanicalComponents,Motor,Nanotubes,NCPowder,OpticalFiber,Oscillator,PentaxidCrystal,PentaxidElement,PentaxidOre,PlasticMaterial,PowerCoil,PromethiumOre,PromethiumPellets,RawDiamond,RockDust,SmallOptronicBridge,SmallOptronicMatrix,SteelPlate,SteelPlateArmored,WaterJug,WoodLogs,WoodPlanks,XenoSubstrate,ZascosiumAlloy,",
- "EdenComponents"        = ",AluminiumCoil,AluminiumOre,AluminiumPowder,Coolant,Eden_ComputerT2,Eden_DarkMatter,Eden_DarkMatterSmall,Eden_DiamondCut,Eden_DroneSalvageCore,Eden_DroneSalvageProcessor,Eden_Electromagnet,Eden_GaussRail,Eden_ModularPulseLaserLensLarge,Eden_ModularPulseLaserLensSmall,Eden_PlasmaCoil,Eden_PowerRegulator,Eden_ProgenitorArtifact,Eden_Semiconductor,Eden_Voidium,Fertilizer,HeatExchanger,HeliumBottle,NitrogenBottle,QuantumProcessor,RadiationShielding,ReactorCore,SolarCell,Superconductor,ThrusterComponents,XenonBottle,SmallUpgradeKit,LargeUpgradeKit,AdvancedUpgradeKit,Eden_MagmaciteIngot,Eden_MagmacitePlate,Eden_Deuterium,Eden_OreDenseT1Ingot,Eden_OreDenseT2Ingot,Eden_OreDenseT3Ingot,Eden_OreDenseT4Ingot,Eden_OreDenseT5Ingot,AncientRelics,LJArtifact1,LJArtifact2,LJArtifact3,LJArtifact4,NaqahdahOre,NaqahdahIngot,NaqahdahPlate,Naquadria,LJSandOre,LJEarthOre,Eden_MagmacitePlate,Eden_AugmentedMold,",
- "Armor"                 = ",ArmorHeavy,ArmorHeavyEpic,ArmorLight,ArmorLightEpic,ArmorMedium,ArmorMediumEpic,Eden_ArmorAbyssLight,Eden_ArmorHeavyEpicReinforced,Eden_ArmorHeavyReinforced,Eden_ArmorLightAugmented,Eden_ArmorLightReinforced,Eden_ArmorMediumReinforced,AsgardArmor,AsgardArmorDonat",
- "IngredientBasic"       = ",Meat,Spice,AlienParts01,AlienParts02,AlienParts03,Bread,Fruit,Grain,Egg,NaturalStimulant,AlienTooth,Milk,Cheese,RottenFood,HerbalLeaves,ConfettiMossScrapings,FireMossScrapings,PlantProtein,MushroomBrown,AloeVera,AlienThorn,Vegetables,Flour,Ham,Berries,Ratatouille,NaturalSweetener,FruitJuice,Buds,",
- "IngredientExtra"       = ",PromethiumPellets,RockDust,PlasticMaterial,PentaxidElement,PlatinOunce,ErestrumGel,XenoSubstrate,NutrientSolution,WaterBottle,Eden_SilverIngot,",
- "IngredientExtraMod"    = ",Medikit04,RadiationImmunityShot,Bandages,AdrenalineShot,AntiRadiationInjection,EnergyDrink,",
- "OreFurnace"            = ",IronOre,CobaltOre,SiliconOre,NeodymiumOre,CopperOre,ErestrumOre,ZascosiumOre,SathiumOre,GoldOre,TitanOre,PlatinOre,Eden_MagmaciteOre,Eden_TungstenOre,",
- "Deconstruct"           = ",Eden_IceDense,Eden_IceRich,Eden_IceHeavy,Eden_Salvage1,Eden_Salvage2,Eden_Salvage3,Eden_Salvage4,Eden_Salvage5,Eden_OreDenseT1,Eden_OreDenseT2,Eden_OreDenseT3,Eden_OreDenseT4,Eden_OreDenseT5,",
- "AmmoAllEnergy"         = ",DrillCharge,PulseLaserChargePistol,PulseLaserChargeRifle,MultiCharge,SciFiCannonPlasmaCharge,PlasmaCannonAlienCharge,PlasmaCannonChargeSS,PulseLaserChargeSS,PulseLaserChargeMS,TurretGVPlasmaCharge,TurretMSPlasmaCharge,TurretEnemyLaserCharge,PulseLaserChargeBA,TurretBAPlasmaCharge,PulseLaserChargeMSWeapon,PlasmaCartridge,PulseLaserChargeMST2,TurretMSPlasmaChargeT2,PulseLaserChargeBAT2,TurretBAPlasmaChargeT2,TurretGVPlasmaChargeT2,PulseLaserChargeSST2,ZiraxMSPlasmaCharge,HeatSinkSmall,HeatSinkLarge,AsgardPlazmerAmmo,LJDrillChargeEpic,Eden_ModularPulseLaserSVIR_Ammo,Eden_ModularPulseLaserSVUV_Ammo,Eden_ModularPulseLaserSVGamma_Ammo,Eden_PlasmaChargeEntropic,Eden_PlasmaRifleXCorp_Ammo,Eden_BlasterCV_Ammo,Eden_ShieldBoosterSV_Ammo,Eden_PlasmaRifleRoyal_Ammo,",
- "AmmoAllProjectile"     = ",50Caliber,8.3mmBullet,5.8mmBullet,12.7mmBullet,15mmBullet,ShotgunShells,FlameThrowerCanister,RailgunBullet,30mmBullet,FlamethrowerTank,CrossbowBoltPlayer,40mmBullet,20mmBullet,Eden_TurretRailgun_Ammo,Eden_TurretRailgunHeavy_Ammo,Eden_VulcanAmmo,Eden_TurretBolterBA_Ammo,Eden_TurretBolterCV_Ammo,Eden_TurretVulcanCV_Ammo,",
- "AmmoAllRocket"         = ",SlowRocket,SlowRocketHoming,FastRocket,LargeRocket,FastRocketMS,FlakRocket,ArtilleryRocket,FastRocketHoming,FlakRocketMS,LargeRocketMS,FastRocketBA,TurretEnemyRocketAmmo,FastRocketGV,SVBomb,LightRocketCV,HeavyRocketMS,FlakRocketMST2,ArtilleryShellCVT2,FlakRocketBAT2,ArtilleryShellBAT2,SwarmRocketHV,ArtilleryShellHVT2,HeavyRocketBA,TorpedoSV,Eden_TurretFlakClose_Ammo,Eden_TurretRocketRapid_Ammo,Eden_TurretMissileLight_Ammo,Eden_TurretMissileLightT2_Ammo,Eden_TurretMissileCruiseCV_Ammo,Eden_TurretMissileCruiseEMPCV_Ammo,Eden_TurretMissileSwarmCV_Ammo,Eden_CVTorpedoRapid_Ammo,",
- "WeaponPlayerUpgrades"  = ",PistolKit,RifleKit,SniperKit,ShotgunKit,HeavyWeaponKit,LaserKit,",
- "WeaponPlayerEpic"      = ",PulseRifleEpic,PlasmaCannonAlien,MinigunT2,FlameThrowerT2,AsgardPlazmer,Eden_PlasmaRifleEntropic,Eden_MinigunIncendiary,Eden_LaserRifleEntropic,Eden_ShotgunGauss,Eden_ShotgunDouble,Eden_ScoutRifle,Eden_Uzi,Eden_LightRailgunRifle,Eden_IonRifle,Eden_FarrPlasmaCrossbow,Eden_RifleLightning,Eden_PlasmaRifleXCorp,Eden_PlasmaRifleRoyal,AssaultRifleT3,TalonCrossbowPlayer,HeavyPistol,SubmachineGunT3,LaserPistolT3,ZiraxBeamRifle,AsgardPlazmer,",
- "Deco"                  = ",TurretRadar,AntennaBlocks,DecoBlocks,ConsoleBlocks,IndoorPlants,DecoBlocks2,DecoStoneBlocks,ChristmasTree,DecoVesselBlocks,DecoTribalBlocks,PosterARest,PosterBiker,PosterDontHide,PosterForeignWorld,PosterJump,PosterNewWorld,PosterSoleDesert,PosterStranger,PosterSurvivor,PosterTakingABreak,PosterTalon,PosterTrader,PosterZiraxAlienWorld,",
- "DataPads"              = ",Eden_UnlockPoint,Eden_WarpUpgrade,Eden_DataChipT1,Eden_DataChipT2,Eden_DataChipT3,",
- "Oxygen"                = ",OxygenBottleLarge,",
- "Fuel"                  = ",EnergyCell,EnergyCellLarge,FusionCell,",
- "Pentaxid"              = ",PentaxidCrystal,"
 
Für das deconstruct script zu löschende Blöcke:
- "RemoveBlocks"          = ",ContainerUltraRare,AlienContainer,AlienContainerRare,AlienContainerVeryRare,AlienContainerUltraRare,AlienDeviceBlocks,Eden_AlienBlocksPOI,Eden_CoreNPCSpecial,Eden_CoreNPCFake,"

Die Listen beginnen und enden mit einem Komma so das sie einfach mit dem Befehl `concat` kombiniert werden können.
```
(concat @root.Ids.WeaponHV @root.Ids.WeaponSV @root.Ids.WeaponCV)
oder
(concat '1234,5568' @root.Ids.ArmorMod)
```


-----------------------------------------------------
## Welcher Spieler ist auf der Basis/Schiff gerade aktiv

Eingabe im LCD (alles ohne "")
```
Targets:"NAME DES ANZUZEIGENDEN LCD"Eingabe im LCD (alles ohne "")
"TEXT optional"
{{#each P.Players}}
 "-" {{Name}}
{{/each}}
```
Bsp.
```
Targets:LCD Info W1
Player:
{{#each P.Players}}
 - {{Name}}
{{/each}}
```
------------------------------------------------------
## Datum und Uhrzeit anzeigen lassen

Eingabe im LCD (alles ohne "")
```
Targets:"NAME DES ANZUZEIGENDEN LCD"Eingabe im LCD (alles ohne "")
"TEXT optional"
{{datetime}}

{{datetime 'HH:mm'}}

{{datetime 'dd MMM HH:mm:ss' '+7'}}
```
Bsp.
```
Targets:LCD UHRZEIT
Wie spät ist es?
{{datetime}}

{{datetime 'HH:mm'}}

{{datetime 'dd MMM HH:mm:ss' '+7'}}
```
----------------------------------------------------
## SCROLLEN:
Wenn zu viele Produkte nicht angzeigt werden können, dann kann man auch Scrollen
Hier werden 5 Produkte angezeigt mit 2 Sekunden Scrollgeschwindigkeit, wenn mehr als 5 Items zur Verfügung stehen. 
```
{{#scroll 5 2}}
{{#items E.S '"Name der Kiste"'}}
{{Count}} {{i18n Key}}
{{/items}}
```
Bsp.
```
{{#scroll 5 2}}
{{#items E.S 'Kühlschrank 1'}}
{{Count}} {{i18n Key}}
{{/items}}

{{#scroll 10 1}}
{{#each E.S.Items}}
 - [{{Id}}]:{{Name}}
{{/each}}
{{/scroll}}
```
----------------------------------------------------
## Intervalle:
Es kann alles in Intervallen angezeigt werden. Hier im Beispiel wäre es ein Pfeil
Man kann auch den Inhalt von 2 Kisten anzeigen lassen
```
{{#intervall 1}}
= = = = = = = = = = = = = = = = >
{{else}}
 = = = = = = = = = = = = = = = =>
{{/intervall}}
```
oder hier sind sind 2 Kisten die abwechselnd angezeigt werden.
```
{{#intervall 2}}
"Text optional"
{{#items E.S '"Name der Kiste"'}}
{{Count}} {{i18n Key}}
{{/items}}
{{else}}
"Text optional"
{{#items E.S '"Name der Kiste2"'}}
{{Count}} {{i18n Key}}
{{/items}}
{{/intervall}}
```
Bsp.
```
{{#intervall 2}}

Kühlschrank 1:

{{#items E.S 'Kühlschrank 1'}}
{{Count}} {{i18n Key}}
{{/items}}
{{else}}

Kühlschrank 2:

{{#items E.S 'Kühlschrank 2'}}
{{Count}} {{i18n Key}}
{{/items}}
{{/intervall}}
```
----------------------------------------------------
## Farbe Schrift und Hintergrund, Schriftgrösse und Intervall
Im folgendem Beispiel wechselt alle 5 Sekunden das Wort "Hallo" und "Welt"
dann wechselt auch alle 5 Sekunden die Schriftgrösse
Es wechselt jede Sekunde die Schriftfarbe und jede Sekunde der Hintergrund
```
{{#intervall 5}}
Hallo
{{else}}
Welt
{{/intervall}}

{{#intervall 5}}
{{fontsize 8}}
{{else}}
{{fontsize 15}}
{{/intervall}}

{{#intervall 1}}
{{color 'ff0000'}}
{{else}}
{{color '00ff00'}}
{{/intervall}}

{{#intervall 1}}
{{bgcolor 'ffff00'}}
{{else}}
{{bgcolor '000000'}}
{{/intervall}}
```
----------------------------------------------------
## ORE and INGOT IDENTIFICATION NUMBER:

@root.Ids.Ore

+ Item Id: 4296, Name: Magnesium Ore
+ Item Id: 4297, Name: Iron Ore
+ Item Id: 4298, Name: Cobalt Ore
+ Item Id: 4299, Name: Silicon Ore
+ Item Id: 4300, Name: Neodymium Ore
+ Item Id: 4301, Name: Copper Ore
+ Item Id: 4302, Name: Promethium
+ Item Id: 4317, Name: Erestrum Ore
+ Item Id: 4318, Name: Zascosium Ore
+ Item Id: 4332, Name: Sathium Ore
+ Item Id: 4341, Name: Pentaxid Ore
+ Item Id: 4345, Name: Gold Ore
+ Item Id: 4359, Name: Titanium Ore

---
@root.Ids.Ingots

+ Item Id: 4319, Name: Magnesium Powder
+ Item Id: 4320, Name: Iron Ingot
+ Item Id: 4321, Name: Cobalt Ingot
+ Item Id: 4322, Name: Silicon Ingot
+ Item Id: 4323, Name: Neodymium Ingot
+ Item Id: 4324, Name: Copper Ingot
+ Item Id: 4325, Name: Promethium Pellets
+ Item Id: 4326, Name: Erestrum Ingot
+ Item Id: 4327, Name: Zascosium Ingot
+ Item Id: 4328, Name: Stone
+ Item Id: 4329, Name: Rock Dust
+ Item Id: 4333, Name: Sathium Ingot
+ Item Id: 4342, Name: Pentaxid Crystals
+ Item Id: 4346, Name: Gold Ingot
+ Item Id: 4360, Name: Titanium Rods

---
# Technical
Syntaxdocu:
+ http://handlebarsjs.com/
+ http://handlebarsjs.com/reference.html#data
+ https://zordius.github.io/HandlebarsCookbook/index.html
+ https://zordius.github.io/HandlebarsCookbook/0014-path.html
+ https://github.com/rexm/Handlebars.Net

## Items
Items have the following basic data

* Id : Complete one-to-one number. For tokens it is a combination of 'TokenId * 100000 + ItemId'.
* IsToken: 'true' if it is a token otherwise 'false'.
* ItemId: The token-independent part of the Id (this is equivalent to the token item in Empyrion).
* TokenId: The Id of the token if it is a token.

## Conditions
* {{#test Select Op Value}}
  * Op: eq is =
  * Op: neq is <> or !=
  * Op: leq is <=
  * Op: le is <
  * Op: geq is >=
  * Op: ge is >
  * Op: in (Delimitters are: ,;#+ )
    * Value: '1,2,3,42'
    * Value: '1-3,42'
    * Value: 'A,xyz,mag'

* {{regex value regex}}
    * Checks the value 'value' with the regular expression 'regex' on success/failure the result of the expression is passed to the next section as '.'.

## Items
+ {{configid name}}
   + Reads the attribute 'id' from the configuration of the block/item 'name'

+ {{configattr id attrname}}
   + Reads the attribute 'attrname' from the configuration of the block/item 'id'

+ {{configattrbyname name attrname}}
   + Reads the attribute 'attrname' from the configuration of the block/item 'name'
   
+ {{configbyid id}}
  + Reads the config section block/item for 'id'

+ {{configbyname name}}
  + Reads the config section block/item for 'name'

+ {{resourcesforid id}}
   + List of the required resources for the block / item with the 'id'

### Logiccheck
* {{#ok data}}
   * Execute block if (data) has a value (not equal to '') or (data) is equal to 'true' or not equal to 0
   * otherwise the {{else}} part is executed

* {{#if data}}
   * Execute block if (data) has a value not equal to '' or 0 (data) or is equal to 'true'
   * otherwise the {{else}} part is executed

* {{not data}}
   * Negation of (data)
   
### (intervall)
* {{#intervall sec}}
  * intervall in seconds

### (scroll)
* {{#scroll lines delay \[step\]}}
  * Text scroll block with (lines) od text, scrolls with (delay) seconds
  * optional (step) lines per step

+ {{#getitems structure 'box1; box2; fridge *; ...'}} = Determine all items from the containers (names) = 'box1; box2; fridge *; ...' and deliver them as a list e.g. for itemlist

* {{#itemlist list 'id1;id2;id3'}}
  * Itemlist the the selected items (ids) even if they don't in the list (list)

* {{#itemlistarray list 'id1;id2;id3,...'}}
  * Filter the list of items (list) to the items with the IDs 'id1;id2;id3,...'. 
    If an id is not available, it is inserted with a number 0.
    Returns the result as a list which can be further processed using foreach or other list functions

+ {{#orderbylist list 'id1;id2;id3,...'}}
  + sort list of items based on the idlist. Items that are not in the list are sorted to the end

### (i18n)
* {{#i18n Select \['Language'\]}}
  * Language: English,Deutsch,Français,Italiano,Spanish,...
    look at \[ESG\]\\Content\\Extras\\Localization.csv at the first line
    from default it's get from the CultureInfo lcd.

* {{#toid Name[;Name2;...]}}
  * Converted the symbolic names to the current ids

* {{#toname id[;id2;...]}}
  * Converted the ids to the symbolic names

### (datetime)
+ {{datetime}} = Display the Datetime
+ {{datetime 'format'}} = uses the formatstring
+ {{datetime 'format' '+5'}} = adds N hours

DateTime format:
+ https://docs.microsoft.com/en-us/dotnet/api/system.datetime.tostring?view=netframework-4.8#System_DateTime_ToString_System_String_

### (move)
+ {{move item structure names \[maxLimit\]}}
  + Item (item) into the structure (structure) in the container with the names (names) move
  + \[maxLimit] is an optional parameter which one is limited the amount in the target container

+ {{fill item structure tank \[max\]}}
  + Fills up the tank = fuel/oxygen/pentaxide with the item in the structure. The percentage fill level can be optionally limited with (max). Default is 100.

+ {{deconstruct entity container \[CorePrefix\] \[RemoveItemsIds1,Id2,...\]}}
   + Deconstruct the entity 'entity' and moves parts to container named as 'container''
   + Note: The core of the structure must be called 'Core-Destruct-ID' (where ID stands for the id of the structure)
   + With the configuration setting DeconstructBlockSubstitution a replacement (by another block type) / deletion (by 0) of block types can be defined
   + The costs per 'AmountPerNumberOfBlocks' can be configured via 'DeconstructSalary' in the configuration file. Default: 100 money cards are due per 10 blocks

+ {{recycle entity container \[CorePrefix\]}}
   + Dismantles the 'entity' structure and transports the raw materials (of the known recipes) into the container with the name given by 'container'
   + Note: The core of the structure must be called 'Core-Recycle-ID' (where ID stands for the ID of the structure)
   + With the configuration setting DeconstructBlockSubstitution, a replacement (by another block type) / deletion (by 0) of block types can be defined
   + The costs per 'AmountPerNumberOfBlocks' can be configured via 'RecycleSalary' in the configuration file. Standard: 200 money cards are due per 10 blocks    

+ {{harvest structure block's target gx gy gz \[removeDeadPlants\]}}
  + The command can be used to mine plants. This requires a "gardener" (NPC crew) and money (as payment) in the refrigerator. 
    If desired, the dead plants can also be disposed of. However, this costs 100 times the price

+ {{pickupplants structure block's target gx gy gz \[removeDeadPlants\]}}
  + With this command, plants can be removed. This requires a "gardener" (NPC crew) and money (as payment) in the fridge. 
    If desired, the dead plants can also be disposed of. However, this costs 100 times the price

+ {{replantplants structure target}}
  + With this command, the plants removed with 'pickupplants' can be replanted. 
    Note: This is only possible if the playfield has not been changed or a logout has not taken place.

## Positions (structure and world)
+ {{vector x y z}}
  + Creates a vector from x y z

+ {{structtoglobalpos structure (vector | x y z)}}
  + Returns the world coorinates of structurepos structure (vector | x y z)

+ {{globaltostructpos structure (vector | x y z)}}
  + Returns the position (vector | x y z) of the world coordinates from the point of view of the structure

## Chat
+ {{chatbysignal SignalName sender}}message{{/chatbysignal}}
  + Sends the chat message to the player who triggers the signal. In the block, the player is available with Id and name

+ {{char sender text}}
  + Sends a chat message to the owner of the structure (faction or player) with the text 'text' and the sender 'sender'

## Chat (Admin only)
+ {{chatglobal sender text}}
  + Sends a global chat message with the text 'text' and the sender 'sender'

+ {{chatserver sender text}}
  + Sends a server chat message with the text 'text' and the sender 'sender'

+ {chatplayer playerId sender text}}
  + Sends a private chat message to the player 'playerId' with the text 'text' and the sender 'sender'

+ {{chatfaction factionId sender text}}
  + Sends a faction chat message to the faction 'facrionId' with the text 'text' and the sender 'sender'

## Teleport
+ {{teleportplayer player (device | toPos | x y z)}}
 + Teleports the player to the device/block of the structure.
 + 'toPos' and 'x y z' are only allowed in elevated scripts

## JSON
+ {{jsontodictionary string}}
  + returns a dictionary data structure resulting from the JSON 'string'.

+ {{fromjson string}}
  + returns a data structure resulting from the JSON 'string'.

{{tojson object}}
  + creates a JSON data string from the object

## Flying
Note: Flying only works when there is no pilot controlling the ship and the engines are switched on.

+ {{movestop}}
    + Stop flight
+ {{moveto vector3 | x y z}}
    + Set flight direction
+ {{moveforward speed}}
    + Set flight speed

## Dialog windows
+ {{dialog player|Id|SignalName title body}} 
  + Displays a dialog to the player (at SignalName when one of these signals is triggered): [player | playerId | SignalName] [title] [body] [ButtonTexts] [ButtonIdxForEnter] [ButtonIdxForEsc] [MaxChars] [InitialPlayerInput] [closeOnLinkClick] [DialogData] [Placeholder]

+ {{dialogbox player|Id|SignalName}} 
  + Displays a dialog to the player (at SignalName when one of these signals is triggered): (player | playerId | SignalName) [ButtonIdxForEnter] [ButtonIdxForEsc] [MaxChars] [InitialPlayerInput] [closeOnLinkClick] [DialogData] [Placeholder]
  + (title) (body) (ButtonTexts) are determined from the {{else}} where the player data is available as this 1st line=title, last line=buttons, the lines in between = body

## DB Access
+ {{db queryname [top] [orderBy] [additionalWhereAnd] [parameters]}}
  + 'queryname' the name of the SQL query which is provided by the configuration.
  + 'top' provide only the first X entries
  + 'orderby' query with the field sort. By default ascending 'fieldname asc' but can also be set to descending 'fieldname desc'.
  + 'additionalWhereAnd' additional 'where' condition
  + 'parameter' if the script requires additional parameters their values can be specified here the parameters are then available under @1..N of the query

To search the 'global.db' database of the EmpyrionSavegame and to add own queries to the configuration you can use the SQLiteBrowser https://sqlitebrowser.org/.

By default the following parameters are available for the query:
 + @PlayerId = @root.E.S.Pilot.Id   
 + @FactionId = @root.E.Faction.Id   
 + @FactionGroup (int) = @root.E.Faction.Group
 + @EntityId = @root.E.Id           

The following queries are available:

### Entities
```
SELECT * FROM Structures 
JOIN Entities ON Structures.entityid = Entities.entityid
JOIN Playfields ON Entities.pfid = Playfields.pfid
JOIN SolarSystems ON SolarSystems.ssid = Playfields.ssid
WHERE (isremoved = 0 AND (facgroup = 0 OR facgroup = 1) AND (facid = @PlayerId OR facid = @FactionId)) {additionalWhereAnd}
```

### DiscoveredPOIs
```
SELECT * FROM DiscoveredPOIs
JOIN Entities ON DiscoveredPOIs.poiid = Entities.entityid
JOIN Playfields ON Entities.pfid = Playfields.pfid
JOIN SolarSystems ON SolarSystems.ssid = Playfields.ssid
WHERE (Entities.isremoved = 0 AND (DiscoveredPOIs.facgroup = 0 OR DiscoveredPOIs.facgroup = 1) AND (DiscoveredPOIs.facid = @PlayerId OR DiscoveredPOIs.facid = @FactionId)) {additionalWhereAnd}
```

### TerrainPlaceables
```
SELECT * FROM TerrainPlaceables 
JOIN Entities ON TerrainPlaceables.entityid = Entities.entityid
JOIN Playfields ON TerrainPlaceables.pfid = Playfields.pfid
JOIN SolarSystems ON SolarSystems.ssid = Playfields.ssid
WHERE (isremoved = 0 AND (facgroup = 0 OR facgroup = 1) AND (facid = @PlayerId OR facid = @FactionId OR TerrainPlaceables.entityid = @PlayerId OR TerrainPlaceables.entityid = @FactionId)) {additionalWhereAnd}
```

### TerrainPlaceablesRes
```
SELECT * FROM TerrainPlaceables
JOIN PlayfieldResources ON (PlayfieldResources.pfid = TerrainPlaceables.pfid AND abs(PlayfieldResources.blockx - TerrainPlaceables.blockx) <= 20 AND abs(TerrainPlaceables.blockz - PlayfieldResources.blockz) <= 20)
JOIN Entities ON TerrainPlaceables.entityid = Entities.entityid
JOIN Playfields ON TerrainPlaceables.pfid = Playfields.pfid
JOIN SolarSystems ON SolarSystems.ssid = Playfields.ssid
WHERE (isremoved = 0 AND (facgroup = 0 OR facgroup = 1) AND (facid = @PlayerId OR facid = @FactionId OR TerrainPlaceables.entityid = @PlayerId OR TerrainPlaceables.entityid = @FactionId)) {additionalWhereAnd}
```


### Playfields
```
SELECT * FROM Playfields
LEFT JOIN DiscoveredPlayfields ON DiscoveredPlayfields.pfid = playfields.pfid
JOIN SolarSystems ON SolarSystems.ssid = Playfields.ssid
WHERE playfields.ssid IN (
SELECT ssid FROM Playfields
LEFT JOIN DiscoveredPlayfields ON DiscoveredPlayfields.pfid = playfields.pfid
WHERE (DiscoveredPlayfields.facgroup = 0 OR DiscoveredPlayfields.facgroup = 1) AND (DiscoveredPlayfields.facid = @PlayerId OR DiscoveredPlayfields.facid = @FactionId)
GROUP BY playfields.ssid
) {additionalWhereAnd}
```

### PlayfieldResources
```
SELECT * FROM Playfields
LEFT JOIN DiscoveredPlayfields ON DiscoveredPlayfields.pfid = playfields.pfid
JOIN SolarSystems ON SolarSystems.ssid = Playfields.ssid
JOIN PlayfieldResources ON PlayfieldResources.pfid = Playfields.pfid
WHERE playfields.ssid IN (
SELECT ssid FROM Playfields
LEFT JOIN DiscoveredPlayfields ON DiscoveredPlayfields.pfid = playfields.pfid
WHERE (DiscoveredPlayfields.facgroup = 0 OR DiscoveredPlayfields.facgroup = 1) AND (DiscoveredPlayfields.facid = @PlayerId OR DiscoveredPlayfields.facid = @FactionId)
GROUP BY playfields.ssid
) {additionalWhereAnd}
```

### PlayerData
```
SELECT * FROM PlayerData 
JOIN Entities ON Entities.entityid = PlayerData.entityid
JOIN Playfields ON Playfields.pfid = PlayerData.pfid
JOIN SolarSystems ON SolarSystems.ssid = Playfields.ssid
WHERE ((Entities.facgroup = 0 OR Entities.facgroup = 1 OR facgroup = 0 OR facgroup = 1) AND (Entities.facid = @PlayerId OR facid = @PlayerId OR Entities.facid = @FactionId OR facid = @FactionId)) {additionalWhereAnd}
```

### Bookmarks
```
SELECT * FROM Bookmarks
JOIN Entities ON Bookmarks.entityid = Entities.entityid
JOIN Playfields ON Bookmarks.pfid = Playfields.pfid
JOIN SolarSystems ON SolarSystems.ssid = Playfields.ssid
WHERE ((Bookmarks.facgroup = 0 OR Bookmarks.facgroup = 1) AND (Bookmarks.facid = @PlayerId OR Bookmarks.facid = @FactionId)) {additionalWhereAnd}
```

## External data
+ {{external 'Key' [args]}}
  + 'Key' Key for accessing the external data.
  + [args] Additional parameters needed by the external data access method.
    
Here with it is possible to retrieve external data from the AddOnAssemblys. Which parameters are additionally required must be taken from the documentation 
of the respective external data source.

### Providing a DLL for the external data (with an example from the EmpyrionGalaxyNavtigator).
This Dll must have a class which implements the IMod interface. Also, the must implement a property 'ScriptExternalDataAccess'.
```
public class ExternalDataAccess : IMod
{
    public IDictionary<string, Func<IEntity, object[], object>> ScriptExternalDataAccess { get; }

    public ExternalDataAccess()
    {
        ScriptExternalDataAccess = new Dictionary<string, Func<IEntity, object[], object>>()
        {
            ["Navigation"] = (entity, args) => entity?.Structure?.Pilot?.Id > 0 ?         Navigation(entity) : null,
            ["MaxWarp" ]   = (entity, args) => entity?.Structure?.Pilot?.Id > 0 ? (object)MaxWarp   (entity) : null,
        };
    }
```

Furthermore the path to the DLL must be specified in the configuration file of the EmpyrionScripting. (Basis is the mod directory of the EmpyrionScripting in the savegame).
```
"AddOnAssemblies": [
    "..\\EmpyrionGalaxyNavigator\\EmpyrionGalaxyNavigatorDataAccess.dll"
],
```

And the DLL must be copied to its intended place (in the case of EmyprionGalaxyNavigator in its mod directory in the savegame)
![](Screenshots/AddOnAssembly.png)
The DLL can be found in this file 'EmpyrionGalaxyNavigatorDataAccess.zip' which is included in the ModLoaderpaket or can be downloaded from https://github.com/GitHub-TC/EmpyrionGalaxyNavigator/releases. 

When the functions are called, the current entity and the passed 'args' are also passed.
(in this case no further parameters are needed to access the GalaxyNavigator).
```
{{#external 'MaxWarp'}}
Maximum jump range: {{.}}
{{/external}}
```

## Elevated scripts (Savegame or Adm structures)
+ {{lockdevice structure device | x y z}}
  + Locks a device

+ {{additems container item id count}}
  + Add (itemid) (count) times to the container (this should be locked)

+ {{removeitems container itemid maxcount}}
  + Removes (itemid) (count) from the container (it should be locked)

+ {{replaceblocks entity RemoveItemsIds1,Id2,... ReplaceId}}
  + Replace the block with the id 'RemoveItemsIds1,Id2,...' to 'ReplaceId'
  + Replace = 0 remove the block

## SaveGame scripts
This special form of scripts can be stored in the SaveGame. The basic path for this is the
\[Savegame\]\\Mods\\EmpyrionScripting\\Scripts

This path is avaiable with @root.MainScriptPath

in this directory script files with the extension *.hbs are searched for according to the following pattern
* EntityType
* EntityName
* PlayfieldName
* PlayfieldName\\EntityType
* PlayfieldName\\EntityName
* EntityId
* in the directory itself

Note: EntityType is BA,CV,SV or HV

### CustomHelpers-SaveGameScripts (readfile)
+ {{readfile dir filename}}
   + (dir)\\(filename) file content is supplied as a LineArray
   + If the file does not exist, the {{else}} part will be executed

### CustomHelpers-SaveGameScripts (writefile)
+ {{writefile dir filename}}
   + (dir)\\(filename) Content of the block is written to the file

### CustomHelpers-SaveGameScripts (fileexists)
+ {{fileexists dir filename}}
  + If exists then templane oterwirse exec else part

## Prioritization of scripts
If many scripts are built in it makes sense to downgrade the ones that don't need to be executed so often, 
so that the other scripts are executed more often.
To do this, you can write a number from 0-9 BEFORE the name of a script, so that this script is executed only every N cycles.
e.g.
1Script:abc
3Script:uvw
4Script:xyz

1: abc
2: abc
3: abc, uvw
4: abc, xyz
5: abc
6: abc, uvw
7: abc
8: abc, xyz
...

The difference between a script WITHOUT a number or 0 and a script with a '1' is that scripts with a priority
with a priority >= 1 will run regardless if the ScriptLCD is switched off.

Scripts with 0 or no number will only be executed if the LCD is on. 
(Note: you can set the font color (for projectors) to transparent to make it "invisible")

## Automatic Amount Adjustments
For Fuel, O2,... it can be useful to use the current values from the Ecf configuration (of the scenario)
The “EcfAmountTag” entry can be specified for this purpose. The value for “Amount” is then automatically determined from the item with the value of the tag.
If the value is not to be determined automatically, “EcfAmountTag:”” can be specified.

Examples of automatic determination
```
{
    “ItemName": ‘EnergyCellLarge’,
    “Amount": 250,
    “EcfAmountTag": ”FuelValue”
}
...
{
  “ItemName": ‘OxygenBottleLarge’,
  “Amount": 250,
  “EcfAmountTag": ”O2Value”
}
```

no automatic determination
```
{
    “ItemName": ‘EnergyCellLarge’,
    “Amount": 250,
    “EcfAmountTag": ””
}
```


## Allgemeines
Wenn die Struktur ausgeschaltet ist oder keinen Strom hat werden keine InGameScripte von ihr ausgeführt.
Somit verbrauchen "alte" oder nicht mehr benutze Strukturen auch keine Leistung der Scriptengine.

## Ausführung und Leistung der Scripts einstellen
+ "InGameScriptsIntervallMS": 2000,
  In welchen Intervallen wird die Scriptausführung im Hintergrund gestartet
+ "DeviceLockOnlyAllowedEveryXCycles": 10,
  Scripte welche ein "DeviceLock" benötigen werden nur alle X Zyklen ausgeführt
+ "SaveGameScriptsIntervallMS": 10000,
  In welchen Intervallen wird die Scriptausführung der Savegamescripte im Hintergrund gestartet
+ "UseEveryNthGameUpdateCycle": 10,
  Nur jeder N te GameUpdate Aufruf wird für die Scriptausführung benutzt
+ "ScriptsSyncExecution": 2,
  Wie viele Scripte werden pro Zyklus im GameUpdate ausgeführt
+ "ScriptsParallelExecution": 4,
  Wie viele Scripte werden pro Zyklus in der Hintergrundverarbeitung ausgeführt


## Hint: ...\Saves\Games\[Savegamename]\Mods\EmpyrionScripting\Configuration.json
```
"SaveGameScriptsIntervallMS": 10000,
"ScriptsSyncExecution": 2,
"ScriptsParallelExecution": 4,
...
"ProcessMaxBlocksPerCycle": 200,
```
Are set VERY conservatively by default so as not to put the server (and the game) under load.
Double the values (SaveGameScriptsIntervallMS must lower) and the scripts will run more "smoothly", but there may be micro-jerks in the game.

### Whats next?


ASTIC/TC
