﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taxi.Common.Enum;

namespace Taxi.Web.Data.Entities
{
    public class UserGroupRequestEntity
    {
        public int Id { get; set; }

        public UserEntity ProposalUser { get; set; }

        public UserEntity RequiredUser { get; set; }

        public UserGroupStatus Status { get; set; }

        public Guid Token { get; set; }

    }
}
