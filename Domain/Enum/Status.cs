using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enum
{
    public enum Status
    {
        DEACTIVATED,
        ACTIVE
    }
    public enum IndividualStatus
    {
        COMPLETED,
        IN_PROGRESS
    }
    public enum FarmActivityStatus
    {
        DEACTIVATED,
        ACTIVE,
        COMPLETED,
        IN_PROGRESS
    }
    public enum ProductStatus
    {
        DEACTIVED,
        ACTIVE,
        //OUT_OF_STOCK,
        //DISCONTINUED
    }
    public enum DiseaseStatus
    {
        None = 0,             // Không có bệnh
        DownyMildew,          // Nấm phấn trắng
        PowderyMildew,        // Nấm phấn
        LeafSpot,             // Đốm lá
        BacterialSoftRot,     // Thối nhũn do vi khuẩn
        FusariumWilt,         // Héo vàng Fusarium
        Anthracnose,          // Thán thư
        DampingOff,           // Chết cây con
        BlackRot,             // Thối đen
        MosaicVirus,          // Virus khảm
        AphidInfestation,     // Rệp
        ThripsDamage,         // Bọ trĩ
        WhiteflyInfestation   // Ruồi trắng
    }
    public enum CropStatus
    {
        ACTIVE,
        INACTIVE,
        IN_STOCK // đang được lên product
    }
    public enum AccountStatus
    {
        DEACTIVATED,
        ACTIVE,
        SUSPENDED,
        BANNED

    }
    public enum PaymentStatus
    {
        UNPAID,
        PAID,
        UNDISCHARGED,
        PENDING,
        CANCELLED,
        COMPLETED,
        DELIVERED
    }

}
