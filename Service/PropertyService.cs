using LLMS.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LLMS.Service
{
    internal class PropertyService : IPropertyService
    {
        private readonly IImageService _imageService;

        public PropertyService(IImageService imageService)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
        }

        /*--- CRUD ---*/
        public async Task<PropertyDto> CreatePropertyAsync(PropertyDto propertyDto)
        {
            if (propertyDto.ImageUrl == null)
            {
                throw new ApplicationException("An image must be associated with the property.");
            }
            try
            {
                using (var context = new testdb1Entities())
                {

                    var propertyEntity = MapToModel(propertyDto);

                    context.properties.Add(propertyEntity);
                    await context.SaveChangesAsync();
                    var newProperty = await GetPropertyByIdAsync(propertyEntity.id);
                    return newProperty;
                }
            }
            catch (DbUpdateException ex)
            {
                Trace.TraceError($"DbUpdateException in CreatePropertyAsync: {ex.Message}");
                throw new ApplicationException("An error occurred while accessing the database.", ex);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception in CreatePropertyAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }


        public async Task<bool> DeletePropertyAsync(int id)
        {
            try
            {
                using (var context = new testdb1Entities())
                {
                    var propertyEntity = await context.properties.FindAsync(id);
                    if (propertyEntity != null)
                    {
                        context.properties.Remove(propertyEntity);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;
                }
            }
            catch (DbUpdateException ex)
            {
                Trace.TraceError($"DbUpdateException in DeletePropertyAsync: {ex.Message}");
                throw new ApplicationException("An error occurred while accessing the database.", ex);
            }
            catch (InvalidOperationException ex)
            {
                Trace.TraceError($"InvalidOperationException in DeletePropertyAsync: {ex.Message}");
                throw new ApplicationException("An invalid operation was attempted.", ex);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception in DeletePropertyAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
        {
            try
            {
                using (var context = new testdb1Entities())
                {
                    var propertyEntities = await context.properties.ToListAsync();
                    var propertyDtosTasks = propertyEntities.Select(p => MapToDtoAsync(p));
                    var propertyDtos = await Task.WhenAll(propertyDtosTasks);
                    return propertyDtos;
                }
            }
            catch (DbUpdateException ex)
            {
                Trace.TraceError($"DbUpdateException in GetAllPropertiesAsync: {ex.Message}");
                throw new ApplicationException("An error occurred while accessing the database.", ex);
            }
            catch (InvalidOperationException ex)
            {
                Trace.TraceError($"InvalidOperationException in GetAllPropertiesAsync: {ex.Message}");
                throw new ApplicationException("An invalid operation was attempted.", ex);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception in GetAllPropertiesAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }


        public async Task<PropertyDto> GetPropertyByIdAsync(int id)
        {
            try
            {

                using (var context = new testdb1Entities())
                {
                    var propertyEntity = await context.properties.FindAsync(id);
                    if (propertyEntity != null)
                    {
                        return await MapToDtoAsync(propertyEntity);
                    }
                }
                return null;
            }
            catch (DbUpdateException ex)
            {
                Trace.TraceError($"DbUpdateException in GetPropertyByIdAsync: {ex.Message}");
                throw new ApplicationException("An error occurred while accessing the database.", ex);
            }
            catch (InvalidOperationException ex)
            {
                Trace.TraceError($"InvalidOperationException in GetPropertyByIdAsync: {ex.Message}");
                throw new ApplicationException("An invalid operation was attempted.", ex);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception in GetPropertyByIdAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        public async Task<PropertyDto> UpdatePropertyAsync(PropertyDto propertyDto)
        {
            if (propertyDto.ImageUrl == null)
            {
                throw new ApplicationException("An image must be associated with the property.");
            }
            try
            {
                using (var context = new testdb1Entities())
                {
                    var propertyEntity = await context.properties.FindAsync(propertyDto.Id);
                    if (propertyEntity != null)
                    {
                        if (!string.Equals(propertyEntity.address, propertyDto.Address))
                        {
                            propertyEntity.address = propertyDto.Address;
                        }
                        if (propertyEntity.number_of_units != propertyDto.NumberOfUnits)
                        {
                            propertyEntity.number_of_units = propertyDto.NumberOfUnits;
                        }
                        if (!string.Equals(propertyEntity.property_type, propertyDto.PropertyType))
                        {
                            propertyEntity.property_type = propertyDto.PropertyType;
                        }
                        if (propertyEntity.size_in_sq_ft != propertyDto.SizeInSqFt)
                        {
                            propertyEntity.size_in_sq_ft = propertyDto.SizeInSqFt;
                        }
                        if (propertyEntity.year_built != propertyDto.YearBuilt)
                        {
                            propertyEntity.year_built = propertyDto.YearBuilt;
                        }
                        if (propertyEntity.rental_price != propertyDto.RentalPrice)
                        {
                            propertyEntity.rental_price = propertyDto.RentalPrice;
                        }
                        if (!string.Equals(propertyEntity.amenities, propertyDto.Amenities))
                        {
                            propertyEntity.amenities = propertyDto.Amenities;
                        }
                        if (!string.Equals(propertyEntity.status, propertyDto.Status))
                        {
                            propertyEntity.status = propertyDto.Status;
                        }
                        if (!string.Equals(propertyEntity.lease_terms, propertyDto.LeaseTerms))
                        {
                            propertyEntity.lease_terms = propertyDto.LeaseTerms;
                        }
                        if (!string.Equals(propertyEntity.description, propertyDto.Description))
                        {
                            propertyEntity.description = propertyDto.Description;
                        }

                        int? imageId = await _imageService.GetImageIdByUrlAsync(propertyDto.ImageUrl);

                        if (propertyEntity.image_id != imageId)
                        {
                            propertyEntity.image_id = await _imageService.CreateImageRecordAsync(propertyDto.ImageUrl);
                        }

                        await context.SaveChangesAsync();
                        return await MapToDtoAsync(propertyEntity);
                    }
                }

                return null;

            }
            catch (DbUpdateException ex)
            {
                Trace.TraceError($"DbUpdateException in UpdatePropertyAsync: {ex.Message}");
                throw new ApplicationException("An error occurred while accessing the database.", ex);
            }
            catch (InvalidOperationException ex)
            {
                Trace.TraceError($"InvalidOperationException in UpdatePropertyAsync: {ex.Message}");
                throw new ApplicationException("An invalid operation was attempted.", ex);
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception in UpdatePropertyAsync: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred.", ex);
            }
        }

        private async Task<PropertyDto> MapToDtoAsync(property propertyEntity)
        {
            string imageUrl = null;
            if (propertyEntity.image_id > 0)
            {
                imageUrl = await _imageService.GetImageUrlByIdAsync(propertyEntity.image_id);
            }

            return new PropertyDto
            {
                Id = propertyEntity.id,
                Address = propertyEntity.address,
                NumberOfUnits = propertyEntity.number_of_units,
                PropertyType = propertyEntity.property_type,
                SizeInSqFt = propertyEntity.size_in_sq_ft,
                YearBuilt = propertyEntity.year_built,
                RentalPrice = propertyEntity.rental_price,
                Amenities = propertyEntity.amenities,
                Status = propertyEntity.status,
                LeaseTerms = propertyEntity.lease_terms,
                ImageUrl = imageUrl,
                Description = propertyEntity.description
            };
        }

        private property MapToModel(PropertyDto dto)
        {
            try
            {
                return new property
                {
                    address = dto.Address,
                    number_of_units = dto.NumberOfUnits,
                    property_type = dto.PropertyType,
                    size_in_sq_ft = dto.SizeInSqFt,
                    year_built = dto.YearBuilt,
                    rental_price = dto.RentalPrice,
                    amenities = dto.Amenities,
                    status = dto.Status, 
                    lease_terms = dto.LeaseTerms,
                    image_id = dto.ImageId,
                    description = dto.Description,
                    created_at = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Trace.TraceError($"Exception in MapToModel: {ex.Message}");
                throw new ApplicationException("An unexpected error occurred during the mapping process.", ex);
            }
        }
    }
}
