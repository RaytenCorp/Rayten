using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

namespace Content.Shared.Vanilla.Background
{
    [Prototype("BackgroundSpecial")]
    public sealed partial class BackgroundSpecialPrototype : IPrototype
    {
        [IdDataField]
        public string ID { get; set; } = default!;
        
        [DataField("name")]
        public string Name { get; set; } = string.Empty;

        [DataField("desc")]
        public string Description { get; set; } = string.Empty;

        [DataField("color")]
        public Color color { get; set; } = Color.Yellow;

        [DataField("mindRoles")]
        public List<EntProtoId>? MindRoles;

        [DataField("items")]
        public List<EntProtoId>? Items;

        [DataField("components")]
        public ComponentRegistry? Components { get; private set; }

    }
}
