using LLMS.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPropertyService
{
    Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
    Task<PropertyDto> GetPropertyByIdAsync(int id);
    Task<PropertyDto> CreatePropertyAsync(PropertyDto propertyDto);
    Task<PropertyDto> UpdatePropertyAsync(PropertyDto propertyDto);
    Task<bool> DeletePropertyAsync(int id);
    // More Methods
}

