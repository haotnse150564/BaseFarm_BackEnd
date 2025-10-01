using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum PlantStage
    {
        Sowing,             // Gieo hạt: bắt đầu quá trình trồng cây bằng cách gieo hạt giống
        Germination,        // Nảy mầm: hạt bắt đầu nảy mầm, xuất hiện rễ và mầm
        CotyledonLeaves,    // Ra lá mầm: lá mầm đầu tiên xuất hiện sau khi cây nảy mầm
        TrueLeavesGrowth,   // Phát triển lá thật: cây bắt đầu ra lá thật và phát triển tán lá
        VigorousGrowth,     // Tăng trưởng mạnh: cây phát triển nhanh, lá xanh tốt
        ReadyForHarvest,    // Sẵn sàng thu hoạch: lá đủ lớn và đạt chất lượng để thu hoạch
        PostHarvest         // Sau thu hoạch: giai đoạn sau khi thu hoạch, có thể tái sinh hoặc kết thúc vòng đời
    }
}
