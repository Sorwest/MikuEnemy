using System;
using System.Collections.Generic;

namespace Sorwest.MikuEnemy;

[AIMeta(homeZone = typeof(MapFirst))]
public class MikuAI : AI
{
    public int aiCounter;
    public override string Name() => ModEntry.Instance.Localizations.Localize(["character", "miku", "name"]);
    public override void OnCombatStart(State s, Combat c)
    {
        c.bg = new BGCrystalNebula();
    }
    public override string GetLocName()
    {
        return ModEntry.Instance.Localizations.Localize(["character", "miku", "name"]);
    }
    public override Ship BuildShipForSelf(State s)
    {
        character = new()
        {
            type = "miku",
            deckType = ModEntry.Instance.MikuDeck.Deck
        };
        int hp = 10;
        int shield = 10;
        bool hard = s.GetHarderElites();
        return new Ship()
        {
            x = 4,
            hull = hp,
            hullMax = hp,
            shieldMaxBase = hard ? 8 : shield,
            chassisUnder = ModEntry.Instance.MikuChassis.UniqueName,
            ai = this,
            parts = new List<Part>()
            {
                new Part()
                {
                    type = PType.wing,
                    skin = ModEntry.Instance.MikuWing.UniqueName,
                    damageModifier = hard ? PDamMod.armor : PDamMod.none
                },
                new Part()
                {
                    type = PType.cannon,
                    skin = ModEntry.Instance.MikuCannon.UniqueName
                },
                new Part()
                {
                    type = PType.empty,
                    skin = ModEntry.Instance.MikuEmpty.UniqueName
                },
                new Part()
                {
                    type = PType.cannon,
                    skin = ModEntry.Instance.MikuCannon.UniqueName
                },
                new Part()
                {
                    type = PType.cannon,
                    skin = ModEntry.Instance.MikuCannon.UniqueName
                },
                new Part()
                {
                    type = PType.missiles,
                    skin = ModEntry.Instance.MikuMissiles.UniqueName
                },
                new Part()
                {
                    type = PType.wing,
                    skin = ModEntry.Instance.MikuWing.UniqueName,
                    flip = true,
                    damageModifier = hard ? PDamMod.armor : PDamMod.none
                }
            }
        };
    }
    public override EnemyDecision PickNextIntent(State s, Combat c, Ship ownShip)
    {
        MissileType m1 = MissileType.normal;
        int b1 = 1;
        int d1 = 1;
        int p1 = 1;
        if (s.GetHarderElites())
        {
            m1 = MissileType.heavy;
            d1 = 2;
        }
        if (aiCounter > 9)
        {
            m1 = MissileType.seeker;
            p1 = 2;
        }
        return AI.MoveSet(aiCounter++, () => new EnemyDecision
        {
            actions = AIHelpers.MoveToAimAt(s, ownShip, s.ship, 0, 5, movesFast: false, attackWeakPoints: false, avoidAsteroids: false, avoidMines: false),
            intents = new List<Intent>
            {
                new IntentAttack
                {
                    damage = aiCounter / 3,
                    fromX = 0,
                    status = Status.tempShield,
                },
                new IntentAttack
                {
                    damage = d1 + (aiCounter / 3),
                    fromX = 1,
                    status = Status.boost,
                    statusAmount = b1
                },
                new IntentAttack
                {
                    damage = d1,
                    fromX = 3
                },
                new IntentAttack
                {
                    damage = 0,
                    fromX = 4
                },
                new IntentAttack
                {
                    damage = aiCounter / 3,
                    fromX = 6,
                    status = Status.shield,
                    statusAmount = b1
                },
            }
        }, () => new EnemyDecision
        {
            intents = new List<Intent>
            {
                s.GetHarderElites() == true ? new IntentMissile
                {
                    targetPlayer = true,
                    missileType = m1,
                    fromX = 0
                } : new IntentSpawn
                {
                    thing = new Asteroid(),
                    fromX = 0,
                },
                new IntentMissile
                {
                    targetPlayer = true,
                    missileType = m1,
                    fromX = 5
                },
                new IntentStatus
                {
                    status = Status.shield,
                    amount = 2,
                    targetSelf = true,
                    fromX = 6
                }
            }
        }, () => new EnemyDecision
        {
            actions = AIHelpers.MoveToAimAt(s, ownShip, s.ship, 1, 5, movesFast: false, attackWeakPoints: true, avoidAsteroids: false, avoidMines: false),
            intents = new List<Intent>
            {
                new IntentAttack
                {
                    damage = d1,
                    fromX = 1,
                    status = Status.boost,
                    statusAmount = b1
                },
                new IntentAttack
                {
                    damage = 1,
                    fromX = 4,
                    status = Status.boost
                },
                new IntentStatus
                {
                    status = Status.powerdrive,
                    amount = p1,
                    targetSelf = true,
                    fromX = 6,

                }
            }
        });
    }
}
