namespace GamaEdtech.Backend.Common.Data
{
    using GamaEdtech.Backend.Common.DataAnnotation;
    using GamaEdtech.Backend.Common.Resources;

    public class ExportType : Enumeration.Enumeration<byte>
    {
        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(ExportType))]
        public static readonly ExportType Excel = new(nameof(Excel), 0);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(ExportType))]
        public static readonly ExportType Pdf = new(nameof(Pdf), 1);

        [Display(ResourceType = typeof(GlobalResource), EnumType = typeof(ExportType))]
        public static readonly ExportType Csv = new(nameof(Csv), 2);

        public ExportType()
        {
        }

        public ExportType(string name, byte value)
            : base(name, value)
        {
        }
    }
}
