using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlerterService.Models
{
    public class ArkhamResponse
    {
        public Transfer? Transfer { get; set; }
        public string AlertName { get; set; } = string.Empty;
        public int Id { get; set; }
    }

    public class Transfer
    {
        public string Id { get; set; } = string.Empty;
        public string TransactionHash { get; set; } = string.Empty;
        public List<FromAddressDto>? FromAddresses { get; set; }
         public List<FromAddressDto>? ToAddresses { get; set; }
        public AddressInfo? ToAddress { get; set; }
        public AddressInfo? FromAddress { get; set; }
        public bool FromIsContract { get; set; }
       
        public bool ToIsContract { get; set; }
        public string TokenAddress { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime BlockTimestamp { get; set; }
        public long BlockNumber { get; set; }
        public string BlockHash { get; set; } = string.Empty;
        public string TokenName { get; set; } = string.Empty;
        public string TokenSymbol { get; set; } = string.Empty;
        public int TokenDecimals { get; set; }
        public double UnitValue { get; set; }
        public string TokenId { get; set; } = string.Empty;
        public double HistoricalUSD { get; set; }
        public string Chain { get; set; } = string.Empty;
    }

    public class AddressInfo
    {
        public string Address { get; set; } = string.Empty;
        public string Chain { get; set; } = string.Empty;
        public ArkhamEntity? ArkhamEntity { get; set; }
        public ArkhamLabel? ArkhamLabel { get; set; }
        public bool IsUserAddress { get; set; }
        public bool Contract { get; set; }
    }
    public class FromAddressDto
    {
        public AddressInfo? Address { get; set; }
        public double Value { get; set; }
    }

    public class ArkhamEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? Service { get; set; }
        public List<string>? Addresses { get; set; }
        public string Website { get; set; } = string.Empty;
        public string? Twitter { get; set; }
        public string? Crunchbase { get; set; }
        public string? Linkedin { get; set; }
    }

    public class ArkhamLabel
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string ChainType { get; set; } = string.Empty;
    }


}