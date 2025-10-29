using EVChargingStation.CARC.Infrastructure.HuyPD.Interfaces;

namespace EVChargingStation.CARC.Infrastructure.HuyPD.Commons
{
    public class CurrentTime : ICurrentTime
    {
        public DateTime GetCurrentTime()
        {
            return DateTime.UtcNow; // Đảm bảo trả về thời gian UTC
        }
    }
}
