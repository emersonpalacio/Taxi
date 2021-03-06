﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taxi.Web.Data.Entities
{
    public class UserGroupDetailEntity
    {
        public int Id { get; set; }

        public UserEntity User { get; set; }

        public ICollection<UserGroupDetailEntity> Users { get; set; }

    }
}
