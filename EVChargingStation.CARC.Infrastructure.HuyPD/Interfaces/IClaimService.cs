namespace EVChargingStation.CARC.Infrastructure.HuyPD.Interfaces
{
    public interface IClaimsService
    {
        public Guid GetCurrentUserId { get; }

        public string? IpAddress { get; }
    }
}
