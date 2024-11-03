﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Core.Entities.DTO
{
    public class HotelFormDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DestinationId { get; set; }

        public string Address { get; set; }
        public decimal PricePerNight { get; set; }

    }
}
