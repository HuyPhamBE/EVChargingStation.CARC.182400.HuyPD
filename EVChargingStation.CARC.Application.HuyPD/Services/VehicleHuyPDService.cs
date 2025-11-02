using EVChargingStation.CARC.Domain.HuyPD.DTOs.VehiclesDTO;
using EVChargingStation.CARC.Domain.HuyPD.Entities;
using EVChargingStation.CARC.Domain.HuyPD.Enums;
using EVChargingStation.CARC.Infrastructure.HuyPD.Commons;
using EVChargingStation.CARC.Infrastructure.HuyPD.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVChargingStation.CARC.Application.HuyPD.Services
{
    public interface IVehicleHuyPDService
    {
        public Task<Pagination<VehiclesResponseDTO>> getAllVehicleAsync(int page, int pageSize, string? model, string? brand, ConnectorType? connectorType);
        public Task<VehiclesResponseDTO> getVehicleByIdAsync(Guid id);
        public Task<bool> AddVehicleAsync(VehiclesResponseDTO vehicleDTO, Guid userId);
        public Task<bool> UpdateVehicleAsync(VehiclesResponseDTO vehicleDTO, Guid guid);
        public Task<bool> DeleteVehicleAsync(Guid id);
    }
    public class VehicleHuyPDService : IVehicleHuyPDService
    {
        IUnitOfWork _unitOfWork;
        public VehicleHuyPDService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Pagination<VehiclesResponseDTO>> getAllVehicleAsync(int page, int pageSize,string? model,string? brand ,ConnectorType? connectorType=null)
        {
            var vehicles = await _unitOfWork.VehicleHuyPDs.GetAllAsync(f => !f.IsDeleted);
            var query= vehicles.AsQueryable();
            if(connectorType.HasValue)
            {
                query = query.Where(v => v.ConnectorType == connectorType.Value);
            }
            if(model is not null)
            {
                query = query.Where(v => v.Model.Contains(model, StringComparison.OrdinalIgnoreCase));
            }
            if(brand is not null)
            {
                query = query.Where(v => v.Make.Contains(brand, StringComparison.OrdinalIgnoreCase));
            }
            query = query.OrderByDescending(v => v.Year);
            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedItems = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            var result = pagedItems.Select(f => new VehiclesResponseDTO
            {
                HuyPDID = f.HuyPDID,
                Make = f.Make,
                Model = f.Model,
                Year = f.Year,
                LicensePlate = f.LicensePlate,
                ConnectorType = f.ConnectorType
            }).ToList();
            var response =new Pagination<VehiclesResponseDTO>(result, totalItems, page, pageSize);
            return response;
        }
        public async Task<VehiclesResponseDTO> getVehicleByIdAsync(Guid id)
        {
            var vehicle = await _unitOfWork.VehicleHuyPDs.FirstOrDefaultAsync(f => f.HuyPDID == id && !f.IsDeleted);
            if (vehicle == null)
            {
                return null;
            }
            var response = new VehiclesResponseDTO
            {
                HuyPDID = vehicle.HuyPDID,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                LicensePlate = vehicle.LicensePlate,
                ConnectorType = vehicle.ConnectorType
            };
            return response;
        }
        public async Task<bool> AddVehicleAsync(VehiclesResponseDTO vehicleDTO, Guid userId)
        {
            var vehicle = new VehicleHuyPD
            {
                HuyPDID = Guid.NewGuid(),
                Make = vehicleDTO.Make,
                Model = vehicleDTO.Model,
                Year = vehicleDTO.Year,
                LicensePlate = vehicleDTO.LicensePlate,
                ConnectorType = vehicleDTO.ConnectorType,
                UserId = userId,
                IsDeleted = false
            };
            await _unitOfWork.VehicleHuyPDs.AddAsync(vehicle);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        public async Task<bool> UpdateVehicleAsync(VehiclesResponseDTO vehicleDTO,Guid guid)
        {
            var vehicle =await _unitOfWork.VehicleHuyPDs.FirstOrDefaultAsync(f=>f.HuyPDID == guid);
            if(vehicle == null)
            {
                return false;
            }
            vehicle.Make = vehicleDTO.Make;
            vehicle.Model = vehicleDTO.Model;
            vehicle.Year = vehicleDTO.Year;
            vehicle.LicensePlate = vehicleDTO.LicensePlate;
            vehicle.ConnectorType = vehicleDTO.ConnectorType;
            await _unitOfWork.VehicleHuyPDs.Update(vehicle);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
        public async Task<bool> DeleteVehicleAsync(Guid id)
        {
            await _unitOfWork.VehicleHuyPDs.HardRemove(v => v.HuyPDID == id);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
        }
    }
}
