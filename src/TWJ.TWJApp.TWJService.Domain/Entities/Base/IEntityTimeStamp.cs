﻿using System;

namespace TWJ.TWJApp.TWJService.Domain.Entities.Base
{
    public interface IEntityTimeStamp
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}