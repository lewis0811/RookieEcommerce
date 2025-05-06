using Riok.Mapperly.Abstractions;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.SharedViewModels.CartDtos;
using RookieEcommerce.SharedViewModels.CustomerDtos;
using RookieEcommerce.SharedViewModels.ProductImageDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RookieEcommerce.Application.Mappers
{
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public static partial class CustomerMapper
    {
        public static partial CustomerDetailsDto CustomerToCustomerDetailsDto(Customer customer);

        public static partial List<CustomerDetailsDto> CustomerListToCustomerDetailsDto(List<Customer> customers);
    }
}
