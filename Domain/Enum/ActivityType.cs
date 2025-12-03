using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum ActivityType
    {
        [Description("Chuẩn bị đất trước gieo")]
        SoilPreparation,

        [Description("Gieo hạt")]
        Sowing,

        [Description("Tỉa cây con cho đều")]
        Thinning,

        [Description("Bón phân pha loãng (NPK 20–30%)")]
        FertilizingDiluted,

        [Description("Nhổ cỏ nhỏ")]
        Weeding,

        [Description("Phòng trừ sâu bằng thuốc sinh học")]
        PestControl,

        [Description("Bón phân cho lá (N, hữu cơ)")]
        FertilizingLeaf,

        [Description("Thu hoạch")]
        Harvesting,

        [Description("Dọn dẹp đồng ruộng")]
        CleaningFarmArea

    }
}
