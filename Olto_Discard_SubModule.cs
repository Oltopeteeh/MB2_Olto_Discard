using System;
using System.IO;
using System.Xml.Serialization;
using TaleWorlds.CampaignSystem.CharacterDevelopment;      // 172 or later
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.GameComponents;            // 172
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Library;

namespace Olto_Discard
{
    public class Olto_Discard_SubModule : MBSubModuleBase
    {
        public static float discard_xp_mod;
        public static int discard_max;
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            gameStarterObject.AddModel((GameModel)new OltoItemDiscardModel());
        }
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            this.loadSettings();
        }
        private void loadSettings()
        {
            Settings settings = new XmlSerializer(typeof(Settings)).Deserialize((Stream)File.OpenRead(Path.Combine(BasePath.Name, "Modules/Olto_Discard/settings.xml"))) as Settings;
            Olto_Discard_SubModule.discard_xp_mod = settings.discard_xp_mod;
            Olto_Discard_SubModule.discard_max = settings.discard_max;
        }
    }

    [Serializable]
    public class Settings
    {
        public float discard_xp_mod;
        public int discard_max;
    }


    internal class OltoItemDiscardModel : DefaultItemDiscardModel
    {
        public override bool PlayerCanDonateItem(ItemObject item) { return true; }
        public override int GetXpBonusForDiscardingItem(ItemObject item, int amount = 1)
        {
            int num = 0;
            float num1 = item.Value * Olto_Discard_SubModule.discard_xp_mod;
            float num2 = 0; // 37.5f;

            if (item.HasWeaponComponent && MobileParty.MainParty.HasPerk(DefaultPerks.Steward.GivingHands)) num1 *= 2;
            else if (item.HasArmorComponent && MobileParty.MainParty.HasPerk(DefaultPerks.Steward.PaidInPromise, checkSecondaryRole: true)) num1 *= 2;

            if (item.HasWeaponComponent || item.HasArmorComponent)
                if (item.Tierf >= 0) num2 = 37.5f * MathF.Pow(2, item.Tierf);
                else { num2 = 37; };

            num = (int)MathF.Round(MathF.Max(num1, num2));
            if (num > Olto_Discard_SubModule.discard_max) num = (int)(Olto_Discard_SubModule.discard_max + MathF.Sqrt(num - Olto_Discard_SubModule.discard_max));

            return num * amount;
        }
    }

}