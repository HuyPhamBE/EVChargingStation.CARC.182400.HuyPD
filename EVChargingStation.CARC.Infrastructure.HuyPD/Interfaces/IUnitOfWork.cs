using EVChargingStation.CARC.Domain.HuyPD.Entities;

namespace EVChargingStation.CARC.Infrastructure.HuyPD.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {        
        IGenericRepository<User> Users { get; }        
        IGenericRepository<VehicleHuyPD> VehicleHuyPDs { get; }
        Task<int> SaveChangesAsync();
    }
}
