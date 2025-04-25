namespace GamaEdtech.Data.Dto.Transaction
{
    public sealed class GetStatisticsResponseDto
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public bool IsDebit { get; set; }
    }
}
