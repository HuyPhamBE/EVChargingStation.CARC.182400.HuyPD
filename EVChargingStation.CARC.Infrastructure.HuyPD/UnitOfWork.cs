using EVChargingStation.CARC.Domain.HuyPD;
using EVChargingStation.CARC.Domain.HuyPD.Entities;
using EVChargingStation.CARC.Infrastructure.HuyPD.Interfaces;

namespace EVChargingStation.CARC.Infrastructure.HuyPD
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FA25_SWD392_SE182400_G6_EvChargingStation _dbContext;

        public UnitOfWork(FA25_SWD392_SE182400_G6_EvChargingStation dbContext,
            IGenericRepository<User> userRepository,
            IGenericRepository<VehicleHuyPD> vehicleHuyPDRepository)
        {
            _dbContext = dbContext;
            Users = userRepository;
            VehicleHuyPDs = vehicleHuyPDRepository;
        }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<VehicleHuyPD> VehicleHuyPDs { get; }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }
    }
}
