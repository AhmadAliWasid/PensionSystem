﻿using PensionSystem.Entities.Models;
using PensionSystem.ViewModels;

namespace WebAPI.Interfaces
{
    public interface IWWFSanction : ICrud<WWFSanction>
    {
        public Task<IEnumerable<SelectOptions>> GetOptions(int PDUId);
    }
}
