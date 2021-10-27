using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inheritance.MapObjects
{
    public interface IHaveOwner
    {
        int Owner { get; set; }
    }

    public interface IHaveArmy
    {
        Army Army { get; set; }
    }

    public interface IHaveTreasure
    {
        Treasure Treasure { get; set; }
    }

    public class Dwelling : IHaveOwner
    {
        public int Owner { get; set; }
    }

    public class Mine : IHaveOwner, IHaveTreasure, IHaveArmy
    {
        public int Owner { get; set; }
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Creeps : IHaveTreasure, IHaveArmy
    {
        public Army Army { get; set; }
        public Treasure Treasure { get; set; }
    }

    public class Wolves : IHaveArmy
    {
        public Army Army { get; set; }
    }

    public class ResourcePile : IHaveTreasure
    {
        public Treasure Treasure { get; set; }
    }

    public static class Interaction
    {
        public static void Make(Player player, object mapObject)
        {
            if (mapObject is IHaveArmy haveArmyObject)
            {
                if (!player.CanBeat(haveArmyObject.Army))
                    player.Die();
            }

            if (!player.Dead)
            {
                if (mapObject is IHaveOwner haveOwnerObject)
                    haveOwnerObject.Owner = player.Id;

                if (mapObject is IHaveTreasure haveTreasureObject)
                    player.Consume(haveTreasureObject.Treasure);
            }
        }
    }
}
