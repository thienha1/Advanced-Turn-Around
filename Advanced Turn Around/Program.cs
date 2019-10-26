using System.Threading;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;
using SharpDX;

namespace Advanced_Turn_Around
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Game.OnUpdate += delegate
            {
                var onGameLoadThread = new Thread(Game_OnGameLoad);
                onGameLoadThread.Start();
            };
        }

        private static void Game_OnGameLoad()
        {
            Internal.AddChampions();

            Variable.Config = new Menu("ATA", "Roach's Advanced Turn Around#", true);

            Variable.Config.Add(new MenuBool("Enabled", "Enable the Script", true));

            var Champions = Variable.Config.Add(new Menu("CAS", "Champions and Spells"));
            foreach (var champ in Variable.ExistingChampions)
            {
                var Champion = Champions.Add(new Menu(champ.CharName, champ.CharName + "'s Spells to Avoid"));
                Champion.Add(new MenuBool(champ.Slot.ToString(), champ.SpellName, true));
            }

            Variable.Config.Attach();

            Game.Print(
                "<font color=\"#FF440A\">Advanced Turn Around# -</font> <font color=\"#FFFFFF\">Loaded</font>");

            AIBaseClient.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        private static void Obj_AI_Base_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var enabled = Variable.Config["Enabled"].GetValue<MenuBool>().Enabled;
            if (!enabled || !Variable.Player.IsTargetable || (sender == null || sender.Team == Variable.Player.Team))
            {
                return;
            }

            foreach (var champ in Variable.ExistingChampions)
            {
                var champion = Variable.Config["CAS"][champ.CharName];
                if (champion == null || champion[champ.Slot.ToString()] == null || !champion[champ.Slot.ToString()].GetValue<MenuBool>().Enabled)
                {
                    continue;
                }

                if (champ.Slot != (sender as AIHeroClient).GetSpellSlot(args.SData.Name) ||
                    (!(Variable.Player.Distance(sender.Position) <= champ.Range) && args.Target != Variable.Player))
                {
                    continue;
                }

                var vector =
                    new Vector3(
                        Variable.Player.Position.X +
                        ((sender.Position.X - Variable.Player.Position.X)*(Internal.MoveTo(champ.Movement))/
                         Variable.Player.Distance(sender.Position)),
                        Variable.Player.Position.Y +
                        ((sender.Position.Y - Variable.Player.Position.Y)*(Internal.MoveTo(champ.Movement))/
                         Variable.Player.Distance(sender.Position)), 0);
                Variable.Player.IssueOrder(GameObjectOrder.MoveTo, vector);
                Orbwalker.MovementState = false;
                DelayAction.Add((int) (champ.CastTime + 0.1)*1000, () => Orbwalker.MovementState = true);
            }
        }
    }
}