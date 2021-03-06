﻿using System;

namespace Minifutbol.BL.DTO
{
    public class UserClaimDto
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int? AreaPrefix { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Description { get; set; }
    }
}
