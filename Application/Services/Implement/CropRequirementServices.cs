using Application.Interfaces;
using AutoMapper;
using Domain.Enum;
using Domain.Model;
using Infrastructure.ViewModel.Request;
using Infrastructure.ViewModel.Response;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Infrastructure.ViewModel.Response.CropRequirementResponse;

namespace Application.Services.Implement
{
    public class CropRequirementServices : ICropRequirementServices
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly ICurrentTime _currentTime;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;
        public CropRequirementServices(IUnitOfWorks unitOfWork, ICurrentTime currentTime, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentTime = currentTime;
            this.configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ResponseDTO?> CreateCropRequirementAsync(CropRequirementRequest cropRequirement, PlantStage plantStage, long cropId)
        {
            try
            {
                var cropReq = _mapper.Map<CropRequirement>(cropRequirement);
                cropReq.CropId = cropId;
                cropReq.PlantStage = plantStage;
                cropReq.CreatedDate = _currentTime.GetCurrentTime();
                await _unitOfWork.cropRequirementRepository.AddAsync(cropReq);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<CropRequirementView>(cropReq);
                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }

        public async Task<ResponseDTO> DeleteCropRequirementAsync(long cropRequirementId)
        {
            try
            {
                var cropReq = await _unitOfWork.cropRequirementRepository.GetByIdAsync(cropRequirementId);
                if (cropReq == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                await _unitOfWork.cropRequirementRepository.DeleteAsync(cropReq);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<CropRequirementView>(cropReq);
                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }

        public async Task<ResponseDTO> DuplicateCropRequirementAsynce(long cropRequirementId, PlantStage plantStage, long cropId)
        {
            try
            {
                var cropReqOrigin = await _unitOfWork.cropRequirementRepository.GetByIdAsync(cropRequirementId);
                var newRequest = _mapper.Map<CropRequirementRequest>(cropReqOrigin);
                if (cropReqOrigin == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                var newCropReq = _mapper.Map<CropRequirement>(newRequest);
                newCropReq.PlantStage = plantStage;
                newCropReq.CropId = cropId;
                await _unitOfWork.cropRequirementRepository.AddAsync(newCropReq);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<CropRequirementView>(newCropReq);
                return new ResponseDTO(Const.SUCCESS_CREATE_CODE, Const.SUCCESS_CREATE_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }
        public async Task<ResponseDTO> GetAll()
        {
            try
            {
                var cropReqs =await _unitOfWork.cropRequirementRepository.GetAllAsync();
                if (cropReqs == null || cropReqs.Count == 0)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                var result = _mapper.Map<List<CropRequirementView>>(cropReqs);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetCropRequirementByIdAsync(long cropId)
        {
            try
            {
                var cropReq = await _unitOfWork.cropRequirementRepository.GetByIdAsync(cropId);
                if (cropReq == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                var result = _mapper.Map<CropRequirementView>(cropReq);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }

        public async Task<ResponseDTO> GetListCropRequirementByCropId(long cropId)
        {
            try
            {
                var cropReqs = await _unitOfWork.cropRequirementRepository.GetByCropIdAsynce(cropId);
                if (cropReqs == null || cropReqs.Count == 0)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                var result = _mapper.Map<List<CropRequirementView>>(cropReqs);
                return new ResponseDTO(Const.SUCCESS_READ_CODE, Const.SUCCESS_READ_MSG, result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }
        public async Task<ResponseDTO?> UpdateCropRequirementAsync(CropRequirementRequest cropRequirement, PlantStage plantStage, long cropId)
        {
            try
            {
                var cropReq = await _unitOfWork.cropRequirementRepository.GetByIdAsync(cropId);
                if (cropReq == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                _mapper.Map(cropRequirement, cropReq);
                cropReq.PlantStage = plantStage;
                cropReq.UpdatedDate = _currentTime.GetCurrentTime();
                await _unitOfWork.cropRequirementRepository.UpdateAsync(cropReq);
                await _unitOfWork.SaveChangesAsync();

                var result = _mapper.Map<CropRequirementView>(cropReq);
                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);

            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdatePlantStage(long cropId, PlantStage plantStage)
        {
            try
            {
                var cropReq = await _unitOfWork.cropRequirementRepository.GetByIdAsync(cropId);
                if (cropReq == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }

                cropReq.PlantStage = plantStage;
                cropReq.UpdatedDate = _currentTime.GetCurrentTime();
                await _unitOfWork.cropRequirementRepository.UpdateAsync(cropReq);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.Map<CropRequirementView>(cropReq);
                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);

            }
            catch
            (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }

        public async Task<ResponseDTO> UpdateStatus(long cropId)
        {
            try
            {
                var cropReq = await _unitOfWork.cropRequirementRepository.GetByIdAsync(cropId);
                if (cropReq == null)
                {
                    return new ResponseDTO(Const.FAIL_READ_CODE, Const.FAIL_READ_MSG);
                }
                cropReq.IsActive = !cropReq.IsActive;
                cropReq.UpdatedDate = _currentTime.GetCurrentTime();

                await _unitOfWork.cropRequirementRepository.UpdateAsync(cropReq);
                await _unitOfWork.SaveChangesAsync();
                var result = _mapper.Map<CropRequirementView>(cropReq);
                return new ResponseDTO(Const.SUCCESS_UPDATE_CODE, Const.SUCCESS_UPDATE_MSG, result);

            }
            catch (Exception ex)
            {
                return new ResponseDTO(Const.FAIL_READ_CODE, ex.Message);
            }
        }
    }
}
