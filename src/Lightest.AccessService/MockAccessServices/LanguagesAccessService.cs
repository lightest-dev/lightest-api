﻿using System;
using System.Threading.Tasks;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;

namespace Lightest.AccessService.MockAccessServices
{
    internal class LanguagesAccessService : IAccessService<Language>
    {
        public Task<bool> CanAdd(Language item, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanRead(Guid id, ApplicationUser requester) => Task.FromResult(true);

        public Task<bool> CanEdit(Guid id, ApplicationUser requester) => Task.FromResult(true);
    }
}
