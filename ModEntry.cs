using HarmonyLib;
using Microsoft.Extensions.Logging;
using Nanoray.PluginManager;
using Nickel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sorwest.MikuEnemy;

public sealed class ModEntry : SimpleMod
{
    public static string Name => "Sorwest.MikuEnemy";
    internal static ModEntry Instance { get; private set; } = null!;
    internal Harmony Harmony { get; }
    internal ILocalizationProvider<IReadOnlyList<string>> AnyLocalizations { get; }
    internal ILocaleBoundNonNullLocalizationProvider<IReadOnlyList<string>> Localizations { get; }

    internal IPartEntry MikuWing {  get; }
    internal IPartEntry MikuCannon { get; }
    internal IPartEntry MikuMissiles { get; }
    internal IPartEntry MikuCockpit { get; }
    internal IPartEntry MikuEmpty { get; }
    internal ISpriteEntry MikuChassis { get; }
    internal IDeckEntry MikuDeck { get; }
    internal ISpriteEntry MikuNeutral { get; }
    public ModEntry(IPluginPackage<IModManifest> package, IModHelper helper, ILogger logger) : base(package, helper, logger)
    {
        Instance = this;
        Harmony = new(package.Manifest.UniqueName);

        this.AnyLocalizations = new JsonLocalizationProvider(
            tokenExtractor: new SimpleLocalizationTokenExtractor(),
            localeStreamFunction: locale => package.PackageRoot.GetRelativeFile($"i18n/{locale}.json").OpenRead()
        );
        this.Localizations = new MissingPlaceholderLocalizationProvider<IReadOnlyList<string>>(
            new CurrentLocaleOrEnglishLocalizationProvider<IReadOnlyList<string>>(this.AnyLocalizations)
        );

        MikuNeutral = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikuportrait.png"));

        MikuWing = Helper.Content.Ships.RegisterPart("MikuPart.Wing", new PartConfiguration()
        {
            Sprite = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikuwing.png")).Sprite
        });
        MikuCannon = Helper.Content.Ships.RegisterPart("MikuPart.Cannon", new PartConfiguration()
        {
            Sprite = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikucannon.png")).Sprite
        });
        MikuMissiles = Helper.Content.Ships.RegisterPart("MikuPart.Missiles", new PartConfiguration()
        {
            Sprite = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikumissiles.png")).Sprite
        });
        MikuCockpit = Helper.Content.Ships.RegisterPart("MikuPart.Cockpit", new PartConfiguration()
        {
            Sprite = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikucockpit.png")).Sprite
        });
        MikuEmpty = Helper.Content.Ships.RegisterPart("MikuPart.Scaffolding", new PartConfiguration()
        {
            Sprite = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikuempty.png")).Sprite
        });
        MikuChassis = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikuchassis.png"));

        MikuDeck = Helper.Content.Decks.RegisterDeck("miku", new()
        {
            Definition = new() { color = Colors.enemyName, titleColor = new Color("000000") },
            DefaultCardArt = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikubg.png")).Sprite,
            BorderSprite = Helper.Content.Sprites.RegisterSprite(package.PackageRoot.GetRelativeFile("assets/mikucard.png")).Sprite,
            Name = AnyLocalizations.Bind(["character", "miku", "name"]).Localize
        });
        _ = new ModdedEnemyManager();
    }
}
