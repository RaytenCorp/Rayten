
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Vanilla.Skill
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class RequiresSkillComponent : Component
    {

        [DataField("NeedCraftableSkills"), AutoNetworkedField]
        public bool NeedCraftableSkills { get; set; } = false;

        //Медицина
        [DataField("RequiresMedicineLevel"), AutoNetworkedField]
        public SkillLevel RequiresMedicineLevel { get; set; } = 0;

        //Инженерия
        [DataField("RequiresEngineeringLevel"), AutoNetworkedField]
        public SkillLevel RequiresEngineeringLevel { get; set; } = 0;

        //Лёгкие навыки

        //Пилотирование
        [DataField("RequiresPiloting"), AutoNetworkedField]
        public bool RequiresPiloting { get; set; } = false;
        //Муз. инструменты
        [DataField("RequiresMusInstruments"), AutoNetworkedField]
        public bool RequiresMusInstruments { get; set; } = false;
        //Ботаника
        [DataField("RequiresBotany"), AutoNetworkedField]
        public bool RequiresBotany { get; set; } = false;
        //исследования
        [DataField("RequiresResearch"), AutoNetworkedField]
        public bool RequiresResearch { get; set; } = false;
    }
}
