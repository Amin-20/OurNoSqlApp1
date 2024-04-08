using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Models
{
    public class Store:TableEntity
    {
        public string? CountryName { get; set; }
        public string? CityName { get; set; }
        public string? Address { get; set; }
    }
}
